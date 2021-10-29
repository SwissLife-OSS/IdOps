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
          name: "Environments",
          route: "Environment",
          permission: "ENV_MANAGE",
        },
        { name: "Tenants", route: "Tenant", permission: "TENANT_MANAGE" },
        {
          name: "Identity Servers",
          route: "IdentityServer",
          permission: "IDSRV_MANAGE",
        },
      ],
    };
  },
};
</script>