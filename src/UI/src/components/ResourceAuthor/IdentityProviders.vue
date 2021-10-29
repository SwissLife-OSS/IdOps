<template>
  <div>
    <h4 class="blue--text text--darken-3">Providers</h4>
    <div
      class="ap-item"
      v-for="(provider, i) in providers"
      :class="{ enabled: provider.enabled }"
      :key="i"
    >
      <v-row dense>
        <v-col md="10"
          ><h2>{{ provider.name }}</h2></v-col
        >
        <v-col md="2"></v-col>
      </v-row>
      <v-row dense>
        <v-col md="6">
          <v-switch
            label="Enabled"
            @change="onChanged"
            v-model="provider.enabled"
          ></v-switch>
        </v-col>
        <v-col md="6">
          <v-checkbox
            @change="onChanged"
            label="Require MFA"
            :disabled="!provider.enabled"
            v-model="provider.requestMfa"
          ></v-checkbox>
        </v-col>
      </v-row>
      <v-row dense>
        <v-col>
          <v-text-field
            @change="onChanged"
            label="UserId ClaimType"
            v-model="provider.userIdClaimType"
          ></v-text-field>
        </v-col>
      </v-row>
    </div>
  </div>
</template>

<script>
export default {
  props: ["value", "tenant"],
  data() {
    return {
      providers: [],
    };
  },
  watch: {
    value: {
      immediate: true,
      handler: function () {
        //if (this.providers.length > 0) return;
        const providers = [];
        let providersValue = [];

        if (this.value && this.value.length > 0) {
          providersValue = this.value;
        }
        for (let i = 0; i < this.allproviders.length; i++) {
          const provider = this.allproviders[i];

          const existing = providersValue.find((x) => x.name === provider);
          if (existing) {
            providers.push({
              enabled: true,
              name: provider,
              requestMfa: existing.requestMfa,
              userIdClaimType: existing.userIdClaimType,
            });
          } else {
            providers.push({
              enabled: false,
              name: provider,
              requestMfa: false,
              userIdClaimType: null,
            });
          }
        }
        this.providers = providers;
      },
    },
    providers: {
      deep: true,
      handler: function () {
        //this.setModel();
      },
    },
  },
  computed: {
    allproviders: function () {
      const setting = this.$store.getters["system/getModuleSetting"](
        this.tenant,
        "AuthProviders",
        "ProviderNames"
      );
      return setting.split(",");
    },
  },
  methods: {
    onChanged: function () {
      this.setModel();
    },
    setModel() {
      var model = this.providers
        .filter((x) => x.enabled)
        .map((p) => {
          return {
            name: p.name,
            requestMfa: p.requestMfa,
            userIdClaimType:
              p.userIdClaimType != null && p.userIdClaimType.length > 0
                ? p.userIdClaimType
                : null,
          };
        });

      this.$emit("input", model);
    },
  },
};
</script>

<style scoped>
.ap-item {
  border: 1px solid rgba(5, 20, 102, 0.815);
  margin-top: 12px;
  padding: 8px;
  width: 400px;
  border-radius: 6px;
  -webkit-transition: background-color 500ms linear;
  -ms-transition: background-color 500ms linear;
  transition: background-color 500ms linear;
}

.ap-item.enabled {
  background-color: #e3f2fd;
}
</style>