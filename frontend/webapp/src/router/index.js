import { createRouter, createWebHistory } from 'vue-router';

import Home from '../views/Home.vue';
import Blog from '../views/Blog.vue';

const defaultTitle = 'ツ箫声断丶何处莫凭栏 | 浅析';

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/blog',
    name: 'Blog',
    component: Blog
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
  // 滚动行为
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition;
    } else {
      return { top: 0 };
    }
  }
});

router.beforeEach((to, from, next) => {
  // 设置页面标题
  document.title = to.meta.title ? `${to.meta.title}` : defaultTitle;
  next();
});

function isAuthenticated() {
  // 这里实现你的认证检查逻辑
  return localStorage.getItem('token') !== null;
}

export default router;
