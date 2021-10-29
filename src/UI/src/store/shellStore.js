
const shellStore = {
    namespaced: true,
    state: () => ({
        statusMessage: null,
        console: [],
    }),
    mutations: {

        MESSAGE_ADDED(state, message) {
            state.statusMessage = message;

            window.setTimeout(() => {
                state.statusMessage = null;
            }, 5000)
        },
        CONSOLE_MESSAGE(state, message) {
            state.console.push(message);
        }
    },
    actions: {

        addMessage: function ({ commit }, message) {
            commit("MESSAGE_ADDED", message)
        },
    },
    getters: {

    }
};

export default shellStore;
