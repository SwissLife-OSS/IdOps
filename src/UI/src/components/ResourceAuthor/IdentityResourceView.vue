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
            v-for="res in identityResources"
            :key="res.id"
            selectable
            @click="onSelectResource(res)"
          >
            <v-list-item-content>
              <v-list-item-title v-text="res.name"></v-list-item-title>
              <div
                class="chip"
                :style="{ 'background-color': res.identityServerGroup.color }"
              >
                {{ res.identityServerGroup.name }}
              </div>
              <v-list-item-subtitle
                v-text="res.displayName"
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
    identityResources: function () {
      const regex = new RegExp(`.*${this.searchText}.*`, "i");
      return this.$store.state.idResource.identityResource.items.filter((x) =>
        regex.test(x.name)
      );
    },
  },
  methods: {
    onClickAdd: function () {
      this.$router.push({ name: "IdentityResource_New" });
    },
    onSelectResource: function (res) {
      this.$router.push({
        name: "IdentityResource_Edit",
        params: { id: res.id },
      });
    },
  },
};
</script>

<style scoped>
.chip {
  height: 20px;
  top: 0px;
  margin-top: 12px;
  padding-left: 4px;
  padding-right: 4px;
  background-color: #f1f1f1;
  right: 10px;
  position: absolute;
  border-radius: 10px;
  font-size: 10px;
  color: #fff;
  line-height: 20px;
  text-align: center;
}
</style>