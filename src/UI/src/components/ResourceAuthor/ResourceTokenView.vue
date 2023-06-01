<template>
  <div>
    <v-card flat>
      <div>
        <v-switch label="Enable Token generation" v-model="client.allowTokenGeneration"
          :disabled="!hasSavedSecrets"></v-switch>
        <v-switch label="Log allowTokenGeneration" @click="log({ allowTokenGeneration })"></v-switch>
      </div>
      <v-toolbar elevation="0" color="grey lighten-6" height="38">
        <v-toolbar-title>Available Tokenflows</v-toolbar-title>
      </v-toolbar>
      <v-card-text>
        <v-data-table height="300" :headers="headers" :items="listItems" item-key="id" hide-default-footer>
          <template v-slot:item.id="{ item }">
            <td>{{ item.id }}</td>
          </template>
          <template v-slot:item.action="{ item }">
            <v-btn @click="startTokenFlow(item.id)" :disabled="!allowTokenGeneration">
              {{ item.action }}
            </v-btn>
          </template>
        </v-data-table>
      </v-card-text>
    </v-card>
    <TokenRequestDialog v-if="openTokenAlert === true" :client="client" :activator.sync="openTokenAlert"
      :grantType.sync="currentGrantType">
    </TokenRequestDialog>
  </div>
</template>





<script>
import TokenRequestDialog from "./TokenRequestDialog.vue";
import { mapActions } from "vuex";

export default {
  props: ["client"],
  components: {
    TokenRequestDialog
  },
  data() {
    return {
      openTokenAlert: false,
      currentGrantType: "none",
      secrets: false,
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
    };
  },
  computed: {
    allowTokenGeneration: function () {
      return this.client.allowTokenGeneration;
    },
    hasSavedSecrets: function () {
      return this.client.clientSecrets.some(secret => secret.encryptedSecret !== null);
    },
    grantTypes: function () {
      return this.client.allowedGrantTypes;
    },
    listItems: function () {
      return this.grantTypes.map(grantType => ({
        id: grantType,
        action: "Generate " + grantType + " Token"
      }));
    }
  },
  methods: {
    ...mapActions("idResource", [
      "updateClient",
    ]),
    async startTokenFlow(grantType) {
      this.currentGrantType = grantType;
      this.openTokenAlert = true;
    },
    log(obj) {
      console.log(obj)
    }
  },
  watch: {
    client: {
      immediate: true,
      handler: function () {
        this.client.clientSecrets
      }
    },
    allowTokenGeneration: {
      immediate: true,
      handler: async function () {
        const update = Object.assign({}, this.client);
        delete update.clientId;
        console.log("local update");
        console.log(update);
        const result = await this.updateClient(update)
        console.log("fetched client");
        console.log(result.allowTokenGeneration)
        console.log(result);
      }
    }
  }
};
</script>

<style></style>
