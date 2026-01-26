export interface StoreState {
  token: string | null;
  user: UserStore;
}

export interface UserStore {
  userName: string;
  role: 'none' | 'user' | 'admin';
}
