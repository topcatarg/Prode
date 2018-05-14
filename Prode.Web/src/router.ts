import Vue from 'vue';
import Router from 'vue-router';
import About from './views/About.vue';
import Home from './views/Home.vue';
import Main from './views/Main.vue';
import Suggestion from './views/Suggestion.vue';
import Users from './views/Users.vue';

Vue.use(Router);

export default new Router({
  routes: [
    {
      path: '/',
      name: 'Main',
      component: Main
    },
    {
      path: '/Users',
      name: 'Users',
      component: Users
    },
    {
      path: '/Suggestions',
      name: 'Suggestions',
      component: Suggestion
    }
  ]
});
