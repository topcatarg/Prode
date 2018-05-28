<template>
    <div class="container">
        <b-table striped hover :items="items" :fields="fields">

        </b-table>
    </div>
</template>
<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import IResultTableFields from '../helpers/ResultTableFields';
import IResults from '../models/Results';

@Component
export default class Positiones extends Vue {

    public items: IResults[] = [];
    public fields: IResultTableFields[] = [];

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
        Axios.get(process.env.VUE_APP_BASE_URI + 'results?' + this.UserGroupId, {withCredentials: true})
        .then(data => this.items = data.data);
    }

    get UserGroupId(): number {
        return this.$store.getters.UserGroup;
    }
}
</script>