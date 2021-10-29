<template>
  <v-card class="ma-4">
    <v-card-title>
      <span>Create new Client</span>
    </v-card-title>
    <v-card-text>
      <v-form ref="form" v-model="valid" lazy-validation>
        <v-row>
          <v-col md="4">
            <v-select
              v-if="!manualClient"
              :items="clientIdGenerators"
              label="Client ID Generator"
              append-outer-icon="mdi-pencil-outline"
              @click:append-outer="manualClient = true"
              v-model="clientIdGenerator"
            ></v-select
            ><v-text-field
              v-else
              label="ClientId"
              v-model="client.clientId"
              append-outer-icon="mdi-slot-machine-outline"
              @click:append-outer="manualClient = false"
            ></v-text-field
          ></v-col>
          <v-col md="4"
            ><v-text-field label="Name" v-model="client.name"></v-text-field
          ></v-col>
          <v-col md="2">
            <v-autocomplete
              label="Environments"
              v-model="client.environments"
              :items="environments"
              item-text="name"
              item-value="id"
              chips
              multiple
              small-chips
              deletable-chips
            ></v-autocomplete
          ></v-col>
          <v-col md="2">
            <v-select
              label="Tenant"
              v-model="client.tenant"
              :items="tenants"
              item-text="id"
              item-value="id"
            ></v-select
          ></v-col>
        </v-row>

        <v-row>
          <v-col md="4">
            <v-autocomplete
              label="Grant Types"
              v-model="client.allowedGrantTypes"
              :items="grantTypes"
              item-text="name"
              item-value="id"
              small-chips
              multiple
              deletable-chips
            ></v-autocomplete>
          </v-col>
          <v-col md="4">
            <v-autocomplete
              label="Identity Scopes"
              v-model="client.identityScopes"
              :items="identityScopes"
              item-text="name"
              item-value="id"
              chips
              multiple
              small-chips
              deletable-chips
            ></v-autocomplete
          ></v-col>
          <v-col md="4">
            <v-autocomplete
              label="Api Scopes"
              v-model="client.apiScopes"
              :items="apiScopes"
              item-text="name"
              item-value="id"
              chips
              multiple
              small-chips
              deletable-chips
            ></v-autocomplete
          ></v-col>
        </v-row>
      </v-form>
    </v-card-text>
    <v-card-actions>
      <v-spacer></v-spacer>
      <v-btn :disabled="!valid" color="primary" elevation="2" @click="onSave"
        >Save</v-btn
      >
    </v-card-actions>
  </v-card>
</template>

<script>
import { mapActions } from "vuex";
export default {
  mounted() {
    if (this.tenants.length === 1) {
      this.client.tenant = this.tenants[0].id;
    }
  },
  data() {
    return {
      tab: null,
      valid: null,
      manualClient: false,
      clientIdGenerator: "GUID",
      client: {
        clientId: null,
        name: null,
        identityScopes: [],
        apiScopes: [],
        allowedGrantTypes: [],
        displayName: null,
        description: null,
        enabled: true,
        tenant: null,
        environments: [],
      },
    };
  },
  computed: {
    clientIdGenerators: function () {
      return ["GUID"];
    },
    grantTypes: function () {
      return this.$store.state.idResource.grantType.items;
    },
    apiScopes: function () {
      return this.$store.getters["idResource/apiScopesByTenant"](
        this.client.tenant
      );
    },
    identityScopes: function () {
      return this.$store.getters["idResource/identityScopesByTenant"](
        this.client.tenant
      );
    },
    environments: function () {
      return this.$store.state.system.environment.items;
    },
    tenants: function () {
      return this.$store.state.system.tenant.items;
    },
  },
  methods: {
    ...mapActions("idResource", ["createClient"]),
    setScope: function () {
      if (this.id) {
        const scope = this.$store.state.idResource.apiScope.items.find(
          (x) => x.id === this.id
        );
        this.scope = Object.assign({}, scope);
        delete this.scope.__typename;
      } else {
        this.resetForm();
      }
    },
    resetForm: function () {
      this.scope = {
        id: null,
        name: null,
        displayName: null,
        description: null,
        showInDiscoveryDocument: false,
        enabled: true,
        tenant: null,
      };
      this.$refs.form.resetValidation();
    },
    async onSave() {
      this.$refs.form.validate();
      const client = await this.createClient({
        clientId: this.client.clientId,
        name: this.client.name,
        tenant: this.client.tenant,
        environments: this.client.environments,
        allowedGrantTypes: this.client.allowedGrantTypes,
        apiScopes: this.client.apiScopes,
        identityScopes: this.client.identityScopes,
        clientIdGenerator: this.clientIdGenerator,
      });

      this.$router.replace({
        name: "Client_Edit",
        params: { id: client.id },
      });
    },
  },
};
</script>

<style>
</style>
