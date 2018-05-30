<template>
    <div class="container">
        <h3>Para ver los cambios en esta pantalla debe salir y volver a entrar</h3>
        <ChangePassword class="mt-2"/>
        <ChangeTeamName class="mt-2"/>
        <ChangeMailsReceived class="mt-2"/>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import ChangeMailsReceived from '../components/profile/ChangeMailsReceived.vue';
import ChangePassword from '../components/profile/ChangePassword.vue';
import ChangeTeamName from '../components/profile/ChangeTeamName.vue';

@Component ({
  components: {
    ChangeMailsReceived,
    ChangePassword,
    ChangeTeamName
  }
})
export default class UserData extends Vue {

}
</script>
