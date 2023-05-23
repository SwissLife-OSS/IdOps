<template>
    <v-dialog
     v-model="activator"
     width="600">
      <v-card height="300" v-if="grantType === 'client_credentials'" key="client_credentials">
        <v-card-title class="headline">Token</v-card-title>
        <v-card-text>{{ token }}</v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="primary" text @click.native="close">Close</v-btn>
        </v-card-actions>
      </v-card>
      <v-card height="300" v-else-if="grantType === 'authorization_code'" key="authorization_code">
        <v-card-title class="headline">Token</v-card-title>
        <v-card-text>Authorization_Code hasn't been implemented yet</v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="primary" text @click.native="close">Close</v-btn>
        </v-card-actions>
      </v-card>
      <v-card height="300" v-else key="default">
        <v-card-title class="headline">Token</v-card-title>
        <v-card-text>Grant_Type "{{ grantType }}" not available</v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="primary" text @click.native="close">Close</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
</template>

<script>

export default {
  props: ["client", "grantType", "activator"],
  computed: {
    token() {
      return  this.grantType + " - This is token " + JSON.stringify(this.client);
    }
  },
  methods: {
    close() {
        this.$emit('update:activator', false)
      },
    log(message) {
      console.log(message);
    }
  }
};
</script>

<style></style>
