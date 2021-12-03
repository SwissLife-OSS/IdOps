<template>
  <me-loader>
    <template slot="error">
      <v-app>
        <router-view name="root"></router-view>
      </v-app>
    </template>
    <v-app>
      <v-system-bar window dark color="indigo darken-2" height="42" app>
        <v-icon size="26" light color="white"
          >mdi-shield-account-outline</v-icon
        >
        <span class="white--text">IdOps | {{ $route.name }}</span>
        <v-spacer></v-spacer>

        <div class="mr-4 status-message" v-if="statusMessage">
          <v-icon
            :color="statusMessage.color"
            v-text="statusMessage.icon"
          ></v-icon>
          <span class="white--text">{{ statusMessage.text }}</span>
        </div>
        <tenant-selector-menu></tenant-selector-menu>

        <v-icon @click="dialog = !dialog">mdi-console</v-icon>

        <v-icon>mdi-account</v-icon>
        <span v-if="me">{{ me.name }}</span>
        <signal-shell></signal-shell>
      </v-system-bar>
      <v-navigation-drawer width="62" class="nav" app>
        <div
          class="nav-item"
          v-for="(nav, i) in navBarItems"
          :key="i"
          @click="onNavigate(nav)"
        >
          <v-icon class="icon" large light :color="nav.color">{{
            nav.icon
          }}</v-icon>
        </div>
      </v-navigation-drawer>
      <v-snackbar
        top
        centered
        :value="pwa.updateExists"
        :timeout="-1"
        color="green darken-1"
      >
        <v-icon class="mr-4"> mdi-gift-outline</v-icon>
        <span>An new version of IdOps is availlable...</span>

        <v-btn
          color="green lighten-2"
          dark
          elevation="2"
          class="ml-4"
          @click="refreshApp"
        >
          Update now</v-btn
        >
      </v-snackbar>
      <v-main>
        <router-view></router-view>
      </v-main>
      <v-dialog v-model="dialog" width="750">
        <v-card height="500" color="grey darken-4">
          <v-toolbar height="32" dark color="black">
            <v-toolbar-title class="white--text">Console</v-toolbar-title>
          </v-toolbar>
          <v-card-text class="console">
            <v-container fluid class="ma-0 pa-0">
              <v-row dense v-for="(msg, i) in console" :key="i">
                <v-col md="12">
                  <v-icon v-if="msg.severity == 'SUCCESS'" small color="green"
                    >mdi-check-circle</v-icon
                  >
                  {{ msg.message }}</v-col
                >
              </v-row>
            </v-container>
          </v-card-text>
        </v-card>
      </v-dialog>
    </v-app>
  </me-loader>
</template>

