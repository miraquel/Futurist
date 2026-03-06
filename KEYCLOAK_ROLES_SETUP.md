# Keycloak Roles Integration Guide

## Overview
This document explains how the Futurist application extracts roles from Keycloak JWT tokens and makes them available for authorization in ASP.NET Core controllers.

## JWT Token Structure
The Keycloak JWT token contains roles in the following structure:
```json
{
  "resource_access": {
    "futurist": {
      "roles": [
        "costing",
        "admin"
      ]
    },
    "account": {
      "roles": [
        "manage-account",
        "manage-account-links",
        "view-profile"
      ]
    }
  }
}
```

## Implementation

### 1. JWT Bearer Events Configuration
The roles are extracted directly from the JWT token using the `OnTokenValidated` event in the JWT Bearer authentication configuration. This approach is more reliable than using `IClaimsTransformation` because it has direct access to the JWT payload.

**File:** `Futurist.Web/Program.cs`

Key features in the `OnTokenValidated` event:
- Accesses the `resource_access` claim directly from the validated JWT token
- Parses the JSON structure to navigate to `resource_access.futurist.roles`
- Extracts each role and adds it as a `ClaimTypes.Role` claim to the user's identity
- Handles JSON parsing errors gracefully to prevent authentication failures
- Executes only once per token validation, avoiding duplicate role claims

### 2. JWT Bearer Configuration
The JWT Bearer authentication is configured in `Program.cs` with:
- `MapInboundClaims = false` to preserve original JWT claim names
- `OnTokenValidated` event handler that extracts roles from `resource_access.futurist.roles`
- Proper issuer and audience validation
- `NameClaimType` set to `preferred_username`
- `RoleClaimType` set to `ClaimTypes.Role`

**Example Configuration:**
```csharp
options.MapInboundClaims = false; // Preserve original claim names from JWT
options.Events = new JwtBearerEvents
{
    OnTokenValidated = context =>
    {
        if (context.Principal?.Identity is ClaimsIdentity identity)
        {
            var resourceAccessClaim = identity.FindFirst("resource_access");
            
            if (resourceAccessClaim != null && !string.IsNullOrEmpty(resourceAccessClaim.Value))
            {
                try
                {
                    using var doc = JsonDocument.Parse(resourceAccessClaim.Value);
                    
                    if (doc.RootElement.TryGetProperty("futurist", out var futuristElement) &&
                        futuristElement.TryGetProperty("roles", out var rolesElement))
                    {
                        foreach (var role in rolesElement.EnumerateArray())
                        {
                            var roleValue = role.GetString();
                            if (!string.IsNullOrEmpty(roleValue))
                            {
                                identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Failed to parse resource_access claim: {ex.Message}");
                }
            }
        }
        return Task.CompletedTask;
    }
};
```

### 3. Why OnTokenValidated Instead of IClaimsTransformation?
The `OnTokenValidated` approach is superior because:
- **Direct Access**: Has immediate access to the JWT payload and all claims after token validation
- **Single Execution**: Runs only once per token validation, not on every authorization check
- **No Duplicate Issues**: Claims are added during authentication, not repeatedly during authorization
- **Better Performance**: Avoids the overhead of claims transformation on every request
- **Reliable**: The `resource_access` claim is guaranteed to be present in the validated token

## Usage in Controllers

### Single Role Authorization
```csharp
[Authorize(Roles = "admin")]
public IActionResult Index()
{
    // Only users with the "admin" role in resource_access.futurist.roles can access
    return View();
}
```

### Multiple Roles Authorization (OR logic)
```csharp
[Authorize(Roles = "costing,sc,admin")]
public IActionResult Index()
{
    // Users with "costing" OR "sc" OR "admin" roles can access
    return View();
}
```

### Multiple Roles Authorization (AND logic)
```csharp
[Authorize(Roles = "costing")]
[Authorize(Roles = "admin")]
public IActionResult Index()
{
    // Users must have BOTH "costing" AND "admin" roles to access
    return View();
}
```

## Example Controllers
The following controllers already use role-based authorization:

- **AdminController**: Requires `admin` role
- **BomStdController**: Requires `costing`, `sc`, `rni`, `finance`, or `admin` roles
- **FgCostController**: Requires various combinations of roles
- **ExchangeRateController**: Requires `costing`, `sc`, or `admin` roles
- **ItemAdjustmentController**: Requires `costing`, `sc`, or `admin` roles

## Testing

### View User Claims
Navigate to `/Admin/Index` (requires `admin` role) to view all claims associated with the current user, including the transformed role claims.

### Expected Role Claims
After authentication, users should have role claims like:
- `http://schemas.microsoft.com/ws/2008/06/identity/claims/role: admin`
- `http://schemas.microsoft.com/ws/2008/06/identity/claims/role: costing`

These are extracted from the JWT token's `resource_access.futurist.roles` array.

## Troubleshooting

### Roles Not Working
1. Verify the JWT token contains `resource_access.futurist.roles` with the expected roles
2. Check that the claims transformation is registered in `Program.cs`
3. Ensure the token is being passed correctly (Bearer token in Authorization header)
4. View claims at `/Admin/Index` to verify roles are being transformed correctly

### Token Validation Failures
1. Check that the `Authority` configuration matches the token's `iss` claim
2. Verify the `Audience` matches the token's `aud` claim
3. Review authentication logs in Serilog

## Security Notes
- The implementation validates JWT tokens from Keycloak
- Only roles from `resource_access.futurist.roles` are extracted
- Roles from other clients (like `account` or `inventory_management_system`) are ignored
- Token validation includes issuer, audience, and lifetime checks

