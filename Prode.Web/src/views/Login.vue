<template>
    <div class="container">
        <b-tabs class="mt-2">
            <b-tab title="Ingresar" active>
                <LoginUser/>
            </b-tab>
            <b-tab title="Crear Usuario">
                <CreateUser/>
            </b-tab>
            <b-tab title="Recuperar Password">
               <RecoverMail />
            </b-tab>
        </b-tabs>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import CreateUser from '../components/CreateUser.vue';
import LoginUser from '../components/LoginUser.vue';
import RecoverMail from '../components/RecoverMail.vue';

@Component({
  components: {
    CreateUser,
    LoginUser,
    RecoverMail
  }
})
export default class Login extends Vue {
}
</script>