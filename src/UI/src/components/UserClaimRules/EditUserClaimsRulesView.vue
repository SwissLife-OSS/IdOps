<template>
  <div>
    <resource-edit-card
      :title="title"
      :loading="loading"
      :resource="rule"
      :tools="['PUBLISH', 'AUDIT', 'LOG']"
      @Save="onSave"
    >
      <v-tabs vertical v-model="tab">
        <v-tab>
          <v-icon left> mdi-file-document-edit-outline </v-icon>
        </v-tab>
        <v-tab>
          <v-icon left>mdi-account-key-outline</v-icon>
        </v-tab>
        <v-tab>
          <v-icon left>mdi-bomb</v-icon>
        </v-tab>
        <v-tab-item key="core">
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-row>
              <v-col md="8"
                ><v-text-field
                  label="Name"
                  :rules="[(v) => !!v || 'Required']"
                  v-model="rule.name"
                  :disabled="id != null"
                ></v-text-field
              ></v-col>

              <v-col md="4">
                <v-select
                  label="Tenant"
                  v-model="rule.tenant"
                  :items="tenants"
                  item-text="id"
                  :disabled="id != null"
                  item-value="id"
                ></v-select
              ></v-col>
            </v-row>
            <v-row>
              <v-col md="8">
                <v-autocomplete
                  label="Application"
                  :items="applications"
                  v-model="rule.applicationId"
                  item-text="name"
                  item-value="id"
                  clearable
                ></v-autocomplete>
              </v-col>
            </v-row>
            <v-row>
              <v-col md="12">
                <h4 class="blue--text text--darken-3">Rules</h4>
                <div class="mt-4" v-if="rule.rules">
                  <v-row v-for="(r, i) in rule.rules" :key="i">
                    <v-col md="2">
                      <v-select
                        label="Environment"
                        :items="environments"
                        item-text="name"
                        item-value="id"
                        dense
                        v-model="r.environmentId"
                      >
                      </v-select>
                    </v-col>
                    <v-col md="3">
                      <v-text-field
                        dense
                        label="Claim type"
                        :rules="[(v) => !!v || 'Required']"
                        v-model="r.claimType"
                      ></v-text-field
                    ></v-col>
                    <v-col md="4"
                      ><v-text-field
                        dense
                        label="Value"
                        :rules="[(v) => !!v || 'Required']"
                        v-model="r.value"
                      ></v-text-field
                    ></v-col>
                    <v-col md="2">
                      <v-select
                        label="Match"
                        :items="matchModes"
                        dense
                        v-model="r.matchMode"
                      >
                      </v-select>
                    </v-col>
                    <v-col md="1">
                      <v-icon @click="onRemoveRule(i)"
                        >mdi-delete-outline</v-icon
                      >
                    </v-col>
                  </v-row>

                  <v-toolbar elevation="0">
                    <v-menu
                      :close-on-content-click="false"
                      :nudge-width="600"
                      offset-y
                      v-model="rimport.menu"
                    >
                      <template v-slot:activator="{ on, attrs }">
                        <v-icon v-bind="attrs" v-on="on"
                          >mdi-database-import-outline</v-icon
                        >
                      </template>
                      <v-card>
                        <v-card-text>
                          <v-textarea
                            label="Import Json"
                            outlined
                            v-model="rimport.json"
                            rows="10"
                          ></v-textarea>
                        </v-card-text>
                        <v-card-actions>
                          <v-spacer></v-spacer>
                          <v-btn @click="importRule">Import</v-btn>
                        </v-card-actions>
                      </v-card>
                    </v-menu>

                    <v-spacer></v-spacer>
                    <v-btn text color="primary" @click="onAddRule">
                      Add Rule<v-icon>mdi-pencil-plus</v-icon></v-btn
                    >
                  </v-toolbar>
                </div>
              </v-col>
            </v-row>
          </v-form>
        </v-tab-item>
        <v-tab-item key="claims">
          <v-row>
            <v-col md="8">
              <h4 class="blue--text text--darken-3">Claims</h4>
              <div class="mt-4" v-if="rule.claims">
                <v-row class="mt-4" v-for="(claim, i) in rule.claims" :key="i">
                  <v-col md="4">
                    <v-text-field
                      dense
                      label="Claim type"
                      v-model="claim.type"
                      :rules="[(v) => !!v || 'Required']"
                    ></v-text-field
                  ></v-col>
                  <v-col md="7"
                    ><v-text-field
                      dense
                      label="Value"
                      v-model="claim.value"
                      :rules="[(v) => !!v || 'Required']"
                    ></v-text-field
                  ></v-col>
                  <v-col md="1">
                    <v-icon @click="onRemoveClaim(i)"
                      >mdi-delete-outline</v-icon
                    >
                  </v-col>
                </v-row>
                <v-toolbar elevation="0">
                  <v-spacer></v-spacer>
                  <v-btn text color="primary" @click="onAddClaim">
                    Add Claim<v-icon>mdi-pencil-plus</v-icon></v-btn
                  >
                </v-toolbar>
              </div>
            </v-col>
          </v-row>
        </v-tab-item>
      </v-tabs>
    </resource-edit-card>
  </div>
