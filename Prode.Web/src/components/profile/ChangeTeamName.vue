<template>
    <div class="container border border-secondary rounded">
        <h2> Nombre del equipo </h2>
        <b-form @submit="onSubmit" class="mt-4 mb-2 ">
            <b-form-group
                horizontal
                label-cols="4"
                id="pass2"
                label="Nombre del equipo">
                <b-form-input v-model="TeamName" required />
            </b-form-group>
            <b-button type="submit" variant="primary" class="mr-3" :disabled="ButtonOcupied">
                <div v-if="!ButtonOcupied">
                    Cambiar Nombre
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
export default class ChangeTeamName extends Vue {
    private GeneralError: string = '';
    private GeneralMessage: string = '';
    private ButtonOcupied: boolean = false;
    private TeamName: string = '';

    private mounted() {
        this.TeamName = this.$store.getters.UserTeamName;
    }

    private onSubmit() {
        this.ButtonOcupied = true;
        this.ClearMessages();
        // test group
        Axios.post(process.env.VUE_APP_BASE_URI + 'changeteamname?UserId=' + this.$store.getters.UserId +
        '&TeamName=' + this.TeamName,
        {},
        {withCredentials: true})
        .then(response => {
            this.GeneralMessage = 'Nombre cambiado con exito';
            this.ButtonOcupied = false;
            this.$store.dispatch('GetUserData');
        })
        .catch(error => {
            this.GeneralError = 'Ocurrio un error (reintentalo)';
            this.ButtonOcupied = false;
        });
    }

    private ClearMessages() {
        this.GeneralError = '';
        this.GeneralMessage = '';
    }
}
</script>