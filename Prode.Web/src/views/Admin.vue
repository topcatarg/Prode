<template>
    <div class="container">
        admin
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

@Component
export default class Admin extends Vue {
    private mounted() {
        if (this.$store.state === undefined) {
            this.$router.push('/');
            return;
        }
        if (this.$store.state.user === undefined) {
            this.$router.push('/');
            return;
        }
        if (!this.$store.state.user.IsAdmin) {
            this.$router.push('/');
            return;
        }
    }
}
</script>
