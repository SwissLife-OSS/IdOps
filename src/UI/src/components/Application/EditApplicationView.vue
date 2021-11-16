<template>
  <resource-edit-card
    :title="title"
    :loading="loading"
    :resource="application"
    :tools="['DEPENDENCIES', 'AUDIT']"
    type="Application"
    @Save="onSave"
  >
    <v-tabs vertical v-model="tab">
      <v-tab>
        <v-icon left> mdi-file-document-edit-outline </v-icon>
      </v-tab>
      <v-tab @click="onClickClientTab">
        <v-icon left>mdi-format-list-bulleted</v-icon>
      </v-tab>
      <v-tab>
        <v-icon left>mdi-account-check-outline</v-icon>
      </v-tab>
      <v-tab>
        <v-icon left>mdi-bomb</v-icon>
      </v-tab>
      <v-tab-item key="core">
        <v-form ref="form" v-model="valid" lazy-validation>
          <v-row>
            <v-col md="8"
              ><v-text-field
                label="Name"
                v-model="application.name"
              ></v-text-field
            ></v-col>

            <v-col md="4">
              <v-select
                label="Tenant"
                v-model="application.tenant"
                :items="tenants"
                disabled
                item-text="id"
                item-value="id"
              ></v-select
            ></v-col>
          </v-row>

          <v-row>
            <v-col md="4">
              <v-autocomplete
                label="Identity Scopes"
                v-model="application.identityScopes"
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
                v-model="application.apiScopes"
                :items="apiScopes"
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
                label="Grant Types"
                v-model="application.allowedGrantTypes"
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
            <v-col md="12">
              <combo-box-editor
                label="Redirect Uris"
                v-model="application.redirectUris"
              ></combo-box-editor>
            </v-col>
          </v-row>
        </v-form>

        <v-alert v-if="application.template" type="info" outlined>
          This application is linked to template:
          <strong>
            <router-link
              style="color: inherit"
              :to="{
                name: 'ClientTemplate_Edit',
                params: { id: application.template.id },
              }"
            >
              {{ application.template.name }}</router-link
            >
          </strong>
        </v-alert>
      </v-tab-item>
      <v-tab-item key="clients">
        <h4 class="blue--text text--darken-3">Clients</h4>

        <v-row>
          <v-col md="6">
            <v-list style="max-height: 400px" class="overflow-y-auto">
              <v-list-item
                v-for="client in application.clients"
                :key="client.id"
                @click="onClickClient(client)"
              >
                <v-list-item-avatar>
                  <v-icon small class="purple darken-4" dark>
                    mdi-application-cog
                  </v-icon>
                </v-list-item-avatar>
                <v-list-item-content>
                  <v-list-item-title v-text="client.name"></v-list-item-title>
                  <v-list-item-subtitle
                    v-text="client.clientId"
                  ></v-list-item-subtitle>
                  <v-list-item-subtitle
                    v-text="client.clientUri"
                  ></v-list-item-subtitle>
                </v-list-item-content>
                <v-list-item-action>
                  <v-btn icon @click.stop="onRemoveClient(client.id)">
                    <v-icon color="grey lighten-1">mdi-delete-outline</v-icon>
                  </v-btn>
                </v-list-item-action>
              </v-list-item>
            </v-list></v-col
          >
          <v-col md="6">
            <div v-if="unusedEnvironments.length > 0">
              <h4 class="blue--text text--darken-3">Add new Environment</h4>
              <v-list style="max-height: 400px" class="overflow-y-auto">
                <v-list-item
                  v-for="environment in unusedEnvironments"
                  :key="environment.id"
                >
                  <v-list-item-avatar>
                    <v-icon small class="purple darken-2" dark>
                      mdi-application-cog
                    </v-icon>
                  </v-list-item-avatar>
                  <v-list-item-content>
                    <v-list-item-title
                      v-text="environment.name"
                    ></v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-action>
                    <v-btn icon @click.stop="onAddEnvironment(environment.id)">
                      <v-icon color="grey lighten-1">mdi-plus</v-icon>
                    </v-btn>
                  </v-list-item-action>
                </v-list-item>
              </v-list>
            </div>
            <h4 class="blue--text text--darken-3">Add existing client</h4>
            <v-text-field
              clearable
              v-model="searchText"
              placeholder="Search"
              prepend-icon="mdi-magnify"
            ></v-text-field>
            <v-list style="max-height: 300px" class="overflow-y-auto">
              <v-list-item
                v-for="client in this.filteredClients"
                :key="client.id"
              >
                <v-list-item-avatar>
                  <v-icon small class="purple darken-2" dark>
                    mdi-application-cog
                  </v-icon>
                </v-list-item-avatar>
                <v-list-item-content>
                  <v-list-item-title v-text="client.name"></v-list-item-title>
                  <v-list-item-subtitle
                    v-text="client.clientId"
                  ></v-list-item-subtitle>
                  <v-list-item-subtitle
                    v-text="client.clientUri"
                  ></v-list-item-subtitle>
                </v-list-item-content>
                <v-list-item-action>
                  <v-btn icon @click.stop="onAddClient(client.id)">
                    <v-icon color="grey lighten-1">mdi-plus</v-icon>
                  </v-btn>
                </v-list-item-action>
              </v-list-item>
            </v-list>
          </v-col>
        </v-row>
      </v-tab-item>

      <v-tab-item key="claimRules">
        <h4 class="blue--text text--darken-3">User Claims rules</h4>

        <v-row>
          <v-col md="6">
            <v-list style="max-height: 400px" class="overflow-y-auto">
              <v-list-item
                v-for="rule in application.userClaimRules"
                :key="rule.id"
                @click="onClickRule(rule)"
              >
                <v-list-item-avatar>
                  <v-icon small class="green darken-2" dark>
                    mdi-account-check-outline
                  </v-icon>
                </v-list-item-avatar>
                <v-list-item-content>
                  <v-list-item-title v-text="rule.name"></v-list-item-title>
                </v-list-item-content>
              </v-list-item> </v-list
          ></v-col>
        </v-row>
      </v-tab-item>
    </v-tabs>
  </resource-edit-card>
