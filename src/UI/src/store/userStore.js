import { getMe } from "../services/userService";
import { excuteGraphQL } from "./graphqlClient";

const insightsStore = {
  namespaced: true,
  state: () => ({
    me: null
  }),
  mutations: {
    ME_LOADED(state, user) {
      state.me = user;
    }
  },
  actions: {
    async getMe({ commit, dispatch }) {
      const result = await excuteGraphQL(() => getMe(), dispatch);
      if (result.success) {
        commit("ME_LOADED", result.data.me);
      }
    }
  },
  getters: {
    hasPermission: state => perm => {
      return state.me && state.me.permissions.includes(perm);
    }
  }
};

export default insightsStore;
