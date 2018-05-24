<template>
    <div>
        <b-container class="mt-2 mb-2">
            <b-row>
                <b-col cols="8">
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
                <b-col>
                    <b-button variant="primary">
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
                            <b-form-input type="number">
                            </b-form-input>
                        </b-col>
                        <b-col cols="2">
                            -
                        </b-col>
                        <b-col cols="5">
                            <b-form-input type="number">
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
                <b-button size="sm" variant="primary">
                    Guardar este resultado
                </b-button>
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
export default class Forecast extends Vue {
    private fields: IFixtureTableFields[] = [];
    private items: IFixtureTableData[] = [];
    private filteredItems: IFixtureTableData[] = this.items;
    private FilterValue: string = '';
    private CurrentTime?: Date = undefined;

    constructor() {
        super();
        this.fields.push(new IFixtureTableFields('date', 'Fecha'));
        this.fields.push(new IFixtureTableFields('team1Name', 'Equipo'));
        this.fields.push(new IFixtureTableFields('Result', 'Resultado'));
        this.fields.push(new IFixtureTableFields('team2Name', 'Equipo'));
        this.fields.push(new IFixtureTableFields('grupo', 'Grupo'));
        this.fields.push(new IFixtureTableFields('actions', ''));
    }

    private mounted() {
        Axios.get(process.env.VUE_APP_BASE_URI + 'getstandartime')
        .then(Response => this.CurrentTime = Response.data);
        Axios.get(process.env.VUE_APP_BASE_URI + 'fixture/allmatchs')
        .then(Response => this.items = this.filteredItems = Response.data);
    }

    @Watch('FilterValue')
    private filtrar() {
        if (this.FilterValue === '') {
            this.filteredItems = this.items;
            return;
        }
        this.filteredItems = [];
        this.items.forEach(element => {
            if (element.grupo === this.FilterValue) {
                this.filteredItems.push(element);
            }
        });
    }
}
</script>
