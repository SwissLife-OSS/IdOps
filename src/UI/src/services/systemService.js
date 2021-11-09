import MUTATION_ENVIRONMENT_SAVE from "../graphql/Environment/Save.gql";
import MUTATION_IDENTITY_SERVER_SAVE from "../graphql/IdentityServer/Save.gql";
import MUTATION_IDENTITY_SERVER_GROUP_SAVE from "../graphql/IdentityServerGroup/Save.gql";
import MUTATION_TENANT_SAVE from "../graphql/Tenant/Save.gql";
import QUERY_IDENTITY_SERVER_GET_BYID from "../graphql/IdentityServer/GetById.gql";
import QUERY_SYSTEM_DATA from "../graphql/SystemData.gql";
import QUERY_TENANT_GET_ALL from "../graphql/Tenant/GetAll.gql";
import apollo from "../apollo";

export const getSystemData = async () => {
    return await apollo.query({
        query: QUERY_SYSTEM_DATA,
        variables: {}
    });
};

export const getAllTenants = async () => {
    return await apollo.query({
        query: QUERY_TENANT_GET_ALL,
        variables: {}
    });
};

export const saveTenant = async (input) => {
    return await apollo.mutate({
        mutation: MUTATION_TENANT_SAVE,
        variables: {
            input
        }
    });
};

export const saveEnvironment = async (input) => {
    return await apollo.mutate({
        mutation: MUTATION_ENVIRONMENT_SAVE,
        variables: {
            input
        }
    });
};

export const saveIdentityServer = async (input) => {
    return await apollo.mutate({
        mutation: MUTATION_IDENTITY_SERVER_SAVE,
        variables: {
            input
        }
    });
};

export const saveIdentityServerGroup = async (input) => {
  return await apollo.mutate({
      mutation: MUTATION_IDENTITY_SERVER_GROUP_SAVE,
      variables: {
          input
      }
  });
};

export const getIdentityServer = async (id) => {
    return await apollo.query({
        query: QUERY_IDENTITY_SERVER_GET_BYID,
        variables: { id }
    });
};
