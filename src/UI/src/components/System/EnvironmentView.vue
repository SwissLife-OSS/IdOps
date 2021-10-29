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
            v-for="environment in environments"
            :key="environment.id"
            selectable
            @click="onSelectEnvironment(environment)"
          >
            <v-list-item-content>
              <v-list-item-title v-text="environment.name"></v-list-item-title>
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
    environments: function () {
      return this.$store.state.system.environment.items;
    },
  },
  methods: {
    onClickAdd: function () {
      this.$router.push({ name: "Environment_New" });
    },
    onSelectEnvironment: function (environment) {
      this.$router.push({
        name: "Environment_Edit",
        params: { id: environment.id },
      });
    },
  },
};
</script>

<style>
</style>