<template>
    <div class="container">
        <b-table striped hover small stacked="md" :items="items" :fields="fields">
              <template slot="Result" slot-scope="data">
                {{data.item.team1Forecast}} - {{data.item.team2Forecast}}
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
import IFixtureData from '../../helpers/FixtureData';
import IFixtureTableFields from '../../helpers/FixtureTableFields';

const AppProps = Vue.extend({
  props: {
    id: Number
  }
});

@Component
export default class UserForecast extends AppProps {

    private otherid: number = 0;
    private fields: IFixtureTableFields[] = [];
    private items: IFixtureData[] = [];

    constructor() {
        super();
        this.fields.push(new IFixtureTableFields('team1Name', 'Equipo'));
        this.fields.push(new IFixtureTableFields('Result', 'Resultado'));
        this.fields.push(new IFixtureTableFields('team2Name', 'Equipo'));
        this.fields.push(new IFixtureTableFields('score', 'Puntaje'));
    }

    @Watch('id')
    private MostrarError(newvalue: number, oldvalue: number) {
        this.otherid = newvalue;
        Axios.get(process.env.VUE_APP_BASE_URI + 'forecast/others?UserId=' + this.otherid, {withCredentials: true})
        .then(Response => this.items = Response.data);
    }
}
</script>