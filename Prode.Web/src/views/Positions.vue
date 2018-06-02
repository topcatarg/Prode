<template>
    <div class="container">
        <b-container class="mt-2">
            <b-row>
                <b-col>
                    <b-form-select v-model="selected" :options="options" class="mb-3" />
                </b-col>
            </b-row>
        </b-container>
        <b-table striped hover :items="items" :fields="fields" class="mt-2">

        </b-table>
    </div>
</template>
<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import IResultTableFields from '../helpers/ResultTableFields';
import ISelectInput from '../helpers/SelectInputHelper';
import GameGroups from '../models/GameGroups';
import IResults from '../models/Results';

@Component
export default class Positiones extends Vue {

    public items: IResults[] = [];
    public fields: IResultTableFields[] = [];
    public selected: number = 0;
    public options: ISelectInput[] = [];

    constructor() {
        super();
        this.fields.push( {
            key: 'teamName',
            label: 'Nombre del equipo'
        });
        this.fields.push( {
            key: 'score',
            label: 'Puntajes'
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

    @Watch ('selected')
    private ChangeGroup() {
        Axios.get(process.env.VUE_APP_BASE_URI + 'results?GroupNumber='
        + this.selected,
        {withCredentials: true})
        .then(data => this.items = data.data);
    }

    get UserGroupId(): number {
        return this.$store.getters.UserGroup;
    }
}
</script>