<template>
    <div class="container">
        <b-container class="mt-2">
            <b-row>
                <b-col>
                    <b-form-select v-model="selected" :options="options" class="mb-3" />
                </b-col>
            </b-row>
        </b-container>
        <b-table striped hover stacked="md" :items="ChangeGroup" :fields="fields" class="mt-2">
            <template slot="userdata" slot-scope="data">
                <b-button @click="ShowModal(data.item.userId)" variant="primary">Ver puntajes</b-button>
            </template>
        </b-table>
        <b-modal v-model="showmodal" size="lg" centered ok-only hide-header>
            <UserForecast :id="this.userid"/>
        </b-modal>
    </div>
</template>
<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import UserForecast from '../components/modals/UserForecast.vue';
import IFixtureData from '../helpers/FixtureData';
import IResultTableFields from '../helpers/ResultTableFields';
import ISelectInput from '../helpers/SelectInputHelper';
import GameGroups from '../models/GameGroups';
import IResults from '../models/Results';
import { resolve } from 'url';

@Component({
  components: {
    UserForecast
  }
})
export default class Positiones extends Vue {

    public items: IResults[] = [];
    public fields: IResultTableFields[] = [];
    public selected: number = 0;
    public options: ISelectInput[] = [];
    public userid: number = 0;
    private showmodal: boolean = false;
    private otheritems: IFixtureData[] = [];

    constructor() {
        super();
        this.fields.push( {
            key: 'position',
            label: 'Posición'
        });
        this.fields.push( {
            key: 'teamName',
            label: 'Nombre del equipo'
        });
        this.fields.push( {
            key: 'score',
            label: 'Puntajes'
        });
        this.fields.push( {
            key: 'userdata',
            label: ''
        });
    }

    private mounted() {
        Axios.get(process.env.VUE_APP_BASE_URI + 'GetUserGroups',
        {withCredentials: true})
        .then(response => {
            response.data.forEach((element: GameGroups)  => {
                this.options.push({
                    value: element.id,
                    text: element.gameGroup
                });
            });
        });
    }

    private get ChangeGroup(): IResults[] {
        console.log('computed');
        const r = await Axios.get(process.env.VUE_APP_BASE_URI + 'results?GroupNumber='
        + this.selected,
        {withCredentials: true});
        console.log(r);
        return [];
    }

    @Watch ('selected')
    private ChangeGroup2() {
        Axios.get(process.env.VUE_APP_BASE_URI + 'results?GroupNumber='
        + this.selected,
        {withCredentials: true})
        .then(data => {
            this.items = data.data;
            let i = 1;
            this.items.forEach(
                (e: IResults) => {
                    e.position = i;
                    i++;
                }
            );
        });
    }

    get UserGroupId(): number {
        return this.$store.getters.UserGroup;
    }

    private ShowModal(id: number) {

        // Axios.get(process.env.VUE_APP_BASE_URI + 'forecast/my?others=' + id, {withCredentials: true})
        // .then(Response => this.otheritems = Response.data);
        this.userid = id;
        this.showmodal = true;
    }
}
</script>