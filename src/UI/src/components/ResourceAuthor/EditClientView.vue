<template>
  <resource-edit-card
    :title="client.name"
    :loading="loading"
    :resource="client"
    :tools="['DEPENDENCIES', 'PUBLISH', 'INSIGHTS', 'AUDIT', 'LOG']"
    type="Client"
    @Save="onSave"
  >
    <v-form ref="form" v-model="valid" lazy-validation>
      <v-tabs vertical v-model="tab">
        <v-tab> <v-icon left> mdi-format-list-checkbox </v-icon> </v-tab>
        <v-tab>
          <v-icon left> mdi-key-outline </v-icon>
        </v-tab>
        <v-tab>
          <v-icon left> mdi-text-box-check-outline </v-icon>
        </v-tab>
        <v-tab>
          <v-icon left> mdi-cog-outline </v-icon>
        </v-tab>
        <v-tab>
          <v-icon left> mdi-timer-sand </v-icon>
        </v-tab>
        <v-tab>
          <v-icon left> mdi-one-up </v-icon>
        </v-tab>
        <v-tab>
          <v-icon left> mdi-logout </v-icon>
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
            <v-col>
              <v-alert outlined type="warning" v-if="hasApplicationLink">
                This client is linked to application
                <strong
                  ><router-link
                    style="color: inherit"
                    :to="{
                      name: 'Application_Edit',
                      params: { id: client.application.id },
                    }"
                  >
                    {{ client.application.name }}</router-link
                  > </strong
                >. Some fields can not be edited an will be overwritten when
                saving the application.
              </v-alert>
            </v-col>
          </v-row>
          <v-row>
            <v-col md="4">
              <v-text-field
                label="ClientId"
                disabled
                v-model="client.clientId"
              ></v-text-field
            ></v-col>
            <v-col md="4"
              ><v-text-field
                label="Name"
                :disabled="hasApplicationLink"
                v-model="client.name"
              ></v-text-field
            ></v-col>
            <v-col md="2">
              <v-autocomplete
                label="Environments"
                v-model="client.environments"
                :items="environments"
                :disabled="hasApplicationLink"
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
                disabled
                v-model="client.tenant"
                :items="tenants"
                item-text="id"
                item-value="id"
              ></v-select
            ></v-col>
          </v-row>

          <v-row>
            <v-col md="6">
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
            <v-col md="6">
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
          </v-row>
          <v-row>
            <v-col md="12">
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

          <v-row>
            <v-col md="4">
              <v-select
                label="Access Token type"
                v-model="client.accessTokenType"
                item-text="text"
                item-value="value"
                :items="accessTokenTypes"
              ></v-select
            ></v-col>
            <v-col md="4">
              <v-text-field
                label="ClientClaimsPrefix"
                v-model="client.clientClaimsPrefix"
              ></v-text-field
            ></v-col>
            <v-col md="4">
              <v-text-field
                label="PairWiseSubjectSalt"
                v-model="client.PairWiseSubjectSalt"
              ></v-text-field
            ></v-col>
          </v-row>
          <v-row>
            <v-col md="4">
              <v-text-field
                label="Description"
                v-model="client.description"
              ></v-text-field
            ></v-col>

            <v-col md="4">
              <v-text-field
                label="Client Uri"
                v-model="client.clientUri"
              ></v-text-field
            ></v-col>
            <v-col md="4">
              <v-text-field
                label="Logo Uri"
                v-model="client.logoUri"
              ></v-text-field
            ></v-col>
          </v-row>
        </v-tab-item>
        <v-tab-item key="secrets">
          <v-row>
            <v-col md="6">
              <secret-list
                :id="id"
                :secrets="client.clientSecrets"
                @Remove="onRemoveSecret"
              ></secret-list>
            </v-col>
            <v-col md="6">
              <v-row>
                <v-col md="12"
                  ><v-switch
                    label="Require client secret"
                    v-model="client.requireClientSecret"
                  ></v-switch
                ></v-col>
                <v-col md="12">
                  <add-secret
                    :id="this.id"
                    @AddSecret="onAddSecret"
                  ></add-secret>
                </v-col>
              </v-row>
            </v-col>
          </v-row>
        </v-tab-item>
        <v-tab-item key="redir">
          <v-row>
            <v-col md="12">
              <combo-box-editor
                label="Redirect Uris"
                v-model="client.redirectUris"
              ></combo-box-editor>
            </v-col>
          </v-row>
          <v-row>
            <v-col md="12">
              <combo-box-editor
                label="Allows cors origins"
                v-model="client.allowedCorsOrigins"
              ></combo-box-editor
            ></v-col>
          </v-row>
        </v-tab-item>
        <v-tab-item key="settings">
          <v-row>
            <v-col md="3"
              ><v-switch
                label="Require Pkce"
                v-model="client.requirePkce"
              ></v-switch
            ></v-col>
            <v-col md="3"
              ><v-switch
                label="Allow plaintext Pkce"
                v-model="client.allowPlainTextPkce"
              ></v-switch
            ></v-col>
            <v-col md="3"
              ><v-switch
                label="Allow offline access"
                v-model="client.allowOfflineAccess"
              ></v-switch
            ></v-col>
            <v-col md="3"
              ><v-switch
                label="Allow access tokens via browser"
                v-model="client.allowAccessTokensViaBrowser"
              ></v-switch
            ></v-col>
          </v-row>
          <v-row>
            <v-col md="3"
              ><v-switch
                label="Require consent"
                v-model="client.requireConsent"
              ></v-switch
            ></v-col>
            <v-col md="3"
              ><v-switch
                label="Allow remember consent"
                v-model="client.allowRememberConsent"
              ></v-switch
            ></v-col>
            <v-col md="3"
              ><v-switch
                label="Require request object"
                v-model="client.requireRequestObject"
              ></v-switch
            ></v-col>
            <v-col md="3"
              ><v-switch
                label="Always include userclaims in Id Token"
                v-model="client.alwaysIncludeUserClaimsInIdToken"
              ></v-switch
            ></v-col>
          </v-row>
          <v-row>
            <v-col md="3"
              ><v-switch
                label="UpdateAccessTokenClaimsOnRefresh"
                v-model="client.updateAccessTokenClaimsOnRefresh"
              ></v-switch
            ></v-col>
            <v-col md="3"
              ><v-switch
                label="Include JwtId"
                v-model="client.IncludeJwtId"
              ></v-switch
            ></v-col>
            <v-col md="3"
              ><v-switch
                label="AlwaysSendClientClaims"
                v-model="client.alwaysSendClientClaims"
              ></v-switch
            ></v-col>
          </v-row>
        </v-tab-item>
        <v-tab-item key="expirations">
          <v-row>
            <v-col md="12">
              <h4 class="blue--text text--darken-3">Tokens and codes</h4></v-col
            ></v-row
          >
          <v-row>
            <v-col md="3">
              <v-text-field
                type="number"
                suffix="s"
                label="Identity token"
                v-model.number="client.identityTokenLifetime"
              ></v-text-field>
            </v-col>

            <v-col md="3">
              <v-text-field
                type="number"
                suffix="s"
                label="Access Token"
                v-model.number="client.accessTokenLifetime"
              ></v-text-field>
            </v-col>
            <v-col md="3">
              <v-text-field
                type="number"
                suffix="s"
                label="Authorization Code"
                v-model.number="client.authorizationCodeLifetime"
              ></v-text-field>
            </v-col>
            <v-col md="3">
              <v-text-field
                type="number"
                suffix="s"
                label="Device Code"
                v-model.number="client.deviceCodeLifetime"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col md="12">
              <h4 class="blue--text text--darken-3">Refresh Token</h4></v-col
            ></v-row
          >
          <v-row>
            <v-col md="3">
              <v-select
                label="Expiration Type"
                v-model="client.refreshTokenExpiration"
                item-text="text"
                item-value="value"
                :items="refreshTokenExpirations"
              ></v-select>
            </v-col>
            <v-col md="3">
              <v-select
                label="Refresh token usage"
                v-model="client.refreshTokenUsage"
                item-text="text"
                item-value="value"
                :items="refreshTokenUsages"
              ></v-select>
            </v-col>
            <v-col md="3">
              <v-text-field
                type="number"
                suffix="s"
                label="Absolute lifetime"
                v-model.number="client.absoluteRefreshTokenLifetime"
              ></v-text-field>
            </v-col>
            <v-col md="3">
              <v-text-field
                type="number"
                suffix="s"
                label="Sliding lifetime"
                v-model.number="client.slidingRefreshTokenLifetime"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col md="12">
              <h4 class="blue--text text--darken-3">
                Others (Empty is no expiration)
              </h4></v-col
            ></v-row
          >
          <v-row>
            <v-col md="3">
              <v-text-field
                type="number"
                suffix="s"
                label="Consent"
                v-model="client.consentLifetime"
              ></v-text-field>
            </v-col>
            <v-col md="3">
              <v-text-field
                type="number"
                suffix="s"
                label="User SSO"
                v-model="client.UserSsoLifetime"
              ></v-text-field>
            </v-col>
          </v-row>
        </v-tab-item>
        <v-tab-item key="info">
          <h4 class="blue--text text--darken-3">Properties</h4>
          <div class="mt-4" v-if="client.properties">
            <v-row v-for="(prop, i) in client.properties" :key="i">
              <v-col md="4"
                ><v-text-field
                  dense
                  label="Name"
                  v-model="prop.key"
                  :rules="[(v) => !!v || 'Required']"
                ></v-text-field
              ></v-col>
              <v-col md="7"
                ><v-text-field
                  dense
                  label="Value"
                  v-model="prop.value"
                  :rules="[(v) => !!v || 'Required']"
                ></v-text-field
              ></v-col>
              <v-col md="1">
                <v-icon @click="onRemoveProperty(i)">mdi-delete-outline</v-icon>
              </v-col>
            </v-row>
          </div>

          <v-toolbar elevation="0">
            <v-spacer></v-spacer>
            <v-btn text color="primary" @click="onAddNewProperty">
              Add property<v-icon>mdi-pencil-plus</v-icon></v-btn
            >
          </v-toolbar>

          <h4 class="blue--text text--darken-3">Claims</h4>
          <div class="mt-4" v-if="client.claims">
            <v-row v-for="(claim, i) in client.claims" :key="i">
              <v-col md="4"
                ><v-text-field
                  dense
                  label="Type"
                  v-model="claim.type"
                  :rules="[(v) => !!v || 'Required']"
                ></v-text-field
              ></v-col>
              <v-col md="4"
                ><v-text-field
                  dense
                  label="Value"
                  v-model="claim.value"
                  :rules="[(v) => !!v || 'Required']"
                ></v-text-field
              ></v-col>
              <v-col md="3"
                ><v-text-field
                  dense
                  label="ValueType"
                  v-model="claim.valueType"
                  :rules="[(v) => !!v || 'Required']"
                ></v-text-field
              ></v-col>
              <v-col md="1">
                <v-icon @click="onRemoveClaim(i)">mdi-delete-outline</v-icon>
              </v-col>
            </v-row>
          </div>

          <v-toolbar elevation="0">
            <v-spacer></v-spacer>
            <v-btn text color="primary" @click="onAddNewClaim">
              Add claim<v-icon>mdi-pencil-plus</v-icon></v-btn
            >
          </v-toolbar>
        </v-tab-item>
        <v-tab-item key="logout">
          <v-row>
            <v-col md="12">
              <combo-box-editor
                label="Post logout urls"
                v-model="client.postLogoutRedirectUris"
              ></combo-box-editor
            ></v-col>
          </v-row>
          <v-row>
            <v-col md="8">
              <v-text-field
                label="FrontChannelLogoutUri"
                v-model="client.frontChannelLogoutUri"
              ></v-text-field>
            </v-col>
            <v-col md="4">
              <v-switch
                label="FrontChannelLogoutSessionRequired"
                v-model="client.frontChannelLogoutSessionRequired"
              ></v-switch>
            </v-col>
          </v-row>
          <v-row>
            <v-col md="8">
              <v-text-field
                label="BackChannelLogoutUri"
                v-model="client.backChannelLogoutUri"
              ></v-text-field>
            </v-col>
            <v-col md="4">
              <v-switch
                label="BackChannelLogoutSessionRequired"
                v-model="client.backChannelLogoutSessionRequired"
              ></v-switch>
            </v-col>
          </v-row>
        </v-tab-item>
        <v-tab-item key="dataconnector">
          <data-connector-module
            v-if="dataConnectorsEnabled"
            v-model="client.dataConnectors"
            :tenant="client.tenant"
          ></data-connector-module
        ></v-tab-item>

        <v-tab-item key="providers">
          <identity-providers
            v-if="authProvidersEnabled"
            v-model="client.enabledProviders"
            :tenant="client.tenant"
          ></identity-providers>
        </v-tab-item>
        <v-tab-item key="danger"></v-tab-item>
      </v-tabs>
    </v-form>
  </resource-edit-card>
