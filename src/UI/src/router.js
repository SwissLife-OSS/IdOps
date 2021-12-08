import AccessDenied from "./components/Common/AccessDenied";
import ApiResourceView from "./components/ResourceAuthor/ApiResourceView";
import ApiScopesView from "./components/ResourceAuthor/ApiScopeView";
import ApplicationCreatedView from "./components/Application/ApplicationCreatedView";
import ApplicationView from "./components/Application/ApplicationView";
import ClientTemplateView from "./components/ResourceAuthor/ClientTemplateView";
import ClientView from "./components/ResourceAuthor/ClientView";
import CreateApplicationView from "./components/Application/CreateApplicationView";
import CreateClientView from "./components/ResourceAuthor/CreateClientView";
import EditApiResourceView from "./components/ResourceAuthor/EditApiResourceView";
import EditApiScopeView from "./components/ResourceAuthor/EditApiScopeView";
import EditApplicationView from "./components/Application/EditApplicationView";
import EditClientTemplateView from "./components/ResourceAuthor/EditClientTemplateView";
import EditClientView from "./components/ResourceAuthor/EditClientView";
import EditEnvironmentView from "./components/System/EditEnvironmentView";
import EditGrantTypeView from "./components/ResourceAuthor/EditGrantTypeView";
import EditIdentityResourceView from "./components/ResourceAuthor/EditIdentityResourceView";
import EditTenantView from "./components/System/EditTenantView";
import EditUserClaimsRulesView from "./components/UserClaimRules/EditUserClaimsRulesView";
import EnvironmentView from "./components/System/EnvironmentView";
import GrantTypeView from "./components/ResourceAuthor/GrantTypeView";
import IdentityResourceView from "./components/ResourceAuthor/IdentityResourceView";
import IdentityServerEventsView from "./components/Insights/IdentityServerEventsView";
import EditIdentityServerGroupView from "./components/System/EditIdentityServerGroupView";
import IdentityServerGroupView from "./components/System/IdentityServerGroupView";
import EditIdentityServerView from "./components/System/EditIdentityServerView";
import IdentityServerView from "./components/System/IdentityServerView";
import InsightsPage from "./components/Insights/InsightsPage";
import PublishAllView from "./components/Publish/PublishAllView";
import PublishPage from "./components/Publish/PublishPage";
import ResourceAuthorPage from "./components/ResourceAuthor/ResourceAuthorPage";
import SessionExpired from "./components/Common/SessionExpired";
import SystemPage from "./components/System/SystemPage";
import TenantView from "./components/System/TenantView";
import UserClaimRulesPage from "./components/UserClaimRules/UserClaimRulesPage";
import UserClaimsRulesView from "./components/UserClaimRules/UserClaimsRulesView";
import PersonalAccessTokenView from "./components/ResourceAuthor/PersonalAccessTokenView";
import CreatePersonalAccessTokenView from "./components/ResourceAuthor/CreatePersonalAccessTokenView";
import EditPersonalAccessTokenView from "./components/ResourceAuthor/EditPersonalAccessTokenView";
import Vue from "vue";
import VueRouter from "vue-router";

// Avoid Redundant route exception
const originalPush = VueRouter.prototype.push;
VueRouter.prototype.push = function push(location) {
  return originalPush.call(this, location).catch(err => err);
};

Vue.use(VueRouter);

