using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Templates
{
    public interface IClientTemplateService
    {
        Task<(Client client, string? secret)> CreateClientAsync(
            ClientTemplate template,
            Model.Environment environment,
            Application application,
            CancellationToken cancellationToken);

        Task<(Client client, string? secret)> CreateClientAsync(
            Guid templateId,
            Model.Environment environment,
            Application application,
            CancellationToken cancellationToken);

        Task<IEnumerable<ClientTemplate>> GetAllAsync(
            CancellationToken cancellationToken);

        Task<ClientTemplate> GetByIdAsync(
            Guid id, CancellationToken cancellationToken);

        Task<ClientTemplate> SaveClientTemplate(SaveClientTemplateRequest input,
            CancellationToken cancellationToken);

        Task<Client> UpdateClientAsync(Client client, Application application,
            CancellationToken cancellationToken);
    }

    public interface ITemplateRenderer
    {
        string Render(string template, object data);
    }
}
