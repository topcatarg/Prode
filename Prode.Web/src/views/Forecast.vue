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
                        Ver solo los restantes
                    </b-form-checkbox>
                </b-col>
                <b-col>
                    <b-button variant="primary" @click="SaveAll">
                        Guardar todos los cambios
                    </b-button>
                </b-col>
            </b-row>
        </b-container>
        <b-table striped hover :items="filteredItems" :fields="fields" class="ml-2 mr-2">
            <template slot="team1Name" slot-scope="data">
                <b-img v-if="data.item.team1Flag!=null" :src="'http://www.countryflags.io/'+ data.item.team1Flag +'/shiny/24.png'" />
                {{data.item.team1Name}}
            </template>
            <template slot="Result" slot-scope="data">
                <b-container >
                    <b-row>
                        <b-col cols="5">
                            <b-form-input type="number" v-model="data.item.team1Forecast">
                            </b-form-input>
                        </b-col>
                        <b-col cols="2">
                            -
                        </b-col>
                        <b-col cols="5">
                            <b-form-input type="number" v-model="data.item.team2Forecast">
                            </b-form-input>
                        </b-col>
                    </b-row>
                </b-container>
            </template>
            <template slot="team2Name" slot-scope="data">
                <b-img v-if="data.item.team2Flag!=null" :src="'http://www.countryflags.io/'+ data.item.team2Flag +'/shiny/24.png'" />
                {{data.item.team2Name}}
            </template>
            <template slot="actions" slot-scope="data">
                <b-button size="sm" variant="primary" 
                    @click="StoreRow(data.item.id, data.item.team1Forecast, data.item.team2Forecast)"
                    v-if="data.item.canUpdate">
                    Guardar este resultado
                </b-button>
            </template>
        </b-table>
    </div>
</template>

<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import IFixtureData from '../helpers/FixtureData';
import IFixtureTableData from '../helpers/FixtureTableData';
import IFixtureTableFields from '../helpers/FixtureTableFields';
import IMatchData from '../helpers/MatchData';

@Component
export default class Forecast extends Vue {
    private fields: IFixtureTableFields[] = [];
    private items: IFixtureData[] = [];
    private filteredItems: IFixtureData[] = this.items;
    private FilterValue: string = '';
    private CurrentTime?: Date = undefined;
    private OnlyAvailables: boolean = false;

    constructor() {
        super();
        this.fields.push(new IFixtureTableFields('date', 'Fecha'));
        this.fields.push(new IFixtureTableFields('team1Name', 'Equipo'));
        this.fields.push(new IFixtureTableFields('Result', 'Resultado'));
        this.fields.push(new IFixtureTableFields('team2Name', 'Equipo'));
        this.fields.push(new IFixtureTableFields('wwGroup', 'Grupo'));
        this.fields.push(new IFixtureTableFields('actions', ''));
    }

    private mounted() {
        Axios.get(process.env.VUE_APP_BASE_URI + 'getstandartime')
        .then(Response => this.CurrentTime = Response.data);
        Axios.get(process.env.VUE_APP_BASE_URI + 'forecast/my?UserId=' + this.ComputedUserId, {withCredentials: true})
        .then(Response => this.items = this.filteredItems = Response.data);
    }

    get ComputedUserId(): number {
        return this.$store.getters.UserId;
    }

    private StoreRow(id: number, goals1: number, goals2: number) {
        Axios.post(process.env.VUE_APP_BASE_URI + 'forecast/fillgame',
        {
            MatchId : id,
            UserId : this.ComputedUserId,
            Team1Forecast: goals1,
            Team2Forecast: goals2
        }, {withCredentials: true});
    }

    private SaveAll() {
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
        {withCredentials: true});
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
}
</script>
