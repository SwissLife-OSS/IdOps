<template>
  <v-row>
    <v-col md="6">
      <h4 class="blue--text text--darken-3">Connectors</h4>
      <v-list style="max-height: 400px" class="overflow-y-auto">
        <v-list-item
          v-for="(connector, i) in connectors"
          :key="connector.id"
          @click="onSelect(i)"
        >
          <v-list-item-avatar>
            <v-icon class="deep-purple darken-3" dark> mdi-power-plug </v-icon>
          </v-list-item-avatar>

          <v-list-item-content>
            <v-list-item-title v-text="connector.name"></v-list-item-title>
          </v-list-item-content>
          <v-list-item-action>
            <v-row>
              <v-col>
                <v-btn icon v-if="i > 0">
                  <v-icon @click.stop="onUp(i)">mdi-arrow-up</v-icon>
                </v-btn>
              </v-col>
              <v-col>
                <v-btn icon @click="onRemoveConnector(i)">
                  <v-icon color="grey lighten-1">mdi-delete-outline</v-icon>
                </v-btn>
              </v-col>
            </v-row>
          </v-list-item-action>
        </v-list-item>
      </v-list>
      <v-toolbar elevation="0" v-if="!connector.isNew">
        <v-spacer> </v-spacer>
        <v-btn text color="primary" @click="onAddNew"
          ><v-icon> mdi-plus</v-icon> Add new</v-btn
        >
      </v-toolbar>
    </v-col>
    <v-col md="6">
      <v-card elevation="2">
        <v-card-title>
          {{ connector.isNew ? "Add new connector" : `Edit ${connector.name}` }}
        </v-card-title>
        <v-card-text>
          <v-row>
            <v-col md="12">
              <v-select
                :items="connectorTypes"
                :disabled="!connector.isNew"
                label="Connector Type"
                v-model="connector.name"
                item-text="type"
                item-value="type"
              ></v-select
            ></v-col>
          </v-row>
          <v-row>
            <v-col md="12">
              <v-select
                :items="connectorProfileTypes"
                label="Profile Types"
                v-model="connector.profileTypeFilter"
                multiple
                item-text="text"
                item-value="value"
              ></v-select
            ></v-col>
          </v-row>
          <h4 class="blue--text text--darken-3">Properties</h4>
          <div class="mt-6" v-if="propertyNames.length > 0">
            <v-row v-for="(prop, i) in connector.properties" :key="i">
              <v-col md="4"
                ><v-text-field
                  dense
                  label="Name"
                  disabled
                  v-model="prop.name"
                ></v-text-field
              ></v-col>
              <v-col md="8"
                ><v-text-field
                  dense
                  label="Value"
                  v-model="prop.value"
                  clearable
                ></v-text-field
              ></v-col>
            </v-row>
          </div>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            color="primary"
            elevation="2"
            :disabled="!connector.name"
            @click="onSave"
            >{{ connector.isNew ? "Add" : "Save" }}</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-col>
  </v-row>
</template>

<script>
export default {
  props: ["value", "tenant"],
  data() {
    return {
      connectors: [],
      connector: {
        name: "",
        isNew: true,
        profileTypeFilter: ["USER_INFO"],
        properties: [],
      },
    };
  },
  watch: {
    "connector.name": function () {
      if (this.connector.properties.length === 0) {
        this.connector.properties = this.propertyNames.map((x) => {
          return { name: x, value: "" };
        });
      }
    },
    value: {
      immediate: true,
      handler: function (newValue) {
        this.connectors = newValue;
        if (this.connectors && this.connectors.length > 0) {
          this.connector = this.connectors[0];
        } else {
          this.connectors = [];
          this.onAddNew();
        }
      },
    },
  },
  computed: {
    connectorTypes: function () {
      const setting = this.$store.getters["system/getModuleSetting"](
        this.tenant,
        "DataConnector",
        "Connectors"
      );
      const dcs = eval(setting);

      return dcs;
    },
    propertyNames: function () {
      if (this.connector.name) {
        var type = this.connectorTypes.find(
          (x) => x.type === this.connector.name
        );
        return type.propertyNames;
      }
      return [];
    },
    connectorProfileTypes: function () {
      return this.$store.state.idResource.enums.connectorProfileTypes.values;
    },
  },
  methods: {
    onSelect: function (index) {
      this.connector = Object.assign(this.connectors[index], { isNew: false });
    },
    onSave: function () {
      if (this.connector.isNew) {
        this.connectors.push({
          name: this.connector.name,
          properties: this.connector.properties,
          profileTypeFilter: this.connector.profileTypeFilter,
          isNew: false,
        });

        this.connector = this.connectors.find(
          (x) => x.name === this.connector.name
        );
      } else {
        const index = this.connectors.findIndex(
          (x) => x.name === this.connector.name
        );

        this.connectors[index] = Object.assign({}, this.connector);
      }

      this.setModel();
    },
    onAddNew: function () {
      this.connector = {
        name: "",
        isNew: true,
        properties: [],
        profileTypeFilter: ["USER_INFO"],
      };
    },
    onRemoveConnector: function (index) {
      this.connectors.splice(index, 1);
    },
    onUp: function (index) {
      this.connectors.splice(index - 1, 0, this.connectors.splice(index, 1)[0]);
      this.setModel();
    },
    setModel() {
      const cons = [];
      //debugger; // eslint-disable-line no-debugger

      for (let i = 0; i < this.connectors.length; i++) {
        const conn = this.connectors[i];
        if (!conn.isNew) {
          if (!conn.properties) {
            conn.properties = [];
          }
          const newConn = {
            name: conn.name,
            enabled: true,
            profileTypeFilter: conn.profileTypeFilter,
            properties: conn.properties.filter((x) => {
              return x.value.length > 0;
            }),
          };
          cons.push(newConn);
        }
      }
      this.$emit("input", cons);
    },
  },
};
</script>

<style>
</style>