<template>
    <div>
        <b-container class="bv-example-row">
            <b-row>
                <b-col>
                    Usuario existente:
                    <b-form-input 
                    v-model="user"/>
                    Password:
        <b-form-input 
            placeholder="Enter password" 
            type="password"
            v-model="password"/>
        <b-button @click="Login">
            Ingresar
        </b-button>
        {{this.logueado}}
                </b-col>
                <b-col>
                    Nuevo Usuario
                    <b-row>
                        <b-col>
                            Nombre: 
                        </b-col>
                        <b-col>
                            <b-form-input v-model="newuser" required/>
                        </b-col>
                    </b-row>
                    <b-row>
                        <b-col>
                            Direccion de correo:
                        </b-col>
                        <b-col>
                            <b-form-input type="email" v-model="mail" required/>
                        </b-col>
                    </b-row>
                    <b-row>
                        <b-col>
                            Password:
                        </b-col>
                        <b-col>
                            <b-form-input placeholder="Enter password" type="password" v-model="newpassword" required/>
                        </b-col>
                    </b-row>
                    <b-row>
                        <b-col>
                            <b-button @click="Create"> 
                                Crear Usuario
                            </b-button>
                        </b-col>
                    </b-row>
                </b-col>
        </b-row>
        </b-container>
    </div>
</template>
<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue } from 'vue-property-decorator';
import store from '../store';

@Component
export default class Login extends Vue {
    private user: string = '';
    private newuser: string = '';
    private password: string = '';
    private newpassword: string = '';
    private logueado: string = 'nose';
    private mail: string = '';
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
        Axios.post(process.env.VUE_APP_BASE_URI + 'create',
        {
            name: this.newuser,
            password: this.newpassword,
            mail: this.mail
        })
        .then(data => this.logueado = 'Creado')
        .catch(data => this.logueado = 'Parametros vacios');
    }
}
</script>