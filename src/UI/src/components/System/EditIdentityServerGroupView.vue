<template>
  <resource-edit-card
    :title="title"
    :loading="loading"
    :resource="group"
    :tools="[]"
    @Save="onSave"
  >
    <v-form ref="form" v-model="valid" lazy-validation>
      <v-tabs vertical v-model="tab">
        <v-tab>
          <v-icon left> mdi-file-document-edit-outline </v-icon>
        </v-tab>
        <v-tab>
          <v-icon left> mdi-bomb</v-icon>
        </v-tab>
        <v-tab-item key="core">
          <v-row>
            <v-col md="4">
              <v-text-field
                required
                label="Name"
                :disabled="!isNew"
                :rules="[
                  (v) => !!v || 'Required',
                  (v) => (v && v.length > 2) || 'Must be at least 3 characters',
                ]"
                v-model="group.name">
              </v-text-field>
            </v-col>
          <v-col md="3">
            <v-autocomplete
              label="Tenants"
              :rules="[(v) => !!v || 'Required']"
              v-model="group.tenants"
              :items="tenants"
              item-text="id"
              item-value="id"
              chips
              multiple
              small-chips
              deletable-chips>
            </v-autocomplete>
          </v-col>
          </v-row>
          <v-row>
            <v-col md="4">
              <v-text-field
                label="Color"
                v-model="group.color"
                hide-details
                class="ma-0 pa-0"
              >
                <template v-slot:append>
                  <v-menu
                    v-model="menu"
                    top
                    nudge-bottom="105"
                    nudge-left="16"
                    :close-on-content-click="false"
                  >
                    <template v-slot:activator="{ on }">
                      <div :style="swatchStyle" v-on="on" />
                    </template>
                    <v-card>
                      <v-card-text class="pa-0">
                        <v-color-picker
                          v-model="group.color"
                          show-swatches
                          flat
                        />
                      </v-card-text>
                    </v-card>
                  </v-menu>
                </template>
              </v-text-field>
            </v-col>
          </v-row>
        </v-tab-item>
      </v-tabs>
    </v-form>
  </resource-edit-card>
</template>

<script>
import { mapActions } from "vuex";
import ResourceEditCard from "../ResourceAuthor/ResourceEditCard.vue";
export default {
  components: { ResourceEditCard },
  props: ["id"],
  watch: {
    id: {
      immediate: true,
      handler: function () {
        this.setGroup();
      },
    },
  },
  data() {
    return {
      tab: null,
      valid: null,
      loading: false,
      mask: "!#XXXXXXXX",
      menu: false,
      group: {
        id: null,
        name: null,
        tenants: [],
        color: null,
      }
    };
  },
  computed: {
    title: function () {
      return this.isNew ? "Group" : this.group.name;
    },
    isNew: function () {
      return !this.group.id;
    },
    tenants: function () {
      return this.$store.state.system.tenant.items;
    },
    swatchStyle() {
      return {
        backgroundColor: this.group.color,
        cursor: "pointer",
        height: "30px",
        width: "30px",
        "margin-bottom": "5px",
        borderRadius: this.menu ? "50%" : "4px",
        transition: "border-radius 200ms ease-in-out",
      };
    },
  },
  methods: {
    ...mapActions("system", ["saveIdentityServerGroup"]),
    setGroup: function () {
      if (this.id) {
        const group = this.$store.state.system.identityServerGroup.items.find(
          (x) => x.id === this.id
        );
        this.group = Object.assign({}, group);
        delete this.group.__typename;
      } else {
        this.resetForm();
      }
    },
    onColorChanged: function () {
      this.group.color = this.colorPicker.hex;
    },
    resetForm: function () {
      this.group = {
        name: null,
        tenants: [],
      };
      //this.$refs.form.resetValidation();
    },
    async onSave(event) {
      this.$refs.form.validate();

      const group = await this.saveIdentityServerGroup(this.group);

      event.done();
      this.setGroup(group);

      if (group && this.$route.name === "IdentityServerGroup_New") {
        this.$router.replace({
          name: "IdentityServerGroup_Edit",
          params: { id: group.id },
        });
      }
    },
  },
};
</script>

<style>
</style>
