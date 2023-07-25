import MUTATION_CLIENT_TOKEN_REQUEST from "../graphql/Client/GetToken.gql";
import MUTATION_CLIENT_SAVE_SESSION from "../graphql/Client/SaveSession.gql"
import apollo from "../apollo";

export const getClientCredentialsToken = async RequestClientCredentialsTokenInput => {
  return await apollo.mutate({
    mutation: MUTATION_CLIENT_TOKEN_REQUEST,
    variables: { input: RequestClientCredentialsTokenInput }
  });
};

export const saveSession = async SessionInput => {
  return await apollo.mutate({
    mutation: MUTATION_CLIENT_SAVE_SESSION,
    variables: { input: SessionInput }
  });
};
