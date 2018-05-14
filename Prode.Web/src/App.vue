<template>
  <div id="app">
    <Header :config=this.config>
    </Header>
    what?
    <div id="nav">
      <router-link to="/">Home</router-link> |
      <router-link to="/about">About</router-link>
    </div>
    <router-view/>
  </div>
</template>

<script lang='ts'>

import Header from '@/components/Header.vue';
import IConfig from '@/models/Config.ts';
import IStats from '@/models/Stats.ts';
import IUserInfo from '@/models/UserInfo.ts';
import axios from 'axios';
import { Component, Prop, Vue } from 'vue-property-decorator';

@Component({
  components: {
    Header
  }
})
export default class App extends Vue {

  public user?: IUserInfo;
  public config?: IConfig;
  public stats?: IStats;

  private data() {
    return {
      // Will be reactive
      user: undefined,
      config: undefined,
      stats: undefined
    };
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
