import { searchIdentityServerEvents } from "../services/insightsService";
import { excuteGraphQL } from "./graphqlClient";

const insightsStore = {
    namespaced: true,
    state: () => ({
        idEvents: {
            loading: false,
            totalCount: 0,
            filter: {
                pageNr: 0,
                pageSize: 100,
                clientId: null,
                environment: null
            },
            items: []
        }
    }),
    mutations: {
        ID_EVENTS_LOADED(state, events) {
            state.idEvents.items = events.items;
            state.idEvents.totalCount = events.totalCount
            state.idEvents.loading = false;
        },
        ID_EVENTS_LOADING_SET(state, value) {
            state.idEvents.loading = value;
        },
        ID_EVENTS_PAGING_SET(state, paging) {
            state.idEvents.filter.pageNr = paging.pageNr;
            state.idEvents.filter.pageSize = paging.pageSize;
        },
        ID_EVENTS_FILTER_SET(state, filter) {
            state.idEvents.filter = Object.assign(state.idEvents.filter, filter);
        }
    },
    actions: {
        async searchIdentityServerEvents({ state, commit, dispatch }, filter) {
            commit("ID_EVENTS_LOADING_SET", true);
            commit("ID_EVENTS_FILTER_SET", filter)
            const result = await excuteGraphQL(() => searchIdentityServerEvents(state.idEvents.filter), dispatch);
            if (result.success) {
                commit("ID_EVENTS_LOADED", result.data.searchIdentityServerEvents);
            }
            else {
                commit("ID_EVENTS_LOADING_SET", false);
            }
        },
        setIdentityServerEventsPaging: function ({ commit, dispatch }, paging) {
            commit("ID_EVENTS_PAGING_SET", paging);
            dispatch('searchIdentityServerEvents');
        }
    },
    getters: {

    }
};

export default insightsStore;
