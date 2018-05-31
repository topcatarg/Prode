<template>
    <div class="container">
        <b-form @submit="onSubmit" class="mt-4">
            <b-form-group
                horizontal
                label-cols="4"
                id="fieldset1"
                label="Direccion de correo">
                <b-form-input v-model.trim="UserMail" required type="email"/>
            </b-form-group>
            <b-button type="submit" variant="primary" class="mr-3"
            :disabled="ButtonOcupied">
                <div v-if="!ButtonOcupied">
                    Recuperar password
                </div>
                <div v-else-if="ButtonOcupied">
                    <i class="fa fa-cog fa-spin fa-fw"></i>
                </div>
            </b-button>
            <ErrorAlert :message="this.GeneralError" class="mt-2"/>
            <SuccessAlert :message="this.GeneralMessage" class="mt-2"/>
        </b-form>
    </div>
</template>

<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import ErrorAlert from '../components/ErrorAlert.vue';
import SuccessAlert from '../components/SuccesAlert.vue';

@Component({
  components: {
    ErrorAlert,
    SuccessAlert
  }
})
export default class RecoverMail extends Vue {
    private GeneralError: string = '';
    private GeneralMessage: string = '';
    private UserMail: string = '';
    private ButtonOcupied: boolean = false;

    private onSubmit() {
        this.ClearMessages();
        this.ButtonOcupied = true;
        Axios.post(process.env.VUE_APP_BASE_URI + 'recorverpassword?mail=' + this.UserMail,
        {},
        {withCredentials: true})
        .then(response => {
            this.GeneralMessage = 'Se ha enviado el mail';
            this.ButtonOcupied = false;
        })
        .catch(error => {
            this.GeneralError = 'Ocurrio un error al enviar el mail (la direccion de correo es correcta?)';
            this.ButtonOcupied = false;
        });
    }

    private ClearMessages() {
        this.GeneralError = '';
        this.GeneralMessage = '';
    }
}
</script>

