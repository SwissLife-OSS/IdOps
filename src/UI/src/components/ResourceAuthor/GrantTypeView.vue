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
            v-for="grantType in grantTypes"
            :key="grantType.id"
            selectable
            @click="onSelectGrantType(grantType)"
          >
            <v-list-item-content>
              <v-list-item-title v-text="grantType.name"></v-list-item-title>
              <v-list-item-subtitle
                v-text="grantType.id"
              ></v-list-item-subtitle>
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
  created() {},
  data() {
    return {
      searchText: "",
    };
  },
  computed: {
    grantTypes: function () {
      return this.$store.state.idResource.grantType.items;
    },
  },
  methods: {
    onClickAdd: function () {
      this.$router.push({ name: "GrantType_New" });
    },
    onSelectGrantType: function (grantType) {
      this.$router.push({
        name: "GrantType_Edit",
        params: { id: grantType.id },
      });
    },
  },
};
</script>

<style>
</style>