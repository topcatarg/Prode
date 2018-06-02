<template>
    <div class="container">
        <b-table striped hover :items="items" :fields="fields">
            <template slot="date"/>
            <template slot="team1Name" slot-scope="data">
                <b-form-select v-model="data.item.team1" :options="teamoptions" class="mb-3" />
            </template>
            <template slot="team1Goals" slot-scope="data">
                <b-form-select v-model="data.item.team1Goals" :options="options" class="mb-3" />
            </template>
            <template slot="team2Name" slot-scope="data">
                <b-form-select v-model="data.item.team2" :options="teamoptions" class="mb-3" />
            </template>
            <template slot="team2Goals" slot-scope="data">
                <b-form-select v-model="data.item.team2Goals" :options="options" class="mb-3" />
            </template>
            <template slot="closed" slot-scope="data">
                <b-form-checkbox v-model="data.item.closed" disabled/>
            </template>
            <template slot="Actions" slot-scope="data">
                <b-button size="sm" variant="primary" @click="Update(data.item.id)" :disabled="ButtonOcupied">
                    <div v-if="!ButtonOcupied">
                        Actualizar resultado
                    </div>
                    <div v-else-if="ButtonOcupied">
                        <i class="fa fa-cog fa-spin fa-fw"></i>
                    </div>
                </b-button>
                <b-button size="sm" variant="primary" @click="UpdateTeams(data.item.id)" :disabled="ButtonOcupied" class="mt-2">
                    <div v-if="!ButtonOcupied">
                        Actualizar equipos
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
import IFixtureData from '../../helpers/FixtureData';
import IResultTableFields from '../../helpers/ResultTableFields';
import ISelectInput from '../../helpers/SelectInputHelper';
import IMatchResult from '../../models/MatchResult'
import ITeams from '../../models/Teams'

@Component
export default class AdminMatchs extends Vue {

    public items: IFixtureData[] = [];
    public fields: IResultTableFields[] = [];
    public options: ISelectInput[] = [];
    public teamoptions: ISelectInput[] = [];
    private ButtonOcupied: boolean = false;

    constructor() {
        super();
        this.fields.push( {
            key: 'date',
            label: 'Fecha'
        });
        this.fields.push( {
            key: 'team1Name',
            label: 'Equipo 1'
        });
        this.fields.push( {
            key: 'team1Goals',
            label: 'Goles 1'
        });
        this.fields.push( {
            key: 'team2Name',
            label: 'Equipo 2'
        });
        this.fields.push( {
            key: 'team2Goals',
            label: 'Goles 2'
        });
        this.fields.push( {
            key: 'closed',
            label: 'Contado'
        });
        this.fields.push( {
            key: 'Actions',
            label: ''
        });
        var i: number;
        for (i = 0; i < 51; i++) {
            this.options.push( {
                value: i,
                text: i.toString()
            })
        }  
    }

    private mounted() {
        this.teamoptions = [];
        Axios.get(process.env.VUE_APP_BASE_URI + 'admin/GetTeamList', {withCredentials: true})
        .then(response => {
            response.data.forEach((element: ITeams) => {
                this.teamoptions.push({
                    value: element.id,
                    text: element.team
                })
            });
        })
        this.GetAllResults();
    }

    private Update(id: number) {
        this.ButtonOcupied = true;
        var i = this.items.find(p => p.id === id);
        if (i === undefined)
        {
            return;
        }
        var m: IMatchResult = {
            MatchId: i.id,
            Team1: i.team1,
            Team2: i.team2,
            Team1Goals: i.team1Goals === undefined ? 0 : i.team1Goals,
            Team2Goals: i.team2Goals === undefined ? 0 : i.team2Goals
        }
        Axios.post(process.env.VUE_APP_BASE_URI + 'admin/UpdateGame',
        m,
        {withCredentials: true})
        .then(response => {
            this.ButtonOcupied = false;
            this.GetAllResults();
        })
        .catch(error => this.ButtonOcupied = false);
    }

    private UpdateTeams(id: number){
        this.ButtonOcupied = true;
        var i = this.items.find(p => p.id === id);
        if (i === undefined)
        {
            return;
        }
        var m: IMatchResult = {
            MatchId: i.id,
            Team1: i.team1,
            Team2: i.team2,
            Team1Goals: i.team1Goals === undefined ? 0 : i.team1Goals,
            Team2Goals: i.team2Goals === undefined ? 0 : i.team2Goals
        }
        console.log(m);
        Axios.post(process.env.VUE_APP_BASE_URI + 'admin/UpdateTeams',
        m,
        {withCredentials: true})
        .then(response => {
            this.ButtonOcupied = false;
            this.GetAllResults();
        })
        .catch(error => this.ButtonOcupied = false);
    }

    private GetAllResults() {
        Axios.get(process.env.VUE_APP_BASE_URI + 'admin/GetAllResults', {withCredentials: true})
        .then(response => this.items = response.data);
    }
}
</script>
