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
            v-for="group in groups"
            :key="group.name"
            selectable
            @click="onSelectGroup(group)"
          >
            <v-list-item-content>
              <v-list-item-title v-text="group.name"></v-list-item-title>
              <div
                class="chip-tenant"
                :style="{ 'background-color': group.color }"
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
export default {
  created() { },
  data() {
    return {
      searchText: "",
    };
  },
  computed: {
    groups: function () {
      let groups = this.$store.state.system.identityServerGroup.items;

      if (this.searchText) {
        const regex = new RegExp(`.*${this.searchText}.*`, "i");
        groups = groups.filter((x) => regex.test(x.name));
      }

      return groups;
    },
  },
  methods: {
    onClickAdd: function () {
      this.$router.push({ name: "IdentityServerGroup_New" });
    },
    onSelectGroup: function (group) {
      this.$router.push({ name: "IdentityServerGroup_Edit", params: { id: group.id } });
    },
  },
};
</script>

<style></style>
