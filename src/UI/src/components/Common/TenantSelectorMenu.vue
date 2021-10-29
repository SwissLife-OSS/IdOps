<template>
  <v-menu
    v-model="menu"
    :close-on-content-click="false"
    :nudge-width="150"
    offset-x
    rounded="2"
    top
  >
    <template v-slot:activator="{ on, attrs }">
      <v-btn text v-bind="attrs" v-on="on"
        >Tenants ({{ selectedTenants.length }})</v-btn
      >
    </template>
    <v-list dense two-line>
      <v-list-item-group v-model="selectedTenants" multiple>
        <template v-for="tenant in tenants">
          <v-list-item class="my-1" :key="tenant.id">
            <template v-slot:default="{ active }">
              <v-list-item-action>
                <v-checkbox :input-value="active"></v-checkbox>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-title>{{ tenant.id }}</v-list-item-title>
                <v-list-item-subtitle>{{
                  tenant.description
                }}</v-list-item-subtitle>
                <div
                  class="tenant-box"
                  :style="{ 'background-color': tenant.color }"
                ></div>
              </v-list-item-content>
            </template>
            <v-divider></v-divider>
          </v-list-item>
        </template>
      </v-list-item-group>
    </v-list>
  </v-menu>
</template>

<script>
import { mapActions } from "vuex";
export default {
  data() {
    return {
      menu: false,
      selectedTenants: [],
    };
  },
  watch: {
    tenants: {
      immediate: true,
      handler: function () {
        this.selectedTenants = this.tenants.map((x, i) => i);
      },
    },
    menu: function (value) {
      var tenants = [];
      if (value) {
        return;
      }

      for (let index = 0; index < this.selectedTenants.length; index++) {
        const tenant = this.tenants[this.selectedTenants[index]];
        tenants.push(tenant);
      }

      this.setTenantFilter(tenants);
    },
  },
  computed: {
    tenants: function () {
      return this.$store.state.system.tenant.items;
    },
    tenantFilter: function () {
      return this.$store.state.system.tenantFilter;
    },
  },
  methods: {
    ...mapActions("system", ["setTenantFilter"]),
  },
};
</script>

<style>
</style>