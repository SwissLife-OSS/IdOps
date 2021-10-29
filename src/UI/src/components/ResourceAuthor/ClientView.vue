<template>
  <v-row>
    <v-col md="2">
      <v-btn-toggle dense v-model="environment" @change="onEnvChange" rounded>
        <v-btn small value="all">All</v-btn>
        <v-btn small v-for="env in environments" :key="env.id" :value="env.id">
          {{ env.name }}
        </v-btn>
      </v-btn-toggle>
      <v-text-field
        clearable
        v-model="searchText"
        placeholder="Search"
        prepend-icon="mdi-magnify"
        append-outer-icon="mdi-plus"
        @click:append-outer="onClickAdd"
        :loading="loading"
      ></v-text-field>

      <v-list
        two-line
        rounded
        dense
        :style="{ 'max-height': $vuetify.breakpoint.height - 200 + 'px' }"
        class="overflow-y-auto mt-0"
      >
        <v-list-item-group color="primary" select-object>
          <v-list-item
            v-for="client in clients"
            :key="client.id"
            selectable
            dense
            :title="client.tenantInfo.id"
            @click="onSelectClient(client)"
          >
            <v-list-item-content>
              <v-list-item-title v-text="client.name"></v-list-item-title>
              <v-list-item-subtitle
                class="grey--text text--lighten-1"
                v-text="client.clientId"
              ></v-list-item-subtitle>
              <div
                class="chip-tenant"
                :style="{ 'background-color': client.tenantInfo.color }"
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
    this.searchClients();
  },
  data() {
    return {
      searchText: "",
      environment: "all",
    };
  },
  computed: {
    clients: function () {
      let clients = this.$store.state.idResource.client.items;

      if (this.searchText) {
        const regex = new RegExp(`.*${this.searchText}.*`, "i");
        clients = clients.filter(
          (x) => regex.test(x.name) || regex.test(x.clientId)
        );
      }

      return clients;
    },
    environments: function () {
      return this.$store.state.system.environment.items;
    },
    loading: function () {
      return this.$store.state.idResource.client.loading;
    },
  },
  methods: {
    ...mapActions("idResource", ["searchClients", "setClientFilter"]),
    onClickAdd: function () {
      this.$router.push({ name: "Client_New" });
    },
    onSelectClient: function (res) {
      this.$router.push({ name: "Client_Edit", params: { id: res.id } });
    },
    onEnvChange: function (value) {
      this.setClientFilter({
        environmentId: value == "all" ? null : value,
      });
    },
  },
};
</script>

<style scoped>
</style>