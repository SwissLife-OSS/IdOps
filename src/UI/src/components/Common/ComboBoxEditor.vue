<template>
  <div>
    <v-combobox
      :label="label"
      v-model="comboValue"
      chips
      multiple
      clearable
      deletable-chips
      small-chips
      append-icon="mdi-pencil-outline"
      @click:append="dialog = true"
    ></v-combobox>
    <v-dialog hide-overlay width="800" v-model="dialog">
      <v-card elevation="0" height="400">
        <v-toolbar elevation="0" height="42" color="blue lighten-4">
          <v-toolbar-title>{{ label }}</v-toolbar-title>
          <v-spacer> </v-spacer>
          <v-icon @click="dialog = false">mdi-close</v-icon>
        </v-toolbar>
        <v-card-text class="mt-6 mb-0">
          <v-textarea outlined rows="10" v-model="text"> </v-textarea>
        </v-card-text>
      </v-card>
    </v-dialog>
  </div>
</template>

<script>
export default {
  props: ["label", "value"],
  data() {
    return {
      dialog: false,
    };
  },
  computed: {
    text: {
      get: function () {
        return this.value.join("\n");
      },
      set: function (value) {
        this.$emit(
          "input",
          value.split("\n").filter((x) => x && x.length > 0)
        );
      },
    },
    comboValue: {
      get() {
        return this.value;
      },
      set(val) {
        this.$emit("input", val);
      },
    },
  },
};
</script>

<style>
</style>