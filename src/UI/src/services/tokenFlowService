import MUTATION_CLIENT_TOKEN_REQUEST from "../graphql/Client/GetToken.gql";
import apollo from "../apollo";

export const getClientCredentialsToken = async requestTokenInput => {
  return await apollo.mutate({
    mutation: MUTATION_CLIENT_TOKEN_REQUEST,
    variables: { input: requestTokenInput }
  });
};
