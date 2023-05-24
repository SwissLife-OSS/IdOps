import {
  addClientToApplication,
  addEnvironmentToApplication,
  createApplication,
  getClientTemplates,
  removeClientFromApplication,
  saveClientTemplate,
  searchApplications,
  updateApplication
} from "../services/applicationService";

import Vue from "vue";
import { excuteGraphQL } from "./graphqlClient";

const applicationStore = {
  namespaced: true,
  state: () => ({
    list: {
      loading: false,
      hasMore: true,
      items: [],
      filter: {
        pageNr: 0,
        pageSize: 1000,
        searchText: null
      }
    },
    lastCreated: null,
    clientTemplate: {
      loading: false,
      items: []
    },
    apiScope: {
      loading: false,
      items: []
    },
  }),
  mutations: {
    APPLICATIONS_LOADED(state, result) {
      state.list.items = result.items;
      state.list.hasMore = result.hasMore;
      state.list.loading = false;
    },
    APPLICATION_CREATED(state, result) {
      state.list.items.push(result.application);
      state.lastCreated = result;
    },
    APPLICATION_ENVIRONMENT_CREATED(state, result) {
      state.lastCreated = result;
    },
    APPLICATION_UPDATED(state, application) {
      const index = state.list.items.findIndex(x => x.id === application.id);
      if (index > -1) {
        Vue.set(state.list.items, index, application);
      }
    },
    LOADING_SET(state, value) {
      state.list.loading = value;
    },
    CLIENT_TEMPLATES_LOADED(state, templates) {
      state.clientTemplate.items = templates;
    },
    CLIENT_TEMPLATE_SAVED(state, clientTemplate) {
      const items = state.clientTemplate.items;
      const index = items.findIndex(x => x.id === clientTemplate.id);
      if (index > -1) {
        Vue.set(items, index, clientTemplate);
      } else {
        items.push(clientTemplate);
      }
      state.lastCreated = clientTemplate;
    }
  },
  actions: {
    async search({ state, commit, getters, dispatch }) {
      commit("LOADING_SET", true);
      const result = await excuteGraphQL(
        () => searchApplications(Object.assign(state.list.filter, { tenants: getters.tenantFilter })),
        dispatch
      );

      if (result.success) {
        commit("APPLICATIONS_LOADED", result.data.searchApplications);
      } else {
        commit("LOADING_SET", false);
      }
    },
    async createApplication({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => createApplication(input),
        dispatch
      );

      if (result.success) {
        commit("APPLICATION_CREATED", result.data.createApplication);

        return result.data.createApplication;
      }
      return null;
    },
    async updateApplication({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => updateApplication(input),
        dispatch
      );

      if (result.success) {
        const { application } = result.data.updateApplication;
        commit("APPLICATION_UPDATED", application);
        return application;
      }
      return null;
    },
    async removeClientFromApplication({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => removeClientFromApplication(input),
        dispatch
      );

      if (result.success) {
        const { application } = result.data.removeClientFromApplication;
        commit("APPLICATION_UPDATED", application);
        return application;
      }
      return null;
    },
    async addClientToApplication({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => addClientToApplication(input),
        dispatch
      );

      if (result.success) {
        const { application } = result.data.addClientToApplication;
        commit("APPLICATION_UPDATED", application);
        return application;
      }
      return null;
    },

    async getClientTemplates({ commit, dispatch }) {
      const result = await excuteGraphQL(() => getClientTemplates(), dispatch);

      if (result.success) {
        commit("CLIENT_TEMPLATES_LOADED", result.data.clientTemplates);
      }
    },
    async addEnvironmentToApplication({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => addEnvironmentToApplication(input),
        dispatch
      );

      if (result.success) {
        const application = result.data.addEnvironmentToApplication;
        commit("APPLICATION_UPDATED", application);
        commit("APPLICATION_ENVIRONMENT_CREATED", application);

        return application;
      }
      return null;
    },
    tenantFilterUpdated: function ({ dispatch }) {
      dispatch("search");
    },

    async saveClientTemplate({ commit, dispatch }, input) {
      const result = await excuteGraphQL(
        () => saveClientTemplate(input),
        dispatch
      );

      if (result.success) {
        const { clientTemplate } = result.data.saveClientTemplate;
        commit("CLIENT_TEMPLATE_SAVED", clientTemplate);
        return clientTemplate;
      }

      return null;
    },
  },

  getters: {
    tenantFilter: (state, getters, rootState) => {
      return rootState.system.tenantFilter.map(x => x.id)
    },
    clientTemplatesByTenant: (state) => (tenant) => {
      if (tenant) {
        return state.clientTemplate.items.filter(
          (x) => {
            return x.tenant === tenant
          }
        );
      } else {
        return [];
      }
    },
    apiScopesByTenantAndTemplate: (clientTemplate, apiScopes) => {
      if (clientTemplate) {
        return apiScopes.filter(
          (x) => {
            return x.clientTemplate === clientTemplate
          }
        );
      } else {
        return [];
      }
    }
  }
};

export default applicationStore;
