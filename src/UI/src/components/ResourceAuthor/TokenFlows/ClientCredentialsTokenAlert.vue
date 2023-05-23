<template>
    <v-dialog
     v-model="activator"
     width="600">
      <v-card height="300">
        <v-card-title class="headline">Token</v-card-title>
        <v-card-text>{{ token }}</v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="primary" text @click.native="close">Close</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
</template>

<script>
export default {
  props: ["id", "grantType", "activator"],
  data() {
    return {
      token: this.grantType + " - This is token " + this.id,
    };
  },
  watch: {
    grantType: {
            immediate: true,
            handler: function () {
                this.token = this.grantType + " - This is token " + this.id;
            },
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