const routes = [
  {
    path: "/resources",
    name: "ResourceAuthor",
    component: ResourceAuthorPage,
    children: [
      {
        path: "apiresource",
        name: "ApiResource",
        component: ApiResourceView,
        children: [
          {
            path: "new",
            name: "ApiResource_New",
            component: EditApiResourceView
          },
          {
            path: "edit/:id",
            name: "ApiResource_Edit",
            component: EditApiResourceView,
            props: true
          }
        ]
      },
      {
        path: "scopes",
        name: "ApiScope",
        component: ApiScopesView,
        children: [
          {
            path: "new",
            name: "ApiScope_New",
            component: EditApiScopeView
          },
          {
            path: "edit/:id",
            name: "ApiScope_Edit",
            component: EditApiScopeView,
            props: true
          }
        ]
      },
      {
        path: "client",
        name: "Client",
        alias: "",
        component: ClientView,
        children: [
          {
            path: "new",
            name: "Client_New",
            component: CreateClientView
          },
          {
            path: "edit/:id",
            name: "Client_Edit",
            component: EditClientView,
            props: true
          }
        ]
      },
      {
        path: "personalAccessToken",
        name: "PersonalAccessToken",
        alias: "",
        component: PersonalAccessTokenView,
        children: [
          {
            path: "new",
            name: "PersonalAccessToken_New",
            component: CreatePersonalAccessTokenView
          },
          {
            path: "edit/:id",
            name: "PersonalAccessToken_Edit",
            component: EditPersonalAccessTokenView,
            props: true
          }
        ]
      },
      {
        path: "application",
        name: "Application",
        alias: "",
        component: ApplicationView,
        children: [
          {
            path: "new",
            name: "Application_New",
            component: CreateApplicationView
          },
          {
            path: "created",
            name: "Application_Created",
            component: ApplicationCreatedView
          },
          {
            path: "edit/:id",
            name: "Application_Edit",
            component: EditApplicationView,
            props: true
          }
        ]
      },
      {
        path: "identity",
        name: "IdentityResource",
        component: IdentityResourceView,
        children: [
          {
            path: "new",
            name: "IdentityResource_New",
            component: EditIdentityResourceView
          },
          {
            path: "edit/:id",
            name: "IdentityResource_Edit",
            component: EditIdentityResourceView,
            props: true
          }
        ]
      },
      {
        path: "granttype",
        name: "GrantType",
        component: GrantTypeView,
        children: [
          {
            path: "new",
            name: "GrantType_New",
            component: EditGrantTypeView
          },
          {
            path: "edit/:id",
            name: "GrantType_Edit",
            component: EditGrantTypeView,
            props: true
          }
        ]
      },
      {
        path: "clientTemplate",
        name: "ClientTemplate",
        component: ClientTemplateView,
        children: [
          {
            path: "new",
            name: "ClientTemplate_New",
            component: EditClientTemplateView
          },
          {
            path: "edit/:id",
            name: "ClientTemplate_Edit",
            component: EditClientTemplateView,
            props: true
          }
        ]
      }
    ]
  },
  {
    path: "/claimrules",
    name: "UserClaimRules",
    component: UserClaimRulesPage,
    children: [
      {
        path: "rules",
        name: "UserClaimRules_Rules",
        component: UserClaimsRulesView,
        children: [
          {
            path: "new",
            name: "UserClaimRules_Rule_New",
            component: EditUserClaimsRulesView
          },
          {
            path: "edit/:id",
            name: "UserClaimRules_Rule_Edit",
            component: EditUserClaimsRulesView,
            props: true
          }
        ]
      }
    ]
  },
  {
    path: "/system",
    name: "System",
    component: SystemPage,
    children: [
      {
        path: "tenant",
        alias: "",
        name: "Tenant",
        component: TenantView,
        children: [
          {
            path: "new",
            name: "Tenant_New",
            component: EditTenantView
          },
          {
            path: "edit/:id",
            name: "Tenant_Edit",
            component: EditTenantView,
            props: true
          }
        ]
      },
      {
        path: "environment",
        name: "Environment",
        component: EnvironmentView,
        children: [
          {
            path: "new",
            name: "Environment_New",
            component: EditEnvironmentView
          },
          {
            path: "edit/:id",
            name: "Environment_Edit",
            component: EditEnvironmentView,
            props: true
          }
        ]
      },
      {
        path: "identityservergroup",
        name: "IdentityServerGroup",
        component: IdentityServerGroupView,
        children: [
          {
            path: "new",
            name: "IdentityServerGroup_New",
            component: EditIdentityServerGroupView
          },
          {
            path: "edit/:id",
            name: "IdentityServerGroup_Edit",
            component: EditIdentityServerGroupView,
            props: true
          }
        ]
      },
      {
        path: "identityserver",
        name: "IdentityServer",
        component: IdentityServerView,
        children: [
          {
            path: "new",
            name: "IdentityServer_New",
            component: EditIdentityServerView
          },
          {
            path: "edit/:id",
            name: "IdentityServer_Edit",
            component: EditIdentityServerView,
            props: true
          }
        ]
      }
    ]
  },
  {
    path: "/insights",
    name: "Insights",
    component: InsightsPage,
    children: [
      {
        path: "idevents/:input?",
        name: "IdentityServerEvents",
        component: IdentityServerEventsView,
        props: true
      }
    ]
  },
  {
    path: "/publish",
    name: "Publish",
    component: PublishPage,
    children: [
      {
        path: "/resources",
        name: "Publish",
        component: PublishAllView
      }
    ]
  },
  {
    path: "/session/expired",
    name: "SessionExpired",
    components: { root: SessionExpired },
    meta: { isRoot: true }
  },
  {
    path: "/session/denied",
    name: "AccessDenied",
    components: { root: AccessDenied },
    meta: { isRoot: true }
  }
];

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes
});

export default router;
