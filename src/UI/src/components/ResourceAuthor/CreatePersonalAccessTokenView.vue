<template>
  <v-card class="ma-4">
    <v-card-title>
      <span>Create new personal access token</span>
    </v-card-title>
    <v-card-text>
      <v-form ref="form" v-model="valid" lazy-validation>
        <v-row>
          <v-col md="4"
            ><v-text-field
              label="Username"
              v-model="personalAccessToken.userName"
            ></v-text-field
          ></v-col>
          <v-col md="2">
            <v-autocomplete
              label="Environment"
              v-model="personalAccessToken.environmentId"
              :items="environments"
              item-text="name"
              item-value="id"
            ></v-autocomplete
          ></v-col>
          <v-col md="2">
            <v-select
              label="Tenant"
              v-model="personalAccessToken.tenant"
              :items="tenants"
              item-text="id"
              item-value="id"
            ></v-select
          ></v-col>
          <v-col md="2">
            <v-autocomplete
              label="Hash Algorithm"
              v-model="personalAccessToken.hashAlgorithm"
              :items="hashAlgorithms"
              item-text="name"
              item-value="id"
            ></v-autocomplete>
          </v-col>
          <v-col md="2">
            <v-autocomplete
              label="Sources"
              v-model="personalAccessToken.source"
              :items="sources"
              item-text="name"
              item-value="id"
            ></v-autocomplete>
          </v-col>
        </v-row>

        <v-row>
          <v-col md="4">
            <v-autocomplete
              label="Allowed Applications"
              v-model="personalAccessToken.allowedApplicationIds"
              :items="allowedApplications"
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
              label="Allowed Scopes"
              v-model="personalAccessToken.allowedScopes"
              :items="allowedScopes"
              item-text="name"
              item-value="id"
              chips
              multiple
              small-chips
              deletable-chips
            ></v-autocomplete
          ></v-col>
        </v-row>
        <v-row>
          <v-col>
            <h4 class="blue--text text--darken-3">Claims Extensions</h4>
          </v-col>
        </v-row>
        <v-row
          v-for="(claim, i) in personalAccessToken.claimsExtensions"
          :key="i"
        >
          <v-col md="4"
            ><v-text-field
              dense
              label="Type"
              v-model="claim.type"
            ></v-text-field
          ></v-col>
          <v-col md="4"
            ><v-text-field
              dense
              label="Value"
              v-model="claim.value"
            ></v-text-field
          ></v-col>
          <v-col md="1">
            <v-icon @click="onRemoveClaim(i)">mdi-delete-outline</v-icon>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-toolbar elevation="0">
              <v-spacer></v-spacer>
              <v-btn text color="primary" @click="onAddNewClaim">
                Add claim<v-icon>mdi-pencil-plus</v-icon></v-btn
              >
            </v-toolbar>
          </v-col>
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
  watch: {
    "personalAccessToken.tenant": {
      handler: function(v) {
        if (!v) {
          return;
        }
        this.personalAccessToken.environmentId = this.environments?.[0]?.id;
        this.personalAccessToken.hashAlgorithm = this.hashAlgorithms?.[0]?.id;
        this.personalAccessToken.source = this.sources?.[0];
      }
    }
  },
  mounted() {
    this.personalAccessToken.tenant = this.tenants?.[0]?.id;
  },
  data() {
    return {
      tab: null,
      valid: null,
      personalAccessToken: {
        userName: null,
        tenant: null,
        source: null,
        environmentId: null,
        allowedApplicationIds: [],
        allowedScopes: [],
        claimsExtensions: []
      }
    };
  },
  computed: {
    allowedApplications: function() {
      return this.$store.state.application.list.items.filter(
        x => x.tenantInfo.id === this.personalAccessToken.tenant
      );
    },
    allowedScopes: function() {
      const apiscopes = this.$store.getters["idResource/apiScopesByTenant"](
        this.personalAccessToken.tenant
      );
      const identityScopes = this.$store.getters[
        "idResource/identityScopesByTenant"
      ](this.personalAccessToken.tenant);

      return [...apiscopes, ...identityScopes].sort((a, b) => a > b);
    },
    environments: function() {
      return this.$store.state.system.environment.items;
    },
    sources: function() {
      const setting = this.$store.getters["system/getModuleSetting"](
        this.personalAccessToken.tenant,
        "PersonalAccessTokens",
        "Sources"
      );
      if (!setting) {
        return [];
      }
      return setting.split(",");
    },
    hashAlgorithms: function() {
      return [
        { id: "SSHA", name: "SSHA" },
        { id: "PBKDF2", name: "PBKDF2" }
      ];
    },
    tenants: function() {
      return this.$store.state.system.tenant.items;
    }
  },
  methods: {
    ...mapActions("idResource", ["createPersonalAccessToken"]),
    onAddNewClaim: function() {
      this.personalAccessToken.claimsExtensions.push({
        type: "",
        value: ""
      });
    },
    onRemoveClaim: function(index) {
      this.personalAccessToken.claimsExtensions.splice(index, 1);
    },
    setScope: function() {
      if (this.id) {
        const scope = this.$store.state.idResource.apiScope.items.find(
          x => x.id === this.id
        );
        this.scope = Object.assign({}, scope);
        delete this.scope.__typename;
      } else {
        this.resetForm();
      }
    },
    resetForm: function() {
      this.scope = {
        id: null,
        name: null,
        displayName: null,
        description: null,
        showInDiscoveryDocument: false,
        enabled: true,
        tenant: null,
        source: null,
        hashAlgorithm: null
      };
      this.$refs.form.resetValidation();
    },
    async onSave() {
      this.$refs.form.validate();
      const personalAccessToken = await this.createPersonalAccessToken({
        userName: this.personalAccessToken.userName,
        tenant: this.personalAccessToken.tenant,
        source: this.personalAccessToken.source,
        hashAlgorithm: this.personalAccessToken.hashAlgorithm,
        environmentId: this.personalAccessToken.environmentId,
        allowedApplicationIds: this.personalAccessToken.allowedApplicationIds,
        allowedScopes: this.personalAccessToken.allowedScopes.filter(x =>
          this.allowedScopes.find(y => y.id == x)
        ),
        claimsExtensions: this.personalAccessToken.claimsExtensions
      });

      this.$router.replace({
        name: "PersonalAccessToken_Edit",
        params: { id: personalAccessToken.id }
      });
    }
  }
};
</script>

<style></style>
