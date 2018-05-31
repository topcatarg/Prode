<template>
    <div class="container">
        <b-container>
            <b-row>
                <b-col>
                    <b-button variant="primary" 
                    @click="UpdateScores()" 
                    :disabled="ButtonOcupied">
                        <div v-if="!ButtonOcupied">
                            Recalcular puntajes
                        </div>
                        <div v-else-if="ButtonOcupied">
                            <i class="fa fa-cog fa-spin fa-fw"></i>
                        </div>
                    </b-button>
                </b-col>
            </b-row>
            <b-row>
                <b-col>
                    <ErrorAlert :message=this.ErrorMessage class="mt-2"/>
                </b-col>
            </b-row>
        </b-container>
    </div>
</template>

<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue } from 'vue-property-decorator';
import ErrorAlert from '../components/ErrorAlert.vue';

@Component ({
    components: {
        ErrorAlert
    }
})
export default class AdminMatchButtons extends Vue {
    private ButtonOcupied: boolean = false;
    private ErrorMessage: string = 'inicio';

    private UpdateScores() {
        this.ErrorMessage = '';
        this.ButtonOcupied = true;
        Axios.post(process.env.VUE_APP_BASE_URI + 'admin/UpdatePoints',
        {},
        {withCredentials: true})
        .then(response => {
            this.ErrorMessage = 'Todo ok';
            this.ButtonOcupied = false;
        })
        .catch(error => {
            this.ErrorMessage = 'Ocurrio un error';
            this.ButtonOcupied = false;
        });
    }
}
</script>
