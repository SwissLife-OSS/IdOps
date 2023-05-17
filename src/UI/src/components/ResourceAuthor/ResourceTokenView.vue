<template>
  <div>
    <v-switch label="Enable Token generation" v-model="allowTokenGeneration" :disabled="!clientHasSavedSecrets"></v-switch>
    <v-switch label="Log allowTokenGeneration" @click="log" ></v-switch>
  </div>
  
</template>





<script>
import { getClientById } from "../../services/idResourceService";

export default {
  props: ["id"],
  data() {
    return {
      allowTokenGeneration: false,
      client: {},
    }
  },
  computed: {
    secrets: function() {
      return this.client.clientSecrets
    },
    clientHasSavedSecrets: function() {
      //undefined if client hasn't been set because of async
      if (this.secrets === undefined) return false;
      return this.secrets.some(secret => secret.encryptedSecret !== null);
    }
  },
  methods: {
    async setClient() {
      this.loading = true;
      const result = await getClientById(this.id);
      this.loading = false;
      const { client } = result.data;
      this.client = Object.assign({}, client);

      delete this.client.__typename;
    },
    log() {
      console.log(this.allowTokenGeneration)
    }

  },
  watch: {
    id: {
      immediate: true,
      handler: function () {
        this.setClient();
      },
    }
  }

};
</script>

<style></style>
