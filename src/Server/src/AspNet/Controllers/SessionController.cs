using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdOps.Server.AspNet
{
    [Route("api/session")]
    public class SessionController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IUserContextAccessor _userContextAccessor;

        public SessionController(IUserContextAccessor userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }

        [AllowAnonymous]
        [Route("")]
        public IActionResult Check()
        {
            return Ok(new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated
            });
        }

        [AllowAnonymous]
        [Route("auth")]
        public async Task<IActionResult> AuthenticateAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            else
            {
                return Challenge();
            }
        }

        [HttpGet]
        [Route("diag")]
        public async Task<IActionResult> DiagnosticAsync(CancellationToken cancellationToken)
        {
            if (_userContextAccessor.Context is null)
            {
                return BadRequest();
            }

            IUserContext userContext = _userContextAccessor.Context;

            var result = new
            {
                user = userContext,
                claims = User.Claims.Select(c => new
                {
                    c.Type,
                    c.Value
                }),
                tenants = await userContext.GetTenantsAsync(cancellationToken)
            };

            return Ok(result);
        }

        [AllowAnonymous]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("oidc");

            return Redirect("/loggedout");
        }
    }
}
