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
          >
            <template v-slot:item.timeStamp="{ item }">
              {{ item.timeStamp | dateformat }}
            </template>
            <template v-slot:footer.page-text="{}">
              <v-icon @click="onClickRefresh">mdi-reload</v-icon>
            </template>

            <template v-slot:top v-if="clientId == null">
              <v-row dense>
                <v-col md="2">
                  <v-btn-toggle
                    @change="onClickRefresh"
                    dense
                    v-model="filter.environment"
                    rounded
                  >
                    <v-btn small value="ALL">All</v-btn>
                    <v-btn
                      small
                      v-for="env in environments"
                      :key="env.id"
                      :value="env.name"
                    >
                      {{ env.name }}
                    </v-btn>
                  </v-btn-toggle></v-col
                >
                <v-col md="2">
                  <v-text-field
                    dense
                    @keyup.enter="onClickRefresh"
                    label="ClientId"
                    v-model="filter.clientId"
                  ></v-text-field>
                </v-col>
                <v-col md="2">
                  <v-autocomplete
                    label="Event Types"
                    dense
                    v-model="filter.eventTypes"
                    :items="eventTypes"
                    @change="onClickRefresh"
                    multiple
                  ></v-autocomplete
                ></v-col>
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
  </div>
</template>

<script>
import { createNamespacedHelpers } from "vuex";
import InsightsDetailCard from "./InsightsDetailCard.vue";
const { mapState, mapActions } = createNamespacedHelpers("insights");

export default {
  components: { InsightsDetailCard },
  props: {
    clientId: {
      type: String,
    },
    detailMode: {
      type: String,
      default: "POPUP",
    },
  },
  data() {
    return {
      dialog: false,
      userSearchTriggered: false,
      details: null,
      options: {},
      filter: {
        environment: "ALL",
        clientId: null,
        eventTypes: [],
        pageSize: 100,
      },
      eventTypes: ["Success", "Error", "Failure"],
      headers: [
        {
          text: "Timestamp",
          align: "start",
          value: "timeStamp",
          sortable: false,
          width: 160,
        },
        { text: "Category", value: "category" },
        { text: "name", value: "name" },
        { text: "Environment", value: "environmentName" },
        { text: "IP address", value: "remoteIpAddress" },
        { text: "EventType", value: "eventType" },
        { text: "Hostname", value: "hostname" },
        { text: "Endpoint", value: "endpoint" },
        { text: "ClientId", value: "clientId" },
        { text: "SubjectId", value: "subjectId" },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        if (this.userSearchTriggered) {
          this.setIdentityServerEventsPaging({
            pageSize: this.options.itemsPerPage,
            pageNr: this.options.page - 1,
          });
        }
      },
      deep: true,
    },
    clientId: {
      immediate: true,
      handler: function () {
        console.log("CL", this.clientId);
        if (this.clientId) {
          this.filter.clientId = this.clientId;
          this.onClickRefresh();
        }
      },
    },
  },
  computed: {
    ...mapState({
      events: (state) => state.idEvents.items,
      totalCount: (state) => state.idEvents.totalCount,
      loading: (state) => state.idEvents.loading,
    }),
    environments: function () {
      return this.$store.state.system.environment.items;
    },
    tableHeight: function () {
      if (this.details && this.detailMode === "INLINE") {
        return this.$vuetify.breakpoint.height - 810;
      } else if (this.detailMode === "POPUP") {
        return this.$vuetify.breakpoint.height - 280;
      } else {
        return this.$vuetify.breakpoint.height - 230;
      }
    },
  },
  methods: {
    ...mapActions([
      "searchIdentityServerEvents",
      "setIdentityServerEventsPaging",
    ]),
    onClickRow: function (item) {
      if (this.detailMode === "POPUP") {
        this.dialog = true;
      }
      this.details = Object.assign({}, item, {
        data: JSON.parse(item.rawData),
        rawData: undefined,
      });
    },
    onClickRefresh: function () {
      this.userSearchTriggered = true;
      this.details = null;
      this.searchIdentityServerEvents(this.filter);
    },
  },
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