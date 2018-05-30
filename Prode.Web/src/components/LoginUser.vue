<template>
    <div class="container">
        <b-form @submit="onSubmit" class="mt-4">
            <b-form-group
                horizontal
                label-cols="4"
                id="fieldset1"
                label="Nombre de usuario"
                :invalid-feedback="invalidUserName"
                :valid-feedback="validUserName"
                :state="stateUserName">
                <b-form-input v-model.trim="UserName" required/>
            </b-form-group>
            <b-form-group
                horizontal
                label-cols="4"
                id="pass2"
                label="Contraseña">
                <b-form-input v-model.trim="UserPassword" required type="password"/>
            </b-form-group>
            <b-button type="submit" variant="primary" class="mr-3">Entrar</b-button>
            <ErrorAlert :message=this.GeneralError class="mt-2"/>
        </b-form>
    </div>
</template>

<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import ErrorAlert from '../components/ErrorAlert.vue';

@Component({
  components: {
    ErrorAlert
  }
})
export default class LoginUser extends Vue {

    private GeneralError: string = '';
    private UserName: string = '';
    private UserPassword: string = '';

    get invalidUserName(): string {
        if (this.UserName.includes(' ')) {
            return 'No puede contener espacios';
        }
        return '';
    }

    get validUserName(): string {
        return '';
    }

    get stateUserName(): boolean {
        return !this.UserName.includes(' ');
    }

    private onSubmit() {
        this.GeneralError = '';
        Axios.post(process.env.VUE_APP_BASE_URI + 'login', {
            name: this.UserName,
            password: this.UserPassword
        },
        {
            withCredentials: true
        })
        .then(data => {
                this.$store.dispatch('GetUserData');
                this.$router.push('/');
                }
            )
        .catch(error => this.GetError(error.response.data));
    }

    private GetError(data: any) {
        this.GeneralError = 'Usuario o password incorrectos';
    }
}
</script>
