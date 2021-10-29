<template>
  <div>
    <v-row v-for="(value, name) in item" :key="name">
      <v-col md="4">{{ name }}</v-col>
      <v-col md="8"
        ><strong>{{ value }}</strong></v-col
      >
    </v-row>
  </div>
</template>

<script>
export default {
  props: ["item"],
};
</script>

<style>
</style>