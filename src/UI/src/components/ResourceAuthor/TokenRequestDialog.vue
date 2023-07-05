<template>
  <v-dialog v-model="activator" width="600">
    <v-card min-height="300" v-if="grantType === 'client_credentials'" key="client_credentials">
      <v-card-title class="headline">Token</v-card-title>
      <v-card-text>{{ accessToken }}</v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" text @click.native="close">Close</v-btn>
      </v-card-actions>
    </v-card>
    <v-card min-height="300" v-else-if="grantType === 'authorization_code'" key="authorization_code">
      <v-card-title class="headline">Token</v-card-title>
      <v-card-text>{{ accessToken }}</v-card-text>
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
import { getIdentityServerGroupByTenant } from "../../services/systemService";
import { getAllIdentityServer } from "../../services/systemService";
import { getPublishedResources } from "../../services/publishingService";
import { authorizationCodeFlow } from "../../tokenFlows/authorizationCodeFlow";
import { clientCredentialsFlow } from "../../tokenFlows/clientCredentialsFlow";

export default {
  props: ["client", "grantType", "activator"],
  data() {
    return {
      accessToken: ""
    };
  },
  methods: {
    close() {
      this.$emit("update:activator", false);
    },
    async startClientCredentialsFlow() {
      const authority = await this.getAuthorityUrl();

      const result = (await clientCredentialsFlow(authority, this.client)).data.requestToken.result;

      if(result.isSuccess){
        this.accessToken = result.accessToken.token;
      } else {
        this.close();
        alert("An error occured: " + result.errorMessage);
      }
    },
    async startAuthorizationCodeFlow(){
      const authority = await this.getAuthorityUrl();
      const redirect_uri = "http://localhost:5000/clients/callback";

      console.log(await authorizationCodeFlow(authority, this.client, redirect_uri))
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
    },
    selectFlow(grantType){
      switch(grantType){
        case "client_credentials":
          this.startClientCredentialsFlow();
          break;
        case "authorization_code":
          this.startAuthorizationCodeFlow();
          break;
        default:
          console.log(grantType);
      }
    }
  },
  watch: {
    grantType: {
      immediate: true,
      handler() {
        this.selectFlow(this.grantType);
      }
    }
  }
};
</script>

<style></style>
