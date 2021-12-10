<template>
  <div>
    <v-container fluid class="fill-height">
      <v-row>
        <v-col>
          <v-data-table
            :headers="headers"
            :height="tableHeight"
            :items="events"
            :options.sync="options"
            :server-items-length="totalCount"
            :loading="loading"
            @click:row="onClickRow"
            item-key="id"
            :footer-props="{ itemsPerPageOptions: [50, 100, 200 - 1] }"
            :items-per-page="filter.pageSize"
            :sort-by.sync="sortBy"
            :sort-desc.sync="sortDesc"
          >
            <template v-slot:header.status="{}">
              <v-icon v-if="input && !input.unwind" @click="navigateToFullView"
                >mdi-arrow-top-right-bold-box-outline</v-icon
              >
            </template>
            <template v-slot:item.status="{ item }">
              <v-icon color="green" v-if="item.eventType == 'Success'"
                >mdi-checkbox-marked-circle</v-icon
              >
              <v-icon color="orange" v-if="item.eventType == 'Failure'"
                >mdi-alert</v-icon
              >
              <v-icon color="red" v-if="item.eventType == 'Error'"
                >mdi-alert-circle</v-icon
              >
            </template>
            <template v-slot:item.timeStamp="{ item }">
              {{ item.timeStamp | dateformat }}
            </template>
            <template v-slot:footer.page-text="{}">
              <v-icon @click="onClickRefresh">mdi-reload</v-icon>
            </template>

            <template v-slot:top v-if="input == null || input.unwind">
              <v-row dense>
                <v-col md="1">
                  <v-autocomplete
                    @change="refreshFilter"
                    :items="filterTypes"
                    v-model="filterType"
                    label="Filter type"
                    dense
                    hide-details
                    class="pa-0"
                  ></v-autocomplete>
                </v-col>
                <v-col md="4">
                  <v-autocomplete
                    @change="onClickRefresh"
                    dense
                    v-model="filter.clients"
                    v-if="filterType == 'Client Name'"
                    :items="clients"
                    item-text="name"
                    item-value="clientId"
                    multiple
                    clearable
                    small-chips
                    deletable-chips
                  />
                  <v-autocomplete
                    @change="onClickRefresh"
                    dense
                    v-model="filter.clients"
                    v-if="filterType == 'Client Id'"
                    :items="clients"
                    item-text="clientId"
                    item-value="clientId"
                    multiple
                    clearable
                    small-chips
                    deletable-chips
                  />
                  <v-autocomplete
                    @change="onClickRefresh"
                    dense
                    v-model="filter.applications"
                    v-if="filterType == 'Application'"
                    :items="applications"
                    item-text="name"
                    item-value="id"
                    multiple
                    clearable
                    small-chips
                    deletable-chips
                  />
                </v-col>
                <v-col md="2">
                  <v-autocomplete
                    label="Event Types"
                    dense
                    v-model="filter.eventTypes"
                    :items="eventTypes"
                    @change="onClickRefresh"
                    multiple
                    clearable
                  ></v-autocomplete
                ></v-col>
                <v-spacer></v-spacer>
                <v-col md="3">
                  <v-btn-toggle
                    @change="onClickRefresh"
                    dense
                    v-model="filter.environments"
                    rounded
                    multiple
                    mandatory
                  >
                    <v-btn
                      small
                      v-for="env in environments"
                      :key="env.id"
                      :value="env.name"
                    >
                      {{ env.name }}
                    </v-btn>
                  </v-btn-toggle>
                </v-col>
                <v-spacer> </v-spacer>
                <v-col md="1">
                  <v-btn color="primary" @click="onClickRefresh"
                    ><v-icon>mdi-magnify</v-icon>Search</v-btn
                  ></v-col
                >
              </v-row>
            </template>
          </v-data-table></v-col
        >
      </v-row>
      <v-row>
        <v-col>
          <insights-detail-card
            v-if="details && detailMode === 'INLINE'"
            @close="details = null"
            :data="details"
          ></insights-detail-card>
        </v-col>
      </v-row>
    </v-container>

    <v-dialog hide-overlay width="800" v-model="dialog">
      <insights-detail-card
        @close="
          details = null;
          dialog = false;
        "
        :data="details"
      ></insights-detail-card>
    </v-dialog>
    <div class="ma-2 pa-0">
      <router-view></router-view>
    </div>
  </div>
</template>

<script>
import { mapState, mapActions } from "vuex";
import InsightsDetailCard from "./InsightsDetailCard.vue";