</template>

<script>
import { mapActions } from "vuex";
import { getClientById } from "../../services/idResourceService";
import AddSecret from "./AddSecret.vue";
import SecretList from "./SecretList.vue";
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
    AddSecret,
    SecretList,
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
        this.setClient();
      },
    },
  },
  data() {
    return {
      loading: false,
      tab: null,
      redirectUriDialog: false,
      valid: null,
      manualClient: false,
      clientIdGenerator: "GUID",
      client: {
        id: null,
        clientId: null,
        name: null,
        identityScopes: [],
        apiScopes: [],
        allowedGrantTypes: [],
        tenant: null,
        environments: [],
        postLogoutRedirectUris: [],
        redirectUris: [],
        allowedCorsOrigins: [],
        requireClientSecret: true,
        requirePkce: true,
        allowPlainTextPkce: false,
        allowOfflineAccess: false,
        allowAccessTokensViaBrowser: false,
        identityTokenLifetime: null,
        AccessTokenLifetime: null,
        authorizationCodeLifetime: null,
        deviceCodeLifetime: null,
        absoluteRefreshTokenLifetime: null,
        slidingRefreshTokenLifetime: null,
        refreshTokenExpiration: null,
        consentLifetime: null,
        accessTokenType: null,
        description: null,
        clientUri: null,
        logoUri: null,
        requireConsent: null,
        allowRememberConsent: null,
        requireRequestObject: null,
        alwaysIncludeUserClaimsInIdToken: null,
        updateAccessTokenClaimsOnRefresh: null,
        alwaysSendClientClaims: null,
        clientClaimsPrefix: null,
        frontChannelLogoutUri: null,
        frontChannelLogoutSessionRequired: false,
        backChannelLogoutUri: null,
        backChannelLogoutSessionRequired: false,
        properties: [],
        claims: [],
        dataConnectors: [],
        enabledProviders: [],
        application: null,
      },
    };
  },
  computed: {
    grantTypes: function () {
      return this.$store.state.idResource.grantType.items;
    },
    refreshTokenExpirations: function () {
      return this.$store.state.idResource.enums.tokenExpiration.values;
    },
    accessTokenTypes: function () {
      return this.$store.state.idResource.enums.accessTokenType.values;
    },
    refreshTokenUsages: function () {
      return this.$store.state.idResource.enums.tokenUsage.values;
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
    dataConnectorsEnabled: function () {
      return this.$store.getters["system/isModuleEnabled"](
        this.client.tenant,
        "DataConnector"
      );
    },
    authProvidersEnabled: function () {
      return this.$store.getters["system/isModuleEnabled"](
        this.client.tenant,
        "AuthProviders"
      );
    },
    hasApplicationLink: function () {
      return this.client.application != null;
    },
  },
  methods: {
    ...mapActions("idResource", [
      "updateClient",
      "addClientSecret",
      "removeClientSecret",
    ]),
    async setClient() {
      this.loading = true;
      const result = await getClientById(this.id);
      this.loading = false;
      const { client } = result.data;
      this.client = Object.assign({}, client);

      delete this.client.__typename;
    },
    async onAddSecret(secret, callback) {
      const result = await this.addClientSecret(secret);
      this.setClient(result.client);
      callback(result.secret);
    },
    async onRemoveSecret(id) {
      const client = await this.removeClientSecret({
        clientId: this.id,
        id: id,
      });

      this.setClient(client);
    },
    onAddNewProperty: function () {
      this.client.properties.push({
        key: "",
        value: "",
      });
    },
    onRemoveProperty: function (index) {
      this.properties.splice(index, 1);
    },

    onAddNewClaim: function () {
      this.client.claims.push({
        type: "",
        value: "",
        valueType: "http://www.w3.org/2001/XMLSchema#string",
      });
    },
    onRemoveClaim: function (index) {
      this.client.claims.splice(index, 1);
    },
    async onSave(event) {
      if(!this.$refs.form.validate())
      {
        this.$store.dispatch(
            "shell/addMessage",
            { text: "Input validation failed.", type: "ERROR" },
            { root: true }
        );

        event.done(true);
        return;
      }

      const update = Object.assign({}, this.client);
      delete update.allowedScopes;
      delete update.secrets;
      delete update.clientId;

      removeTypeName(update.properties);
      removeTypeName(update.claims);

      if (update.dataConnectors) {
        removeTypeName(update.dataConnectors);
        for (let i = 0; i < update.dataConnectors.length; i++) {
          const conn = update.dataConnectors[i];
          removeTypeName(conn.properties);
        }
      }
      if (update.enabledProviders) {
        removeTypeName(update.enabledProviders);
        for (let i = 0; i < update.enabledProviders.length; i++) {
          const ep = update.enabledProviders[i];
          removeTypeName(ep);
        }
      }

      await this.updateClient(update);
      this.setClient();
      event.done(true);
    },
  },
};
</script>

<style>
</style>
