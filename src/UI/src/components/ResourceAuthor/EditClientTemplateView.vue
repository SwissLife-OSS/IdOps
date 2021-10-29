<template>
  <resource-edit-card
    :title="title"
    :loading="loading"
    :resource="clientTemplate"
    :tools="['DEPENDENCIES', 'AUDIT']"
    type="ClientTemplate"
    @Save="onSave"
  >
    <template>
      <v-form ref="form" v-model="valid" lazy-validation>
        <v-tabs vertical v-model="tab">
          <v-tab> <v-icon left> mdi-format-list-checkbox </v-icon> </v-tab>
          <v-tab>
            <v-icon left> mdi-key-outline </v-icon>
          </v-tab>
          <v-tab :disabled="!dataConnectorsEnabled">
            <v-icon left>mdi-power-plug</v-icon>
          </v-tab>
          <v-tab :disabled="!authProvidersEnabled">
            <v-icon left>mdi-card-account-details-outline</v-icon>
          </v-tab>
          <v-tab>
            <v-icon left> mdi-bomb</v-icon>
          </v-tab>
          <v-tab-item key="core">
            <v-row>
              <v-col md="6"
                ><v-text-field
                  required
                  label="Name"
                  :rules="[(v) => !!v || 'Required']"
                  v-model="clientTemplate.name"
                ></v-text-field
              ></v-col>

              <v-col md="6">
                <v-select
                  label="Tenant"
                  :rules="[(v) => !!v || 'Required']"
                  v-model="clientTemplate.tenant"
                  :items="tenants"
                  item-text="id"
                  item-value="id"
                  :disabled="id != null"
                ></v-select>
              </v-col>
            </v-row>
            <v-row>
              <v-col md="6"
                ><v-text-field
                  required
                  label="Name Template"
                  :rules="[(v) => !!v || 'Required']"
                  v-model="clientTemplate.nameTemplate"
                ></v-text-field
              ></v-col>

              <v-col md="6"
                ><v-text-field
                  required
                  label="Url Template"
                  :rules="[(v) => !!v || 'Required']"
                  v-model="clientTemplate.urlTemplate"
                ></v-text-field
              ></v-col>
            </v-row>
            <v-row>
              <v-col md="6">
                <v-select
                  v-if="!manualClient"
                  :items="clientIdGenerators"
                  label="Client ID Generator"
                  append-outer-icon="mdi-pencil-outline"
                  @click:append-outer="manualClient = true"
                  v-model="clientTemplate.clientIdGenerator"
                ></v-select
                ><v-text-field
                  v-else
                  label="ClientId"
                  v-model="clientTemplate.Id"
                  append-outer-icon="mdi-slot-machine-outline"
                  @click:append-outer="manualClient = false"
                ></v-text-field
              ></v-col>
              <v-col md="6">
                <v-autocomplete
                  label="Grant Types"
                  :rules="[(v) => !!v || 'Required']"
                  v-model="clientTemplate.allowedGrantTypes"
                  :items="grantTypes"
                  item-text="name"
                  item-value="id"
                  small-chips
                  multiple
                  deletable-chips
                ></v-autocomplete>
              </v-col>
            </v-row>

            <v-row>
              <v-col md="6">
                <v-autocomplete
                  label="Api Scopes"
                  :rules="[(v) => !!v || 'Required']"
                  v-model="clientTemplate.apiScopes"
                  :items="apiScopes"
                  item-text="name"
                  item-value="id"
                  chips
                  multiple
                  small-chips
                  deletable-chips
                ></v-autocomplete
              ></v-col>

              <v-col md="6">
                <v-autocomplete
                  label="Identity Scopes"
                  :rules="[(v) => !!v || 'Required']"
                  v-model="clientTemplate.identityScopes"
                  :items="identityScopes"
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
              <v-col md="6">
                <combo-box-editor
                  label="Redirect Uris"
                  v-model="clientTemplate.redirectUris"
                ></combo-box-editor>
              </v-col>

              <v-col md="6">
                <combo-box-editor
                  label="Post Logout Redirect Uris"
                  v-model="clientTemplate.postLogoutRedirectUris"
                ></combo-box-editor
              ></v-col>
            </v-row>

            <v-row>
              <v-col md="3"
                ><v-switch
                  label="RequirePkce"
                  v-model="clientTemplate.requirePkce"
                ></v-switch
              ></v-col>
              <v-col md="3"
                ><v-switch
                  label="AllowOfflineAccess"
                  v-model="clientTemplate.allowOfflineAccess"
                ></v-switch
              ></v-col>
              <v-col md="3"
                ><v-switch
                  label="RequireClientSecret"
                  v-model="clientTemplate.requireClientSecret"
                ></v-switch
              ></v-col>
              <v-col md="3"
                ><v-switch
                  label="AllowAccessTokensViaBrowser"
                  v-model="clientTemplate.allowAccessTokensViaBrowser"
                ></v-switch
              ></v-col>
            </v-row>
          </v-tab-item>

          <v-tab-item key="secrets">
            <v-row v-for="(sec, i) in clientTemplate.secrets" :key="i">
              <v-col md="12">
                <v-text-field label="Secret" v-model="sec.value">
                  <template v-slot:prepend-inner>
                    <strong style="width: 40px; padding-top: 4px">{{
                      sec.environment
                    }}</strong>
                  </template>
                </v-text-field>
              </v-col>
            </v-row>

            <v-alert type="warning" outlined>
              The secrets will be hashed after saving, you will not be able to
              retrieve the secrets again.
            </v-alert>
          </v-tab-item>

          <v-tab-item key="dataconnector">
            <data-connector-module
              v-if="dataConnectorsEnabled"
              v-model="clientTemplate.dataConnectors"
              :tenant="clientTemplate.tenant"
            ></data-connector-module
          ></v-tab-item>

          <v-tab-item key="providers">
            <identity-providers
              v-if="authProvidersEnabled"
              v-model="clientTemplate.enabledProviders"
              :tenant="clientTemplate.tenant"
            ></identity-providers>
          </v-tab-item>

          <v-tab-item key="danger "></v-tab-item>
        </v-tabs>
      </v-form> </template
  ></resource-edit-card>
