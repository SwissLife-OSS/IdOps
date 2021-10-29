import apollo from "../apollo";

import QUERY_IDENTITY_SERVER_EVENT_SEARCH from "../graphql/Insights/SearchIdentityServerEvents.gql";

export const searchIdentityServerEvents = async (input) => {
    return await apollo.query({
        query: QUERY_IDENTITY_SERVER_EVENT_SEARCH,
        variables: {
            input
        }
    });
};
