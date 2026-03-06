# Implementation Notes: Keycloak Roles Extraction Fix

## Problem
The `resource_access` claim was always returning `null` in the `IClaimsTransformation` implementation, causing roles to never be extracted from the JWT token.

## Root Cause
The `resource_access` property in a JWT token is part of the token's payload, but it's a complex JSON object. When using `IClaimsTransformation`:
1. The transformation runs on every authorization check (performance issue)
2. The `resource_access` claim might not be properly mapped as a string claim
3. The claim transformation happens after initial authentication, potentially missing the raw token data

## Solution
Changed from `IClaimsTransformation` to using the `OnTokenValidated` event in JWT Bearer authentication configuration.

### Why OnTokenValidated Works Better

1. **Direct Access to Token**: The `OnTokenValidated` event fires immediately after the JWT token is validated, giving direct access to all claims in their original form.

2. **MapInboundClaims = false**: By setting this to `false`, we preserve the original claim names from the JWT token, including complex JSON claims like `resource_access`.

3. **Single Execution**: The event runs only once per authentication, not on every authorization check.

4. **Guaranteed Claim Availability**: At this point in the authentication pipeline, all claims from the JWT payload are present.

## Changes Made

### 1. Program.cs
- Removed `IClaimsTransformation` service registration
- Added `MapInboundClaims = false` to JWT Bearer options
- Implemented `OnTokenValidated` event handler to extract roles from `resource_access.futurist.roles`
- Removed unused using directives

### 2. KeycloakRolesClaimsTransformation.cs
- Marked as `[Obsolete]` with deprecation notice
- Kept for reference but no longer in use

### 3. KEYCLOAK_ROLES_SETUP.md
- Updated documentation to reflect the new implementation
- Added explanation of why `OnTokenValidated` is superior to `IClaimsTransformation`

## How It Works Now

```
1. User authenticates → JWT token received
2. JWT Bearer validates token
3. OnTokenValidated event fires
4. Event handler:
   - Finds the "resource_access" claim
   - Parses it as JSON
   - Navigates to resource_access.futurist.roles
   - Adds each role as a ClaimTypes.Role claim
5. User now has proper role claims for authorization
```

## Testing
To verify the implementation works:
1. Authenticate with a user that has roles in `resource_access.futurist.roles`
2. Navigate to `/Admin/Index` (requires `admin` role)
3. Check that role claims are visible in the claims list
4. Verify controller authorization works with `[Authorize(Roles = "...")]`

## Expected Claims After Transformation
For a user with roles `["costing", "admin"]` in the JWT token:
- `http://schemas.microsoft.com/ws/2008/06/identity/claims/role: costing`
- `http://schemas.microsoft.com/ws/2008/06/identity/claims/role: admin`

These will be extracted from:
```json
{
  "resource_access": {
    "futurist": {
      "roles": ["costing", "admin"]
    }
  }
}
```

