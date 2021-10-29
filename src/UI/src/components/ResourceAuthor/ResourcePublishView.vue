<template>
  <v-card flat>
    <v-toolbar elevation="0" color="grey lighten-6" height="38">
      <v-toolbar-title v-if="this.currentVersion">
        Current version: {{ this.currentVersion.version }} |
        {{ this.currentVersion.createdAt | dateformat }}</v-toolbar-title
      >
      <v-spacer></v-spacer>
      <v-icon @click="onRefresh">mdi-refresh</v-icon>
    </v-toolbar>
    <v-card-text>
      <div v-if="currentVersion">
        <v-data-table
          :headers="headers"
          height="300"
          :items="items"
          item-key="id"
        >
          <template v-slot:item.publishedAt="{ item }">
            {{ item.publishedAt | dateformat }}
          </template>
          <template v-slot:item.state="{ item }">
            <v-icon v-if="item.state === 'Latest'" color="green"
              >mdi-check-circle</v-icon
            >
            <v-icon v-if="item.state === 'NotDeployed'" color="grey"
              >mdi-circle-outline</v-icon
            >
            <v-icon v-if="item.state === 'Outdated'" color="amber darken-4"
              >mdi-update</v-icon
            >
            <v-icon v-if="item.state === 'NotApproved'" color="grey"
              >mdi-clock-alert-outline</v-icon
            >
          </template>
          <template v-slot:item.actions="{ item }">
            <v-icon
              @click="onClickPubish(item.environment)"
              v-if="
                item.state !== 'Latest' &&
                  item.state !== 'Publishing' &&
                  item.state !== 'NotApproved'
              "
              color="blue"
              >mdi-cloud-upload-outline</v-icon
            >

            <v-btn
              :disabled="!canApprove()"
              v-if="item.state === 'NotApproved'"
              @click="onClickApprove(item)"
              color="primary"
            >
              Approve
            </v-btn>
            <v-progress-circular
              indeterminate
              color="blue"
              v-if="item.state === 'Publishing'"
            ></v-progress-circular>
          </template>
        </v-data-table>
      </div>
    </v-card-text>
  </v-card>
</template>

<script>
import { mapActions, mapGetters } from "vuex";
export default {
  props: ["resourceId"],
  watch: {
    resourceId: {
      immediate: true,
      handler: function() {
        this.loadPublished();
      }
    }
  },
  data() {
    return {
      expanded: [],
      singleExpand: true,
      isLoading: false,
      headers: [
        {
          text: "State",
          width: 60,
          align: "start",
          value: "state",
          sortable: false
        },
        {
          text: "Environment",
          align: "start",
          value: "environment.name",
          sortable: false
        },
        { text: "Version", value: "version" },
        { text: "Published at", value: "publishedAt" },
        { text: "Actions", value: "actions" }
      ]
    };
  },
  computed: {
    items: function() {
      return this.$store.state.idResource.editor.publish.environments;
    },
    currentVersion: function() {
      return this.$store.state.idResource.editor.publish.currentVersion;
    },
    type: function() {
      return this.$store.state.idResource.editor.type;
    }
  },
  methods: {
    ...mapActions("idResource", [
      "publishResources",
      "getPublishedByResource",
      "approveResources"
    ]),
    ...mapGetters("user", ["hasPermission"]),
    canApprove: function() {
      return this.hasPermission("CA_Approval");
    },
    loadPublished: function() {
      this.getPublishedByResource(this.resourceId);
    },
    onClickPubish: function(environment) {
      this.publishResources({
        destinationEnvionmentId: environment.id,
        resources: [this.resourceId]
      });
    },
    onClickApprove: async function(item) {
      await this.approveResources({
        resources: [
          {
            resourceId: this.resourceId,
            environmentId: item.environment.id,
            type: this.type,
            version: this.currentVersion.version
          }
        ]
      });
      this.onRefresh();
    },
    onRefresh: function() {
      this.loadPublished();
    }
  }
};
</script>

<style></style>
