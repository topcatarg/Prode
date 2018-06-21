<template>
  <div id="app">
    <Header :config=this.config>
    </Header>
    <router-view/>
  </div>
</template>

<script lang='ts'>

import Header from '@/components/Header.vue';
import IStats from '@/models/Stats.ts';
import Axios from 'axios';
import { Component, Prop, Vue } from 'vue-property-decorator';
import store from './store';

@Component({
  components: {
    Header
  }
})
export default class App extends Vue {

  public stats?: IStats;

  private data() {
    return {
      // Will be reactive
      user: undefined,
      config: undefined,
      stats: undefined
    };
  }

  private mounted() {
    store.dispatch('GetUserData');
  }

}
</script>

<style>
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
#nav {
  padding: 30px;
}

#nav a {
  font-weight: bold;
  color: #2c3e50;
}

#nav a.router-link-exact-active {
  color: #42b983;
}
</style>