export default {
  created() {
    this.searchClients();
    this.search();

    if (this.input == null) {
      this.filter.environments = this.environments.map(env => env.name);
      this.onClickRefresh();
    }
  },
  components: { InsightsDetailCard },
  props: {
    input: {
      type: Object
    },
    detailMode: {
      type: String,
      default: "POPUP"
    }
  },
  data() {
    return {
      sortBy: "timeStamp",
      sortDesc: true,
      dialog: false,
      userSearchTriggered: false,
      details: null,
      options: {},
      filterTypes: ["Client Name", "Client Id", "Application"],
      filterType: "Client Name",
      filter: {
        applications: [],
        environments: [],
        clients: [],
        eventTypes: [],
        pageSize: 100
      },
      eventTypes: ["Success", "Error", "Failure"]
    };
  },
  watch: {
    options: {
      handler() {
        if (this.userSearchTriggered) {
          this.setIdentityServerEventsPaging({
            pageSize: this.options.itemsPerPage,
            pageNr: this.options.page - 1
          });
        }
      },
      deep: true
    },
    input: {
      immediate: true,
      handler: function() {
        if (this.input) {
          this.filterType = this.input.filterType;
          this.filter.clients = this.input.clients;
          this.filter.applications = this.input.applications;

          if (this.input.environments) {
            this.filter.environments = this.input.environments.map(
              env => this.environments.find(e => e.id == env).name
            );
          } else {
            this.filter.environments = this.environments.map(env => env.name);
          }
          this.onClickRefresh();
        }
      }
    }
  },
  computed: {
    ...mapState("insights", {
      events: state => state.idEvents.items,
      totalCount: state => state.idEvents.totalCount,
      loading: state => state.idEvents.loading
    }),
    headers: function() {
      const headers = [];
      headers.push(
        {
          align: "center",
          value: "status",
          sortable: false
        },
        {
          text: "Timestamp",
          align: "center",
          value: "timeStamp",
          sortable: true
        },
        { text: "Name", align: "center", sortable: false, value: "name" },
        {
          text: "Environment",
          align: "center",
          sortable: false,
          value: "environmentName"
        },
        {
          text: "Event Type",
          align: "center",
          sortable: false,
          value: "eventType"
        },
        {
          text: "Client Id",
          align: "center",
          sortable: false,
          value: "clientId"
        },
        {
          text: "Subject Id",
          align: "center",
          sortable: false,
          value: "subjectId"
        }
      );
      if (this.input == null || this.input.unwind) {
        headers.splice(2, 0, {
          text: "Category",
          align: "center",
          sortable: false,
          value: "category"
        });
        headers.splice(5, 0, {
          text: "IP Address",
          align: "center",
          sortable: false,
          value: "remoteIpAddress"
        });
        headers.splice(7, 0, {
          text: "Hostname",
          align: "center",
          sortable: false,
          value: "hostname"
        });
        headers.splice(8, 0, {
          text: "Endpoint",
          align: "center",
          sortable: false,
          value: "endpoint"
        });
      }
      return headers;
    },
    environments: function() {
      return this.$store.state.system.environment.items;
    },
    clients: function() {
      return this.$store.state.idResource.client.items;
    },
    applications: function() {
      return this.$store.state.application.list.items;
    },
    tableHeight: function() {
      if (this.details && this.detailMode === "INLINE") {
        return this.$vuetify.breakpoint.height - 810;
      } else if (this.detailMode === "POPUP") {
        return this.$vuetify.breakpoint.height - 280;
      } else {
        return this.$vuetify.breakpoint.height - 230;
      }
    }
  },
  methods: {
    ...mapActions("insights", [
      "searchIdentityServerEvents",
      "setIdentityServerEventsPaging"
    ]),
    ...mapActions("idResource", ["searchClients"]),
    ...mapActions("application", ["search"]),
    onClickRow: function(item) {
      if (this.detailMode === "POPUP") {
        this.dialog = true;
      }
      this.details = Object.assign({}, item, {
        data: JSON.parse(item.rawData),
        rawData: undefined
      });
    },
    refreshFilter: function() {
      this.filter.clients = [];
      this.filter.applications = [];
    },
    onClickRefresh: function() {
      this.userSearchTriggered = true;
      this.details = null;
      this.searchIdentityServerEvents(this.filter);
    },
    navigateToFullView: function() {
      this.$router.push({
        name: "IdentityServerEvents",
        params: {
          input: {
            unwind: true,
            ...this.input
          }
        }
      });
    }
  }
};
</script>

<style scoped>
a {
  text-decoration: none;
  color: rgb(1, 11, 163) !important;
}
.json-wrapper {
  height: 500px;
  overflow: auto;
}
</style>
