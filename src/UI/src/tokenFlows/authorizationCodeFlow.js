export async function authorizationCodeFlow(authority, scope, clientClientId, redirectUri){
  const request = await createAuthorizationRequest(authority, scope, clientClientId, redirectUri);
  return request;
}

async function createAuthorizationRequest(authority, scope, clientClientId, redirectUri){
  const url = new URL(authority+"/connect/authorize");
  url.searchParams.append("response_type", "code")
  url.searchParams.append("scope", scope);
  url.searchParams.append("client_id",clientClientId);
  url.searchParams.append("state", "foo");
  url.searchParams.append("redirect_uri", redirectUri);
  url.searchParams.append("code_challenge", await createCodeChallenge());
  url.searchParams.append("code_challenge_method", "S256");

  return url.href;

}

async function createCodeChallenge() {
  var hashed = await sha256(createCodeVerifier());
  var base64encoded = base64UrlEncode(hashed);
  return base64encoded;
}

function createCodeVerifier() {
  var array = new Uint32Array(56/2);
  window.crypto.getRandomValues(array);
  return Array.from(array, decToHex).join('');
}

function decToHex(dec) {
  return ('0' + dec.toString(16)).substr(-2)
}

function sha256(plainText) {
  const encoder = new TextEncoder();
  const data = encoder.encode(plainText);
  return window.crypto.subtle.digest('SHA-256', data);
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
