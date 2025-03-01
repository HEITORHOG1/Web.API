using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Web.Domain.Enums;

namespace Web.API.Services
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userRoles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            foreach (var role in userRoles)
            {
                if (UserRoles.Permissions.TryGetValue(role, out var permissions))
                {
                    if (permissions.Contains(requirement.Permission))
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}