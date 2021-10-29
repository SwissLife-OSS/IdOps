<template>
  <div>
    <v-treeview
      v-if="!loading"
      class="overflow-y-auto mt-0"
      :return-object="true"
      :items="nodes"
      ref="tree"
      item-key="id"
      :active.sync="active"
      :open-on-click="true"
      :multiple-active="false"
      dense
      @update:active="onSelect"
      activatable
      transition
    >
      <template v-slot:prepend="{ open, item }">
        <v-icon
          size="20"
          color="yellow darken-2"
          v-if="item.type === 'DIRECTORY'"
        >
          {{ open ? "mdi-folder-open" : "mdi-folder" }}
        </v-icon>
        <v-icon size="22" v-else>mdi-file</v-icon>
      </template>
    </v-treeview>
    <v-progress-linear indeterminate v-else></v-progress-linear>
  </div>
</template>

<script>
import { mapActions } from "vuex";
import { getAllDependencies } from "../../services/idResourceService";

export default {
  props: ["resourceId", "type"],
  watch: {
    resourceId: {
      immediate: true,
      handler: function () {
        this.getDependencies();
      },
    },
  },
  data() {
    return {
      active: null,
      loading: false,
      input: {
        id: null,
        type: null,
      },
      nodes: [],
    };
  },

  methods: {
    ...mapActions("idResource", ["getAllDependencies"]),
    async getDependencies() {
      if (this.resourceId) {
        this.loading = true;

        this.input.id = this.resourceId;
        this.input.type = this.type;

        const dependencies = (await getAllDependencies(this.input)).data
          .dependencies;

        const nodes = [];
        const mapping = {
          apiResources: {
            title: "API Resources",
            route: "ApiResource_Edit",
          },
          apiScopes: {
            title: "API Scopes",
            route: "ApiScope_Edit",
          },
          identityResources: {
            title: "Identity Resources",
            route: "IdentityResource_Edit",
          },
        };

        for (const key in mapping) {
          if (Object.hasOwnProperty.call(mapping, key)) {
            const map = mapping[key];
            const deps = dependencies[key];
            if (deps.length > 0) {
              const node = {
                id: map.title,
                name: map.title,
                type: "DIRECTORY",
                children: [],
              };

              if (deps) {
                node.children = deps.map((x) => {
                  return {
                    name: x.title,
                    id: x.id,
                    route: map.route,
                  };
                });

                nodes.push(node);
              }
            }
          }
        }

        this.nodes = nodes;
        this.loading = false;

        this.$nextTick(() => {
          this.$refs.tree.updateAll(true);
        });
      }
    },
    onSelect: function (nodes) {
      const node = nodes[0];
      this.$router.replace({
        name: node.route,
        params: { id: node.id },
      });
    },
  },
};
</script>

<style>
</style>