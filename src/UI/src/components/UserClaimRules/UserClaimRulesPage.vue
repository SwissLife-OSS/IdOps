<template>
  <sub-navigation
    :tabs="tabs"
    color="light-blue darken-3"
    name="UserClaimRules"
  ></sub-navigation>
</template> 

<script>
import SubNavigation from "../Common/SubNavigation.vue";
export default {
  components: { SubNavigation },
  data() {
    return {
      tabs: [{ name: "Rules", route: "UserClaimRules_Rules" }],
    };
  },
};
</script>