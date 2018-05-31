<template>
    <div>
        <b-alert :show="dismissCountDown"
             dismissible
             variant="danger"
             @dismissed="dismissCountDown=0"
             @dismiss-count-down="countDownChanged">
            <p>{{this.TextToShow}}</p>
            <b-progress variant="danger"
                  :max="dismissSecs"
                  :value="dismissCountDown"
                  height="4px">
            </b-progress>
        </b-alert>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';

const AppProps = Vue.extend({
  props: {
    message: String
  }
});

@Component
export default class ErrorAlert extends AppProps {
    private dismissSecs: number = 5;
    private dismissCountDown: number = 0;
    private TextToShow: string = '';

    private countDownChanged(dismissCountDown: number) {
      this.dismissCountDown = dismissCountDown;
    }

    @Watch('message')
    private MostrarError(newvalue: string, oldvalue: string) {
        if (newvalue.length !== 0) {
            this.TextToShow = newvalue;
            this.dismissCountDown = this.dismissSecs;
        }
    }

}
</script>

