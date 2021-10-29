<template>
  <div>
    <slot v-if="ready"></slot>
    <slot v-if="isRootSlot" name="error"></slot>
    <v-app v-if="!ready">
      <div style="width: 100%; height: 60vh">
        <v-container fill-height fluid>
          <v-row align="center" justify="center">
            <v-col class="d-flex justify-center">
              <div class="text-center">
                <v-row>
                  <v-col>
                    <v-progress-circular
                      :size="110"
                      color="blue"
                      indeterminate
                    ></v-progress-circular>
                  </v-col>
                </v-row>

                <v-row>
                  <v-subheader class="text-center" style="margin: auto"
                    >Loading IdOps...</v-subheader
                  >
                </v-row>
                <br />
              </div>
            </v-col>
          </v-row>
        </v-container>
      </div>
    </v-app>
  </div>
</template>

<script>
import { mapActions, mapState } from "vuex";
export default {
  data: () => ({
    message: "Authenticating user...",
  }),

  created() {
    this.getMe();
    this.loadResourceData();
  },
  watch: {
    error: function (newValue) {
      if (newValue && !this.$route.meta.isRoot) {
        this.$router.push({ name: "Error" });
      }
    },
  },
  computed: {
    ...mapState("user", ["me", "error"]),
    ...mapState("idResource", ["resourceDataReady"]),
    ready: function () {
      return (
        this.me != null && this.resourceDataReady && !this.$route.meta.isRoot
      );
    },
    isRootSlot: function () {
      return this.error || this.$route.meta.isRoot;
    },
  },
  methods: {
    ...mapActions("user", ["getMe"]),
    ...mapActions("idResource", ["loadResourceData"]),
  },
};
</script>

<style>
</style>

