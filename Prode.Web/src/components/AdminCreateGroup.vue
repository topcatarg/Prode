<template>
    <div class="container border border-secondary rounded">
        <b-form @submit="onSubmit" class="mt-4 mb-2 ">
            <b-form-group
                horizontal
                label-cols="4"
                id="pass2"
                label="Nombre del grupo">
                <b-form-input v-model.trim="GroupName" required />
            </b-form-group>
            <b-button type="submit" variant="primary" class="mr-3" :disabled="ButtonOcupied">
                <div v-if="!ButtonOcupied">
                    Crear Grupo
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
import ErrorAlert from '../components/ErrorAlert.vue';
import SuccessAlert from '../components/SuccesAlert.vue';

@Component({
    components: {
        ErrorAlert,
        SuccessAlert
    }
})
export default class AdminCreateGroup extends Vue {
    private GeneralError: string = '';
    private GeneralMessage: string = '';
    private ButtonOcupied: boolean = false;
    private GroupName: string = '';

    private onSubmit() {
        this.ButtonOcupied = true;
        this.ClearMessages();
        // test group
        Axios.post(process.env.VUE_APP_BASE_URI + 'admin/CreateGroup?GroupName=' + this.GroupName,
        {},
        {withCredentials: true})
        .then(response => {
            this.GeneralMessage = 'Grupo creado con exito';
            this.ButtonOcupied = false;
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