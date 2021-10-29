<template>
  <resource-edit-card
    :title="title"
    :loading="loading"
    :resource="resource"
    :tools="['PUBLISH', 'AUDIT']"
    @Save="onSave"
  >
    <v-form ref="form" v-model="valid" lazy-validation>
      <v-row>
        <v-col md="5"
          ><v-text-field label="Name" v-model="resource.name"></v-text-field
        ></v-col>
        <v-col md="7"
          ><v-text-field
            label="Display name"
            v-model="resource.displayName"
          ></v-text-field
        ></v-col>
      </v-row>
      <v-row>
        <v-col md="12">
          <v-combobox
            label="User Claims"
            v-model="resource.userClaims"
            :items="[]"
            chips
            multiple
            clearable
            small-chips
            deletable-chips
          ></v-combobox
        ></v-col>
      </v-row>
      <v-row>
        <v-col md="12"
          ><v-text-field
            label="Description"
            v-model="resource.description"
          ></v-text-field
        ></v-col>
      </v-row>
      <v-row>
        <v-col md="6">
          <v-select
            label="IdentityServer group"
            v-model="resource.identityServerGroupId"
            :items="identityServerGroups"
            item-text="name"
            item-value="id"
          ></v-select>
        </v-col>
        <v-col md="6">
          <v-autocomplete
            label="Tenants"
            v-model="resource.tenants"
            :items="tenants"
            chips
            multiple
            small-chips
            deletable-chips
          ></v-autocomplete>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="2"
          ><v-switch label="Enabled" v-model="resource.enabled"></v-switch
        ></v-col>
        <v-col md="2"
          ><v-switch label="Required" v-model="resource.required"></v-switch
        ></v-col>
        <v-col md="2"
          ><v-switch
            label="Discovery"
            v-model="resource.showInDiscoveryDocument"
          ></v-switch
        ></v-col>
        <v-col md="2"
          ><v-switch label="Emphasize" v-model="resource.emphasize"></v-switch
        ></v-col>
      </v-row>
    </v-form>
  </resource-edit-card>
</template>
<script>
import { mapActions } from "vuex";
import ResourceEditCard from "./ResourceEditCard.vue";

export default {
  components: { ResourceEditCard },
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
      loading: false,
      valid: null,
      resource: {
        name: null,
        userClaims: [],
        displayName: null,
        description: null,
        enabled: true,
        showInDiscoveryDocument: false,
        required: false,
        emphasize: false,
        identityServerGroupId: null,
        tenants: []
      }
    };
  },
  computed: {
    title: function() {
      if (this.id) {
        return this.resource.name;
      }
      return "Identity Resource";
    },
    tenants: function() {
      if (this.resource.identityServerGroupId) {
        const group = this.$store.state.system.identityServerGroup.items.find(
          x => x.id === this.resource.identityServerGroupId
        );

        if (group) {
          return group.tenants;
        }
      }
      return [];
    },
    identityServerGroups: function() {
      return this.$store.state.system.identityServerGroup.items;
    }
  },
  methods: {
    ...mapActions("idResource", ["saveIdentityResource"]),
    setResource: function() {
      if (this.id) {
        const resource = this.$store.state.idResource.identityResource.items.find(
          x => x.id === this.id
        );

        this.resource = Object.assign({}, resource);
        delete this.resource.__typename;
      } else {
        this.resetForm();
      }
    },
    resetForm: function() {
      this.resource = {
        name: null,
        displayName: null,
        description: null,
        enabled: true,
        showInDiscoveryDocument: false,
        required: false,
        emphasize: false,
        userClaims: [],
        identityServerGroupId: null,
        tenants: []
      };
    },
    async onSave(event) {
      this.$refs.form.validate();
      // eslint-disable-next-line no-unused-vars
      const { identityServerGroup, ...cleaned } = this.resource;
      const resource = await this.saveIdentityResource(cleaned);

      event.done(true);

      this.setResource(resource);
      if (resource && this.$route.name === "IdentityResource_New") {
        this.$router.replace({
          name: "IdentityResource_Edit",
          params: { id: resource.id }
        });
      }
    }
  }
};
</script>

<style></style>
