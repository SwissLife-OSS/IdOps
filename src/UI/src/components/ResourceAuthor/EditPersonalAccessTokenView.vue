<template>
  <resource-edit-card
    :title="personalAccessToken.title"
    :loading="loading"
    :resource="personalAccessToken"
    :tools="['DEPENDENCIES', 'PUBLISH', 'INSIGHTS', 'AUDIT']"
    type="PersonalAccessToken"
    @Save="onSave"
  >
    <v-form ref="form" v-model="valid" lazy-validation>
      <v-tabs vertical v-model="tab">
        <v-tab> <v-icon left> mdi-format-list-checkbox </v-icon> </v-tab>
        <v-tab>
          <v-icon left> mdi-key-outline </v-icon>
        </v-tab>
        <v-tab>
          <v-icon left> mdi-bomb</v-icon>
        </v-tab>
        <v-tab-item key="core">
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
                    disabled
                  ></v-autocomplete
                ></v-col>
                <v-col md="2">
                  <v-select
                    label="Tenant"
                    v-model="personalAccessToken.tenant"
                    :items="tenants"
                    item-text="id"
                    item-value="id"
                    disabled
                  ></v-select
                ></v-col>
                <v-col md="2">
                  <v-autocomplete
                    label="Hash Algorithm"
                    v-model="personalAccessToken.hashAlgorithm"
                    :items="hashAlgorithms"
                    item-text="name"
                    item-value="id"
                    disabled
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
        </v-tab-item>
        <v-tab-item key="secrets">
          <pat-secret-list
            :id="id"
            :secrets="personalAccessToken.tokens"
            @Remove="onRemoveSecret"
          ></pat-secret-list>

          <add-pat-secret
            :id="this.id"
            @AddSecret="onAddSecret"
          ></add-pat-secret>
        </v-tab-item>
        <v-tab-item key="danger"></v-tab-item>
      </v-tabs>
    </v-form>
  </resource-edit-card>
</template>

<script>
import { mapActions } from "vuex";
import { getPersonalAccessTokenById } from "../../services/idResourceService";
import ResourceEditCard from "./ResourceEditCard.vue";
import AddPatSecret from "./AddPatSecret.vue";
import PatSecretList from "./PatSecretList.vue";

export default {
  components: {
    AddPatSecret,
    PatSecretList,
    ResourceEditCard
  },
  props: ["id"],
  watch: {
    id: {
      immediate: true,
      handler: function() {
        this.setPersonalAccessToken();
      }
    }
  },
  data() {
    return {
      loading: false,
      tab: null,
      redirectUriDialog: false,
      valid: null,
      personalAccessToken: {
        userName: null,
        expiresAt: null,
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
    ...mapActions("idResource", [
      "updatePersonalAccessToken",
      "addPersonalAccessTokenSecret",
      "removePersonalAccessTokenSecret"
    ]),
    async setPersonalAccessToken() {
      this.loading = true;
      const result = await getPersonalAccessTokenById(this.id);
      this.loading = false;
      const { personalAccessToken } = result.data;
      this.personalAccessToken = Object.assign({}, personalAccessToken);
      this.personalAccessToken.claimsExtensions = this.personalAccessToken.claimsExtensions.map(
        x => {
          // eslint-disable-next-line no-unused-vars
          const { __typename, ...other } = x;
          return other;
        }
      );

      delete this.personalAccessToken.__typename;
    },
    async onAddSecret(secret, callback) {
      const result = await this.addPersonalAccessTokenSecret(secret);
      this.setPersonalAccessToken(result.token);
      callback(result.secret);
    },
    async onRemoveSecret(id) {
      const personalAccessToken = await this.removePersonalAccessTokenSecret({
        tokenId: this.id,
        secretId: id
      });

      this.setPersonalAccessToken(personalAccessToken);
    },
    onAddNewClaim: function() {
      this.personalAccessToken.claimsExtensions.push({
        type: "",
        value: ""
      });
    },
    onRemoveClaim: function(index) {
      this.personalAccessToken.claimsExtensions.splice(index, 1);
    },
    async onSave(event) {
      this.$refs.form.validate();
      await this.updatePersonalAccessToken({
        id: this.personalAccessToken.id,
        userName: this.personalAccessToken.userName,
        source: this.personalAccessToken.source,
        allowedApplicationIds: this.personalAccessToken.allowedApplicationIds,
        allowedScopes: this.personalAccessToken.allowedScopes.filter(x =>
          this.allowedScopes.find(y => y.id == x)
        ),
        claimsExtensions: this.personalAccessToken.claimsExtensions
      });
      this.setPersonalAccessToken();
      event.done(true);
    }
  }
};
</script>

<style></style>
