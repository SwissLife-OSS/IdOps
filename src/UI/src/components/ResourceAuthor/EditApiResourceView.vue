<template>
  <resource-edit-card
    :title="title"
    :loading="loading"
    :resource="resource"
    :tools="['DEPENDENCIES', 'PUBLISH', 'AUDIT', 'LOG']"
    type="ApiResource"
    @Save="onSave"
  >
    <template>
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
            <v-row>
              <v-col md="5"
                ><v-text-field
                  required
                  label="Name"
                  v-model="resource.name"
                ></v-text-field
              ></v-col>
              <v-col md="5"
                ><v-text-field
                  label="Display name"
                  v-model="resource.displayName"
                ></v-text-field
              ></v-col>
              <v-col md="2"
                ><v-switch label="Enabled" v-model="resource.enabled"></v-switch
              ></v-col>
            </v-row>

            <v-row>
              <v-col md="3">
                <v-select
                  label="Tenant"
                  v-model="resource.tenant"
                  :items="tenants"
                  item-text="id"
                  item-value="id"
                ></v-select>
              </v-col>
              <v-col md="9">
                <v-autocomplete
                  label="Scopes"
                  v-model="resource.scopes"
                  :items="allScopes"
                  item-text="name"
                  item-value="id"
                  chips
                  multiple
                  small-chips
                  deletable-chips
                ></v-autocomplete> </v-col
            ></v-row>
            <v-row>
              <v-col md="12"
                ><v-text-field
                  label="Description"
                  v-model="resource.description"
                ></v-text-field
              ></v-col> </v-row
          ></v-tab-item>
          <v-tab-item key="secrets">
            <v-row>
              <v-col md="6">
                <secret-list
                  :id="id"
                  :secrets="resource.apiSecrets"
                  @Remove="onRemoveSecret"
                ></secret-list>
              </v-col>
              <v-col md="6">
                <v-row>
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
          <v-tab-item key="danger "></v-tab-item>
        </v-tabs>
      </v-form> </template
  ></resource-edit-card>
</template>

<script>
import { mapActions } from "vuex";
import ResourceEditCard from "./ResourceEditCard.vue";
import AddSecret from "./AddSecret.vue";
import SecretList from "./SecretList.vue";
export default {
  components: { AddSecret, SecretList, ResourceEditCard },
  props: ["id"],
  watch: {
    id: {
      immediate: true,
      handler: function() {
        this.setResource();
      }
    }
  },
  data() {
    return {
      valid: null,
      loading: false,
      tab: null,
      resource: {
        name: null,
        displayName: null,
        description: null,
        scopes: [],
        enabled: true,
        tenant: null
      }
    };
  },
  computed: {
    allScopes: function() {
      if (this.resource.tenant) {
        return this.$store.state.idResource.apiScope.items.filter(
          x => x.tenant === this.resource.tenant
        );
      }
      return [];
    },
    title: function() {
      if (this.id) {
        return this.resource.name;
      }
      return "API Resource";
    },
    tenants: function() {
      return this.$store.state.system.tenant.items;
    }
  },
  methods: {
    ...mapActions("idResource", [
      "saveApiResource",
      "addApiSecret",
      "removeApiSecret"
    ]),
    setResource: function() {
      if (this.id) {
        const resource = this.$store.state.idResource.apiResource.items.find(
          x => x.id === this.id
        );
        this.resource = Object.assign({}, resource, {
          scopes: resource.scopes.map(x => x.id)
        });
        delete this.resource.__typename;
      } else {
        this.resetForm();
      }
    },
    async onAddSecret(secret, callback) {
      const result = await this.addApiSecret(secret);
      this.setResource(result.apiResource);
      callback(result.secret);
    },
    async onRemoveSecret(id) {
      const apiResource = await this.removeApiSecret({
        apiResourceId: this.id,
        id: id
      });

      this.setResource(apiResource);
    },
    resetForm: function() {
      this.resource = {
        name: null,
        displayName: null,
        description: null,
        scopes: [],
        enabled: true,
        tenant: null
      };
    },
    async onSave(event) {
      this.$refs.form.validate();
      // eslint-disable-next-line no-unused-vars
      const { apiSecrets, tenantInfo, ...cleaned } = this.resource;
      const resource = await this.saveApiResource(cleaned);
      event.done(true);
      this.setResource(resource);
      if (resource && this.$route.name === "ApiResource_New") {
        this.$router.replace({
          name: "ApiResource_Edit",
          params: { id: resource.id }
        });
      }
    }
  }
};
</script>

<style></style>
