import apollo from "../apollo";

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
