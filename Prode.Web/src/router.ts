import Vue from 'vue';
import Router from 'vue-router';
import Admin from './views/Admin.vue';
import Forecast from './views/Forecast.vue';
import Login from './views/Login.vue';
import Logout from './views/Logout.vue';
import Position from './views/Positions.vue';
import Rules from './views/Rules.vue';
import Schedule from './views/Schedule.vue';
import Users from './views/Users.vue';

Vue.use(Router);

export default new Router({
  routes: [
    {
      path: '/Posiciones',
      name: 'Posiciones',
      component: Position
    },
    {
      path: '/',
      name: 'Calendario',
      component: Schedule
    },
    {
      path: '/Forecast',
      name: 'Pronosticos',
      component: Forecast
    },
    {
      path: '/Users',
      name: 'Usuarios',
      component: Users
    },
    {
      path: '/Rules',
      name: 'Reglas',
      component: Rules
    },
    {
      path: '/Login',
      name: 'Logueo',
      component: Login
    },
    {
      path: '/Admin',
      name: 'Administrar',
      component: Admin
    },
    {
      path: '/Logout',
      name: 'Logout',
      component: Logout
    }
  ]
});
