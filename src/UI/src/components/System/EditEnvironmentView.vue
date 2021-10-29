<template>
  <resource-edit-card
    :title="title"
    :loading="loading"
    :resource="environment"
    :tools="[]"
    @Save="onSave"
  >
    <v-form ref="form" v-model="valid" lazy-validation>
      <v-row>
        <v-col md="8"
          ><v-text-field
            required
            label="name"
            :rules="[(v) => !!v || 'Required']"
            v-model="environment.name"
          ></v-text-field
        ></v-col>
        <v-col md="4"
          ><v-text-field
            label="Order"
            type="number"
            v-model="environment.order"
          ></v-text-field
        ></v-col>
      </v-row>
    </v-form>
  </resource-edit-card>
</template>

<script>
import { mapActions } from "vuex";
import ResourceEditCard from "../ResourceAuthor/ResourceEditCard.vue";
export default {
  components: { ResourceEditCard },
  props: ["id"],
  watch: {
    id: {
      immediate: true,
      handler: function () {
        this.setEnvironment();
      },
    },
  },
  data() {
    return {
      valid: null,
      loading: false,
      environment: {
        id: null,
        name: null,
        order: 0,
      },
    };
  },
  computed: {
    title: function () {
      return this.isNew ? "Environment" : this.environment.name;
    },
    isNew: function () {
      return !this.id;
    },
  },
  methods: {
    ...mapActions("system", ["saveEnvironment"]),
    setEnvironment: function () {
      if (this.id) {
        const environment = this.$store.state.system.environment.items.find(
          (x) => x.id === this.id
        );
        this.environment = Object.assign({}, environment);
        delete this.environment.__typename;
      } else {
        this.resetForm();
      }
    },
    resetForm: function () {
      this.environment = {
        id: null,
        name: null,
        order: 0,
      };
      //this.$refs.form.resetValidation();
    },
    async onSave(event) {
      this.$refs.form.validate();
      const environment = await this.saveEnvironment(
        Object.assign({}, this.environment, { order: +this.environment.order })
      );
      event.done();
      this.setEnvironment(environment);

      if (environment && this.$route.name === "Environment_New") {
        this.$router.replace({
          name: "Environment_Edit",
          params: { id: environment.id },
        });
      }
    },
  },
};
</script>

<style>
</style>