</template>
<script>
import { mapActions } from "vuex";
import { getApplicationById } from "../../services/applicationService";
import { searchUnMappedClients } from "../../services/idResourceService";
import ComboBoxEditor from "../Common/ComboBoxEditor.vue";
import ResourceEditCard from "../ResourceAuthor/ResourceEditCard";

export default {
  components: { ResourceEditCard, ComboBoxEditor },
  props: ["id"],
  watch: {
    id: {
      immediate: true,
      handler: function () {
        this.setApplication();
      },
    },
  },
  data() {
    return {
      loading: false,
      redirectUriDialog: false,
      tab: null,
      valid: null,
      searchText: "",
      application: {
        name: null,
        identityScopes: [],
        apiScopes: [],
        allowedGrantTypes: [],
        enabled: true,
        tenant: null,
        environments: [],
        redirectUris: [],
      },
      clients: [],
      environments: [],
    };
  },
  computed: {
    title: function () {
      return this.application.name;
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
    tenants: function () {
      return this.$store.state.system.tenant.items;
    },
    filteredClients: function () {
      if (this.searchText) {
        const regex = new RegExp(`.*${this.searchText}.*`, "i");
        return this.clients.filter(
          (x) => regex.test(x.name) || regex.test(x.clientId)
        );
      } else {
        return this.clients;
      }
    },
    unusedEnvironments: function () {
      const allEnvironments = this.$store.state.system.environment.items;
      const clients = this.application.clients;
      if (clients) {
        let usedEnvironments = [];
        clients.forEach((c) => {
          usedEnvironments = usedEnvironments.concat(c.environments);
        });
        const unusedEnv = allEnvironments.filter(
          (item) => !usedEnvironments.includes(item.id)
        );
        return unusedEnv;
      } else {
        return allEnvironments;
      }
    },
  },
  methods: {
    ...mapActions("application", [
      "updateApplication",
      "getApplicationById",
      "addClientToApplication",
      "removeClientFromApplication",
      "addEnvironmentToApplication",
    ]),
    ...mapActions("idResource", ["searchUnMappedClients"]),
    async setApplication(application) {
      if (!application) {
        this.loading = true;
        const result = await getApplicationById(this.id);
        application = result.data.application;
        this.loading = false;
      }

      this.application = Object.assign({}, application);
    },
    async onSave(event) {
      this.$refs.form.validate();
      const application = await this.updateApplication({
        id: this.application.id,
        name: this.application.name,
        apiScopes: this.application.apiScopes,
        identityScopes: this.application.identityScopes,
        allowedGrantTypes: this.application.allowedGrantTypes,
        redirectUris: this.application.redirectUris,
      });

      event.done();
      this.setApplication(application);
    },
    async onRemoveClient(id) {
      const application = await this.removeClientFromApplication({
        id: this.id,
        clientId: id,
      });
      this.setApplication(application);
      this.loadClients();
    },
    async onAddClient(id) {
      const application = await this.addClientToApplication({
        id: this.id,
        clientId: id,
      });
      this.setApplication(application);
      this.loadClients();
    },
    onClickClientTab: function () {
      this.loadClients();
    },
    async loadClients() {
      const result = await searchUnMappedClients(this.application.tenant);
      this.clients = result.data.searchUnMappedClients;
    },
    onClickClient: function (client) {
      this.$router.push({ name: "Client_Edit", params: { id: client.id } });
    },
    onClickRule: function (rule) {
      this.$router.push({
        name: "UserClaimRules_Rule_Edit",
        params: { id: rule.id },
      });
    },
    async onAddEnvironment(environmentId) {
      await this.addEnvironmentToApplication({
        id: this.id,
        environments: [environmentId],
      });

      this.$router.replace({
        name: "Application_Created",
      });
    },
  },
};
</script>

<style></style>
