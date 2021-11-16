<template>
  <v-data-table
    :headers="headers"
    height="420"
    :items="items"
    single-expand
    :expanded.sync="expanded"
    :sort-by.sync="sortBy"
    :sort-desc.sync="sortDesc"
    item-key="id"
    show-expand
  >
    <template v-slot:expanded-item="{ headers, item }">
      <td :colspan="headers.length">
        <v-simple-table v-if="item.changes.length > 0">
          <template v-slot:default>
            <thead>
              <tr>
                <th class="text-left">Property</th>
                <th class="text-left">Before</th>
                <th class="text-left">After</th>
                <th class="text-left">Delta</th>
                <th class="text-left">Index</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="change in item.changes" :key="change.property">
                <td>{{ change.property }}</td>
                <td>{{ change.before }}</td>
                <td>{{ change.after }}</td>
                <td>{{ change.delta }}</td>
                <td>{{ change.arrayIndex }}</td>
              </tr>
            </tbody>
          </template>
        </v-simple-table>
        <span v-else>No Changes</span>
      </td>
    </template>

    <template v-slot:item.timestamp="{ item }">
      {{ item.timestamp | dateformat }}
    </template>
  </v-data-table>
</template>

<script>
import { searchAudits } from "../../services/idResourceService";
export default {
  props: ["resourceId"],
  created() {
    this.loadAudits();
  },
  watch: {
    resourceId: function () {
      this.loadAudits();
    },
  },
  data() {
    return {
      sortBy: 'version',
      sortDesc: true,
      expanded: [],
      singleExpand: true,
      headers: [
        {
          text: "Type",
          align: "start",
          value: "resourceType",
          sortable: false,
        },
        { text: "Action", value: "action" },
        { text: "Version", value: "version" },
        { text: "Timestamp", value: "timestamp" },
        { text: "User", value: "userId" },
      ],
      items: [],
    };
  },

  methods: {
    async loadAudits() {
      this.items = [];
      const result = await searchAudits({
        pageNr: 0,
        pageSize: 50,
        resourceId: this.resourceId,
      });
      this.items = result.data.searchResourceAudits.items;
    },
  },
};
</script>

<style>
</style>
