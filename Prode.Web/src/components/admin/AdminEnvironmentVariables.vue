<template>
    <div class="container">
        <div class="container border border-secondary rounded">
            <b-form @submit="onSubmit" class="mt-4 mb-2 ">
                <b-form-group
                    horizontal
                    label-cols="4"
                    id="clave"
                    label="Clave">
                    <b-form-input v-model="key" required />
                </b-form-group>
                <b-form-group
                    horizontal
                    label-cols="4"
                    id="valor"
                    label="Valor">
                    <b-form-input v-model="value" required />
                </b-form-group>
                <b-button type="submit" variant="primary" class="mr-3" :disabled="ButtonOcupied">
                    <div v-if="!ButtonOcupied">
                        Agregar Clave
                    </div>
                    <div v-else-if="ButtonOcupied">
                        <i class="fa fa-cog fa-spin fa-fw"></i>
                    </div>
                </b-button>
            </b-form>
            <ErrorAlert :message=this.GeneralError class="mt-2"/>
            <SuccessAlert :message=this.GeneralMessage class="mt-2"/>
        </div>
        <b-table striped hover :items="items" :fields="fields">
            <template slot="Actions" slot-scope="data">
                <b-button size="sm" variant="primary" @click="DeleteKey(data.item.key)" :disabled="ButtonOcupied">
                    <div v-if="!ButtonOcupied">
                        Borrar
                    </div>
                    <div v-else-if="ButtonOcupied">
                        <i class="fa fa-cog fa-spin fa-fw"></i>
                    </div>
                </b-button>
            </template>
        </b-table>
    </div>
</template>

<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue } from 'vue-property-decorator';
import IResultTableFields from '../../helpers/ResultTableFields';
import IEnvironmentVariables from '../../models/EnvironmentVariables';
import IMatchResult from '../../models/MatchResult';
import ErrorAlert from '../ErrorAlert.vue';
import SuccessAlert from '../SuccesAlert.vue';

@Component({
    components: {
        ErrorAlert,
        SuccessAlert
    }
})
export default class AdminEnviromentVariables extends Vue {

    public fields: IResultTableFields[] = [];
    private GeneralError: string = '';
    private GeneralMessage: string = '';
    private ButtonOcupied: boolean = false;
    private key: string = '';
    private value: string = '';
    private items: IEnvironmentVariables[] = [];

    constructor() {
        super();
        this.fields.push( {
            key: 'key',
            label: 'Clave'
        });
        this.fields.push( {
            key: 'value',
            label: 'Valor'
        });
        this.fields.push( {
            key: 'Actions',
            label: 'Valor'
        });
    }

    private mounted() {
        this.GetVariables();
    }

    private GetVariables() {
        Axios.get(process.env.VUE_APP_BASE_URI + 'admin/GetEnvironmentVariables', {withCredentials: true})
        .then(response => this.items = response.data);
    }

    private onSubmit() {
        this.ButtonOcupied = true;
        this.ClearMessages();
        // test group
        Axios.post(process.env.VUE_APP_BASE_URI + 'admin/addenvironmentvariable?key=' + this.key +
        '&value=' + this.value,
        {},
        {withCredentials: true})
        .then(response => {
            this.GeneralMessage = 'Clave añadida';
            this.ButtonOcupied = false;
            this.GetVariables();
        })
        .catch(error => {
            this.GeneralError = 'Ocurrio un error (reintentalo)';
            this.ButtonOcupied = false;
        });
    }

    private DeleteKey(key: string) {
        Axios.delete(process.env.VUE_APP_BASE_URI + 'admin/deleteenvironmentvariable?key=' + key,
            {withCredentials: true})
        .then(response => this.GetVariables());
    }

    private ClearMessages() {
        this.GeneralError = '';
        this.GeneralMessage = '';
    }

}
</script>
