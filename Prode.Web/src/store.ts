import Axios from 'axios';
import Vue from 'vue';
import Vuex from 'vuex';

import { IUserInfo, ProfileState  } from './models/UserInfo';

Vue.use(Vuex);

export const OriginalState: ProfileState = {
  user: undefined
};

export default new Vuex.Store<ProfileState> ({
  state: OriginalState,
  actions: {
    GetUserData(context) {
      Axios.post(process.env.VUE_APP_BASE_URI + 'me')
      .then(data => context.commit('ChangeUser', this.data))
      .catch();
    }
  },
  mutations: {
    ChangeUser(state, newstate: IUserInfo) {
      state.user = newstate;
    }
  },
  strict: process.env.NODE_ENV !== 'production'
});
