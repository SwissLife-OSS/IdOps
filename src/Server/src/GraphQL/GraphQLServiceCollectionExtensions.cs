using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using IdOps.Authorization;
using IdOps.GraphQL.DataLoaders;
using IdOps.GraphQL.GraphQL.Serialization;
using IdOps.GraphQL.Hashing;
using IdOps.GraphQL.Publish;
using IdOps.Model;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps.GraphQL
{
    public static class GrapQLServiceCollectionExtensions
    {
        public static IIdOpsServerBuilder AddGraphQLServer(this IIdOpsServerBuilder builder)
        {
            builder
                .Services
                .AddGraphQLServer()
                .AddGraphQLTypes();

            builder
                .Services
                .AddHttpResultSerializer<ForbiddenHttpResultSerializer>();

            return builder;
        }

        public static IRequestExecutorBuilder AddGraphQLTypes(this IRequestExecutorBuilder builder)
        {
            builder
                .AddQueries()
                .AddMutations()
                .AddTypes()
                .AddErrors()
                .RenameRequests()
                .AddDataLoaders()
                .AddType(new UuidType("Uuid", defaultFormat: 'N'))
                .AddAuthorization();

            return builder;
        }

        private static IRequestExecutorBuilder AddQueries(this IRequestExecutorBuilder builder)
        {
            builder
                .AddQueryType(d => d.Name(RootTypes.Query)
                    .Authorize(AuthorizationPolicies.Names.ApiAccess))
                .AddType<ResourceInterfaceType>()
                .AddType<ApiScopeQueries>()
                .AddType<ClientQueries>()
                .AddType<ApplicationQueries>()
                .AddType<TenantQueries>()
                .AddType<EnvironmentQueries>()
                .AddType<GrantTypeQueries>()
                .AddType<PublishQueries>()
                .AddType<InsightsQueries>()
                .AddType<IdentityResourceQueries>()
                .AddType<ResourceAuditQueries>()
                .AddType<UserQueries>()
                .AddType<IdentityServerQueries>()
                .AddType<ClientTemplateQueries>()
                .AddType<UserClaimRuleQueries>()
                .AddType<PersonalAccessTokenQueries>()
                .AddType<ApiResourceQueries>()
                .AddType<HashAlgorithmQueryExtensions>()
                .AddType<ApprovalQueries>()
                .AddType<DependencyQueries>();

            return builder;
        }

        private static IRequestExecutorBuilder AddMutations(this IRequestExecutorBuilder builder)
        {
            builder
                .AddMutationType(d => d.Name(RootTypes.Mutation)
                    .Authorize(AuthorizationPolicies.Names.ApiAccess))
                .AddType<ApiScopeMutations>()
                .AddType<IdentityResourceMutations>()
                .AddType<ClientMutations>()
                .AddType<ApplicationMutations>()
                .AddType<TenantMutations>()
                .AddType<PublishMutations>()
                .AddType<PersonalAccessTokensMutations>()
                .AddType<GrantTypeMutations>()
                .AddType<EnvironmentMutations>()
                .AddType<IdentityServerMutations>()
                .AddType<ClientTemplateMutations>()
                .AddType<UserClaimRuleMutations>()
                .AddType<ApprovalMutations>()
                .AddType<ApiResourceMutations>();

            return builder;
        }

        private static IRequestExecutorBuilder AddTypes(this IRequestExecutorBuilder builder)
        {
            builder
                .AddType<ApiResourceType>()
                .AddType<EnumString>()
                .AddType<PublishedResourceEnvironmentType>()
                .AddType<ResourceApprovalEnvironmentType>()
                //.AddType<ResourceType>()
                .AddType<UserType>()
                .AddType<ClientType>()
                .AddType<PersonalAccessTokenType>()
                .AddType<ApplicationType>()
                .AddType<IdentityServerType>()
                .AddType<ObjectType<IdentityServerGroup>>()
                .AddType<IdentityResourceType>()
                .AddType<ApiScopeType>()
                .AddType<IdentityServerEventType>()
                .AddType<ClientTemplateType>()
                .AddType<UserClaimRuleType>()
                .AddType<ClientTemplateSecretType>();

            return builder;
        }

        private static IRequestExecutorBuilder AddErrors(this IRequestExecutorBuilder builder)
        {
            builder
                .AddInterfaceType<IError>(x => x.Name("Error"))
                .AddInterfaceType<ICreatePersonalAccessTokenError>(
                    x => x.Name("CreatePersonalAccessTokenError"))
                .AddType<ExpiresAtInvalid>()
                .AddType<HashAlgorithmNotFound>();

            return builder;
        }

        private static IRequestExecutorBuilder AddDataLoaders(this IRequestExecutorBuilder builder)
        {
            builder.AddDataLoader<ApiScopeByIdDataLoader>();
            builder.AddDataLoader<EnvironmentByIdDataLoader>();
            builder.AddDataLoader<IdentityServerGroupByIdDataLoader>();
            builder.AddDataLoader<ClientTemplateByIdDataLoader>();
            builder.AddDataLoader<TenantByIdDataLoader>();

            return builder;
        }

        private static IRequestExecutorBuilder RenameRequests(this IRequestExecutorBuilder builder)
        {
            builder.RenameRequestToInput<SaveApiResourceRequest>();
            builder.RenameRequestToInput<SaveApiScopeRequest>();
            builder.RenameRequestToInput<CreateClientRequest>();
            builder.RenameRequestToInput<CreateApplicationRequest>();
            builder.RenameRequestToInput<UpdateApplicationRequest>();
            builder.RenameRequestToInput<RemoveClientRequest>();
            builder.RenameRequestToInput<AddClientRequest>();
            builder.RenameRequestToInput<UpdateClientRequest>();
            builder.RenameRequestToInput<AddClientSecretRequest>();
            builder.RenameRequestToInput<RemoveClientSecretRequest>();
            builder.RenameRequestToInput<AddApiSecretRequest>();
            builder.RenameRequestToInput<RemoveApiSecretRequest>();
            builder.RenameRequestToInput<SearchClientsRequest>();
            builder.RenameRequestToInput<SearchApplicationsRequest>();
            builder.RenameRequestToInput<SaveTenantRequest>();
            builder.RenameRequestToInput<SaveEnvironmentRequest>();
            builder.RenameRequestToInput<SaveIdentityServerRequest>();
            builder.RenameRequestToInput<SaveIdentityServerGroupRequest>();
            builder.RenameRequestToInput<SaveGrantTypeRequest>();
            builder.RenameRequestToInput<SearchResourceAuditRequest>();
            builder.RenameRequestToInput<PublishedResourcesRequest>();
            builder.RenameRequestToInput<SaveIdentityResourceRequest>();
            builder.RenameRequestToInput<PublishResourceRequest>();
            builder.RenameRequestToInput<ApproveResourcesRequest>();
            builder.RenameRequestToInput<ResourceApprovalRequest>();
            builder.RenameRequestToInput<SearchResourceApprovalLogsRequest>();
            builder.RenameRequestToInput<SearchIdentityServerEventsRequest>();
            builder.RenameRequestToInput<AddEnvironmentToApplicationRequest>();
            builder.RenameRequestToInput<SaveUserClaimRuleRequest>();
            builder.RenameRequestToInput<SaveClientTemplateRequest>();
            builder.RenameRequestToInput<GetDependenciesRequest>();
            builder.RenameRequestToInput<SearchPersonalAccessTokensRequest>();
            builder.RenameRequestToInput<UpdatePersonalAccessTokenRequest>();
            builder.RenameRequestToInput<CreatePersonalAccessTokenRequest>();
            builder.RenameRequestToInput<AddSecretPersonalAccessTokenRequest>();
            builder.RenameRequestToInput<RemoveSecretPersonalAccessTokenRequest>();

            return builder;
        }

        private static IRequestExecutorBuilder RenameRequestToInput<T>(
            this IRequestExecutorBuilder builder)
        {
            var name = typeof(T).Name.Replace("Request", "Input");
            builder.AddInputObjectType<T>(d => d.Name(name));

            return builder;
        }
    }
}
