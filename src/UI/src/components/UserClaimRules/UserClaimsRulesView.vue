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
            v-for="rule in rules"
            :key="rule.id"
            selectable
            @click="onSelectRule(rule)"
          >
            <v-list-item-content>
              <v-list-item-title v-text="rule.name"></v-list-item-title>
              <div
                class="chip-tenant"
                :style="{ 'background-color': rule.tenantInfo.color }"
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
    this.getRules();
  },
  data() {
    return {
      searchText: "",
    };
  },
  computed: {
    rules: function () {
      let rules = this.$store.state.userClaimsRule.rules.items;

      if (this.searchText) {
        const regex = new RegExp(`.*${this.searchText}.*`, "i");
        rules = rules.filter((x) => regex.test(x.name) || regex.test(x.id));
      }

      return rules;
    },
  },
  methods: {
    ...mapActions("userClaimsRule", ["getRules"]),
    onClickAdd: function () {
      this.$router.push({ name: "UserClaimRules_Rule_New" });
    },
    onSelectRule: function (rule) {
      this.$router.push({
        name: "UserClaimRules_Rule_Edit",
        params: { id: rule.id },
      });
    },
  },
};
</script>

<style></style>
