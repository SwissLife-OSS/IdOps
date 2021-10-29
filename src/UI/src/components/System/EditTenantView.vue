<template>
  <resource-edit-card
    :title="title"
    :loading="loading"
    :resource="tenant"
    :tools="[]"
    @Save="onSave"
  >
    <v-form ref="form" v-model="valid" lazy-validation>
      <v-tabs vertical v-model="tab">
        <v-tab>
          <v-icon left> mdi-file-document-edit-outline </v-icon>
        </v-tab>
        <v-tab>
          <v-icon left>mdi-toy-brick-outline</v-icon>
        </v-tab>
        <v-tab>
          <v-icon left>mdi-account-key</v-icon>
        </v-tab>
        <v-tab-item key="core">
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
                v-model="tenant.id"
              ></v-text-field
            ></v-col>
            <v-col md="8"
              ><v-text-field
                label="Description"
                v-model="tenant.description"
              ></v-text-field
            ></v-col>
          </v-row>
          <v-row>
            <v-col md="4">
              <v-text-field
                label="Color"
                v-model="tenant.color"
                hide-details
                class="ma-0 pa-0"
              >
                <template v-slot:append>
                  <v-menu
                    v-model="menu"
                    top
                    nudge-bottom="105"
                    nudge-left="16"
                    :close-on-content-click="false"
                  >
                    <template v-slot:activator="{ on }">
                      <div :style="swatchStyle" v-on="on" />
                    </template>
                    <v-card>
                      <v-card-text class="pa-0">
                        <v-color-picker
                          v-model="tenant.color"
                          show-swatches
                          flat
                        />
                      </v-card-text>
                    </v-card>
                  </v-menu>
                </template>
              </v-text-field>
            </v-col>
            <v-col md="8">
              <v-combobox
                label="Emails"
                v-model="tenant.emails"
                chips
                multiple
                clearable
                deletable-chips
                small-chips
              ></v-combobox>
            </v-col>
          </v-row>
        </v-tab-item>

        <v-tab-item key="modules">
          <div v-for="module in tenant.modules" :key="module.name" class="ma-2">
            <v-card elevation="1" class="mt-4">
              <v-toolbar elevation="0" height="40" color="teal lighten-5">
                <v-toolbar-title>{{ module.name }}</v-toolbar-title>
                <v-spacer> </v-spacer>
                <v-switch class="mt-6" v-model="module.enabled"></v-switch>
              </v-toolbar>
              <v-card-text v-show="module.enabled">
                <div class="mt-4" v-if="module.settings">
                  <v-row v-for="(setting, i) in module.settings" :key="i">
                    <v-col md="12"
                      ><v-textarea
                        outlined
                        style="font-family: consolas"
                        :label="setting.name"
                        v-model="setting.value"
                        rows="3"
                      ></v-textarea
                    ></v-col>
                  </v-row>
                </div>
              </v-card-text>
            </v-card></div
        ></v-tab-item>
        <v-tab-item key="roles">
          <div class="mt-4">
            <v-row v-for="(map, i) in tenant.roleMappings" :key="i">
              <v-col md="3"
                ><v-select
                  dense
                  label="Role"
                  :items="roleMapNames"
                  item-value="name"
                  item-text="name"
                  v-model="map.role"
                ></v-select
              ></v-col>
              <v-col md="3">
                <v-select
                  label="Environment"
                  dense
                  v-model="map.environmentId"
                  :items="environments"
                  :disabled="!roleEnvironmentEnabled(map)"
                  item-text="name"
                  item-value="id"
                  clearable
                ></v-select
              ></v-col>
              <v-col md="5"
                ><v-text-field
                  dense
                  label="Value"
                  v-model="map.claimValue"
                ></v-text-field
              ></v-col>
              <v-col md="1">
                <v-icon @click="onRemoveMapping(i)">mdi-delete-outline</v-icon>
              </v-col>
            </v-row>

            <v-toolbar elevation="0">
              <v-spacer></v-spacer>
              <v-btn text color="primary" @click="onAddNewMapping">
                Add Mapping<v-icon>mdi-pencil-plus</v-icon></v-btn
              >
            </v-toolbar>
          </div></v-tab-item
        >
      </v-tabs>
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
        this.setTenant();
      },
    },
  },
  data() {
    return {
      tab: null,
      valid: null,
      loading: false,
      mask: "!#XXXXXXXX",
      menu: false,
      tenant: {
        id: null,
        description: null,
        modules: [],
        roleMappings: [],
        color: null,
        emails: [],
      },
      roleMapNames: [
        { name: "Admin", byEnvironment: false },
        { name: "Publish", byEnvironment: true },
      ],
    };
  },
  computed: {
    title: function () {
      return this.isNew ? "Tenant" : this.id;
    },
    isNew: function () {
      return !this.id;
    },
    moduleDefinition: function () {
      return [
        { name: "DataConnector", settings: ["Connectors"] },
        { name: "AuthProviders", settings: ["ProviderNames"] },
        { name: "PersonalAccessTokens", settings: ["Sources"] },
      ];
    },
    environments: function () {
      return this.$store.state.system.environment.items;
    },
    swatchStyle() {
      return {
        backgroundColor: this.tenant.color,
        cursor: "pointer",
        height: "30px",
        width: "30px",
        "margin-bottom": "5px",
        borderRadius: this.menu ? "50%" : "4px",
        transition: "border-radius 200ms ease-in-out",
      };
    },
  },
  methods: {
    ...mapActions("system", ["saveTenant"]),
    setTenant: function () {
      if (this.id) {
        const tenant = this.$store.state.system.tenant.items.find(
          (x) => x.id === this.id
        );
        this.tenant = Object.assign({}, tenant);
        this.setModules();
        delete this.tenant.__typename;
      } else {
        this.resetForm();
      }
    },
    setModules: function () {
      const modules = [];
      for (let i = 0; i < this.moduleDefinition.length; i++) {
        const md = this.moduleDefinition[i];
        const existing = this.tenant.modules.find((x) => x.name === md.name);
        const mod = {
          name: md.name,
          enabled: false,
        };

        if (existing) {
          mod.settings = existing.settings;
          mod.enabled = true;
        } else {
          mod.settings = md.settings.map((s) => {
            return { name: s, value: "" };
          });
        }

        modules.push(mod);
      }
      this.tenant.modules = modules;
    },
    onAddNewMapping: function () {
      this.tenant.roleMappings.push({
        role: "",
        environmentId: null,
        claimValue: "",
      });
    },
    onRemoveMapping: function (index) {
      this.tenant.roleMappings.splice(index, 1);
    },
    roleEnvironmentEnabled: function (map) {
      var mapping = this.roleMapNames.find((x) => x.name === map.role);
      if (mapping) {
        return mapping.byEnvironment;
      }

      return false;
    },
    onColorChanged: function () {
      this.tenant.color = this.colorPicker.hex;
    },
    resetForm: function () {
      this.tenant = {
        id: null,
        description: null,
        modules: [],
        roleMappings: [],
        emails: [],
      };
      //this.$refs.form.resetValidation();
    },

    async onSave(event) {
      this.$refs.form.validate();

      const saveTenant = Object.assign({}, this.tenant, {
        modules: this.tenant.modules
          .filter((x) => x.enabled)
          .map((m) => {
            return {
              name: m.name,
              color: this.color,
              settings: m.settings.map((s) => {
                return { name: s.name, value: s.value };
              }),
            };
          }),
        roleMappings: this.tenant.roleMappings
          .filter((x) => x.claimValue.length > 0)
          .map((m) => {
            return {
              role: m.role,
              environmentId: m.environmentId,
              claimValue: m.claimValue,
            };
          }),
      });

      const tenant = await this.saveTenant(saveTenant);

      event.done();
      this.setTenant(tenant);

      if (tenant && this.$route.name === "Tenant_New") {
        this.$router.replace({
          name: "Tenant_Edit",
          params: { id: tenant.id },
        });
      }
    },
  },
};
</script>

<style>
</style>
