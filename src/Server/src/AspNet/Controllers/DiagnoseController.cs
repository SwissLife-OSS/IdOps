using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdOps.Security;
using Microsoft.AspNetCore.Mvc;

namespace IdOps.Server.AspNet.Controllers
{
    [Route("api/diagnose")]
    public class DiagnoseController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IUserContextAccessor _userContextAccessor;

        public DiagnoseController(IUserContextAccessor userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }


    }
}
