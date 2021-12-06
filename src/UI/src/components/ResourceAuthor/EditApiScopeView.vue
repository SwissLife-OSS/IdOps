<template>
  <resource-edit-card
    :title="title"
    :loading="loading"
    :resource="scope"
    :tools="['PUBLISH', 'AUDIT', 'LOG']"
    @Save="onSave"
  >
    <v-form ref="form" v-model="valid" lazy-validation>
      <v-row>
        <v-col md="5"
          ><v-text-field
            required
            label="Name"
            :rules="[
              (v) => !!v || 'Required',
              (v) => (v && v.length > 2) || 'Must be at least 3 characters',
            ]"
            v-model="scope.name"
          ></v-text-field
        ></v-col>
        <v-col md="5">
          <v-select
            label="Tenant"
            :disabled="!isNew"
            v-model="scope.tenant"
            :items="tenants"
            item-text="id"
            item-value="id"
          ></v-select>
        </v-col>
        <v-col md="2"
          ><v-switch label="Enabled" v-model="scope.enabled"></v-switch
        ></v-col>
      </v-row>
      <v-row>
        <v-col md="5"
          ><v-text-field
            label="Display name"
            required
            v-model="scope.displayName"
            :rules="[
              (v) => !!v || ' Required',
              (v) => (v && v.length > 4) || 'Must be at least 5 characters',
            ]"
          ></v-text-field
        ></v-col>
        <v-col md="5"
          ><v-text-field
            label="Description"
            v-model="scope.description"
          ></v-text-field
        ></v-col>
        <v-col md="2"
          ><v-switch
            label="Discovery"
            v-model="scope.showInDiscoveryDocument"
          ></v-switch
        ></v-col>
      </v-row>
    </v-form>
  </resource-edit-card>
</template>

<script>
import { mapActions } from "vuex";
import ResourceEditCard from "./ResourceEditCard.vue";

export default {
  components: {
    ResourceEditCard,
  },
  props: ["id"],
  created() {
    this.setScope();
  },
  watch: {
    id: {
      immediate: true,
      handler: function () {
        this.setScope();
      },
    },
  },
  data() {
    return {
      valid: null,
      loading: false,
      scope: {
        id: null,
        name: null,
        displayName: null,
        description: null,
        showInDiscoveryDocument: false,
        enabled: true,
        tenant: null,
      },
    };
  },
  computed: {
    isNew: function () {
      return !this.id;
    },
    title: function () {
      if (this.id) {
        return this.scope.name;
      }

      return "API Scope";
    },
    tenants: function () {
      return this.$store.state.system.tenant.items;
    },
  },
  methods: {
    ...mapActions("idResource", ["saveApiScope"]),
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
      if (this.id) {
        this.$refs.form.resetValidation();
      }
    },
    async onSave(event) {
      this.$refs.form.validate();
      delete this.scope.tenantInfo;
      const scope = await this.saveApiScope(this.scope);
      event.done(true);
      this.setScope(scope);
      if (scope && this.$route.name === "ApiScope_New") {
        this.$router.replace({
          name: "ApiScope_Edit",
          params: { id: scope.id },
        });
      }
    },
  },
};
</script>

<style>
</style>
