<template>
    <div class="container">
        <b-tabs class="mt-2">
            <b-tab title="Partidos" active>
                <AdminMatchsButtons class="mt-2"/>
                <AdminMatchs class="mt-2"/>
            </b-tab>
            <b-tab title="Usuarios">
                <AdminCreateGroup class="mt-2"/>
                <AdminUserList class="mt-2"/>
            </b-tab>
            <b-tab title="Aplicación">
                <AdminEnvironmentVariables class="mt-2"/>
            </b-tab>
        </b-tabs>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import AdminCreateGroup from '../components/admin/AdminCreateGroup.vue';
import AdminEnvironmentVariables from '../components/admin/AdminEnvironmentVariables.vue';
import AdminMatchs from '../components/admin/AdminMatchs.vue';
import AdminMatchsButtons from '../components/admin/AdminMatchsButtons.vue';
import AdminUserButtons from '../components/admin/AdminUserButtons.vue';
import AdminUserList from '../components/admin/AdminUserLists.vue';

@Component ({
  components: {
    AdminEnvironmentVariables,
    AdminMatchsButtons,
    AdminMatchs,
    AdminUserButtons,
    AdminUserList,
    AdminCreateGroup
  }
})
export default class Admin extends Vue {
    private mounted() {
        if (!this.$store.getters.IsAdmin) {
            this.$router.push('/');
            return;
        }
    }
}
</script>
