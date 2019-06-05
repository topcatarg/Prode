<template>
    <div class="container" >
        <b-jumbotron class="mt-2" border-variant="dark" bg-variant="info">
            <template slot="header">
            Reglas
            </template>
            <p>
            El puntaje será el siguiente:   (Ejemplo basado en Argentina 2 – Colombia 0 )
            </p>
            <ul>
            <li>
            3 puntos por resultados exacto acertado  (Pronosticó Argentina 2 – Colombia 0)
            </li>
            <li>
            2 puntos por acertar el ganador y la diferencia (Pronosticó Argentina 3 -  Colombia 1)
            </li>
            <li>
            1 puntos por acertar el ganador  (Pronosticó Argentina 1 – Colombia 0)
            </li>
            <li>
                Para el caso de empates, se respetan los 3 puntos por resultado exacto, o 1 punto por pronosticar que empataron
            </li>
            <li>
                En el caso de alargue, vale el resultado de los 120 minutos.
            </li>
            </ul>

            <p>
                Se puede modificar cada partido hasta 5 minutos antes del comienzo
            </p>
            <p>
                Por defecto cada participante empieza con todos los partidos 0 - 0
            </p>
            <p>
                <br/>
                Al finalizar la copa america, se premiara de la siguiente forma:
                <ul>
                    <li>
                        70% del total al ganador
                    </li>
                    <li>
                        20% del total al segundo puesto
                    </li>
                    <li>
                        10% del total al tercer puesto
                    </li>
                </ul>
                El monto de inscripcion es a discutir ($100?) <br/>
                <b-button to="/Privacy"> Ver politica de privacidad </b-button>
            </p>
        </b-jumbotron>
    </div>
</template>
<script lang="ts">
import Vue from 'vue';
export default Vue.extend({
  name: 'Positions'
});
</script>