import QUERY_RULES_GET_ALL from "../graphql/UserClaimRule/GetAll.gql";
import QUERY_RULE_GET_BY_ID from "../graphql/UserClaimRule/GetById.gql";
import MUTATION_RULE_SAVE from "../graphql/UserClaimRule/Save.gql";
import apollo from "../apollo";

export const getUserClaimRules = async (input) => {
    return await apollo.query({
        query: QUERY_RULES_GET_ALL,
        variables: input
    });
};

export const getRuleById = async (id) => {
    return await apollo.query({
        query: QUERY_RULE_GET_BY_ID,
        variables: { id }
    });
};

export const saveUserClaimRule = async (input) => {
    return await apollo.mutate({
        mutation: MUTATION_RULE_SAVE,
        variables: input
    });
};