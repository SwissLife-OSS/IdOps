<template>
  <v-card>
    <v-toolbar elevation="0" height="36" dark color="grey darken-3">
      <v-toolbar-title>Event details</v-toolbar-title>
      <v-spacer> </v-spacer>
      <v-btn icon @click="onClose"><v-icon>mdi-close</v-icon></v-btn>
    </v-toolbar>
    <v-card-text>
      <div class="json-wrapper">
        <vue-json-pretty :data="data"> </vue-json-pretty>
      </div>
    </v-card-text>
  </v-card>
</template>

<script>
import VueJsonPretty from "vue-json-pretty";
import "vue-json-pretty/lib/styles.css";
export default {
  components: { VueJsonPretty },
  props: ["data"],
  methods: {
    onClose: function () {
      this.$emit("close");
    },
  },
};
</script>

<style>
</style>