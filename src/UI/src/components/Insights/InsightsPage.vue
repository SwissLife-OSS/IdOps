<template>
  <div>
    <v-system-bar window dark color="deep-purple lighten-1" height="32">
      <router-link
        :to="{ name: tab.route }"
        class="tab-button"
        v-for="tab in tabs"
        :key="tab.name"
        v-text="tab.name"
      ></router-link>
    </v-system-bar>
    <div class="ma-2 pa-0">
      <router-view></router-view>
    </div>
  </div>
</template>

<script>
export default {
  created() {
    if (this.$route.name === "Insights") {
      this.$router.push({
        name: this.tabs[0].route,
      });
    }
  },
  data() {
    return {
      tabs: [{ name: "Identity Server Events", route: "IdentityServerEvents" }],
    };
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