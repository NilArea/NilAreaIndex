import './assets/main.css';

import { createApp } from 'vue';
import router from './router';
import store from './store';
import App from './App.vue';
import ui from '@nuxt/ui/vue-plugin';

const defaultTitle = 'ツ箫声断丶何处莫凭栏 | 浅析';
router.addRoute({
  path: '/*',
  redirect: '/error',
})
router.beforeEach((to, _from, next) => {
  document.title = to.meta.title ? `${to.meta.title}` : defaultTitle;
  if (to.meta.requireAdmin) {
    const user = store.state.user;
    if (user && user.role === 'admin') {
      next();
    } else{
      next('/error');
    }
  }
  if (to.matched.length === 0 && to.path !== '/error') {
    next('/error');
  } else {
    next();
  }
});



const app = createApp(App);
app.use(router);
app.use(store);
app.use(ui);
app.mount('#app');
