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
            v-for="template in clientTemplates"
            :key="template.id"
            selectable
            @click="onSelectTemplate(template)"
          >
            <v-list-item-content>
              <v-list-item-title v-text="template.name"></v-list-item-title>
              <v-list-item-subtitle
                v-text="template.displayName"
              ></v-list-item-subtitle>
              <div
                class="chip-tenant"
                :style="{ 'background-color': template.tenantInfo.color }"
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
  created() {},
  data() {
    return {
      searchText: "",
    };
  },
  computed: {
    clientTemplates: function () {
      let templates = this.$store.state.application.clientTemplate.items;

      if (this.searchText) {
        const regex = new RegExp(`.*${this.searchText}.*`, "i");
        templates = templates.filter(
          (x) => regex.test(x.name) || regex.test(x.id)
        );
      }

      return templates;
    },
  },
  methods: {
    ...mapActions("application", ["getClientTemplateById"]),

    onClickAdd: function () {
      this.$router.push({ name: "ClientTemplate_New" });
    },
    onSelectTemplate: function (template) {
      this.$router.push({
        name: "ClientTemplate_Edit",
        params: { id: template.id },
      });
    },
  },
};
</script>

<style>
</style>