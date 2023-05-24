import {
  addApiSecret,
  addClientSecret,
  createClient,
  getAllDependencies,
  getApiResources,
  getApiScopes,
  getIdentityResources,
  getResourceData,
  removeApiSecret,
  removeClientSecret,
  saveApiResource,
  saveApiScope,
  saveGrantType,
  saveIdentityResource,
  searchClients,
  searchPersonalAccessTokens,
  updataPersonalAccessToken,
  createPersonalAccessToken,
  addPersonalAccessTokenSecret,
  removePersonalAccessTokenSecret,
  searchUnMappedClients,
  updataClient
} from "../services/idResourceService";
import {
  getPublishedResources,
  publishResources
} from "../services/publishingService";
import {
  getResourceApprovals,
  approveResources
} from "../services/approvalService";

import Vue from "vue";
import { excuteGraphQL } from "./graphqlClient";

const addOrUpdateResource = (state, res, type) => {
  const items = state[type].items;
  const index = items.findIndex(x => x.id === res.id);
  if (index > -1) {
    Vue.set(items, index, res);
  } else {
    items.push(res);
  }
};

const updatePublishedEnvironment = (state, id, environmentId, update) => {
  const index = state.publish.items.findIndex(x => x.id === id);
  if (index > -1) {
    const envIndex = state.publish.items[index].environments.findIndex(
      x => x.environment.id === environmentId
    );
    if (envIndex > -1) {
      state.publish.items[index].environments[envIndex] = Object.assign(
        state.publish.items[index].environments[envIndex],
        update
      );
    }
  }
};

