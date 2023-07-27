import { getApiScopes } from "../services/idResourceService";
import { saveSession } from "../services/tokenFlowService";

export async function authorizationCodeFlow(authority, client, redirectUri, socket) {
  const clientClientId = client.clientId;
  const scope = await getClientApiScopes(client);
  const state = createCodeVerifier();
  const codeVerifier = createCodeVerifier();

  console.log(socket);

  saveCurrentSession(client, state, codeVerifier, socket.connectionId);

  const request = await createAuthorizationRequest(
    authority,
    scope,
    clientClientId,
    redirectUri,
    state,
    codeVerifier
  );
  return request;
}

async function createAuthorizationRequest(
  authority,
  scope,
  clientClientId,
  redirectUri,
  state,
  codeVerifier
) {
  const url = new URL(authority + "/connect/authorize");
  url.searchParams.append("response_type", "code");
  url.searchParams.append("scope", scope);
  url.searchParams.append("client_id", clientClientId);
  url.searchParams.append("state", state);
  url.searchParams.append("redirect_uri", redirectUri);
  url.searchParams.append(
    "code_challenge",
    await createCodeChallenge(codeVerifier)
  );
  url.searchParams.append("code_challenge_method", "S256");
  url.searchParams.append("response_mode", "form_post");
  return url.href;
}

async function createCodeChallenge(verifier) {
  var hashed = await sha256(verifier);
  var base64encoded = base64UrlEncode(hashed);
  return base64encoded;
}

function createCodeVerifier() {
  var array = new Uint32Array(56 / 2);
  window.crypto.getRandomValues(array);
  return Array.from(array, decToHex).join("");
}

function decToHex(dec) {
  return ("0" + dec.toString(16)).substr(-2);
}

function sha256(plainText) {
  const encoder = new TextEncoder();
  const data = encoder.encode(plainText);
  return window.crypto.subtle.digest("SHA-256", data);
}

function base64UrlEncode(a) {
  var str = "";
  var bytes = new Uint8Array(a);
  var len = bytes.byteLength;
  for (var i = 0; i < len; i++) {
    str += String.fromCharCode(bytes[i]);
  }
  return btoa(str)
    .replace(/\+/g, "-")
    .replace(/\//g, "_")
    .replace(/=+$/, "");
}

async function getClientApiScopes(client) {
  const scopes = (await getApiScopes()).data.apiScopes;
  const scopeIds = client.apiScopes;
  const result = [];

  scopeIds.forEach(id => {
    result.push(scopes.find(scope => scope.id === id).name);
  });

  return result.join(" ");
}

async function saveCurrentSession(client, state, codeVerifier, callback) {
  const session = {
    id: state,
    clientId: client.id,
    secretId: getLastSavedSecretId(client),
    codeVerifier: codeVerifier,
    callbackUri: callback
  };
  console.log(session);

  await saveSession(session);
}

function getLastSavedSecretId(client) {
  const secret = client.clientSecrets.findLast(
    secret => secret.encryptedSecret !== null
  );
  return secret.id;
}
