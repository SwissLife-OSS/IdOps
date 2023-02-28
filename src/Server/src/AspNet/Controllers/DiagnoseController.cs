
using IdOps.Security;
using Microsoft.AspNetCore.Mvc;

namespace IdOps.Server.AspNet.Controllers
{
    [Route("api/diagnose")]
    public class DiagnoseController : Controller
    {
        private readonly IUserContextAccessor _userContextAccessor;

        public DiagnoseController(IUserContextAccessor userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }


    }
}
