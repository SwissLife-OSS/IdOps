import MUTATION_APPLICATION_CREATE from "../graphql/Application/Create.gql";
import MUTATION_APPLICATION_UPDATE from "../graphql/Application/Update.gql";
import MUTATION_CLIENT_ADD from "../graphql/Application/AddClient.gql";
import MUTATION_CLIENT_REMOVE from "../graphql/Application/RemoveClient.gql";
import MUTATION_ENVIRONMENT_ADDTOAPPLICATION from "../graphql/Application/AddEnvironment.gql";
import MUTATION_CLIENT_TEMPLATE_SAVE from "../graphql/ClientTemplate/Save.gql";
import QUERY_APPLICATION_BY_ID from "../graphql/Application/GetById.gql";
import QUERY_APPLICATION_SEARCH from "../graphql/Application/Search.gql";
import QUERY_CLIENT_TEMPLATE_ALL from "../graphql/ClientTemplate/All.gql";
import QUERY_CLIENT_TEMPLATE_BY_ID from "../graphql/ClientTemplate/GetById.gql";
import QUERY_SECRETS from "../graphql/ClientTemplate/Secrets.gql";
import apollo from "../apollo";

export const getApplicationById = async id => {
    return await apollo.query({
        query: QUERY_APPLICATION_BY_ID,
        variables: { id }
    });
};

export const searchApplications = async input => {
    return await apollo.query({
        query: QUERY_APPLICATION_SEARCH,
        variables: { input }
    });
};

export const createApplication = async input => {
    return await apollo.mutate({
        mutation: MUTATION_APPLICATION_CREATE,
        variables: {
            input
        }
    });
};

export const updateApplication = async input => {
    return await apollo.mutate({
        mutation: MUTATION_APPLICATION_UPDATE,
        variables: {
            input
        }
    });
};

export const getClientTemplates = async () => {
    return await apollo.query({
        query: QUERY_CLIENT_TEMPLATE_ALL,
        variables: {}
    });
};

export const removeClientFromApplication = async input => {
    return await apollo.mutate({
        mutation: MUTATION_CLIENT_REMOVE,
        variables: {
            input
        }
    });
};

export const addClientToApplication = async input => {
    return await apollo.mutate({
        mutation: MUTATION_CLIENT_ADD,
        variables: {
            input
        }
    });
};

export const getClientTemplateById = async id => {
    return await apollo.query({
      query: QUERY_CLIENT_TEMPLATE_BY_ID,
      variables: { id }
    });
  };

  export const saveClientTemplate = async input => {
    return await apollo.mutate({
      mutation: MUTATION_CLIENT_TEMPLATE_SAVE,
      variables: {
        input
      }
    });
  };

  export const getSecrets = async () => {
    return await apollo.query({
        query: QUERY_SECRETS,
        variables: {}
    });
};


export const addEnvironmentToApplication= async input => {
    return await apollo.mutate({
      mutation: MUTATION_ENVIRONMENT_ADDTOAPPLICATION,
      variables: { input }
    });
  };
