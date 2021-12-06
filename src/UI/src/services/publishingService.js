import apollo from "../apollo";

import QUERY_RESOURCE_PUBLISHING_LOG from "../graphql/Publishing/Log.gql";
import QUERY_PUBLISHED_RESOURCES from "../graphql/Publishing/Get.gql";
import MUTATION_PUBLISH from "../graphql/Publishing/Publish.gql";

export const getPublishedResources = async (input) => {
    return await apollo.mutate({
        mutation: QUERY_PUBLISHED_RESOURCES,
        variables: {
            input
        }
    });
};

export const publishResources = async (input) => {
    return await apollo.mutate({
        mutation: MUTATION_PUBLISH,
        variables: {
            input
        }
    });
};

export const getResourcePublishingLog = async input => {
  return await apollo.query({
    query: QUERY_RESOURCE_PUBLISHING_LOG,
    variables: { input }
  });
};
