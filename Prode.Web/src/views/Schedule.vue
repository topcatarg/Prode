<template>
    <div>
        <b-table striped hover :items="items" :fields="fields">
            <template slot="team1Name" slot-scope="data">
                <b-img :src="'http://www.countryflags.io/'+ data.item.team1Flag +'/shiny/24.png'" />
                {{data.item.team1Name}}
            </template>
            <template slot="Result" slot-scope="data">
                {{data.item.team1Goals}} - {{data.item.team2Goals}}
            </template>
            <template slot="team2Name" slot-scope="data">
                <b-img :src="'http://www.countryflags.io/'+ data.item.team2Flag +'/shiny/24.png'" />
                {{data.item.team2Name}}
            </template>
        </b-table>
    </div>
</template>
<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import IFixtureTableData from '../helpers/FixtureTableData';
import IFixtureTableFields from '../helpers/FixtureTableFields';

@Component
export default class Schedule extends Vue {
    private fields: IFixtureTableFields[] = [];
    private items: IFixtureTableData[] = [];

    constructor() {
        super();
        this.fields.push(new IFixtureTableFields('Id', 'Nada'));
        this.fields.push(new IFixtureTableFields('date', 'Fecha'));
        this.fields.push(new IFixtureTableFields('team1Name', 'Equipo'));
        this.fields.push(new IFixtureTableFields('Result', 'Resultado'));
        this.fields.push(new IFixtureTableFields('team2Name', 'Equipo'));
    }

    private mounted() {
        Axios.get(process.env.VUE_APP_BASE_URI + 'fixture/allmatchs')
        .then(Response => this.items = Response.data);
    }
}
</script>