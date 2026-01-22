import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';

import Home from '../views/Home.vue';

const defaultTitle = 'ツ箫声断丶何处莫凭栏 | 浅析';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/blog',
    name: 'Blog',
    component: () => import('../views/Blog.vue')
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
  // 滚动行为
  scrollBehavior(_to, _from, savedPosition) {
    if (savedPosition) {
      return savedPosition;
    } else {
      return { top: 0 };
    }
  }
});

router.beforeEach((to, _from, next) => {
  // 设置页面标题
  document.title = to.meta.title ? `${to.meta.title}` : defaultTitle;
  next();
});

export default router;
