<template>
    <div class="container">
        <b-container class="mt-2 mb-2">
            <b-row>
                <b-col cols="4">
                    <b-form-select v-model="FilterValue">
                        <option value="">Todos los grupos</option>
                        <option value="A">A</option>
                        <option value="B">B</option>
                        <option value="C">C</option>
                        <option value="D">D</option>
                        <option value="E">E</option>
                        <option value="F">F</option>
                        <option value="G">G</option>
                        <option value="H">H</option>
                        <option value="OCTAVOS">OCTAVOS</option>
                        <option value="CUARTOS">CUARTOS</option>
                        <option value="SEMIFINALES">SEMIFINALES</option>
                        <option value="FINALES">FINALES</option>
                    </b-form-select>
                </b-col>
                <b-col cols="4">
                    <b-form-checkbox v-model="OnlyAvailables" @change="filtrar">
                        Ver solo los partidos restantes
                    </b-form-checkbox>
                </b-col>
                <b-col>
                    <b-button variant="primary" @click="SaveAll" :disabled="ButtonOcupied">
                        <div v-if="!ButtonOcupied">
                            Guardar todos los cambios
                        </div>
                        <div v-else-if="ButtonOcupied">
                            <i class="fa fa-cog fa-spin fa-fw"></i>
                        </div>
                    </b-button>
                </b-col>
            </b-row>
            <b-row>
                <b-col>
                    <ErrorAlert :message=this.GeneralError class="mt-2"/>
                    <SuccessAlert :message=this.GeneralMessage class="mt-2"/>
                </b-col>
            </b-row>
        </b-container>
        <b-table striped hover stacked="md" :items="FilteredList" :fields="fields" class="ml-2 mr-2">
            <template slot="wwGroup" slot-scope="data">
                {{data.item.wwGroup}}
            </template>
            <template slot="date" slot-scope="data">
                {{data.item.standardDate}}
            </template>
            <template slot="team1Name" slot-scope="data">
                <b-img v-if="data.item.team1Flag!=null" :src="'http://www.countryflags.io/'+ data.item.team1Flag +'/shiny/24.png'" />
                {{data.item.team1Name}}
            </template>
            <template slot="Result" slot-scope="data">
                <b-container>
                    <b-row>
                        <b-col cols="5.5" class="m-0 p-0">
                            <b-form-select v-model="data.item.team1Forecast" :options="options" :disabled="ButtonOcupied||!data.item.canUpdate"/>
                        </b-col>
                        <b-col cols="1" class="m-0 p-0">
                            -
                        </b-col>
                        <b-col cols="5.5" class="m-0 p-0">
                            <b-form-select v-model="data.item.team2Forecast" :options="options" :disabled="ButtonOcupied||!data.item.canUpdate"/>
                        </b-col>
                    </b-row>
                </b-container>
            </template>
            <template slot="team2Name" slot-scope="data">
                <b-img v-if="data.item.team2Flag!=null" :src="'http://www.countryflags.io/'+ data.item.team2Flag +'/shiny/24.png'" />
                {{data.item.team2Name}}
            </template>
            <template slot="score" slot-scope="data">
                {{data.item.points}}
            </template>
        </b-table>
    </div>
</template>

<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import ErrorAlert from '../components/ErrorAlert.vue';
import SuccessAlert from '../components/SuccesAlert.vue';
import IFixtureData from '../helpers/FixtureData';
import IFixtureTableData from '../helpers/FixtureTableData';
import IFixtureTableFields from '../helpers/FixtureTableFields';
import IMatchData from '../helpers/MatchData';
import ISelectInput from '../helpers/SelectInputHelper';

@Component({
    components: {
        ErrorAlert,
        SuccessAlert
    }
})
export default class Forecast extends Vue {
    private fields: IFixtureTableFields[] = [];
    private items: IFixtureData[] = [];
    private filteredItems: IFixtureData[] = this.items;
    private FilterValue: string = '';
    private CurrentTime?: Date = undefined;
    private OnlyAvailables: boolean = false;
    private ButtonOcupied: boolean = false;
    private options: ISelectInput[] = [];
    private GeneralError: string = '';
    private GeneralMessage: string = '';

    constructor() {
        super();
        this.fields.push(new IFixtureTableFields('wwGroup', 'Grupo'));
        this.fields.push(new IFixtureTableFields('date', 'Fecha'));
        this.fields.push(new IFixtureTableFields('team1Name', 'Equipo'));
        this.fields.push(new IFixtureTableFields('Result', 'Resultado'));
        this.fields.push(new IFixtureTableFields('team2Name', 'Equipo'));
        this.fields.push(new IFixtureTableFields('score', 'Puntaje'));
        let i: number;
        for (i = 0; i < 51; i++) {
            this.options.push( {
                value: i,
                text: i.toString()
            });
        }
    }

    private mounted() {
        this.GetMyForecast();
    }

    get ComputedUserId(): number {
        return this.$store.getters.UserId;
    }

    private StoreRow(id: number, goals1: number, goals2: number) {
        this.ButtonOcupied = true;
        Axios.post(process.env.VUE_APP_BASE_URI + 'forecast/fillgame',
        {
            MatchId : id,
            UserId : this.ComputedUserId,
            Team1Forecast: goals1,
            Team2Forecast: goals2
        }, {withCredentials: true})
        .then(response => this.ButtonOcupied = false);
    }

    private SaveAll() {
        this.ButtonOcupied = true;
        this.ClearMessages();
        const v: IMatchData[] = [];
        this.items.forEach(t => {
            if (t.canUpdate) {
                const m: IMatchData = {
                    MatchId: t.id,
                    UserId: this.ComputedUserId,
                    Team1Forecast: t.team1Forecast === undefined ? 0 : t.team1Forecast,
                    Team2Forecast: t.team2Forecast === undefined ? 0 : t.team2Forecast
                };
                v.push(m);
            }
        });
        Axios.post(process.env.VUE_APP_BASE_URI + 'forecast/fillall',
        v,
        {withCredentials: true})
        .then(response => {
            this.GetMyForecast();
            this.ButtonOcupied = false;
            this.GeneralMessage = 'Se actualizaron sus datos';
        })
        .catch(error => {
            this.ButtonOcupied = false;
            this.GeneralError = error.response;
        });
    }

    private GetMyForecast() {
        Axios.get(process.env.VUE_APP_BASE_URI + 'forecast/my?UserId=' + this.ComputedUserId, {withCredentials: true})
        .then(Response => {
            this.items = this.filteredItems = Response.data;
        });
    }

    @Watch('FilterValue')
    private filtrar() {
        if (this.FilterValue === '') {
            if (!this.OnlyAvailables) {
                this.filteredItems = this.items.filter(i => i.canUpdate);
            } else {
                this.filteredItems = this.items;
            }
        } else {
            if (!this.OnlyAvailables) {
                this.filteredItems = this.items.filter(i => i.canUpdate && i.wwGroup === this.FilterValue);
            } else {
                this.filteredItems = this.items.filter(i => i.wwGroup === this.FilterValue);
            }
        }
    }

    private get FilteredList(): IFixtureData[] {
        if (this.FilterValue === '') {
            if (this.OnlyAvailables) {
                return this.items.filter(i => i.canUpdate);
            } else {
                return this.items;
            }
        } else {
            if (this.OnlyAvailables) {
                return this.items.filter(i => i.canUpdate && i.wwGroup === this.FilterValue);
            } else {
                return this.items.filter(i => i.wwGroup === this.FilterValue);
            }
        }
    }

    private ClearMessages() {
        this.GeneralError = '';
        this.GeneralMessage = '';
    }
}
</script>