</template>

<script>
import { mapActions } from "vuex";
import { getClientTemplateById } from "../../services/applicationService";
import ResourceEditCard from "./ResourceEditCard.vue";
import DataConnectorModule from "./DataConnectorModule.vue";
import IdentityProviders from "./IdentityProviders.vue";
import ComboBoxEditor from "../Common/ComboBoxEditor.vue";

const removeTypeName = (list) => {
  if (list && list.length) {
    for (let i = 0; i < list.length; i++) {
      const item = list[i];
      delete item.__typename;
    }
  }
};

export default {
  components: {
    ResourceEditCard,
    DataConnectorModule,
    IdentityProviders,
    ComboBoxEditor,
  },
  props: ["id"],
  watch: {
    id: {
      immediate: true,
      handler: function () {
        this.setClientTemplate();
      },
    },
  },
  data() {
    return {
      valid: null,
      loading: false,
      redirectUriDialog: false,
      tab: null,
      manualClient: false,
      clientTemplate: {
        id: null,
        name: null,
        nameTemplate: "{{toUpper environment}}_{{application}}",
        urlTemplate: "https://{{application}}.{{environment}}.foo.local",
        allowedGrantTypes: [],
        redirectUris: [],
        postLogoutRedirectUris: [],
        tenant: null,
        requirePkce: true,
        clientIdGenerator: "GUID",
        secretGenerator: "DEFAULT",
        requireClientSecret: true,
        allowAccessTokensViaBrowser: false,
        allowOfflineAccess: false,
        identityScopes: [],
        apiScopes: [],
        dataConnectors: [],
        enabledProviders: [],
        secrets: [],
      },
    };
  },
  computed: {
    clientIdGenerators: function () {
      return ["GUID"];
    },
    title: function () {
      if (this.id) {
        return this.clientTemplate.name;
      }
      return "Client Template";
    },
    grantTypes: function () {
      return this.$store.state.idResource.grantType.items;
    },
    tenants: function () {
      return this.$store.state.system.tenant.items;
    },
    environments: function () {
      return this.$store.state.system.environment.items;
    },
    apiScopes: function () {
      return this.$store.getters["idResource/apiScopesByTenant"](
        this.clientTemplate.tenant
      );
    },
    identityScopes: function () {
      return this.$store.getters["idResource/identityScopesByTenant"](
        this.clientTemplate.tenant
      );
    },
    dataConnectorsEnabled: function () {
      return this.$store.getters["system/isModuleEnabled"](
        this.clientTemplate.tenant,
        "DataConnector"
      );
    },
    authProvidersEnabled: function () {
      return this.$store.getters["system/isModuleEnabled"](
        this.clientTemplate.tenant,
        "AuthProviders"
      );
    },
  },
  methods: {
    ...mapActions("application", ["saveClientTemplate"]),
    async setClientTemplate() {
      if (this.id) {
        this.loading = true;
        const result = await getClientTemplateById(this.id);
        const clientTemplate = result.data.clientTemplateById;
        this.clientTemplate = Object.assign({}, clientTemplate);
        delete this.clientTemplate.__typename;
        this.loading = false;
      } else {
        this.resetForm();
      }
    },
    async resetForm() {
      this.clientTemplate = {
        id: null,
        name: null,
        allowedGrantTypes: [],
        redirectUris: [],
        postLogoutRedirectUris: [],
        tenant: null,
        requirePkce: true,
        clientIdGenerator: "GUID",
        secretGenerator: "DEFAULT",
        requireClientSecret: true,
        allowAccessTokensViaBrowser: false,
        allowOfflineAccess: false,
        dataConnectors: [],
        enabledProviders: [],
        nameTemplate: "{{toUpper environment}}_{{application}}",
        urlTemplate: "https://{{application}}.{{environment}}.foo.local",
        secrets: this.environments.map((x) => {
          return {
            value: null,
            environmentId: x.id,
            environment: x.name,
          };
        }),
      };
      if (this.tenants.length === 1) {
        this.clientTemplate.tenant = this.tenants[0].id;
      }
      if (this.$refs.form) {
        this.$refs.form.reset();
        this.$refs.form.resetValidation();
      }
    },
    async onSave(event) {
      const isValid = this.$refs.form.validate();
      if (!isValid) {
        event.done(true);
        return;
      }
      const template = Object.assign({}, this.clientTemplate);

      if (template.dataConnectors) {
        removeTypeName(template.dataConnectors);
        for (let i = 0; i < template.dataConnectors.length; i++) {
          const conn = template.dataConnectors[i];
          removeTypeName(conn.properties);
        }
      }
      if (template.enabledProviders) {
        removeTypeName(template.enabledProviders);
        for (let i = 0; i < template.enabledProviders.length; i++) {
          const ep = template.enabledProviders[i];
          removeTypeName(ep);
        }
      }

      template.secrets = Object.assign(
        [],
        template.secrets.map((x) => {
          return {
            environmentId: x.environmentId,
            value: x.value,
          };
        })
      );

      const saved = await this.saveClientTemplate(template);
      event.done(true);
      if (saved) {
        this.setClientTemplate();
        if (saved && this.$route.name === "ClientTemplate_New") {
          this.$router.replace({
            name: "ClientTemplate_Edit",
            params: { id: saved.clientTemplate.id },
          });
        }
      }
    },
  },
};
</script>

<style>
</style>