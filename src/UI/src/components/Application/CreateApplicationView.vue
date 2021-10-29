<template>
  <resource-edit-card
    title="Application"
    :loading="loading"
    :hideTools="true"
    :resource="application"
    @Save="onSave"
  >
    <v-form ref="form" v-model="valid" lazy-validation>
      <v-row>
        <v-col md="3"
          ><v-text-field
            label="Name"
            :rules="[(v) => !!v || 'Required']"
            v-model="application.name"
          ></v-text-field
        ></v-col>
        <v-col md="3">
          <v-select
            label="Tenant"
            :rules="[(v) => !!v || 'Required']"
            v-model="application.tenant"
            :items="tenants"
            item-text="id"
            item-value="id"
          ></v-select
        ></v-col>
        <v-col md="3">
          <v-autocomplete
            label="Client Template"
            v-model="application.templateId"
            :items="clientTemplates"
            item-text="name"
            item-value="id"
            auto-select-first
          ></v-autocomplete
        ></v-col>
        <v-col md="3">
          <v-autocomplete
            label="Environments"
            :rules="[(v) => !!v || 'Required']"
            v-model="application.environments"
            :items="environments"
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
        <v-col md="4">
          <v-autocomplete
            label="Identity Scopes"
            :rules="[(v) => !!v || 'Required']"
            v-model="application.identityScopes"
            :items="identityScopes"
            item-text="name"
            item-value="id"
            item-disabled="disabled"
            chips
            multiple
            small-chips
            deletable-chips
          ></v-autocomplete
        ></v-col>
        <v-col md="4">
          <v-autocomplete
            label="Api Scopes"
            :rules="[(v) => !!v || 'Required']"
            v-model="application.apiScopes"
            :items="apiScopes"
            item-text="name"
            item-value="id"
            item-disabled="disabled"
            chips
            multiple
            small-chips
            deletable-chips
          ></v-autocomplete
        ></v-col>
        <v-col md="4">
          <v-autocomplete
            label="Grant Types"
            :rules="[(v) => !!v || 'Required']"
            v-model="application.allowedGrantTypes"
            :items="grantTypes"
            item-text="name"
            item-value="id"
            item-disabled="disabled"
            small-chips
            multiple
            deletable-chips
          ></v-autocomplete>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="12">
          <v-combobox
            label="Redirect Uris"
            v-model="application.redirectUris"
            chips
            multiple
            clearable
            deletable-chips
            small-chips
          ></v-combobox
        ></v-col>
      </v-row>
    </v-form>
  </resource-edit-card>
</template>
<script>
import { mapActions } from "vuex";
import ResourceEditCard from "../ResourceAuthor/ResourceEditCard";

export default {
  components: { ResourceEditCard },
  mounted() {
    if (this.tenants.length === 1) {
      this.application.tenant = this.tenants[0].id;
    }
  },
  watch: {
    "application.tenant": function () {
      if (this.clientTemplates.length > 1) {
        this.application.templateId = this.clientTemplates[0].id;
      }
    },
  },
  data() {
    return {
      loading: false,
      valid: null,
      application: {
        name: null,
        identityScopes: [],
        apiScopes: [],
        allowedGrantTypes: [],
        redirectUris: [],
        tenant: null,
        environments: [],
        templateId: null,
      },
    };
  },
  computed: {
    clientTemplates: function () {
      return this.$store.getters["application/clientTemplatesByTenant"](
        this.application.tenant
      );
    },
    grantTypes: function () {
      return this.$store.getters["idResource/grantTypesByTemplate"](
        this.application.templateId
      );
    },
    apiScopes: function () {
      return this.$store.getters["idResource/apiScopesByTemplate"](
        this.application.templateId
      );
    },
    identityScopes: function () {
      return this.$store.getters["idResource/identityScopesByTemplate"](
        this.application.templateId
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
    ...mapActions("application", ["createApplication"]),

    async onSave(event) {
      this.$refs.form.validate();
      await this.createApplication({
        name: this.application.name,
        tenant: this.application.tenant,
        templateId: this.application.templateId,
        environments: this.application.environments,
        apiScopes: this.application.apiScopes,
        identityScopes: this.application.identityScopes,
        allowedGrantTypes: this.application.allowedGrantTypes,
        redirectUris: this.application.redirectUris,
      });

      event.done();

      this.$router.replace({
        name: "Application_Created",
      });
    },
  },
};
</script>

<style></style>
