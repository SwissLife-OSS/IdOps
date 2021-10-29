import MUTATION_API_RESOURCE_SAVE from "../graphql/ApiResource/Save.gql";
import MUTATION_API_RESOURCE_SECRET_ADD from "../graphql/ApiResource/AddSecret.gql";
import MUTATION_API_RESOURCE_SECRET_REMOVE from "../graphql/ApiResource/RemoveSecret.gql";
import MUTATION_API_SCOPE_SAVE from "../graphql/ApiScope/Save.gql";
import MUTATION_CLIENT_CREATE from "../graphql/Client/Create.gql";
import MUTATION_CLIENT_SECRET_ADD from "../graphql/Client/AddSecret.gql";
import MUTATION_CLIENT_SECRET_REMOVE from "../graphql/Client/RemoveSecret.gql";
import MUTATION_CLIENT_UPDATE from "../graphql/Client/Update.gql";
import MUTATION_GRANT_TYPE_SAVE from "../graphql/GrantType/Save.gql";
import MUTATION_IDENTITY_RESOURCE_SAVE from "../graphql/IdentityResource/Save.gql";
import QUERY_API_RESOURCE_GET_ALL from "../graphql/ApiResource/GetAll.gql";
import QUERY_API_SCOPE_GET_ALL from "../graphql/ApiScope/GetAll.gql";
import QUERY_AUDIT_SEARCH from "../graphql/ResourceAudit/Search.gql";
import QUERY_CLIENT_BY_ID from "../graphql/Client/GetById.gql";
import QUERY_CLIENT_DEPENDENCIES from "../graphql/Dependency/GetAllDependencies.gql";
import QUERY_CLIENT_SEARCH from "../graphql/Client/Search.gql";
import QUERY_CLIENT_SEARCHUNMAPPED from "../graphql/Client/SearchUnMapped.gql";
import MUTATION_PERSONAL_ACCESS_TOKEN_CREATE from "../graphql/PersonalAccessToken/Create.gql";
import MUTATION_PERSONAL_ACCESS_TOKEN_UPDATE from "../graphql/PersonalAccessToken/Update.gql";
import QUERY_PERSONAL_ACCESS_TOKEN_SEARCH from "../graphql/PersonalAccessToken/Search.gql";
import QUERY_PERSONAL_ACCESS_TOKEN_BY_ID from "../graphql/PersonalAccessToken/GetById.gql";
import MUTATION_PERSONAL_ACCESS_TOKEN_SECRET_ADD from "../graphql/PersonalAccessToken/AddSecret.gql";
import MUTATION_PERSONAL_ACCESS_TOKEN_SECRET_REMOVE from "../graphql/PersonalAccessToken/RemoveSecret.gql";
import QUERY_IDENTITY_RESOURCE_GET_ALL from "../graphql/IdentityResource/GetAll.gql";
import QUERY_RESOURCE_DATA from "../graphql/ResourceData.gql";
import apollo from "../apollo";

export const getResourceData = async () => {
  return await apollo.query({
    query: QUERY_RESOURCE_DATA,
    variables: {}
  });
};

export const getApiResources = async input => {
  return await apollo.query({
    query: QUERY_API_RESOURCE_GET_ALL,
    variables: { input }
  });
};

export const saveApiResource = async input => {
  return await apollo.mutate({
    mutation: MUTATION_API_RESOURCE_SAVE,
    variables: {
      input
    }
  });
};

export const addApiSecret = async input => {
  return await apollo.mutate({
    mutation: MUTATION_API_RESOURCE_SECRET_ADD,
    variables: {
      input
    }
  });
};

export const removeApiSecret = async input => {
  return await apollo.mutate({
    mutation: MUTATION_API_RESOURCE_SECRET_REMOVE,
    variables: {
      input
    }
  });
};

export const getIdentityResources = async input => {
  return await apollo.query({
    query: QUERY_IDENTITY_RESOURCE_GET_ALL,
    variables: { input }
  });
};

export const saveIdentityResource = async input => {
  return await apollo.mutate({
    mutation: MUTATION_IDENTITY_RESOURCE_SAVE,
    variables: {
      input
    }
  });
};

export const saveGrantType = async input => {
  return await apollo.mutate({
    mutation: MUTATION_GRANT_TYPE_SAVE,
    variables: {
      input
    }
  });
};

export const getApiScopes = async input => {
  return await apollo.query({
    query: QUERY_API_SCOPE_GET_ALL,
    variables: { input }
  });
};

export const saveApiScope = async input => {
  return await apollo.mutate({
    mutation: MUTATION_API_SCOPE_SAVE,
    variables: {
      input
    }
  });
};

export const createClient = async input => {
  return await apollo.mutate({
    mutation: MUTATION_CLIENT_CREATE,
    variables: {
      input
    }
  });
};

export const updataClient = async input => {
  return await apollo.mutate({
    mutation: MUTATION_CLIENT_UPDATE,
    variables: {
      input
    }
  });
};

export const addClientSecret = async input => {
  return await apollo.mutate({
    mutation: MUTATION_CLIENT_SECRET_ADD,
    variables: {
      input
    }
  });
};

export const removeClientSecret = async input => {
  return await apollo.mutate({
    mutation: MUTATION_CLIENT_SECRET_REMOVE,
    variables: {
      input
    }
  });
};

export const getClientById = async id => {
  return await apollo.query({
    query: QUERY_CLIENT_BY_ID,
    variables: { id }
  });
};

export const searchClients = async input => {
  return await apollo.query({
    query: QUERY_CLIENT_SEARCH,
    variables: { input }
  });
};

export const searchUnMappedClients = async input => {
  return await apollo.query({
    query: QUERY_CLIENT_SEARCHUNMAPPED,
    variables: { input }
  });
};

export const getAllDependencies = async input => {
  return await apollo.query({
    query: QUERY_CLIENT_DEPENDENCIES,
    variables: { input }
  });
};

export const searchAudits = async input => {
  return await apollo.query({
    query: QUERY_AUDIT_SEARCH,
    variables: { input }
  });
};

export const createPersonalAccessToken = async input => {
  return await apollo.mutate({
    mutation: MUTATION_PERSONAL_ACCESS_TOKEN_CREATE,
    variables: {
      input
    }
  });
};

export const updataPersonalAccessToken = async input => {
  return await apollo.mutate({
    mutation: MUTATION_PERSONAL_ACCESS_TOKEN_UPDATE,
    variables: {
      input
    }
  });
};

export const getPersonalAccessTokenById = async id => {
  return await apollo.query({
    query: QUERY_PERSONAL_ACCESS_TOKEN_BY_ID,
    variables: { id }
  });
};

export const searchPersonalAccessTokens = async input => {
  return await apollo.query({
    query: QUERY_PERSONAL_ACCESS_TOKEN_SEARCH,
    variables: { input }
  });
};

export const addPersonalAccessTokenSecret = async input => {
  return await apollo.mutate({
    mutation: MUTATION_PERSONAL_ACCESS_TOKEN_SECRET_ADD,
    variables: {
      input
    }
  });
};

export const removePersonalAccessTokenSecret = async input => {
  return await apollo.mutate({
    mutation: MUTATION_PERSONAL_ACCESS_TOKEN_SECRET_REMOVE,
    variables: {
      input
    }
  });
};
