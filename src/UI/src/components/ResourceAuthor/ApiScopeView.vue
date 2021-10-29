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

      <v-list
        two-line
        rounded
        dense
        :style="{ 'max-height': $vuetify.breakpoint.height - 180 + 'px' }"
        class="overflow-y-auto mt-0"
      >
        <v-list-item-group color="primary" select-object>
          <v-list-item
            v-for="scope in apiScopes"
            :key="scope.id"
            selectable
            @click="onSelectScope(scope)"
          >
            <v-list-item-content>
              <v-list-item-title v-text="scope.name"></v-list-item-title>
              <v-list-item-subtitle
                v-text="scope.displayName"
              ></v-list-item-subtitle>
              <div
                class="chip-tenant"
                :style="{ 'background-color': scope.tenantInfo.color }"
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
import { mapActions } from "vuex";
export default {
  created() {},
  data() {
    return {
      searchText: "",
    };
  },
  computed: {
    apiScopes: function () {
      const regex = new RegExp(`.*${this.searchText}.*`, "i");
      return this.$store.state.idResource.apiScope.items.filter((x) =>
        regex.test(x.name)
      );
    },
  },
  methods: {
    ...mapActions("idResource", ["loadApiResources"]),
    onClickAdd: function () {
      this.$router.push({ name: "ApiScope_New" });
    },
    onSelectScope: function (scope) {
      this.$router.push({ name: "ApiScope_Edit", params: { id: scope.id } });
    },
  },
};
</script>

<style>
</style>