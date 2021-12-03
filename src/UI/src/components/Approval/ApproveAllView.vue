<template>
  <v-data-table
    class="mt-4"
    :headers="headers"
    :height="$vuetify.breakpoint.height - 300"
    :loading="loading"
    fixed-header
    :items="flat"
    item-key="key"
    v-model="selected"
    :single-select="false"
    :show-select="filter.environment != 'ALL'"
    :footer-props="{ itemsPerPageOptions: [50, 100, -1] }"
  >
    <template v-slot:top>
      <v-row>
        <v-col md="3">
          <v-text-field
            v-model="searchText"
            label="Search"
            class="mx-4"
            dense
            prepend-icon="mdi-magnify"
            append-outer-icon="mdi-refresh"
            @click:append-outer="onRefresh"
            v-clipboard
          ></v-text-field
        ></v-col>
        <v-col md="3">
          <v-btn-toggle
            dense
            v-model="filter.environment"
            @change="onRefresh"
            rounded
          >
            <v-btn small value="ALL">All</v-btn>
            <v-btn
              small
              v-for="env in environments"
              :key="env.id"
              :value="env.id"
            >
              {{ env.name }}
            </v-btn>
          </v-btn-toggle></v-col
        >
        <v-col md="2">
          <v-select
            label="Types"
            dense
            v-model="filter.resourceTypes"
            :items="resourceTypes"
            item-text="name"
            item-value="id"
            @change="onRefresh"
            clearable
          ></v-select
        ></v-col>
        <v-col md="2">
          <v-select
            dense
            label="Group"
            v-model="filter.identityServerGroupId"
            :items="identityServerGroups"
            item-text="name"
            @change="onRefresh"
            item-value="id"
          ></v-select>
        </v-col>
        <v-spacer></v-spacer>
        <v-col md="2" style="text-align: right">
          <v-progress-linear
            v-if="approveStarted"
            color="purple"
            stream
            reverse
            buffer-value="0"
          ></v-progress-linear>
          <v-btn
            @click="onApprove"
            class="mr-4"
            color="primary"
            :disabled="selected.length === 0"
            v-else
            >Approve selected</v-btn
          ></v-col
        >
        <v-col></v-col>
      </v-row>
    </template>

    <template v-slot:item.title="{ item }">
      <router-link
        :to="{ name: item.type + '_Edit', params: { id: item.id } }"
        >{{ item.title }}</router-link
      >
    </template>
    <template v-slot:item.currentVersion="{ item }">
      <strong class="mr-2">{{ item.currentVersion.version }}</strong>
      | {{ item.currentVersion.createdAt | dateformat }}
    </template>
    <template v-slot:item.approvedAt="{ item }">
      {{ item.approvedAt | dateformat }}
    </template>
    <template v-slot:item.state="{ item }">
      <v-progress-circular
        v-if="item.state === 'Approving'"
        indeterminate
        :size="22"
        color="blue"
      ></v-progress-circular>

      <v-icon v-if="item.state === 'Latest'" color="green"
        >mdi-check-circle</v-icon
      >
      <v-icon v-if="item.state === 'NotDeployed'" color="grey"
        >mdi-circle-outline</v-icon
      >
      <v-icon v-if="item.state === 'NotApproved'" color="grey"
        >mdi-clock-alert-outline</v-icon
      >
      <v-icon v-if="item.state === 'Outdated'" color="amber darken-4"
        >mdi-update</v-icon
      >
    </template>
    <template v-slot:item.actions="{ item }">
      <v-btn
        v-if="
          item.state !== 'Latest' &&
            item.state !== 'Approving' &&
            !approveStarted
        "
        @click="onApproveItem(item)"
        color="primary"
      >
        Approve
      </v-btn>
    </template>
  </v-data-table>
</template>

<script>
import { mapActions } from "vuex";
export default {
  created() {
    this.filter.identityServerGroupId = this.identityServerGroups[0].id;
    this.onRefresh(this.filter);
  },
  data() {
    return {
      approveStarted: false,
      items: [],
      selected: [],
      filter: {
        environment: "ALL",
        resourceTypes: [],
        identityServerGroupId: null
      },
      searchText: "",
      resourceTypes: [
        "Client",
        "ApiResource",
        "IdentityResource",
        "ApiScope",
        "UserClaimRule",
        "PersonalAccessToken"
      ],
      headers: [
        {
          text: "Resource",
          width: 100,
          align: "start",
          value: "title",
          sortable: false
        },
        {
          text: "Type",
          width: 100,
          align: "start",
          value: "type",
          sortable: false
        },
        {
          text: "Current Version",
          width: 200,
          value: "currentVersion",
          align: "start",
          sortable: false
        },
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
        { text: "Approved at", value: "approvedAt" },
        { text: "Actions", value: "actions" }
      ]
    };
  },
  computed: {
    flat: function() {
      const list = [];
      let items = this.$store.state.idResource.approval.items;

      if (this.searchText && this.searchText.length > 2) {
        const regex = new RegExp(`.*${this.searchText}.*`, "i");
        items = items.filter(x => regex.test(x.title));
      }

      for (let i = 0; i < items.length; i++) {
        const element = items[i];
        if (element.environments) {
          for (let j = 0; j < element.environments.length; j++) {
            const env = element.environments[j];
            list.push({
              id: element.id,
              key: element.id + env.environment.id,
              type: element.type,
              title: element.title,
              currentVersion: element.currentVersion,
              environment: env.environment,
              state: env.state,
              version: env.version,
              approvedAt: env.approvedAt
            });
          }
        }
      }
      return list;
    },
    environments: function() {
      const environments = this.$store.state.system.environment.items;
      return environments;
    },
    identityServerGroups: function() {
      return this.$store.state.system.identityServerGroup.items;
    },
    loading: function() {
      return this.$store.state.idResource.approval.loading;
    }
  },
  methods: {
    ...mapActions("idResource", ["getResourceApprovals", "approveResources"]),
    async onApprove() {
      this.approveStarted = true;

      const input = {
        resources: this.selected.map(x => ({
          resourceId: x.id,
          environmentId: x.environment.id,
          type: x.type,
          version: x.currentVersion.version
        }))
      };
      await this.approveResources(input);
      this.onRefresh();

      this.selected = [];
      this.approveStarted = false;
    },
    async onApproveItem(item) {
      this.approveStarted = true;

      await this.approveResources({
        resources: [
          {
            resourceId: item.id,
            environmentId: item.environment.id,
            type: item.type,
            version: item.currentVersion.version
          }
        ]
      });
      this.onRefresh();
      this.approveStarted = false;
    },
    onRefresh: function() {
      this.getResourceApprovals({
        environmentId:
          this.filter.environment == "ALL" ? null : this.filter.environment,
        resourceTypes: this.filter.resourceTypes,
        identityServerGroupId: this.filter.identityServerGroupId
      });
    }
  }
};
</script>

<style></style>
