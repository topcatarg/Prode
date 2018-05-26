<template>
    <div class="container">
        <b-tabs>
            <b-tab title="Ingresar" active>
                Usuario existente:
                <b-form-input v-model="user" required/>
                Password:
                <b-form-input placeholder="Enter password" type="password" v-model="password" required/>
                <b-button @click="Login">
                    Ingresar 
                </b-button>
            </b-tab>
            <b-tab title="Crear Usuario">
                Nombre: 
                <b-form-input v-model="newuser" required/>
                Direccion de correo:
                <b-form-input type="email" v-model="mail" required/>
                Password:
                <b-form-input placeholder="Enter password" type="password" v-model="newpassword" required/>
                <b-button @click="Create"> 
                    Crear Usuario
                </b-button>
                <ErrorAlert />
            </b-tab>
            <b-tab title="Recuperar Password">
                Direccion de correo:
                <b-form-input type="email" v-model="remail" required/>
                <b-button @click="RecoverPassword"> 
                    Recuperar Password
                </b-button>
            </b-tab>
        </b-tabs>
        <ErrorAlert :message=this.errorAlCrear />
        {{this.errorAlCrear}}
    </div>
</template>

<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import ErrorAlert from '../components/ErrorAlert.vue';
import { CreateUserResult } from '../enums/CreateUserResult';
import store from '../store';

@Component({
  components: {
    ErrorAlert
  }
})
export default class Login extends Vue {
    private user: string = '';
    private newuser: string = '';
    private password: string = '';
    private newpassword: string = '';
    private logueado: string = 'nose';
    private mail: string = '';
    private remail: string = '';
    private errorAlCrear: string = '';
    private Login() {
        // console.log(this.user);
        Axios.post(process.env.VUE_APP_BASE_URI + 'login', {
            name: this.user,
            password: this.password
        },
        {
            withCredentials: true
        })
        .then(data => {
                this.logueado = 'si';
                store.dispatch('GetUserData');
                }
            )
        .catch(data => this.logueado = 'no');
        // console.log('pase');
    }
    private Create() {
        this.errorAlCrear = '';
        Axios.post(process.env.VUE_APP_BASE_URI + 'create',
        {
            name: this.newuser,
            password: this.newpassword,
            mail: this.mail
        })
        .then(data => this.logueado = 'Creado')
        .catch(error => {
            switch (error.response.data) {
                case CreateUserResult.BadParameters: {
                    this.errorAlCrear = 'parametros incompletos';
                    break;
                }
                case CreateUserResult.ErrorOnDatabase: {
                    this.errorAlCrear = 'Error de db';
                    break;
                }
                case CreateUserResult.UserAlreadyExist: {
                    this.errorAlCrear = 'ya existe usuario';
                    break;
                }
                case CreateUserResult.MailAlreadyExist: {
                    this.errorAlCrear = 'ya existe mail';
                    break;
                }
                default: {
                    this.errorAlCrear = 'error desconocido';
                }
            }
        });
    }

    private RecoverPassword() {
        this.errorAlCrear = '';
    }
}
</script>