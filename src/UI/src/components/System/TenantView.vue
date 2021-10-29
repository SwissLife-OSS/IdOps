<template>
  <v-row>
    <v-col md="2">
      <v-text-field
        clearable
        v-model="searchText"
        placeholder="Search"
        prepend-icon="mdi-magnify"
        append-outer-icon="mdi-plus"
        @click:append-outer="onClickAdd"
      ></v-text-field>
      <v-list two-line rounded dense class="mt-0">
        <v-list-item-group color="primary" select-object>
          <v-list-item
            v-for="tenant in tenants"
            :key="tenant.id"
            selectable
            @click="onSelectTenant(tenant)"
          >
            <v-list-item-content>
              <v-list-item-title v-text="tenant.id"></v-list-item-title>
              <v-list-item-subtitle
                v-text="tenant.description"
              ></v-list-item-subtitle>
              <div
                class="chip-tenant"
                :style="{ 'background-color': tenant.color }"
              ></div>
            </v-list-item-content>
          </v-list-item>
        </v-list-item-group>
      </v-list>
    </v-col>
    <v-col md="10">
      <router-view></router-view>
    </v-col>
  </v-row>
</template>

<script>
export default {
  created() {},
  data() {
    return {
      searchText: "",
    };
  },
  computed: {
    tenants: function () {
      return this.$store.state.system.tenant.items;
    },
  },
  methods: {
    onClickAdd: function () {
      this.$router.push({ name: "Tenant_New" });
    },
    onSelectTenant: function (tenant) {
      this.$router.push({ name: "Tenant_Edit", params: { id: tenant.id } });
    },
  },
};
</script>

<style>
</style>