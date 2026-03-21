import { createRouter, createWebHistory } from 'vue-router';
import { warn } from 'vue';

const pages = import.meta.glob('../views/**/page.js', {
  eager: true,
  import: 'default'
});
const components = import.meta.glob('../views/**/index.vue');
const routes = Object.entries(pages)
  .map(([filePath, meta]) => {
    // 转换页面路径为路由路径
    let routePath = filePath
        .replace(/^\.\.\/views/, '')  // 移除 ../views
        .replace(/\/page\.js$/, '')   // 移除 /page.js
        .replace(/\/index$/, '')      // 如果路径以index结尾，移除（但page.js不会在index目录？）
      || '/';                      // 空路径转为根路由

    // 确保以/开头
    if (!routePath.startsWith('/')) {
      routePath = '/' + routePath;
    }

    // 生成路由名称
    const routeName = routePath === '/'
      ? 'index'
      : routePath.slice(1).split('/').filter(Boolean).join('-');

    // 获取对应的组件
    const componentPath = filePath.replace(/page\.js$/, 'index.vue');
    const component = components[componentPath];

    if (!component) {
      warn(`[Route Generation] Missing component for ${filePath}. Expected at ${componentPath}`);
      return null; // 跳过该路由
    }

    return {
      path: routePath,
      name: routeName,
      component, // 懒加载函数
      meta: meta || {} // 确保meta是对象
    };
  })
  .filter(Boolean);


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
export default router;
