using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duende.IdentityServer.Services;
using IdOps.IdentityServer.Events;
using IdOps.IdentityServer.Store;

namespace IdOps.IdentityServer.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IEventService _eventService;

        public CorsPolicyService(
            IClientRepository clientRepository,
            IEventService eventService)
        {
            _clientRepository = clientRepository;
            _eventService = eventService;
        }

        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            HashSet<string> allowedOrigins = await GetAllClientOrigins();
            bool allowed = allowedOrigins.Contains(origin);

            if (!allowed)
            {
                await _eventService.RaiseAsync(
                    new CorsOriginNotAllowedEvent(origin, allowedOrigins));
            }

            return allowed;
        }

        private async Task<HashSet<string>> GetAllClientOrigins()
        {
            HashSet<string>? origins = await _clientRepository
                .GetAllClientOrigins();

            HashSet<string> redirectUOrigins =
                    await _clientRepository.GetAllClientRedirectUriAsync();

            return RemovePathFromUrl(origins.Concat(redirectUOrigins));
        }

        private HashSet<string> RemovePathFromUrl(IEnumerable<string> urls)
        {
            var urlsWithoutPath = new HashSet<string>();

            foreach (var url in urls)
            {
                try
                {
                    if (url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var uri = new Uri(url);
                        var port = !uri.IsDefaultPort ?
                             ":" + uri.Port.ToString(CultureInfo.InvariantCulture) : "";

                        urlsWithoutPath.Add(
                            $"{uri.Scheme}://{uri.Host}{port}");
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }

            return urlsWithoutPath;
        }
    }
}
