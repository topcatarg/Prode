<template>
    <div>
        <b-table striped hover :items="items">

        </b-table>
    </div>
</template>
<script lang="ts">
import Vue from 'vue';

interface ITableFields {
    key: string;
    sortable: boolean;
}

export default Vue.extend({

  name: 'Schedule'
});
</script>