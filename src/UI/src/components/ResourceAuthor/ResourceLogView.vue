<template>
  <v-data-table
    :headers="headers"
    height="420"
    :items="items"
    :sort-by.sync="sortBy"
    :sort-desc.sync="sortDesc"
    item-key="id"
  >
    <template>
      <td :colspan="headers.length">
        <v-simple-table>
          <template v-slot:default>
            <tbody>
              <tr v-for="item in items" :key="item.id">
                <td>{{ item.user }}</td>
                <td>{{ item.timestamp }}</td>
                <td>{{ item.version }}</td>
              </tr>
            </tbody>
          </template>
        </v-simple-table>
      </td>
    </template>

    <template v-slot:item.timestamp="{ item }">
      {{ item.timestamp | dateformat }}
    </template>
  </v-data-table>
</template>

<script>
import { getResourceApprovalLog } from "../../services/approvalService";
import { getResourcePublishingLog } from "../../services/publishingService";
export default {
  props: ["resourceId"],
  created() {
    this.load();
  },
  watch: {
    resourceId: function() {
      this.load();
    }
  },
  data() {
    return {
      sortBy: "timestamp",
      sortDesc: true,
      headers: [
        {
          text: "User",
          align: "left",
          value: "user",
          sortable: false
        },
        {
          text: "Operation",
          align: "center",
          value: "operation",
          sortable: false
        },
        { text: "Version", align: "center", value: "version", sortable: false },
        {
          text: "Timestamp",
          align: "center",
          value: "timestamp",
          sortable: true
        }
      ],
      items: []
    };
  },

  methods: {
    async load() {
      this.items = [];
      const approvalLog = await getResourceApprovalLog({
        resourceId: this.resourceId
      });
      approvalLog.data.resourceApprovalLog.map(log => {
        this.items.push({
          user: log.requestedBy,
          timestamp: log.approvedAt,
          operation: "Approved",
          version: log.version
        });
      });
      const publishingLog = await getResourcePublishingLog({
        resourceId: this.resourceId
      });
      publishingLog.data.resourcePublishingLog.map(log => {
        this.items.push({
          user: log.requestedBy,
          timestamp: log.publishedAt,
          operation: "Published",
          version: log.version
        });
      });
      return this.items;
    }
  }
};
</script>

<style></style>
