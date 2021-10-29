<template>
  <sub-navigation
    :tabs="tabs"
    color="light-blue darken-2"
    name="ResourceAuthor"
  ></sub-navigation>
</template>

<script>
import SubNavigation from "../Common/SubNavigation.vue";
export default {
  components: { SubNavigation },
  data() {
    return {
      tabs: [
        { name: "Applications", route: "Application" },
        { name: "Clients", route: "Client" },
        { name: "API Resource", route: "ApiResource" },
        { name: "API Scope", route: "ApiScope" },
        {
          name: "Identity Resource",
          route: "IdentityResource",
          permission: "CA_IDENTITY_RESOURCE_MANAGE"
        },
        { name: "Templates", route: "ClientTemplate" },
        {
          name: "Grant Types",
          route: "GrantType",
          permission: "CA_GRANT_TYPE_MANAGE"
        },
        {
          name: "Personal Access Token",
          route: "PersonalAccessToken",
          permission: "CA_PAT_PUBLISH"
        }
      ]
    };
  }
};
</script>
