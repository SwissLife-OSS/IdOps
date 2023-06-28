import { getClientCredentialsToken } from "../services/tokenFlowService";

export async function clientCredentialsFlow(authority, client){
  const requestTokenInput = {
    authority: authority,
    clientId: client.id,
    secretId: getLastSavedSecretId(client),
    grantType: "client_credentials",
    saveTokens: false

  };

  const result = getClientCredentialsToken(requestTokenInput);
  return result;
}

function getLastSavedSecretId(client) {
  const secret = client.clientSecrets.findLast(secret => secret.encryptedSecret !== null);
  return secret.id;
}
