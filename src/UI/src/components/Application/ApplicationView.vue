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
            v-for="application in applications"
            :key="application.id"
            selectable
            @click="onSelectApplication(application)"
          >
            <v-list-item-content>
              <v-list-item-title v-text="application.name"></v-list-item-title>
              <div
                class="chip-tenant"
                :style="{ 'background-color': application.tenantInfo.color }"
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
  created() {
    this.search();
    this.getClientTemplates();
  },
  data() {
    return {
      searchText: "",
    };
  },
  computed: {
    applications: function () {
      let apps = this.$store.state.application.list.items;

      if (this.searchText) {
        const regex = new RegExp(`.*${this.searchText}.*`, "i");
        apps = apps.filter((x) => regex.test(x.name) || regex.test(x.id));
      }

      return apps;
    },
  },
  methods: {
    ...mapActions("application", ["search", "getClientTemplates"]),
    onClickAdd: function () {
      this.$router.push({ name: "Application_New" });
    },
    onSelectApplication: function (res) {
      this.$router.push({ name: "Application_Edit", params: { id: res.id } });
    },
  },
};
</script>

<style></style>
