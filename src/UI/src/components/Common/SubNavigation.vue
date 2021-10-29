<template>
  <div>
    <v-system-bar window dark :color="color" height="32">
      <router-link
        :to="{ name: tab.route }"
        class="tab-button"
        v-for="tab in filteredTabs"
        :key="tab.name"
      >
        <v-icon class="mr-1" color="white" v-if="tab.icon">{{
          tab.icon
        }}</v-icon>
        {{ tab.name }}
      </router-link>
    </v-system-bar>
    <div class="ma-2 pa-0">
      <router-view></router-view>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    tabs: {
      type: Array,
      default: () => [],
    },
    name: {
      type: String,
    },
    color: {
      type: String,
    },
  },
  computed: {
    filteredTabs: function () {
      return this.tabs.filter((x) => {
        if (!x.permission) {
          return true;
        } else {
          return this.$store.getters["user/hasPermission"](x.permission);
        }
      });
    },
  },
  mounted() {
    if (this.$route.name === this.name) {
      this.$router.push({ name: this.tabs[0].route });
    }
  },
};
</script>

<style scoped>
.tab-button {
  color: #fff;
  size: 12px;
  margin-left: 10px;
  padding-left: 4px;
  padding-right: 4px;
  text-decoration: none;
}

.tab-button.router-link-active {
  border-bottom: solid 2px #fff;
  border-radius: 1px;
}
</style>