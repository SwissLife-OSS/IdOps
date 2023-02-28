using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IdOps.AspNet.Controllers
{
    [Route("api/discovery")]
    public class IdentityServerDiscoveryController : Controller
    {
        private readonly IIdentityServerService _identityServerService;

        public IdentityServerDiscoveryController(IIdentityServerService identityServerService)
        {
            _identityServerService = identityServerService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> IndexAsync(Guid id, CancellationToken cancellationToken)
        {
            var json = await _identityServerService.GetDiscoveryDocumentAsync(
                id,
                cancellationToken);

            return Content(json, "application/json");
        }
    }
}
