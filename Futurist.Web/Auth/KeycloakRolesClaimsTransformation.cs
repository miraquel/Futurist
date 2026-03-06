using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace Futurist.Web.Auth;

public class KeycloakRolesClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var claimsIdentity = (ClaimsIdentity)principal.Identity!;
        
        // Check if we've already transformed the claims to avoid duplicate processing
        if (claimsIdentity.HasClaim(c => c.Type == "roles_transformed"))
        {
            return Task.FromResult(principal);
        }

        // Get the resource_access claim from the token
        var resourceAccessClaim = claimsIdentity.FindFirst("resource_access");
        
        if (resourceAccessClaim != null)
        {
            try
            {
                // Parse the resource_access JSON
                var resourceAccess = JsonDocument.Parse(resourceAccessClaim.Value);
                
                // Navigate to resource_access.futurist.roles
                if (resourceAccess.RootElement.TryGetProperty("futurist", out var futuristElement))
                {
                    if (futuristElement.TryGetProperty("roles", out var rolesElement))
                    {
                        // Add each role as a role claim
                        foreach (var roleValue in rolesElement.EnumerateArray().Select(role => role.GetString()).Where(roleValue => !string.IsNullOrEmpty(roleValue)))
                        {
                            if (roleValue != null)
                            {
                                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                            }
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // If parsing fails, log but don't throw
                // This prevents authentication from failing
            }
        }
        
        // Mark as transformed
        claimsIdentity.AddClaim(new Claim("roles_transformed", "true"));
        
        return Task.FromResult(principal);
    }
}

