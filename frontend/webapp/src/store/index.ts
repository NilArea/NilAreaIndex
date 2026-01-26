// @ts-ignore
import { createStore, Store } from 'vuex';
import type { StoreState } from '../types';

const store: Store<StoreState> = createStore<StoreState>({
  state: {
    token: 1,
    user: {
      role: 'admin'
    }
  }
});

export default store;
