import './assets/main.css';

import { createApp } from 'vue';
import router from './router';
import store from './store';
import App from './App.vue';
import ui from '@nuxt/ui/vue-plugin'

const app = createApp(App);
app.use(router);
app.use(store);
app.use(ui)
app.mount('#app');
