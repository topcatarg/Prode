<template>
  <div class="home">
        <Header/>
    <img src="../assets/logo.png">
    <HelloWorld msg="Welcome to Your Vue.js + TypeScript App"/>
  </div>
</template>

<script lang="ts">
import Header from '@/components/Header.vue';
import HelloWorld from '@/components/HelloWorld.vue'; // @ is an alias to /src
import { Component, Vue } from 'vue-property-decorator';

@Component({
  components: {
    HelloWorld,
    Header
  }
})
export default class Home extends Vue {}
</script>
