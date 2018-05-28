<template>
  <div>
      <b-navbar toggleable="md" type="dark" variant="info">
        <b-navbar-toggle target="nav_collapse"></b-navbar-toggle>
        <b-navbar-brand>Prode!!!! </b-navbar-brand>
        <b-navbar-brand v-if="ComputedUserName !== ''">Hola  {{ComputedUserName}}</b-navbar-brand>
        <b-collapse is-nav id="nav_collapse">
          <b-navbar-nav class="ml-auto">
            <b-nav-item to="/Posiciones" v-if="ComputedUserName !== ''">Posiciones</b-nav-item>
            <b-nav-item to="/" exact>Calendario</b-nav-item>
            <b-nav-item to="/Forecast" v-if="ComputedUserName !== ''">Pronosticos</b-nav-item>
            <b-nav-item to="/Rules">Reglas</b-nav-item>
            <b-nav-item to="/Login" v-if="ComputedUserName === ''">Entrar</b-nav-item>
            <b-nav-item to="/Logout" v-if="ComputedUserName !== ''">Salir</b-nav-item>
            <b-nav-item to="/Admin" v-if="ComputedIsAdmin">Administrar</b-nav-item>
          </b-navbar-nav>
          <!-- Right aligned nav items -->
        </b-collapse>
      </b-navbar>
    </div>
</template>

<script lang="ts">
import IConfig from '@/models/Config.ts';
import axios from 'axios';
import { Component, Prop, Vue } from 'vue-property-decorator';
import { IUserInfo, ProfileState  } from '../models/UserInfo';
import store from '../store';

const AppProps = Vue.extend({
  props: {
    config: Object as () => IConfig
    // config : {IConfig: {Object}}
  }
});

@Component
export default class Header extends AppProps {
  get ComputedIsAdmin(): boolean {
    return this.$store.getters.IsAdmin;
  }
  get ComputedUserName(): string {
    return this.$store.getters.UserName;
  }
}
</script>