const idResourceStore = {
  namespaced: true,
  state: () => ({
    resourceDataReady: false,
    enums: {
      accessTokenType: [],
      tokenExpiration: [],
      tokenUsage: [],
      connectorProfileTypes: []
    },
    apiResource: {
      loading: false,
      items: []
    },
    identityResource: {
      loading: false,
      items: []
    },
    apiScope: {
      loading: false,
      items: []
    },
    personalAccessToken: {
      search: {
        pageNr: 0,
        pageSize: 1000,
        searchText: null,
        environmentId: null
      },
      loading: false,
      items: []
    },
    grantType: {
      loading: false,
      items: []
    },
    client: {
      loading: false,
      hasMore: true,
      items: [],
      search: {
        pageNr: 0,
        pageSize: 1000,
        searchText: null,
        environmentId: null
      },
      dependencies: {
        loading: false,
        items: []
      }
    },
    publish: {
      loading: false,
      items: []
    },
    approval: {
      loading: false,
      items: []
    },
    editor: {
      resourceId: null,
      type: null,
      publish: {
        currentVersion: null,
        environments: null
      }
    }
  }),
  mutations: {
    SET_LOADING(state, value) {
      state[value.entity].loading = value.loading;
    },
    API_RESOURCES_LOADED(state, items) {
      state.apiResource.items = items;
      state.apiResource.loading = false;
    },
    API_RESOURCE_SAVED(state, resource) {
      addOrUpdateResource(state, resource, "apiResource");
    },
    API_SCOPES_LOADED(state, scopes) {
      state.apiScope.items = scopes;
    },
    API_SCOPE_SAVED(state, scope) {
      addOrUpdateResource(state, scope, "apiScope");
    },
    GRANT_TYPES_LOADED(state, grantTypes) {
      state.grantType.items = grantTypes;
    },
    GRANT_TYPE_SAVED(state, grantType) {
      addOrUpdateResource(state, grantType, "grantType");
    },
    IDENTITY_RESOURCES_LOADED(state, items) {
      state.identityResource.items = items;
      state.identityResource.loading = false;
    },
    ENUMS_LOADED(state, data) {
      const {
        accessTokenType,
        tokenExpiration,
        tokenUsage,
        connectorProfileTypes
      } = data;
      state.enums.tokenUsage = tokenUsage;
      state.enums.accessTokenType = accessTokenType;
      state.enums.tokenExpiration = tokenExpiration;
      state.enums.connectorProfileTypes = connectorProfileTypes;
      state.resourceDataReady = true;
    },
    IDENTITY_RESOURCE_SAVED(state, resource) {
      addOrUpdateResource(state, resource, "identityResource");
    },
    PERSONAL_ACCESS_TOKENS_LOADED(state, result) {
      state.personalAccessToken.items = result.items;
      state.personalAccessToken.hasMore = result.hasMore;
      state.personalAccessToken.loading = false;
    },
    PERSONAL_ACCESS_TOKEN_FILTER_SET(state, filter) {
      state.personalAccessToken.filter = Object.assign(
        state.personalAccessToken.search,
        filter
      );
    },
    PERSONAL_ACCESS_TOKEN_CREATED(state, personalAccessToken) {
      addOrUpdateResource(state, personalAccessToken, "personalAccessToken");
    },
    PERSONAL_ACCESS_TOKEN_UPDATED(state, personalAccessToken) {
      addOrUpdateResource(state, personalAccessToken, "personalAccessToken");
    },
    _LOADED(state, result) {
      state.client.items = result.items;
      state.client.hasMore = result.hasMore;
      state.client.loading = false;
    },
    CLIENT_FILTER_SET(state, filter) {
      state.client.filter = Object.assign(state.client.search, filter);
    },
    CLIENTS_LOADED(state, result) {
      state.client.items = result.items;
      state.client.hasMore = result.hasMore;
      state.client.loading = false;
    },
    CLIENT_CREATED(state, client) {
      addOrUpdateResource(state, client, "client");
    },
    CLIENT_UPDATED(state, client) {
      addOrUpdateResource(state, client, "client");
    },
    CLIENT_DEPENDENCIES_LOADED(state, result) {
      state.client.dependencies.items = result.items;
      state.client.dependencies.loading = false;
    },
    PUBLISHED_BYRESOURCE_LOADED(state, resource) {
      state.editor.publish.environments = resource.environments;
      state.editor.publish.currentVersion = resource.currentVersion;
      state.editor.resourceId = resource.id;
      state.editor.type = resource.type;
    },
    PUBLISHED_RESOURCES_LOADED(state, resources) {
      state.publish.items = resources;
      state.publish.loading = false;
    },
    SET_PUBLISHING_BEGIN(state, input) {
      if (state.editor.publish && state.editor.publish.environments) {
        const index = state.editor.publish.environments.findIndex(
          x => x.environment.id == input.environmentId
        );
        if (index > -1) {
          state.editor.publish.environments[index] = Object.assign(
            state.editor.publish.environments[index],
            { state: "Publishing" }
          );
        }
      }

      for (let i = 0; i < input.resources.length; i++) {
        const resId = input.resources[i];
        updatePublishedEnvironment(state, resId, input.environmentId, {
          state: "Publishing"
        });
      }
    },
    RESOURCE_PUBLISH_SUCCESS(state, data) {
      window.setTimeout(() => {
        updatePublishedEnvironment(state, data.resourceId, data.environmentId, {
          state: "Latest"
        });
        //Sometimes signalR message is faster...
      }, 100);
    },
    APPROVED_RESOURCES_LOADED(state, resources) {
      state.approval.items = resources;
      state.approval.loading = false;
    }
  },
  actions: {
    async loadResourceData({ commit, dispatch }) {
      const result = await excuteGraphQL(() => getResourceData(), dispatch);
      if (result.success) {
        commit("API_RESOURCES_LOADED", result.data.apiResources);
        commit("API_SCOPES_LOADED", result.data.apiScopes);
        commit("IDENTITY_RESOURCES_LOADED", result.data.identityResources);
        commit("GRANT_TYPES_LOADED", result.data.grantTypes);
        commit("ENUMS_LOADED", result.data);
      }
    },

    async loadApiResources({ commit, getters, dispatch }) {
      commit("SET_LOADING", { entity: "apiResource", loading: true });
      const result = await excuteGraphQL(
        () => getApiResources({ tenants: getters.tenantFilter }),
        dispatch
      );
      if (result.success) {
        commit("API_RESOURCES_LOADED", result.data.apiResources);
      } else {
        commit("SET_LOADING", { entity: "apiResource", loading: false });
      }
    },
    async loadIdentityResources({ commit, getters, dispatch }) {
      commit("SET_LOADING", { entity: "identityResource", loading: true });
      const result = await excuteGraphQL(
        () => getIdentityResources({ tenants: getters.tenantFilter }),
        dispatch
      );
      if (result.success) {
        commit("IDENTITY_RESOURCES_LOADED", result.data.identityResources);
      } else {
        commit("SET_LOADING", { entity: "identityResource", loading: false });
      }
    },

    async saveApiResource({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => saveApiResource(input),
        dispatch
      );

      if (result.success) {
        const { apiResource } = result.data.saveApiResource;
        commit("API_RESOURCE_SAVED", apiResource);

        return apiResource;
      }

      return null;
    },
    async loadApiScopes({ commit, getters, dispatch }) {
      commit("SET_LOADING", { entity: "apiScope", loading: true });
      const result = await excuteGraphQL(
        () => getApiScopes({ tenants: getters.tenantFilter }),
        dispatch
      );
      if (result.success) {
        commit("API_SCOPES_LOADED", result.data.apiScopes);
      } else {
        commit("SET_LOADING", { entity: "apiScope", loading: false });
      }
    },
    async saveApiScope({ commit, dispatch }, input) {
      const result = await excuteGraphQL(() => saveApiScope(input), dispatch);

      if (result.success) {
        const { apiScope } = result.data.saveApiScope;
        commit("API_SCOPE_SAVED", apiScope);
        return apiScope;
      }

      return null;
    },
    async saveGrantType({ commit, dispatch }, input) {
      const result = await excuteGraphQL(() => saveGrantType(input), dispatch);

      if (result.success) {
        const { grantType } = result.data.saveGrantType;
        commit("GRANT_TYPE_SAVED", grantType);
        return grantType;
      }

      return null;
    },
    async saveIdentityResource({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => saveIdentityResource(input),
        dispatch
      );

      if (result.success) {
        const { identityResource } = result.data.saveIdentityResource;
        commit("IDENTITY_RESOURCE_SAVED", identityResource);
        return identityResource;
      }

      return null;
    },
    async searchUnMappedClients({ state, commit, dispatch }) {
      commit("SET_LOADING", { entity: "client", loading: true });
      const result = await excuteGraphQL(
        () => searchUnMappedClients(state),
        dispatch
      );

      if (result.success) {
        commit("CLIENTS_LOADED", result.data.searchUnMappedClients);
      } else {
        commit("SET_LOADING", { entity: "client", loading: false });
      }
    },

    async searchClients({ state, commit, getters, dispatch }) {
      commit("SET_LOADING", { entity: "client", loading: true });

      const result = await excuteGraphQL(
        () =>
          searchClients(
            Object.assign(state.client.search, {
              tenants: getters.tenantFilter
            })
          ),
        dispatch
      );

      if (result.success) {
        commit("CLIENTS_LOADED", result.data.searchClients);
      } else {
        commit("SET_LOADING", { entity: "client", loading: false });
      }
    },

    async searchPersonalAccessTokens({ state, commit, getters, dispatch }) {
      commit("SET_LOADING", { entity: "personalAccessToken", loading: true });

      const result = await excuteGraphQL(
        () =>
          searchPersonalAccessTokens(
            Object.assign(state.personalAccessToken.search, {
              tenants: getters.tenantFilter
            })
          ),
        dispatch
      );

      if (result.success) {
        commit(
          "PERSONAL_ACCESS_TOKENS_LOADED",
          result.data.searchPersonalAccessTokens
        );
      } else {
        commit("SET_LOADING", {
          entity: "personalAccessTokens",
          loading: false
        });
      }
    },

    async loadClientDependencies({ commit, dispatch }, input) {
      commit("SET_LOADING", { entity: "clientDependencies", loading: true });
      const result = await excuteGraphQL(
        () => getAllDependencies(input),
        dispatch
      );
      if (result.success) {
        commit("CLIENT_DEPENDENCIES_LOADED", result.data.client.dependencies);
      } else {
        commit("SET_LOADING", { entity: "clientDependencies", loading: false });
      }
    },
    async setPersonalAccessTokenFilter({ commit, dispatch }, filter) {
      commit("PERSONAL_ACCESS_TOKEN_FILTER_SET", filter);
      dispatch("searchPersonalAccessTokens");
    },
    async setClientFilter({ commit, dispatch }, filter) {
      commit("CLIENT_FILTER_SET", filter);
      dispatch("searchClients");
    },
    tenantFilterUpdated: function ({ dispatch }) {
      dispatch("searchClients");
      dispatch("loadApiResources");
      dispatch("loadApiScopes");
      dispatch("loadIdentityResources");
      dispatch("searchPersonalAccessTokens");
    },
    async createPersonalAccessToken({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => createPersonalAccessToken(input),
        dispatch
      );

      if (result.success) {
        const { token } = result.data.createPersonalAccessToken;
        commit("PERSONAL_ACCESS_TOKEN_CREATED", token);

        return token;
      }
      return null;
    },
    async updatePersonalAccessToken({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => updataPersonalAccessToken(input),
        dispatch
      );

      if (result.success) {
        const { token } = result.data.updatePersonalAccessToken;
        commit("PERSONAL_ACCESS_TOKEN_UPDATED", token);
        return token;
      }
      return null;
    },
    async createClient({ commit, dispatch }, input) {
      const result = await excuteGraphQL(() => createClient(input), dispatch);

      if (result.success) {
        const { client } = result.data.createClient;
        commit("CLIENT_CREATED", client);

        return client;
      }
      return null;
    },
    async updateClient({ commit, dispatch }, input) {
      const result = await excuteGraphQL(() => updataClient(input), dispatch);

      if (result.success) {
        const { client } = result.data.updateClient;
        commit("CLIENT_UPDATED", client);
        return client;
      }
      return null;
    },
    async addClientSecret({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => addClientSecret(input),
        dispatch
      );

      if (result.success) {
        const { client } = result.data.addClientSecret;
        commit("CLIENT_UPDATED", client);
        return result.data.addClientSecret;
      }
      return null;
    },
    async removeClientSecret({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => removeClientSecret(input),
        dispatch
      );

      if (result.success) {
        const { client } = result.data.removeClientSecret;
        commit("CLIENT_UPDATED", client);
        return client;
      }
      return null;
    },
    async addApiSecret({ commit, dispatch }, input) {
      const result = await excuteGraphQL(() => addApiSecret(input), dispatch);

      if (result.success) {
        const { apiResource } = result.data.addApiSecret;
        commit("API_RESOURCE_SAVED", apiResource);
        return result.data.addApiSecret;
      }
      return null;
    },
    async removeApiSecret({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => removeApiSecret(input),
        dispatch
      );

      if (result.success) {
        const { apiResource } = result.data.removeApiSecret;
        commit("API_RESOURCE_SAVED", apiResource);
        return apiResource;
      }
      return null;
    },
    async publishResources({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => publishResources(input),
        dispatch
      );

      if (result.success) {
        const { resources } = result.data.publishResource;
        commit("SET_PUBLISHING_BEGIN", {
          resources,
          environmentId: input.destinationEnvionmentId
        });
        getPublishedResources();

        return result;
      }

      return null;
    },
    async getPublishedByResource({ commit, dispatch, getters }, id) {
      const input = { resourceId: [id], tenants: getters.tenantFilter };
      const result = await excuteGraphQL(
        () => getPublishedResources(input),
        dispatch
      );
      if (result.success) {
        commit("PUBLISHED_BYRESOURCE_LOADED", result.data.publishedResouces[0]);
      }
    },
    async getPublishedByFilter({ commit, dispatch, getters }, filter) {
      filter.tenants = getters.tenantFilter;
      commit("SET_LOADING", { entity: "publish", loading: true });
      const result = await excuteGraphQL(
        () => getPublishedResources(filter),
        dispatch
      );
      if (result.success) {
        commit(
          "PUBLISHED_RESOURCES_LOADED",
          result.data.publishedResouces
            .sort((a, b) => (a.title < b.title ? -1 : 1))
            .sort((a, b) => (a.type < b.type ? -1 : 1))
        );
      }
    },
    async approveResources({ dispatch }, input) {
      const result = await excuteGraphQL(
        () => approveResources(input),
        dispatch
      );

      if (result.success) {
        return result;
      }

      return null;
    },
    async getResourceApprovals({ commit, dispatch, getters }, filter) {
      filter.tenants = getters.tenantFilter;
      commit("SET_LOADING", { entity: "approval", loading: true });
      const result = await excuteGraphQL(
        () => getResourceApprovals(filter),
        dispatch
      );
      if (result.success) {
        commit(
          "APPROVED_RESOURCES_LOADED",
          result.data.resourceApprovals

            .sort((a, b) => (a.title < b.title ? -1 : 1))
            .sort((a, b) => (a.type < b.type ? -1 : 1))
        );
      }
    },
    async addPersonalAccessTokenSecret({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => addPersonalAccessTokenSecret(input),
        dispatch
      );

      if (result.success) {
        const { token } = result.data.addSecretToPersonalAccessToken;
        commit("PERSONAL_ACCESS_TOKEN_UPDATED", token);
        return result.data.addSecretToPersonalAccessToken;
      }
      return null;
    },
    async removePersonalAccessTokenSecret({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => removePersonalAccessTokenSecret(input),
        dispatch
      );

      if (result.success) {
        const { token } = result.data.removeSecretOfPersonalAccessToken;
        commit("PERSONAL_ACCESS_TOKEN_UPDATED", token);
        return token;
      }
      return null;
    }
  },
  getters: {
    identityScopesByTenant: state => tenant => {
      if (tenant) {
        return state.identityResource.items.filter(x => {
          return x.tenants.includes(tenant);
        });
      } else {
        return [];
      }
    },
    grantTypesByTemplate: (state, getters, rootState) => templateId => {
      if (!templateId) {
        return [];
      }
      const grantTypes = [];
      const template = rootState.application.clientTemplate.items.find(
        x => x.id == templateId
      );
      for (let i = 0; i < state.grantType.items.length; i++) {
        const gt = state.grantType.items[i];
        grantTypes.push(
          Object.assign({}, gt, {
            disabled: template.allowedGrantTypes.includes(gt.id)
          })
        );
      }
      return grantTypes;
    },
    apiScopesByTemplate: (state, getters, rootState) => templateId => {
      if (!templateId) {
        return [];
      }
      const scopes = [];
      const template = rootState.application.clientTemplate.items.find(
        x => x.id == templateId
      );
      const tenantScopes = getters["apiScopesByTenant"](template.tenant);

      for (let i = 0; i < tenantScopes.length; i++) {
        const scope = tenantScopes[i];

        scopes.push(
          Object.assign({}, scope, {
            disabled: template.apiScopes.includes(scope.id)
          })
        );
      }

      return scopes;
    },
    identityScopesByTemplate: (state, getters, rootState) => templateId => {
      if (!templateId) {
        return [];
      }

      const scopes = [];
      const template = rootState.application.clientTemplate.items.find(
        x => x.id == templateId
      );
      const tenantScopes = getters["identityScopesByTenant"](template.tenant);

      for (let i = 0; i < tenantScopes.length; i++) {
        const scope = tenantScopes[i];

        scopes.push(
          Object.assign({}, scope, {
            disabled: template.identityScopes.includes(scope.id)
          })
        );
      }

      return scopes;
    },
    tenantFilter: (state, getters, rootState) => {
      return rootState.system.tenantFilter.map(x => x.id);
    },
    apiScopesByTenant: state => tenant => {
      if (tenant) {
        return state.apiScope.items.filter(x => {
          return x.tenant === tenant;
        });
      } else {
        return [];
      }
    }
  }
};

export default idResourceStore;
