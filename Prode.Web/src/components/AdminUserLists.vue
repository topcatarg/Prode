<template>
    <div class="container border border-secondary rounded">
        <b-container>
            <b-row class="mt-2">
                <b-col>
                    <b-form-select v-model="SelectedGroup" :options="GroupOptions" class="mb-3" />
                </b-col>
            </b-row>
            <b-row class="mt-2">
                <b-col>
                    <b-table striped hover :items="UserInfo" :fields="fields" class="ml-2 mr-2">
                        <template slot="hasPaid" slot-scope="data">
                            <b-form-checkbox v-model="data.item.hasPaid" disabled/>
                        </template>
                        <template slot="Actions" slot-scope="data">
                            <b-button size="sm" variant="primary" 
                            @click="UserPaid(data.item.id)"
                            :disabled="ButtonOcupied">
                                <div v-if="!ButtonOcupied">
                                    Cambiar Pago
                                </div>
                                <div v-else-if="ButtonOcupied">
                                    <i class="fa fa-cog fa-spin fa-fw"></i>
                                </div>
                            </b-button>
                        </template>
                    </b-table>
                </b-col>
            </b-row>
        </b-container>
    </div>
</template>

<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import IResultTableFields from '../helpers/ResultTableFields';
import ISelectInput from '../helpers/SelectInputHelper';
import GameGroups from '../models/GameGroups';
import { IUserInfo } from '../models/UserInfo';

@Component
export default class AdminUserList extends Vue {

    private SelectedGroup: number = 0;
    private GroupOptions: ISelectInput[] = [];
    private UserInfo: IUserInfo[] = [];
    private fields: IResultTableFields[] = [];
    private ButtonOcupied: boolean = false;

    constructor() {
        super();
        this.fields.push( {
            key: 'id',
            label: 'Id'
        });
        this.fields.push( {
            key: 'teamName',
            label: 'Equipo'
        });
        this.fields.push( {
            key: 'mail',
            label: 'Correo'
        });
        this.fields.push( {
            key: 'hasPaid',
            label: 'Pago'
        });
        this.fields.push( {
            key: 'Actions',
            label: 'mostrar'
        });
    }

    private mounted() {
        Axios.get(process.env.VUE_APP_BASE_URI + 'admin/GetGroupList', {withCredentials: true})
        .then(response => {
            response.data.forEach((element: GameGroups)  => {
                this.GroupOptions.push({
                    value: element.id,
                    text: element.gameGroup
                });
            });
        });
    }

    private UserPaid(id: number) {
        this.ButtonOcupied = true;
        Axios.post(process.env.VUE_APP_BASE_URI + 'admin/ChangePaidValue?UserId=' + id,
        {},
        {withCredentials: true})
        .then(response => {
            this.ButtonOcupied = false;
            this.GetUsersList();
        })
        .catch(error => {
            this.ButtonOcupied = false;
        });
    }

    @Watch ('SelectedGroup')
    private ChangedSelectedGroup() {
        if (this.SelectedGroup === 0) {
            return;
        }
        this.GetUsersList();
    }

    private GetUsersList() {
        Axios.get(process.env.VUE_APP_BASE_URI + 'admin/GetUserList?GroupId=' + this.SelectedGroup,
            {withCredentials: true})
        .then(response => this.UserInfo = response.data);
    }
}
</script>
