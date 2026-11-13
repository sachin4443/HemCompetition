using HemCompetition.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HemCompetition.API.Security;

/// <summary>
/// Requires an admin token whose role currently holds the permission. The role is read from the
/// database (briefly cached), so revoking a permission or deactivating an admin takes effect
/// without waiting for their token to expire.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequirePermissionAttribute : TypeFilterAttribute
{
    public string Permission { get; }

    public RequirePermissionAttribute(string permission) : base(typeof(PermissionFilter))
    {
        Permission = permission;
        Arguments = [permission];
    }

    private sealed class PermissionFilter(AdminAccessService access, string permission) : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity?.IsAuthenticated != true || !user.IsAdmin())
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Admin sign-in required." });
                return;
            }
            var info = await access.GetAsync(user.AdminId());
            if (info == null || !info.IsActive)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "This admin account is not active." });
                return;
            }
            if (info.MustChangePassword)
            {
                context.Result = new ObjectResult(new { message = "Change your password to continue.", code = "PASSWORD_CHANGE_REQUIRED" }) { StatusCode = 403 };
                return;
            }
            if (!info.Has(permission))
                context.Result = new ObjectResult(new { message = "You do not have permission to do this.", permission }) { StatusCode = 403 };
        }
    }
}
