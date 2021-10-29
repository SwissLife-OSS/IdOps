<template>
  <v-row>
    <v-col md="12" :loading="saving">
      <v-toolbar elevation="0">
        <v-spacer></v-spacer>
        <v-menu
          v-model="expiresAtIsOpen"
          :close-on-content-click="false"
          :nudge-right="40"
          transition="scale-transition"
          offset-y
          min-width="auto"
        >
          <template v-slot:activator="{ on, attrs }">
            <v-text-field
              v-model="addSecret.expiresAt"
              label="Expires At"
              prepend-icon="mdi-calendar"
              readonly
              v-bind="attrs"
              v-on="on"
            ></v-text-field>
          </template>
          <v-date-picker
            v-model="addSecret.expiresAt"
            @input="expiresAtIsOpen = false"
          ></v-date-picker>
        </v-menu>
        <v-btn :disabled="saving" text color="primary" @click="onAddSecret">
          Add Secret<v-icon>mdi-pencil-plus</v-icon></v-btn
        >
      </v-toolbar>
    </v-col>
    <v-dialog width="600" v-model="secretDialog" hide-overlay>
      <v-card height="300">
        <v-toolbar height="40" dark color="grey darken-3">Secret</v-toolbar>
        <v-card-text>
          <br />
          <v-alert outlined type="info" prominent border="left">
            Please copy this secret, you will not be able to retrieve this
            secret again.
          </v-alert>
          <v-text-field
            class="mt-4"
            :value="plainTextSecret"
            label="Plain text secret"
            append-outer-icon="mdi-clipboard"
            @click:append-outer="onCopyToClip"
            v-clipboard
          ></v-text-field>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="primary" text @click="secretDialog = false"
            >Close</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script>
export default {
  props: ["id"],
  watch: {
    id: {
      immediate: true,
      handler: function(value) {
        this.addSecret.tokenId = value;
      }
    }
  },
  data() {
    return {
      expiresAtIsOpen: false,
      plainTextSecret: null,
      secretDialog: false,
      saving: false,
      addSecret: {
        expiresAt: null,
        tokenId: this.id
      }
    };
  },
  computed: {
    secretGenerators: function() {
      return ["DEFAULT"];
    }
  },
  methods: {
    async onAddSecret() {
      this.saving = true;
      this.$emit("AddSecret", this.addSecret, secret => {
        this.plainTextSecret = secret;
        this.secretDialog = true;
        this.addSecret.expiresAt = null;
        this.saving = false;
      });
    },
    onCopyToClip: function() {
      this.$clipboard(this.plainTextSecret);
    }
  }
};
</script>

<style></style>
