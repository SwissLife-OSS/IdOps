<template>
  <sub-navigation
    :tabs="tabs"
    color="cyan darken-1"
    name="System"
  ></sub-navigation>
</template>

<script>
import SubNavigation from "../Common/SubNavigation.vue";
export default {
  components: { SubNavigation },
  data() {
    return {
      tabs: [
        {
          name: "Resources",
          route: "resources",
          icon: "mdi-rocket-launch-outline",
          permission: "CA_PUBLISH"
        }
      ]
    };
  }
};
</script>
