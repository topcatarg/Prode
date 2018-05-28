<template>
    <div class="container">
        <b-container class="bv-example-row">
            <b-row>
                <b-col>
                    <b-button variant="primary">
                        <div v-if="!ButtonOcupied">
                            Recalcular puntajes
                        </div>
                        <div v-else-if="ButtonOcupied">
                            <i class="fa fa-cog fa-spin fa-fw"></i>
                        </div>
                    </b-button>
                </b-col>
            </b-row>
        </b-container>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

@Component
export default class AdminMatchButtons extends Vue {
    private ButtonOcupied: boolean = false;
}
</script>
