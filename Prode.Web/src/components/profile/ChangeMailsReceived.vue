<template>
    <div class="container border border-secondary rounded">
        <h2>Recepción de mails</h2>
        <b-form @submit="onSubmit" class="mt-4 mb-2 ">
            <b-form-checkbox
                     v-model="changeforecast"
                     value="true"
                     unchecked-value="false">
                    Recibir un mail cada vez que cambias el pronostico
            </b-form-checkbox>
            <br/>
           <b-form-checkbox
                     v-model="changeforecastadmin"
                     value="true"
                     unchecked-value="false">
                    Recibir un mail cada vez que cambia el pronostico el administrador
            </b-form-checkbox>
            <br/>
            <b-button type="submit" variant="primary" class="mt-2" :disabled="ButtonOcupied">
                <div v-if="!ButtonOcupied">
                    Cambiar opciones
                </div>
                <div v-else-if="ButtonOcupied">
                    <i class="fa fa-cog fa-spin fa-fw"></i>
                </div>
            </b-button>
        </b-form>
        <ErrorAlert :message=this.GeneralError class="mt-2"/>
        <SuccessAlert :message=this.GeneralMessage class="mt-2"/>
    </div>
</template>

<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue } from 'vue-property-decorator';
import ErrorAlert from '../ErrorAlert.vue';
import SuccessAlert from '../SuccesAlert.vue';

@Component({
    components: {
        ErrorAlert,
        SuccessAlert
    }
})
export default class ChangeMailsReceived extends Vue {
    private GeneralError: string = '';
    private GeneralMessage: string = '';
    private ButtonOcupied: boolean = false;
    private changeforecast: boolean = false;
    private changeforecastadmin: boolean = false;

    private mounted() {
        this.changeforecast = this.$store.getters.UserReceiveMails;
        this.changeforecastadmin = this.$store.getters.UserReceiveAdminMails;
    }

    private onSubmit() {
        this.ButtonOcupied = true;
        this.ClearMessages();
        // test group
        Axios.post(process.env.VUE_APP_BASE_URI + 'changemailreceived?UserId=' + this.$store.getters.UserId
        + '&ReceiveMails=' + this.changeforecast + '&ReceiveAdminMails=' + this.changeforecastadmin,
        {},
        {withCredentials: true})
        .then(response => {
            this.GeneralMessage = 'Cambios en los mails correctos';
            this.ButtonOcupied = false;
            this.$store.dispatch('GetUserData');
        })
        .catch(error => {
            this.GeneralError = 'Ocurrio un error';
            this.ButtonOcupied = false;
        });
    }

    private ClearMessages() {
        this.GeneralError = '';
        this.GeneralMessage = '';
    }
}
</script>