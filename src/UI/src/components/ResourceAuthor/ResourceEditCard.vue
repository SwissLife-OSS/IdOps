<template>
  <v-card :loading="loading" :height="$vuetify.breakpoint.height - 100">
    <v-toolbar elevation="0" color="grey lighten-3" height="52">
      <v-toolbar-title>{{ toolbarTitle }}</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn
        v-for="(value, key) in toolsButtons"
        :key="key"
        class="mr-2"
        icon
        @click="onClickTool(key)"
        :color="view === key ? 'blue darken-2' : 'grey darken-3'"
      >
        <v-icon>{{ value.icon }}</v-icon>
      </v-btn>

      <v-btn
        v-if="view === 'DEFAULT'"
        @click="onSave"
        color="green lighten-1"
        class="ma-2 white--text"
        :disabled="saveState === 'UNCHANGED'"
        :loading="buttonLoading"
      >
        Save
        <v-icon size="24" color="white" right>mdi-check </v-icon>
      </v-btn>

      <v-btn v-if="view !== 'DEFAULT'" icon @click="view = 'DEFAULT'">
        <v-icon color="grey darken-3">mdi-arrow-left-circle</v-icon>
      </v-btn>
    </v-toolbar>
    <v-card-text>
      <slot v-if="view === 'DEFAULT'"></slot>
      <div v-if="view === 'AUDIT'">
        <resource-audit-list
         :resourceId="resource.id"
         ></resource-audit-list>
      </div>
      <div v-if="view === 'INSIGHTS'">
        <identity-server-events-view
          :input="this.createInsightsInput()"
          detailMode="POPUP"
        ></identity-server-events-view>
      </div>
      <div v-if="view === 'PUBLISH'">
        <resource-publish-view
          :resourceId="resource.id"
        ></resource-publish-view>
      </div>
      <div v-if="view === 'DEPENDENCIES'">
        <resource-dependencies-view
          :resourceId="resource.id"
          :type="this.type"
        ></resource-dependencies-view>
      </div>
      <div v-if="view === 'LOG'">
        <resource-log-view
         :resourceId="resource.id"
         ></resource-log-view>
      </div>
      <div v-if="view === 'TOKEN'">
        <resource-token-view
         :id="resource.id"
         ></resource-token-view>
      </div>
    </v-card-text>
  </v-card>
</template>

<script>
import ResourceAuditList from "./ResourceAuditList.vue";
import ResourcePublishView from "./ResourcePublishView.vue";
import ResourceLogView from "./ResourceLogView.vue";
import ResourceDependenciesView from "./ResourceDependenciesView.vue";
import ResourceTokenView from "./ResourceTokenView.vue";
import hash from "object-hash";
import IdentityServerEventsView from "../Insights/IdentityServerEventsView.vue";

export default {
  components: {
    ResourceAuditList,
    ResourcePublishView,
    ResourceLogView,
    ResourceDependenciesView,
    ResourceTokenView,
    IdentityServerEventsView
  },
  props: {
    title: {
      type: String
    },
    loading: {
      type: Boolean,
      default: false
    },
    resource: {
      type: Object
    },
    tools: {
      type: Array,
      default: () => ["AUDIT"]
    },
    type: {
      type: String
    }
  },
  data() {
    return {
      saving: false,
      originalHash: null,
      view: "DEFAULT",
      toolsDef: {
        LOG: {
          icon: "mdi-clipboard-text-clock-outline",
          title: "Logs"
        },
        INSIGHTS: {
          icon: "mdi-eye-outline",
          title: "Insights"
        },
        AUDIT: {
          icon: "mdi-timeline-clock-outline",
          title: "Audit"
        },
        DEPENDENCIES: {
          icon: "mdi-graph-outline",
          title: "Dependencies"
        },
        TOKEN: {
          icon: "mdi-cookie-cog-outline",
          title: "Token Settings"
        },
        PUBLISH: {
          icon: "mdi-rocket-launch-outline",
          title: "Publishing"
        }
      }
    };
  },
  watch: {
    resource: {
      immediate: true,
      handler: function() {
        this.originalHash = hash(this.resource);
      }
    }
  },
  computed: {
    toolsButtons: function() {
      if (this.hideTools) {
        return {};
      }
      const filtered = Object.keys(this.toolsDef)
        .filter(key => this.tools.includes(key))
        .reduce((obj, key) => {
          obj[key] = this.toolsDef[key];
          return obj;
        }, {});

      return filtered;
    },
    showAudit: function() {
      return this.resource.id && this.view !== "AUDIT";
    },
    showPublish: function() {
      return this.resource.id && this.view !== "PUBLISH";
    },
    buttonLoading: function() {
      return this.saveState === "SAVING";
    },
    toolbarTitle: function() {
      if (this.resource.id) {
        if (this.view === "DEFAULT") {
          return `Edit ${this.title}`;
        } else {
          return `${this.title} |  ${this.toolsDef[this.view].title}`;
        }
      }

      return `Create new ${this.title}`;
    },
    hash: function() {
      return hash(this.resource);
    },
    saveState: function() {
      if (this.saving) {
        return "SAVING";
      } else {
        if (this.originalHash !== this.hash) {
          return "CHANGED";
        }

        return "UNCHANGED";
      }
    }
  },
  methods: {
    onClickTool: function(tool) {
      this.view = tool;
    },
    createInsightsInput: function() {
      if (this.resource.__typename == "Application") {
        return {
          filterType: "Application",
          applications: [this.resource.id]
        };
      } else if (this.resource.allowedApplicationIds) {
        return {
          filterType: "Application",
          applications: this.resource.allowedApplicationIds,
          environments: [this.resource.environmentId]
        };
      } else {
        return {
          filterType: "Client Name",
          clients: [this.resource.clientId],
          environments: this.resource.environments
        };
      }
    },
    onSave: function() {
      this.saving = true;
      this.$emit("Save", {
        done: () => {
          this.saving = false;
        }
      });
    }
  }
};
</script>

<style></style>
