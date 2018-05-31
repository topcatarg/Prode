<template>
    <div class="container">
        <b-form @submit="onSubmit" @reset="onReset" class="mt-4">
            <b-form-group
                horizontal
                label-cols="4"
                id="fieldset1"
                label="Ingresa el nombre del equipo">
                <b-form-input v-model.trim="UserTeamName" required/>
            </b-form-group>
            <b-form-group
                horizontal
                label-cols="4"
                id="fieldset1"
                label="Ingresa el nombre de usuario para loguearte"
                :invalid-feedback="invalidUserName"
                :valid-feedback="validUserName"
                :state="stateUserName">
                <b-form-input v-model.trim="UserName" required/>
            </b-form-group>
            <b-form-group
                horizontal
                label-cols="4"
                id="pass1"
                label="Ingresa el correo electronico">
                <b-form-input v-model.trim="UserMail" required type="email"/>
            </b-form-group>
            <b-form-group
                horizontal
                label-cols="4"
                id="pass2"
                label="Ingresa la contraseña">
                <b-form-input v-model.trim="UserPassword" required type="password"/>
            </b-form-group>
            <b-form-group
                horizontal
                label-cols="4"
                id="fieldset1"
                label="Repetí la contraseña"
                :invalid-feedback="UnEvenPassword"
                :state="statePassword">
                <b-form-input v-model.trim="UserPassword2" required type="password"/>
            </b-form-group>
            <b-form-group
                horizontal
                label-cols="4"
                id="fieldset1"
                label="Ingresa el nombre del grupo en el cual participas">
                <b-form-input v-model.trim="UserGroup" required />
            </b-form-group>
            <b-button type="submit" variant="primary" class="mr-3">Crear</b-button>
            <b-button type="reset" variant="danger">Limpiar</b-button>
            <ErrorAlert :message=this.GeneralError class="mt-2"/>
        </b-form>
    </div>
</template>

<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import ErrorAlert from '../components/ErrorAlert.vue';
import { CreateUserResult } from '../enums/CreateUserResult';

@Component({
  components: {
    ErrorAlert
  }
})
export default class CreateUser extends Vue {

    private UserTeamName: string = '';
    private UserName: string = '';
    private UserMail: string = '';
    private UserPassword: string = '';
    private UserPassword2: string = '';
    private UserGroup: string = '';
    private GeneralError: string = '';
    private UnEvenPassword: string = 'Las contraseñas no son iguales';
    private UserGroupId: number = 0;

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

    get statePassword(): boolean {
        return this.UserPassword === this.UserPassword2;
    }

    private onSubmit() {
        this.GeneralError = '';
        // test group
        Axios.get(process.env.VUE_APP_BASE_URI + 'ExistGroup?group=' + this.UserGroup)
        .then(response => {
            this.UserGroupId = response.data;
            this.CreateUser(); })
        .catch(error => this.GeneralError = 'El grupo no es valido');
    }

    private onReset() {
        this.UserTeamName = '';
        this.UserName = '';
        this.UserMail = '';
        this.UserPassword = '';
        this.UserPassword2 = '';
        this.UserGroup = '';
        this.UserGroupId = 0;
        this.GeneralError = '';
    }

    private CreateUser() {
        Axios.post(process.env.VUE_APP_BASE_URI + 'create',
        {
            name: this.UserName,
            password: this.UserPassword,
            TeamName: this.UserTeamName,
            mail: this.UserMail,
            GameGroupId: this.UserGroupId,
            GameGroupName: this.UserGroup
        })
        .then(data => this.loginUser())
        .catch(error => this.GetError(error.response.data));
    }

    private loginUser() {
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
            );
    }

    private GetError(dataError: any) {
        switch (dataError) {
            case CreateUserResult.BadParameters: {
                this.GeneralError = 'parametros incompletos';
                break;
            }
            case CreateUserResult.ErrorOnDatabase: {
                this.GeneralError = 'Error de db';
                break;
            }
            case CreateUserResult.UserAlreadyExist: {
                this.GeneralError = 'ya existe usuario';
                break;
            }
            case CreateUserResult.MailAlreadyExist: {
                this.GeneralError = 'ya existe mail';
                break;
            }
            default: {
                this.GeneralError = 'error desconocido';
            }
        }
    }
}
</script>

