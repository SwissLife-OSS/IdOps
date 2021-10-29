<template>
  <div>
    <h4 class="blue--text text--darken-3">Secrets</h4>
    <v-list style="max-height: 400px" class="overflow-y-auto">
      <v-list-item v-for="secret in secrets" :key="secret.id">
        <v-list-item-avatar>
          <v-icon small class="yellow darken-3" dark> mdi-key </v-icon>
        </v-list-item-avatar>

        <v-list-item-content>
          <v-list-item-title v-text="secret.token"></v-list-item-title>
          <v-list-item-subtitle>
            Created:
            {{ secret.createdAt | dateformat }}
          </v-list-item-subtitle>
          <v-list-item-subtitle>
            Expires:
            {{ secret.expiresAt | dateformat }}
          </v-list-item-subtitle>
        </v-list-item-content>
        <v-list-item-action>
          <v-btn icon @click="onRemoveSecret(secret.id)">
            <v-icon color="grey lighten-1">mdi-delete-outline</v-icon>
          </v-btn>
        </v-list-item-action>
      </v-list-item>
    </v-list>
  </div>
</template>

<script>
export default {
  props: ["id", "secrets"],
  methods: {
    async onRemoveSecret(id) {
      this.$emit("Remove", id);
    }
  }
};
</script>

<style></style>
