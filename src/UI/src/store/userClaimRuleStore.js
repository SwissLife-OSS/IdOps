import { getUserClaimRules, saveUserClaimRule } from "../services/userClaimsRuleService";
import { excuteGraphQL } from "./graphqlClient";
import Vue from "vue";

const userClaimsRuleStore = {
    namespaced: true,
    state: () => ({
        rules: {
            loading: false,
            items: []
        },
        enums: {
            matchModes: [],
        }
    }),
    mutations: {
        RULES_LOADING_SET(state, value) {
            state.rules.loading = value;
        },
        RULES_LOADED(state, rules) {
            state.rules.loading = false;
            state.rules.items = rules;
        },
        RULE_SAVED(state, rule) {
            const index = state.rules.items.findIndex(x => x.id === rule.id);
            if (index > -1) {
                Vue.set(state.rules.items, index, rule);
            }
            else {
                state.rules.items.push(rule)
            }
        },
        ENUMS_LOADED(state, enums) {
            state.enums.matchModes = enums.claimRuleMatchModes.values;
        }
    },
    actions: {
        async getRules({ commit, dispatch, getters }) {
            commit("RULES_LOADING_SET", true);
            const input = { tenants: getters.tenantFilter }
            const result = await excuteGraphQL(() => getUserClaimRules({ input }), dispatch);
            if (result.success) {
                commit("RULES_LOADED", result.data.userClaimsRules);
                commit("ENUMS_LOADED", result.data);
            }
            else {
                commit("RULES_LOADING_SET", false);
            }
        },
        async saveRule({ commit, dispatch }, input) {
            const result = await excuteGraphQL(() => saveUserClaimRule({ input }), dispatch);

            if (result.success) {
                const { rule } = result.data.saveUserClaimRule;
                commit("RULE_SAVED", rule);

                return rule;
            }
            else {
                return null;
            }
        }
    },
    getters: {
        tenantFilter: (state, getters, rootState) => {
            return rootState.system.tenantFilter.map(x => x.id)
        },
    }
};

export default userClaimsRuleStore;
