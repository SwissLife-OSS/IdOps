<template>
  <div style="width: 100%; height: 60vh">
    <v-container fill-height fluid>
      <v-row align="center" justify="center">
        <v-col class="d-flex justify-center">
          <v-alert
            class="text-center"
            type="warning"
            elevation="1"
            prominent
            outlined
            width="500"
          >
            You are not authorized to access this page.<br />
            <v-btn class="mt-8" @click="login">Login </v-btn>
          </v-alert></v-col
        >
      </v-row>
    </v-container>
  </div>
</template>

<script>
export default {
  methods: {
    login: function () {
      window.location.href = "/api/session/auth";
    },
  },
};
</script>

<style>
</style>