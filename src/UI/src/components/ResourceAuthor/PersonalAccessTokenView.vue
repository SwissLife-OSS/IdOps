<template>
  <v-row>
    <v-col md="2">
      <v-btn-toggle dense v-model="environment" @change="onEnvChange" rounded>
        <v-btn small value="all">All</v-btn>
        <v-btn small v-for="env in environments" :key="env.id" :value="env.id">
          {{ env.name }}
        </v-btn>
      </v-btn-toggle>
      <v-text-field
        clearable
        v-model="searchText"
        placeholder="Search"
        prepend-icon="mdi-magnify"
        append-outer-icon="mdi-plus"
        @click:append-outer="onClickAdd"
        :loading="loading"
      ></v-text-field>

      <v-list
        two-line
        rounded
        dense
        :style="{ 'max-height': $vuetify.breakpoint.height - 200 + 'px' }"
        class="overflow-y-auto mt-0"
      >
        <v-list-item-group color="primary" select-object>
          <v-list-item
            v-for="personalAccessToken in personalAccessTokens"
            :key="personalAccessToken.id"
            selectable
            dense
            :title="personalAccessToken.title"
            @click="onSelectPersonalAccessToken(personalAccessToken)"
          >
            <v-list-item-content>
              <v-list-item-title
                v-text="formatPersonalAccessToken(personalAccessToken)"
              ></v-list-item-title>
              <v-list-item-subtitle
                class="grey--text text--lighten-1"
                v-text="personalAccessToken.id"
              ></v-list-item-subtitle>
              <div
                class="chip-tenant"
                :style="{
                  'background-color': personalAccessToken.tenantInfo.color
                }"
              ></div>
            </v-list-item-content>
          </v-list-item>
        </v-list-item-group>
      </v-list>
    </v-col>
    <v-col md="10">
      <router-view></router-view>
    </v-col>
  </v-row>
</template>

<script>
import { mapActions } from "vuex";
export default {
  created() {
    this.searchPersonalAccessTokens();
  },
  data() {
    return {
      searchText: "",
      environment: "all"
    };
  },
  computed: {
    personalAccessTokens: function() {
      let personalAccessTokens = this.$store.state.idResource
        .personalAccessToken.items;

      if (this.searchText) {
        const regex = new RegExp(`.*${this.searchText}.*`, "i");
        personalAccessTokens = personalAccessTokens.filter(
          x => regex.test(x.userName) || regex.test(x.id)
        );
      }

      return personalAccessTokens;
    },
    environments: function() {
      return this.$store.state.system.environment.items;
    },
    loading: function() {
      return this.$store.state.idResource.personalAccessToken.loading;
    }
  },
  methods: {
    ...mapActions("idResource", [
      "searchPersonalAccessTokens",
      "setPersonalAccessTokenFilter"
    ]),
    onClickAdd: function() {
      this.$router.push({ name: "PersonalAccessToken_New" });
    },
    onSelectPersonalAccessToken: function(res) {
      this.$router.push({
        name: "PersonalAccessToken_Edit",
        params: { id: res.id }
      });
    },
    formatPersonalAccessToken: function(token) {
      const name =
        this.environments.find(x => x.id === token.environmentId)?.name ?? "-";
      return `${name} - ${token.userName}`;
    },
    onEnvChange: function(value) {
      this.setPersonalAccessTokenFilter({
        environmentId: value == "all" ? null : value
      });
    }
  }
};
</script>

<style scoped></style>
