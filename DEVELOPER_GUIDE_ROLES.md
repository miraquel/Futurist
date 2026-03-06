# Quick Reference: Keycloak Roles in Controllers

## How to Use Roles in Your Controllers

### Single Role
Only users with the `admin` role can access:
```csharp
[Authorize(Roles = "admin")]
public IActionResult AdminOnly()
{
    return View();
}
```

### Multiple Roles (OR logic)
Users with ANY of these roles can access:
```csharp
[Authorize(Roles = "costing,sc,admin")]
public IActionResult CoastingOrScOrAdmin()
{
    return View();
}
```

### Multiple Roles (AND logic)
Users must have ALL of these roles:
```csharp
[Authorize(Roles = "costing")]
[Authorize(Roles = "admin")]
public IActionResult MustHaveBothRoles()
{
    return View();
}
```

### Check Roles in Code
```csharp
public IActionResult Index()
{
    if (User.IsInRole("admin"))
    {
        // Admin-specific logic
    }
    
    var userRoles = User.Claims
        .Where(c => c.Type == ClaimTypes.Role)
        .Select(c => c.Value)
        .ToList();
    
    return View();
}
```

## Available Roles from Your JWT Token
Based on your JWT structure, these are the roles in `resource_access.futurist.roles`:
- `costing`
- `admin`

## How It Works
1. User logs in with Keycloak
2. JWT token is received with `resource_access.futurist.roles: ["costing", "admin"]`
3. Our `OnTokenValidated` event extracts these roles
4. Roles are added as `ClaimTypes.Role` claims
5. ASP.NET Core `[Authorize(Roles = "...")]` attribute checks these claims

## Debugging
To see all claims (including roles):
1. Go to `/Admin/Index` (requires `admin` role)
2. Look for claims with type: `http://schemas.microsoft.com/ws/2008/06/identity/claims/role`

## Common Issues

### "Access Denied" even though user has the role
- Check that the role name matches exactly (case-sensitive)
- Verify the JWT token contains the role in `resource_access.futurist.roles`
- Check `/Admin/Index` to see what roles are actually in the claims

### Role not appearing in claims
- Ensure the role is configured in Keycloak for the `futurist` client
- Check that the JWT token is being passed correctly (Bearer token in Authorization header)
- Verify the token is not expired

