import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';
import store from '../store';

import Home from '../views/Home.vue';
import type { UserStore } from '../types';

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
  },
  {
    path: '/controller',
    name: 'Controller',
    component: () => import('../views/ControlPanel.vue'),
    meta: {
      title: '控制面板',
      hideFooter: true,
      requireAdmin: true
    }
  },
  {
    path: '/sign',
    name: 'Sing',
    component: () => import('../views/Sign.vue'),
    meta: {
      title: '登录面板',
      hideFooter: true
    }
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
  if (!to.meta.requireAdmin) {
    document.title = to.meta.title ? `${to.meta.title}` : defaultTitle;
    next();
    return;
  }
  const user: UserStore = store.state.user;
  if (user && user.role === 'admin') {
    document.title = to.meta.title ? `${to.meta.title}` : defaultTitle;
    next();
  } else
    next({ name: 'Blog' });
});

export default router;
