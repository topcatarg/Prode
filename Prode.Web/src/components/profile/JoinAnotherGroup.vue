<template>
    <div class="container border border-secondary rounded">
        <h2> Unirse a otro grupo </h2>
        <b-form @submit="onSubmit" class="mt-4 mb-2 ">
            <b-form-group
                horizontal
                label-cols="4"
                id="pass2"
                label="Nombre del grupo">
                <b-form-input v-model="GroupName" required />
            </b-form-group>
            <b-button type="submit" variant="primary" class="mr-3" :disabled="ButtonOcupied">
                <div v-if="!ButtonOcupied">
                    Unirse
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
export default class JoinAnotherGroup extends Vue {
    private GeneralError: string = '';
    private GeneralMessage: string = '';
    private ButtonOcupied: boolean = false;
    private GroupName: string = '';

    private onSubmit() {
        this.ButtonOcupied = true;
        this.ClearMessages();
        // test group
        Axios.post(process.env.VUE_APP_BASE_URI + 'JoinOtherGroup?group=' + this.GroupName,
        {},
        {withCredentials: true})
        .then(response => {
            this.GeneralMessage = 'Se ha unido al grupo con exito';
            this.ButtonOcupied = false;
            this.$store.dispatch('GetUserData');
        })
        .catch(error => {
            this.GeneralError = 'Ocurrio un error (probablemente ese grupo no existe)';
            this.ButtonOcupied = false;
        });
    }

    private ClearMessages() {
        this.GeneralError = '';
        this.GeneralMessage = '';
    }
}
</script>