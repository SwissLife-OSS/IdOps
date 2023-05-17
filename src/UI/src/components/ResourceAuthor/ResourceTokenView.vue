<template>
  <v-card flat>
    <div>
      <v-switch label="Enable Token generation" v-model="allowTokenGeneration" :disabled="!hasSavedSecrets"></v-switch>
      <v-switch label="Log allowTokenGeneration" @click="log({ allowTokenGeneration })"></v-switch>
    </div>
    <v-toolbar
     elevation="0"
      color="grey lighten-6"
      height="38">
      <v-toolbar-title>Available Tokenflows</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-icon @click="log({ grantTypes })">mdi-refresh</v-icon>
    </v-toolbar>
    <v-card-text>
      <v-data-table
      height="300"
      :headers="headers"
      :items="listItems"
      item-key="id"
      hide-default-footer>
        <template v-slot:item.id="{ item }">
          <td>{{ item.id }}</td>
        </template>
        <template v-slot:item.action="{ item }">
          <v-btn :disabled="!allowTokenGeneration">
            {{ item.action }}
          </v-btn>
        </template>
      </v-data-table>
    </v-card-text>
  </v-card>
</template>





<script>
import { getClientById } from "../../services/idResourceService";

export default {
  props: ["id"],
  data() {
    return {
      allowTokenGeneration: false,
      client: {},
      headers: [
        {
          text: "Grant Type",
          width: 100,
          align: "center",
          value: "id",
          sortable: false
        },
        {
          text: "Generate",
          align: "center",
          value: "action",
          sortable: false
        }
      ]
    }
  },
  computed: {
    secrets: function () {
      return this.client.clientSecrets
    },
    hasSavedSecrets: function () {
      //undefined if client hasn't been set because of async
      if (!this.secrets) return false;
      return this.secrets.some(secret => secret.encryptedSecret !== null)
    },
    grantTypes: function () {
      //undefined if client hasn't been set because of async
      if (!this.client.allowedGrantTypes) return [];
      return this.client.allowedGrantTypes
    },
    listItems: function () {
      console.log('grantTypes:', this.grantTypes);
      const items = this.grantTypes.map(grantType => ({
        id: grantType,
        action: "Generate " + grantType + " Token"
      }));
      console.log('listItems:', items);
      return items;
    }
  },
  methods: {
    async setClient() {
      this.loading = true;
      const result = await getClientById(this.id);
      this.loading = false;
      const { client } = result.data;
      this.client = Object.assign({}, client);

      delete this.client.__typename;
    },
    log(message) {
      console.log(message)
    }

  },
  watch: {
    id: {
      immediate: true,
      handler: function () {
        this.setClient();
      },
    }
  }

};
</script>

<style></style>
