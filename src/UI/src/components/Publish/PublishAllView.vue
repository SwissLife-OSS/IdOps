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
    @toggle-select-all="selectAllToggle"
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
            v-if="processing"
            color="purple"
            stream
            reverse
            buffer-value="0"
          ></v-progress-linear>
          <span v-else>
            <v-btn
              @click="onPublish"
              class="mr-4"
              color="success"
              :disabled="selected.length === 0 || !selected.some(publishable)"
              >Publish
            </v-btn>
            <v-btn
              @click="onApprove"
              class="mr-4"
              color="primary"
              :disabled="selected.length === 0 || !selected.some(aprovable)">
              Approve
            </v-btn>
          </span>
        </v-col>
        <v-col></v-col>
      </v-row>
    </template>

    <template v-slot:item.data-table-select="{ isSelected, select, item }">
      <v-simple-checkbox
        :value="isSelected"
        @input="select($event)"
        :disabled="item.state == 'Latest'"/>
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
    <template v-slot:item.publishedAt="{ item }">
      {{ item.publishedAt | dateformat }}
    </template>
    <template v-slot:item.approvedAt="{ item }">
      {{ item.approvedAt | dateformat }}
    </template>
    <template v-slot:item.state="{ item }">
      <v-progress-circular
        v-if="item.state === 'Publishing' || item.state === 'Approving'"
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
      <v-icon v-if="item.state === 'NotApproved'" color="grey">mdi-clock-alert-outline</v-icon
      >
      <v-icon v-if="item.state === 'Outdated'" color="amber darken-4"
        >mdi-update</v-icon
      >
    </template>
    <template v-slot:item.actions="{ item }">
      <v-btn
        @click="onPublishItem(item)"
        v-if="publishable(item) && !processing"
        color="success"
        >Publish</v-btn
      >
      <v-btn
        @click="onApproveItem(item)"
        v-if="aprovable(item) && !processing"
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
      processing: false,
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
          align: "center",
          value: "title",
          sortable: false
        },
        {
          text: "Type",
          width: 100,
          align: "center",
          value: "type",
          sortable: false
        },
        {
          text: "Current Version",
          width: 200,
          value: "currentVersion",
          align: "center",
          sortable: false
        },
        {
          text: "State",
          width: 60,
          align: "center",
          value: "state",
          sortable: false
        },
        {
          text: "Environment",
          align: "center",
          value: "environment.name",
          sortable: false
        },
        { text: "Published Version", align: "center", value: "version" },
        { text: "Published at", align: "center", value: "publishedAt" },
        { text: "Approved at", align: "center", value: "approvedAt" },
        { text: "Actions", align: "center", value: "actions" }
      ]
    };
  },
  computed: {
    flat: function() {
      const list = [];
      let items = this.$store.state.idResource.publish.items;

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
              publishedAt: env.publishedAt,
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
      return this.$store.state.idResource.publish.loading;
    }
  },
  methods: {
    ...mapActions("idResource", ["getPublishedByFilter", "publishResources", "approveResources"]),
    async onPublish() {
      this.processing = true;

      const input = {
        destinationEnvionmentId: this.filter.environment,
        resources: this.selected.filter(this.publishable).map(x => x.id)
      };
      await this.publishResources(input);
      this.onRefresh();

      this.selected = [];
      this.processing = false;
    },
    async onPublishItem(item) {
      this.processing = true;

      await this.publishResources({
        destinationEnvionmentId: item.environment.id,
        resources: [item.id]
      });

      this.onRefresh();
      this.processing = false;
    },
    async onApprove() {
      this.processing = true;

      const input = {
        resources: this.selected.filter(this.aprovable).map(x => ({
          resourceId: x.id,
          environmentId: x.environment.id,
          type: x.type,
          version: x.currentVersion.version
        }))
      };
      await this.approveResources(input);
      this.onRefresh();

      this.selected = [];
      this.processing = false;
    },
    async onApproveItem(item) {
      this.processing = true;

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
      this.processing = false;
    },
    publishable(item) {
      return item.state !== 'Latest' &&
        item.state !== 'Publishing' &&
        item.state !== 'NotApproved';
    },
    aprovable(item) {
      return item.state !== 'Latest' &&
        item.state !== 'Approving' &&
        item.state !== 'NotDeployed';
    },
    selectAllToggle(props) {
      if(this.selected.length == 0) {
        props.items.forEach(item => {
          if(item.state !== 'Latest') {
            this.selected.push(item);
          }
        });
      } else {
        this.selected = [];
      }
    },
    onRefresh: function() {
      this.getPublishedByFilter({
        environment:
          this.filter.environment == "ALL" ? null : this.filter.environment,
        resourceTypes: this.filter.resourceTypes,
        identityServerGroupId: this.filter.identityServerGroupId
      });
    }
  }
};
</script>

<style></style>
