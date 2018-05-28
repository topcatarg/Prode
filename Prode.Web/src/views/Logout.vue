<template>
    <div class="container"/>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

@Component
export default class Logout extends Vue {
  private mounted() {
    this.$store.dispatch('LogoutUser');
    this.$router.push({ path: '/' });
  }
}
</script>
