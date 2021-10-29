<template>
  <resource-edit-card
    :title="title"
    :loading="loading"
    :resource="grantType"
    :hideTools="true"
    @Save="onSave"
  >
    <v-form ref="form" v-model="valid" lazy-validation>
      <v-row>
        <v-col md="4"
          ><v-text-field
            required
            label="Id"
            :disabled="!isNew"
            :rules="[
              (v) => !!v || 'Required',
              (v) => (v && v.length > 2) || 'Must be at least 3 characters',
            ]"
            v-model="grantType.id"
          ></v-text-field
        ></v-col>
        <v-col md="8"
          ><v-text-field label="Name" v-model="grantType.name"></v-text-field
        ></v-col>
      </v-row>
      <v-row>
        <v-col md="4"
          ><v-switch label="Is Custom" v-model="grantType.isCustom"></v-switch
        ></v-col>
        <v-col md="8">
          <v-autocomplete
            label="Tenants"
            :disabled="!isNew"
            v-model="grantType.tenants"
            :items="tenants"
            item-text="id"
            item-value="id"
            chips
            multiple
            small-chips
            deletable-chips
          ></v-autocomplete>
        </v-col>
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
      handler: function () {
        this.setGrantType();
      },
    },
  },
  data() {
    return {
      valid: null,
      loading: false,
      grantType: {
        id: null,
        name: null,
        isCustom: false,
        tenants: [],
      },
    };
  },
  computed: {
    title: function () {
      return this.isNew ? "Grant Type" : this.grantType.name;
    },
    isNew: function () {
      return !this.id;
    },
    tenants: function () {
      return this.$store.state.system.tenant.items;
    },
  },
  methods: {
    ...mapActions("idResource", ["saveGrantType"]),
    setGrantType: function () {
      if (this.id) {
        const grantType = this.$store.state.idResource.grantType.items.find(
          (x) => x.id === this.id
        );
        this.grantType = Object.assign({}, grantType);
        delete this.grantType.__typename;
      } else {
        this.resetForm();
      }
    },
    resetForm: function () {
      this.grantType = {
        id: null,
        name: null,
        isCustom: false,
        tenants: [],
      };
      //this.$refs.form.resetValidation();
    },
    async onSave(event) {
      this.$refs.form.validate();
      const grantType = await this.saveGrantType(this.grantType);

      event.done();
      this.setGrantType(grantType);

      if (grantType && this.$route.name === "GrantType_New") {
        this.$router.replace({
          name: "GrantType_Edit",
          params: { id: grantType.id },
        });
      }
    },
  },
};
</script>

<style>
</style>