<template>
  <div v-if="lastCreated">
    <v-alert outlined type="success" prominent border="left" class="ma-4">
      Application <strong>{{ lastCreated.application.name }}</strong> and
      clients created. Please copy client secrets when provided, you will not be
      able to retrieve these secrets again.
    </v-alert>

    <v-list
      :style="{ 'max-height': $vuetify.breakpoint.height - 220 + 'px' }"
      class="overflow-y-auto mt-0"
    >
      <v-list-item v-for="client in lastCreated.clients" :key="client.id">
        <v-list-item-content>
          <v-divider></v-divider>

          <v-list-item-title
            >Name: <strong>{{ client.name }}</strong></v-list-item-title
          >
          <v-list-item-title
            >ClientId:<strong> {{ client.clientId }}</strong></v-list-item-title
          >
          <v-text-field
            class="mt-4"
            v-if="client.secretValue"
            :value="client.secretValue"
            label="Plain text secret"
            append-outer-icon="mdi-clipboard"
            @click="onCopyToClip(client.secretValue)"
            v-clipboard
          ></v-text-field>
        </v-list-item-content>
      </v-list-item>
    </v-list>
  </div>
</template>

<script>
import { mapState } from "vuex";
export default {
  computed: {
    ...mapState("application", ["lastCreated"]),
  },
  methods: {
    onCopyToClip: function (secret) {
      this.$clipboard(secret);
    },
  },
};
</script>

<style>
</style>