<template>
  <v-dialog v-model="activator" width="600">
    <v-card min-height="300" v-if="grantType === 'client_credentials'" key="client_credentials">
      <v-card-title class="headline">Token</v-card-title>
      <v-card-text>{{ clientCredentialsToken }}</v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" text @click.native="close">Close</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import { getClientCredentialsToken } from "../../services/tokenFlowService";

export default {
  props: ["client", "grantType", "activator"],
  data() {
    return {
      clientCredentialsToken: ""
    };
  },
  methods: {
    close() {
      this.$emit("update:activator", false);
    },
    async clientCredentialsFlow() {

      const clientId = this.client.id;
      const secretId = this.getLastSavedSecretId();
      console.log(secretId);

      const tokenRequestInput = {
        authority: "http://localhost:5001",
        clientId: clientId,
        requestId: null,
        secretId: secretId,
        parameters: [],
        saveTokens: false
      };
      const result = await getClientCredentialsToken(tokenRequestInput);
      this.clientCredentialsToken = result.data.requestToken.result.accessToken.token;
    },
    getLastSavedSecretId() {
      const secret = this.client.clientSecrets.findLast(secret => secret.encryptedSecret !== null);
      console.log(secret);
      return secret.id;
    }
  },
  watch: {
    grantType: {
      immediate: true,
      handler() {
        this.clientCredentialsFlow();
      }
    }
  }
};
</script>

<style></style>
