import apollo from "../apollo";

import QUERY_ME from "../graphql/User/GetMe.gql";

export const getMe = async () => {
    return await apollo.query({
        query: QUERY_ME,
        variables: {}
    });
};
