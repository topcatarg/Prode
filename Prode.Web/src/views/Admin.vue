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
        </b-tabs>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import AdminCreateGroup from '../components/AdminCreateGroup.vue';
import AdminMatchs from '../components/AdminMatchs.vue';
import AdminMatchsButtons from '../components/AdminMatchsButtons.vue';
import AdminUserButtons from '../components/AdminUserButtons.vue';
import AdminUserList from '../components/AdminUserLists.vue';

@Component ({
  components: {
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