<script>
import { mapActions, mapState } from "vuex";
import SignalShell from "./Common/SignalShell.vue";
import TenantSelectorMenu from "./Common/TenantSelectorMenu.vue";
import MeLoader from "./User/MeLoader.vue";
export default {
  name: "App",
  components: { SignalShell, MeLoader, TenantSelectorMenu },
  created() {
    this.loadSystemData();
    this.$router.push({ name: this.navItems[0].route });

    document.addEventListener("swUpdated", this.updateAvailable, {
      once: true
    });
    navigator.serviceWorker.addEventListener("controllerchange", () => {
      if (this.pwa.refreshing) return;
      this.pwa.refreshing = true;
      window.location.reload();
    });

    navigator.serviceWorker.addEventListener("message", event => {
      switch (event.data.action) {
        case "ROUTE":
          if (
            this.$route.name != event.data.value &&
            this.$route.name != "AccessDenied"
          ) {
            this.$router.push({ name: event.data.value });
          }
          break;
        default:
          console.warn("Unknown action", event.data);
          break;
      }
    });
  },
  data: () => ({
    pwa: {
      updateExists: false,
      refreshing: false,
      registration: null
    },
    dialog: false,
    menu: null,
    navItems: [
      {
        text: "Identity Resources",
        icon: "mdi-database-outline",
        route: "ResourceAuthor"
      },
      {
        text: "Claim Rules",
        icon: "mdi-account-check-outline",
        route: "UserClaimRules"
      },
      {
        text: "System",
        icon: "mdi-view-grid-outline",
        route: "System",
        permissions: ["ENV_MANAGE", "TENANT_MANAGE", "IDSRV_MANAGE"]
      },
      {
        name: "Publish",
        route: "Publish",
        icon: "mdi-rocket-launch-outline",
        permission: "CA_PUBLISH"
      },
      {
        text: "Insighs",
        icon: "mdi-eye-outline",
        route: "Insights",
        permissions: ["INSIGHTS_ALL"]
      }
    ]
  }),
  computed: {
    ...mapState("user", ["me"]),
    navBarItems: function() {
      return this.navItems
        .filter(x => {
          if (!x.permissions) {
            return true;
          } else {
            for (let i = 0; i < x.permissions.length; i++) {
              const perm = x.permissions[i];
              if (this.$store.getters["user/hasPermission"](perm)) {
                return true;
              }
            }
          }
          return false;
        })
        .map(x => {
          x.active = x.route === this.$route.name;
          x.color = x.active ? "#fff" : "#b3b3b3";

          return x;
        });
    },

    console: function() {
      return this.$store.state.shell.console;
    },
    statusMessage: function() {
      if (this.$store.state.shell.statusMessage) {
        const isError = this.$store.state.shell.statusMessage.type === "ERROR";

        return Object.assign(this.$store.state.shell.statusMessage, {
          color: isError ? "red" : "green",
          icon: isError ? "mdi-nuke" : "mdi-check-circle"
        });
      } else {
        return null;
      }
    }
  },
  methods: {
    ...mapActions("system", ["loadSystemData"]),
    onNavigate: function(nav) {
      if (!nav.active) this.$router.push({ name: nav.route });
    },
    updateAvailable(event) {
      this.pwa.registration = event.detail;
      this.pwa.updateExists = true;
    },
    refreshApp: function() {
      this.pwa.updateExists = false;
      if (!this.pwa.registration || !this.pwa.registration.waiting) {
        return;
      }
      this.pwa.registration.waiting.postMessage({ type: "SKIP_WAITING" });
    }
  }
};
</script>

<style scoped>
.nav {
  background-color: rgba(0, 0, 0, 0.753) !important;
}

.nav-item {
  height: 60px;
  width: 60px;
  transition: 0.4s;
}

.icon {
  margin-left: 12px;
  margin-top: 12px;
}

.nav-item:hover {
  background-color: rgba(0, 0, 0, 0.79);
  border-radius: 40px;
}

.status-message {
  overflow: hidden;
}

.console {
  font-family: Lucida Console, Lucida Sans Typewriter, monaco,
    Bitstream Vera Sans Mono, monospace;

  font-size: 12px;
  height: 95%;
  color: rgba(233, 236, 236, 0.788) !important;
}
</style>

<style>
.chip-tenant {
  height: 26px;
  width: 4px;
  right: 6px;
  position: absolute;
  border-radius: 10px;
}

.tenant-box {
  width: 25px;
  height: 25px;
  position: absolute;
  border-radius: 6px;
  right: 20px;
  top: 16px;
}

::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

::-webkit-scrollbar-track:hover {
  background: #b9dff8;
}
::-webkit-scrollbar-track:active {
  background: #dcdfe0;
}

::-webkit-scrollbar-track {
  background: #e3eaf0;
  border: 0px none #ffffff;
  border-radius: 50px;
}

::-webkit-scrollbar-thumb {
  background: #68bef881;
  border: 0px none #ffffff;
  border-radius: 50px;
}
::-webkit-scrollbar-thumb:hover {
  background: #49a3df;
}
::-webkit-scrollbar-thumb:active {
  background: #3481b4;
}
</style>
