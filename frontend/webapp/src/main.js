import './assets/main.css';

import { createApp } from 'vue';
import router from './router';
import store from './store';
import App from './App.vue';
import ui from '@nuxt/ui/vue-plugin';

const defaultTitle = 'ツ箫声断丶何处莫凭栏 | 浅析';
router.beforeEach((to, _from, next) => {
  if (!to.meta.requireAdmin) {
    document.title = to.meta.title ? `${to.meta.title}` : defaultTitle;
    next();
    return;
  }
  const user = store.state.user;
  if (user && user.role === 'admin') {
    document.title = to.meta.title ? `${to.meta.title}` : defaultTitle;
    next();
  } else
    next({ name: _from.name });

});

const app = createApp(App);
app.use(router);
app.use(store);
app.use(ui);
app.mount('#app');
