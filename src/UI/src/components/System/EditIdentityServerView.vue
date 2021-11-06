<template>
  <resource-edit-card
    :title="title"
    :loading="loading"
    :resource="server"
    :tools="[]"
    @Save="onSave"
  >
    <v-form ref="form" v-model="valid" lazy-validation>
      <v-tabs vertical v-model="tab" @change="onTabChange">
        <v-tab> <v-icon left> mdi-format-list-checkbox </v-icon> </v-tab>
        <v-tab>
          <v-icon left> mdi-code-json </v-icon>
        </v-tab>
        <v-tab>
          <v-icon left> mdi-key-outline </v-icon>
        </v-tab>
        <v-tab>
          <v-icon left> mdi-bomb</v-icon>
        </v-tab>
        <v-tab-item key="core">
          <v-row>
            <v-col md="4"
              ><v-text-field
                required
                label="Name"
                :rules="[
                  (v) => !!v || 'Required',
                  (v) => (v && v.length > 2) || 'Must be at least 3 characters',
                ]"
                v-model="server.name"
              ></v-text-field
            ></v-col>
            <v-col md="4">
              <v-select
                label="Group"
                v-model="server.groupId"
                :items="groups"
                item-text="name"
                item-value="id"
              ></v-select>
            </v-col>
            <v-col md="4">
              <v-select
                label="Environment"
                v-model="server.environmentId"
                :items="environments"
                item-text="name"
                item-value="id"
              ></v-select>
            </v-col>
          </v-row>
          <v-row>
            <v-col md="12">
              <v-text-field
                label="Url"
                v-model="server.url"
                :rules="[(v) => !!v || 'Required']">
              </v-text-field>
            </v-col>
          </v-row>
        </v-tab-item>
        <v-tab-item key="discovery">
          <div class="json-wrapper">
            <vue-json-pretty
              :data="discovery"
              :style="{ 'max-height': $vuetify.breakpoint.height - 220 + 'px' }"
              class="overflow-y-auto mt-0"
            >
            </vue-json-pretty>
          </div>
        </v-tab-item>
        <v-tab-item key="keys">
          <v-list style="max-height: 400px" class="overflow-y-auto">
            <v-list-item v-for="key in keys" :key="key.kid">
              <v-list-item-avatar>
                <v-icon small class="yellow darken-3" dark> mdi-key </v-icon>
              </v-list-item-avatar>

              <v-list-item-content>
                <v-list-item-title class="font-weight-bold mb-2"
                  >{{ key.alg }} | {{ key.kid }}
                </v-list-item-title>
                <v-row v-if="key.subject">
                  <v-col md="2">Subject</v-col>
                  <v-col md="10">{{ key.subject }}</v-col>
                </v-row>
                <v-row v-if="key.serialNumber">
                  <v-col md="2">Serial</v-col>
                  <v-col md="10">{{ key.serialNumber }}</v-col>
                </v-row>
                <v-row v-if="key.thumbprint">
                  <v-col md="2">Thumbprint</v-col>
                  <v-col md="10">{{ key.thumbprint }}</v-col>
                </v-row>
                <v-row v-if="key.validUntil">
                  <v-col md="2">Valid Until</v-col>
                  <v-col md="10">{{ key.validUntil | dateformat }}</v-col>
                </v-row>
              </v-list-item-content>
            </v-list-item>
          </v-list></v-tab-item
        >
        <v-tab-item></v-tab-item>
      </v-tabs>
    </v-form>
  </resource-edit-card>
</template>

<script>
import { mapActions } from "vuex";
import ResourceEditCard from "../ResourceAuthor/ResourceEditCard.vue";
import VueJsonPretty from "vue-json-pretty";
import "vue-json-pretty/lib/styles.css";
import { getIdentityServer } from "../../services/systemService";
export default {
  components: { ResourceEditCard, VueJsonPretty },
  props: ["id"],
  watch: {
    id: {
      immediate: true,
      handler: function () {
        this.setServer();
      },
    },
  },
  data() {
    return {
      tab: null,
      valid: null,
      discovery: {},
      loading: false,
      keys: [],
      server: {
        id: null,
        name: null,
        groupId: null,
        environmentId: null,
        url: null,
      },
    };
  },
  computed: {
    title: function () {
      return this.isNew ? "IdentityServer" : this.server.name;
    },
    isNew: function () {
      return !this.id;
    },
    environments: function () {
      return this.$store.state.system.environment.items;
    },
    groups: function () {
      return this.$store.state.system.identityServerGroup.items;
    },
  },
  methods: {
    ...mapActions("system", ["saveIdentityServer"]),
    setServer: function () {
      if (this.id) {
        const tenant = this.$store.state.system.identityServer.items.find(
          (x) => x.id === this.id
        );
        this.server = Object.assign({}, tenant);
        delete this.server.__typename;
      } else {
        this.resetForm();
      }
      this.keys = [];
    },
    resetForm: function () {
      this.server = {
        id: null,
        name: null,
        environmentId: null,
        groupId: null,
        url: null,
      };
      //this.$refs.form.resetValidation();
    },

    async onSave(event) {
      this.$refs.form.validate();

      const server = await this.saveIdentityServer(this.server);

      event.done();
      this.setServer(server);

      if (server && this.$route.name === "IdentityServer_New") {
        this.$router.replace({
          name: "IdentityServer_Edit",
          params: { id: server.id },
        });
      }
    },
    onTabChange: function (tab) {
      if (tab === 1 && !this.discovery.issuer && this.server.id) {
        this.loading = true;
        fetch(`/api/discovery/${this.server.id}`, {
          credentials: "same-origin",
        })
          .then((r) => r.json())
          .then((data) => {
            this.discovery = data;
            this.loading = false;
          });
      } else if (tab === 2 && this.keys.length === 0) {
        this.loading = true;

        getIdentityServer(this.server.id).then((res) => {
          this.keys = res.data.identityServer.keys;
          this.loading = false;
        });
      }
    },
  },
};
</script>

<style scoped>
</style>
