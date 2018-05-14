<template>
  <div>
      <b-navbar toggleable="md" type="dark" variant="info">
        <b-navbar-toggle target="nav_collapse"></b-navbar-toggle>
        <b-navbar-brand>Prode!!!! </b-navbar-brand>
        <b-collapse is-nav id="nav_collapse">
          <b-navbar-nav class="ml-auto">
            <b-nav-item to="/">Posiciones</b-nav-item>
            <b-nav-item to="/Schedule">Calendario</b-nav-item>
            <b-nav-item to="/Forecast">Pronosticos</b-nav-item>
            <b-nav-item to="/Users">Usuarios</b-nav-item>
            <b-nav-item to="/Rules">Reglas</b-nav-item>
            <b-nav-item to="/Login">Entrar</b-nav-item>
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

const AppProps = Vue.extend({
  props: {
    config: Object as () => IConfig
    // config : {IConfig: {Object}}
  }
});

@Component
export default class Header extends AppProps {}
</script>



