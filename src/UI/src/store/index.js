import Vue from 'vue'
import Vuex from 'vuex'
import applicationStore from './applicationStore'
import idResourceStore from './idResourceStore'
import insightsStore from './insightsStore'
import shellStore from './shellStore'
import systemStore from './systemStore'
import userClaimsRuleStore from './userClaimRuleStore'
import userStore from './userStore'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
  },
  mutations: {
  },
  actions: {
  },
  modules: {
    idResource: idResourceStore,
    shell: shellStore,
    system: systemStore,
    insights: insightsStore,
    user: userStore,
    application: applicationStore,
    userClaimsRule: userClaimsRuleStore
  }
})
