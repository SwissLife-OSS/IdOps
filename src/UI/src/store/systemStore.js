import {
  getSystemData,
  saveEnvironment,
  saveIdentityServer,
  saveTenant
} from "../services/systemService";

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

const systemStore = {
  namespaced: true,
  state: () => ({
    tenant: {
      items: []
    },
    environment: {
      items: []
    },
    identityServer: {
      items: []
    },
    identityServerGroup: {
      items: []
    },
    hashAlgorithms: {
      items: []
    },
    tenantFilter: []
  }),
  mutations: {
    TENANTS_LOADED(state, tenants) {
      state.tenant.items = tenants;
      state.tenantFilter = tenants;
    },
    TENANT_SAVED(state, tenant) {
      addOrUpdateResource(state, tenant, "tenant");
    },
    ENVIRONMENTS_LOADED(state, environments) {
      state.environment.items = environments;
    },
    IDENTITYSERVERS_LOADED(state, servers) {
      state.identityServer.items = servers;
    },
    HASH_ALGORITHMS_LOADED(state, hashAlgorithms) {
      state.hashAlgorithms.items = hashAlgorithms;
    },
    IDENTITYSERVER_GROUPS_LOADED(state, groups) {
      state.identityServerGroup.items = groups;
    },
    ENVIRONMENT_SAVED(state, environment) {
      addOrUpdateResource(state, environment, "environment");
    },
    IDENTITYSERVER_SAVED(state, server) {
      addOrUpdateResource(state, server, "identityServer");
    },
    TENANT_FILTER_SET(state, tenants) {
      state.tenantFilter = tenants;
    }
  },
  actions: {
    async loadSystemData({ commit, dispatch }) {
      const result = await excuteGraphQL(() => getSystemData(), dispatch);
      if (result.success) {
        commit("TENANTS_LOADED", result.data.tenants);
        commit("ENVIRONMENTS_LOADED", result.data.environments);
        commit("IDENTITYSERVERS_LOADED", result.data.identityServers);
        commit("HASH_ALGORITHMS_LOADED", result.data.hashAlgorithms);
        commit(
          "IDENTITYSERVER_GROUPS_LOADED",
          result.data.identityServersGroups
        );
      }
    },
    async saveTenant({ commit, dispatch }, input) {
      const result = await excuteGraphQL(() => saveTenant(input), dispatch);

      if (result.success) {
        const { tenant } = result.data.saveTenant;
        commit("TENANT_SAVED", tenant);
        return tenant;
      }

      return null;
    },
    async saveEnvironment({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => saveEnvironment(input),
        dispatch
      );

      if (result.success) {
        const { environment } = result.data.saveEnvironment;
        commit("ENVIRONMENT_SAVED", environment);
        return environment;
      }

      return null;
    },
    async saveIdentityServer({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => saveIdentityServer(input),
        dispatch
      );

      if (result.success) {
        const { server } = result.data.saveIdentityServer;
        commit("IDENTITYSERVER_SAVED", server);
        return server;
      }

      return null;
    },
    setTenantFilter: function({ commit, dispatch }, tenants) {
      commit("TENANT_FILTER_SET", tenants);
      const ids = tenants.map(x => x.id);
      dispatch("idResource/tenantFilterUpdated", ids, { root: true });
      dispatch("application/tenantFilterUpdated", ids, { root: true });
    }
  },
  getters: {
    getModule: state => (tenantId, moduleName) => {
      const tenant = state.tenant.items.find(x => x.id === tenantId);
      if (tenant) {
        return tenant.modules.find(x => x.name === moduleName);
      }
      return null;
    },
    isModuleEnabled: (state, getters) => (tenantId, moduleName) => {
      const module = getters.getModule(tenantId, moduleName);

      return module != null;
    },
    getModuleSetting: (state, getters) => (tenantName, moduleName, name) => {
      if (!tenantName || !moduleName || !name) {
        return null;
      }
      const module = getters.getModule(tenantName, moduleName);

      if (!module) {
        return null;
      }

      const setting = module.settings.find(x => x.name === name);

      if (setting) {
        return setting.value;
      }
      return null;
    }
  }
};

export default systemStore;
