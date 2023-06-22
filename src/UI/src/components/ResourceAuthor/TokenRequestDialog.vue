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
    <v-card height="300" v-else key="default">
      <v-card-title class="headline">Token</v-card-title>
      <v-card-text>Grant_Type "{{ grantType }}" not available</v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" text @click.native="close">Close</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import { getClientCredentialsToken } from "../../services/tokenFlowService";
import { getIdentityServerGroupByTenant } from "../../services/systemService";
import { getAllIdentityServer } from "../../services/systemService";
import { getPublishedResources } from "../../services/publishingService";

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
      const authority = await this.getAuthorityUrl();

      const requestTokenInput = {
        authority: authority,
        clientId: clientId,
        secretId: secretId,
        grantType: "client_credentials",
        saveTokens: false
      };
      const result = (await getClientCredentialsToken(requestTokenInput)).data.requestToken.result;
      if(result.isSuccess){
        this.clientCredentialsToken = result.accessToken.token;
      } else {
        this.close();
        alert("An error occured: " + result.errorMessage);
      }
    },
    getLastSavedSecretId() {
      const secret = this.client.clientSecrets.findLast(secret => secret.encryptedSecret !== null);
      return secret.id;
    },
    async getAuthorityUrl() {
      const environmentId = await this.getLastPublishedEnvironmentId();
      const serverGroups = (await getIdentityServerGroupByTenant(this.client.tenant)).data;
      const serverGroupId = serverGroups.identityServerGroupByTenant.id
      const authorities = (await getAllIdentityServer()).data.identityServers;
      const result = authorities.find(authority => authority.groupId === serverGroupId && authority.environmentId === environmentId).url;

      return result;
    },
    async getLastPublishedEnvironmentId() {
      const publishedResources = (await getPublishedResources()).data;

      const lastPublishedResource = publishedResources.publishedResouces
        .findLast(publishedResource => publishedResource.type === "Client").environments
        .findLast(environment => environment.state === "Latest");
      return lastPublishedResource.environment.id;
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
