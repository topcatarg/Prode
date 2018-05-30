<template>
    <div class="container border border-secondary rounded">
        <h3>Cambiar contraseña</h3>
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
import ErrorAlert from '../ErrorAlert.vue';
import SuccessAlert from '../SuccesAlert.vue';

@Component({
    components: {
        ErrorAlert,
        SuccessAlert
    }
})
export default class ChangePassword extends Vue {
    private GeneralError: string = '';
    private GeneralMessage: string = '';
    private ButtonOcupied: boolean = false;
    private UserPassword: string = '';
    private UserPassword2: string = '';
    private UnEvenPassword: string = 'Las contraseñas no son iguales';

    private onSubmit() {
        this.ClearMessages();
        this.ButtonOcupied = true;
        Axios.post(process.env.VUE_APP_BASE_URI + 'changepassword?UserId=' + this.$store.getters.UserId +
        '&NewPassword=' + this.UserPassword,
        {},
        {withCredentials: true})
        .then(response => {
            this.GeneralMessage = 'Se ha cambiado el password';
            this.ButtonOcupied = false;
            this.UserPassword = '';
            this.UserPassword2 = '';
        })
        .catch(error => {
            this.GeneralError = 'Ocurrio un error al cambiar el password (volve a intentarlo)';
            this.ButtonOcupied = false;
        });
    }

    get statePassword(): boolean {
        return this.UserPassword === this.UserPassword2;
    }
    private ClearMessages() {
        this.GeneralError = '';
        this.GeneralMessage = '';
    }
}
</script>