</template>
<script>
import { mapActions } from "vuex";
import { getRuleById } from "../../services/userClaimsRuleService";
import ResourceEditCard from "../ResourceAuthor/ResourceEditCard";

export default {
  components: { ResourceEditCard },
  props: ["id"],
  mounted() {
    if (this.id == null && this.tenants.length === 1) {
      this.rule.tenant = this.tenants[0].id;
    }
  },
  watch: {
    id: {
      immediate: true,
      handler: function () {
        this.setRule();
      },
    },
  },
  data() {
    return {
      rimport: {
        menu: false,
        json: null,
      },
      loading: false,
      tab: null,
      valid: null,
      searchText: "",
      rule: {
        id: null,
        name: null,
        tenant: null,
        applicationId: null,
        rules: [],
        claims: [],
      },
    };
  },
  computed: {
    title: function () {
      if (this.id) {
        return this.rule.name;
      }
      return "user claims rule";
    },
    tenants: function () {
      return this.$store.state.system.tenant.items;
    },
    applications: function () {
      return this.$store.state.application.list.items;
    },
    environments: function () {
      return [
        ...[{ id: "GLOBAL", name: "Global" }],
        ...this.$store.state.system.environment.items,
      ];
    },
    matchModes: function () {
      return this.$store.state.userClaimsRule.enums.matchModes;
    },
    rulesJson: {
      set: function (value) {
        console.log(value);
      },
      get: function () {
        return JSON.stringify(this.rule.rules, null, 4);
      },
    },
  },
  methods: {
    ...mapActions("userClaimsRule", ["saveRule"]),
    async setRule(rule) {
      if (this.id) {
        if (!rule) {
          this.loading = true;
          const result = await getRuleById(this.id);
          rule = result.data.userClaimsRule;

          console.log(rule.rules);

          for (let i = 0; i < rule.rules.length; i++) {
            const r = rule.rules[i];
            if (r.environmentId === null) {
              r.environmentId = "GLOBAL";
            }
          }

          this.loading = false;
        }
        this.rule = Object.assign({}, rule);
      } else {
        this.rule.id = null;
        this.rule.name = null;
        this.rule.claims = [];
        this.rule.rules = [];

        this.onAddRule();
        this.onAddClaim();

        this.$refs.form.reset();
      }
    },
    async onSave(event) {
      const isValid = this.$refs.form.validate();
      if (!isValid) {
        event.done();
        return;
      }
      const rule = await this.saveRule({
        id: this.rule.id,
        name: this.rule.name,
        tenant: this.rule.tenant,
        applicationId: this.rule.applicationId,
        rules: this.rule.rules.map((x) => {
          return {
            environmentId: x.environmentId == "GLOBAL" ? null : x.environmentId,
            claimType: x.claimType,
            value: x.value,
            matchMode: x.matchMode,
          };
        }),
        claims: this.rule.claims.map((x) => {
          return {
            type: x.type,
            value: x.value,
          };
        }),
      });
      event.done();

      if (rule && this.$route.name === "UserClaimRules_Rule_New") {
        this.$router.replace({
          name: "UserClaimRules_Rule_Edit",
          params: { id: rule.id },
        });
      }
    },
    onRemoveClaim: function (index) {
      this.rule.claims.splice(index, 1);
    },
    onAddClaim: function () {
      this.rule.claims.push({
        type: "",
        value: "",
      });
    },
    onRemoveRule: function (index) {
      this.rule.rules.splice(index, 1);
    },
    onAddRule: function () {
      this.rule.rules.push({
        claimType: "",
        value: "",
        environmentId: "GLOBAL",
        matchMode: this.matchModes[0].value,
      });
    },
    importRule: function () {
      var data = JSON.parse(this.rimport.json);
      this.rimport.menu = false;

      const rule = {
        name: data._id,
        tenant: data.Tenant,
        rules: [],
        claims: [
          {
            type: "role",
            value: data._id,
          },
        ],
      };

      for (let i = 0; i < data.Maps.length; i++) {
        const map = data.Maps[i];
        let envId = null;
        if (map.Environment) {
          var env = this.environments.find((x) => x.name === map.Environment);
          envId = env.id;
        }
        rule.rules.push({
          environmentId: envId,
          claimType: map.Type,
          value: map.Value,
          matchMode: "EQUALS",
        });
      }
      this.rimport.json = null;
      this.rule = Object.assign({}, rule);
    },
  },
};
</script>

<style></style>
