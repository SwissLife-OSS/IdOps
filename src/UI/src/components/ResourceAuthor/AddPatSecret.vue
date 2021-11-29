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
            :min="minExpiration"
            :max="maxExpiration"
            @input="expiresAtIsOpen = false"
          ></v-date-picker>
        </v-menu>
        <v-btn :disabled="saving || !addSecret.expiresAt" text color="primary" @click="onAddSecret">
          Add Secret<v-icon>mdi-pencil-plus</v-icon></v-btn
        >
      </v-toolbar>
    </v-col>
    <v-dialog width="600" v-model="secretDialog" hide-overlay>
      <v-card height="310">
        <v-toolbar height="40" dark color="grey darken-3">Secret</v-toolbar>
        <v-card-text>
          <br />
          <v-alert outlined type="info" prominent border="left">
            Please copy this secret, you will not be able to retrieve this
            secret again.
          </v-alert>
          <div class="d-flex">
            <v-text-field
              :value="plainTextSecret"
              readonly
              v-on:focus="$event.target.select()"
              label="Plain text secret"
            ></v-text-field>
            <v-btn
              color="primary"
              width="100"
              height="50"
              class="ml-4"
              v-clipboard="() => this.plainTextSecret">Copy</v-btn>
          </div>
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
    },
    minExpiration: function() {
      var now = new Date();
      var tzOffset = now.getTimezoneOffset() * 60000;
      var value = new Date(new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1) - tzOffset);
      return value.toISOString().slice(0,10);
    },
    maxExpiration: function() {
      var now = new Date();
      var tzOffset = now.getTimezoneOffset() * 60000;
      var value = new Date(new Date(now.getFullYear() + 2, now.getMonth(), now.getDate()) - tzOffset);
      return value.toISOString().slice(0,10);
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
    }
  }
};
</script>

<style></style>
