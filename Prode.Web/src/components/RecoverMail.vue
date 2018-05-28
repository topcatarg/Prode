<template>
    <div class="container">
        <b-form @submit="onSubmit" class="mt-4">
            <b-form-group
                horizontal
                label-cols="4"
                id="fieldset1"
                label="Direccion de correo">
                <b-form-input v-model.trim="UserMail" required type="email"/>
            </b-form-group>
            <b-button type="submit" variant="primary" class="mr-3">Recuperar password</b-button>
            <ErrorAlert :message=this.GeneralError class="mt-2"/>
        </b-form>
    </div>
</template>

<script lang="ts">
import Axios from 'axios';
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import ErrorAlert from '../components/ErrorAlert.vue';

@Component({
  components: {
    ErrorAlert
  }
})
export default class RecoverMail extends Vue {
    private GeneralError: string = '';
    private UserMail: string = '';

    private onSubmit() {
        this.GeneralError = '';
    }
}
</script>

