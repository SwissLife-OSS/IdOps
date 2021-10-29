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
            v-for="srv in servers"
            :key="srv.id"
            selectable
            @click="onSelectServer(srv)"
          >
            <v-list-item-content>
              <v-list-item-title v-text="srv.name"></v-list-item-title>
              <v-list-item-subtitle
                class="grey--text text--lighten-1"
                v-text="srv.url"
              ></v-list-item-subtitle>
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
    servers: function () {
      let servers = this.$store.state.system.identityServer.items;

      if (this.searchText) {
        const regex = new RegExp(`.*${this.searchText}.*`, "i");
        servers = servers.filter((x) => regex.test(x.name));
      }

      return servers;
    },
  },
  methods: {
    onClickAdd: function () {
      this.$router.push({ name: "IdentityServer_New" });
    },
    onSelectServer: function (srv) {
      this.$router.push({
        name: "IdentityServer_Edit",
        params: { id: srv.id },
      });
    },
  },
};
</script>

<style>
</style>