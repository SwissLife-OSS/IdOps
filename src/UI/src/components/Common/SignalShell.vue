<template><div v-if="false"></div></template>

<script>
import { mapActions } from "vuex";
export default {
  created() {
    this.$socket.start();
    const self = this;
    this.$socket.on("published", (data) => {
      self.$store.commit("shell/CONSOLE_MESSAGE", data);

      switch (data.type) {
        case "PUBLISHED":
          this.getPublishedByResource(data.data.resourceId);
          self.$store.commit("idResource/RESOURCE_PUBLISH_SUCCESS", data.data);

          break;
      }
    });
  },
  data() {
    return {};
  },
  computed: {},
  methods: {
    ...mapActions("idResource", ["getPublishedByResource"]),
  },
};
</script>

<style scoped>
</style>