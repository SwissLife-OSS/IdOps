<template>
  <div>
    <v-card elevation="2" :loading="saving">
      <v-card-title>Add new secret {{ id }}</v-card-title>
      <v-card-text>
        <v-row>
          <v-col md="12">
            <v-select
              v-if="!manualSecretValue"
              :items="secretGenerators"
              label="Secret generator"
              append-outer-icon="mdi-pencil-outline"
              @click:append-outer="manualSecretValue = true"
              v-model="addSecret.generator"
            ></v-select
            ><v-text-field
              v-else
              label="Secret Value"
              v-model="addSecret.value"
              append-outer-icon="mdi-slot-machine-outline"
              @click:append-outer="manualSecretValue = false"
            ></v-text-field
          ></v-col>
        </v-row>
        <v-row>
          <v-col md="8"
            ><v-text-field label="Name" v-model="addSecret.name"></v-text-field
          ></v-col>
          <v-col md="4">
            <v-switch
              label="Save secret value"
              v-model="addSecret.saveValue"
            ></v-switch>
          </v-col>
        </v-row>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn
          :disabled="!valid || saving"
          color="primary"
          elevation="2"
          @click="onAddSecret"
          >Add</v-btn
        >
      </v-card-actions>
    </v-card>

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
  </div>
</template>

<script>
export default {
  props: ["id"],
  watch: {
    id: {
      immediate: true,
      handler: function (value) {
        this.addSecret.id = value;
      },
    },
  },
  data() {
    return {
      plainTextSecret: null,
      secretDialog: false,
      valid: true,
      saving: false,
      manualSecretValue: false,
      addSecret: {
        value: null,
        generator: "DEFAULT",
        name: null,
        id: this.id,
      },
    };
  },
  computed: {
    secretGenerators: function () {
      return ["DEFAULT"];
    },
  },
  methods: {
    async onAddSecret() {
      this.saving = true;
      this.$emit("AddSecret", this.addSecret, (secret) => {
        this.plainTextSecret = secret;
        this.secretDialog = true;
        this.addSecret.value = null;
        this.addSecret.name = null;
        this.manualSecretValue = false;
        this.saving = false;
      });
    },
    onCopyToClip: function () {
      this.$clipboard(this.plainTextSecret);
    },
  },
};
</script>

<style>
</style>