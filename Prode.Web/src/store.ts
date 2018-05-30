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
      Axios.post(process.env.VUE_APP_BASE_URI + 'me', { }, {withCredentials: true})
      .then(data => {
        context.commit('ChangeUser', data.data);
      })
      .catch();
    },
    LogoutUser(context) {
      Axios.post(process.env.VUE_APP_BASE_URI + 'logout?', {}, {withCredentials: true})
      .then(data => {
        context.commit('ChangeUser', undefined);
      });
    }
  },
  mutations: {
    ChangeUser(state, newstate: IUserInfo) {
      state.user = newstate;
    }
  },
  getters: {
    IsAdmin: state => {
      if (state.user !== undefined) {
        return state.user.isAdmin;
      } else {
        return false;
      }
    },
    UserName: state => {
      if (state.user !== undefined) {
        return state.user.name;
      }
      return '';
    },
    UserId: state => {
      if (state.user !== undefined) {
        return state.user.id;
      }
    },
    UserGroup: state => {
      if (state.user !== undefined) {
        return state.user.gameGroupId;
      }
    },
    UserTeamName: state => {
      if (state.user !== undefined) {
        return state.user.teamName;
      }
    },
    UserReceiveMails: state => {
      if (state.user !== undefined) {
        return state.user.receiveMails;
      }
    },
    UserReceiveAdminMails: state => {
      if (state.user !== undefined) {
        return state.user.receiveAdminMails;
      }
    }

  },
  strict: process.env.NODE_ENV !== 'production'
});
