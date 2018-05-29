<template>
    <div class="container">
        <b-form @submit="onSubmit" class="mt-4">
            <b-form-group
                horizontal
                label-cols="4"
                id="pass2"
                label="Contraseña">
                <b-form-input v-model.trim="UserPassword" required type="password"/>
            </b-form-group>
            <b-form-group
                horizontal
                label-cols="4"
                id="pass2"
                label="Repetí la contraseña"
                :invalid-feedback="UnEvenPassword"
                :state="statePassword">
                <b-form-input v-model.trim="UserPassword2" required type="password"/>
            </b-form-group>
            <b-button type="submit" variant="primary" class="mr-3" :disabled="ButtonOcupied">
                <div v-if="!ButtonOcupied">
                        Actualizar password
                    </div>
                    <div v-else-if="ButtonOcupied">
                        <i class="fa fa-cog fa-spin fa-fw"></i>
                    </div>
                </b-button>
            <ErrorAlert :message=this.GeneralError class="mt-2"/>
            <SuccessAlert :message=this.GeneralMessage class="mt-2"/>
        </b-form>
    </div>
</template>

<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue } from 'vue-property-decorator';
import ErrorAlert from '../components/ErrorAlert.vue';
import SuccessAlert from '../components/SuccesAlert.vue';

@Component({
    components: {
        ErrorAlert,
        SuccessAlert
    }
})
export default class CreateNewPassword extends Vue {

    private UserPassword: string = '';
    private UserPassword2: string = '';
    private guid: string = '';
    private GeneralError: string = '';
    private GeneralMessage: string = '';
    private UnEvenPassword: string = 'Las contraseñas no son iguales';
    private ButtonOcupied: boolean = false;

    private mounted() {
        this.guid = this.$route.query.code;
    }

    get statePassword(): boolean {
        return this.UserPassword === this.UserPassword2;
    }

    private onSubmit() {
        this.ButtonOcupied = true;
        this.ClearMessages();
        // test group
        Axios.post(process.env.VUE_APP_BASE_URI + 'setnewpassword?pass=' + this.UserPassword + '&guid=' + this.guid)
        .then(response => {
            this.GeneralMessage = 'Password cambiada con exito';
            this.ButtonOcupied = false;
        })
        .catch(error => {
            this.GeneralError = 'No se pudo cambiar la password (su codigo vencio?)';
            this.ButtonOcupied = false;
        });
    }

    private ClearMessages() {
        this.GeneralError = '';
        this.GeneralMessage = '';
    }
}
</script>
