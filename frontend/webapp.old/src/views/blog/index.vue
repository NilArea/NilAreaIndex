<template>
  <div>
    <div
      id="nav-bar"
      ref="navBarRef"
      :class="{ sticky: isNavSticky }"
    >
      <span id="nav-logo-container">
        <span id="nav-logo-main"></span>
      </span>
      <div id="nav-link-container">
        <router-link class="nav-link" to="/">首页</router-link>
        <router-link class="nav-link" to="/blog">博客</router-link>
        <account-icon-component v-if="isLogedIn" />
        <router-link v-else class="nav-link" to="/sign">登录</router-link>
      </div>
    </div>
    <div
      id="top-bg"
      :class="{ loaded: isBgLoaded}"
    ></div>
    <div id="spacer"></div>
    <div
      id="graph-1"
      ref="navTriggerRef"
    >
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <h3 class="text-center mb-12">最新文章</h3>
        <!-- 加载中 -->
        <div v-if="pending" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <USkeleton v-for="i in 6" :key="i" class="h-80" />
        </div>
        <!-- 博客卡片网格 -->
        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 text-left">
          <BlogItemComponent
            v-for="post in blogPosts"
            :key="post.slug"
            :title="post.title"
            :excerpt="post.excerpt"
            :date="post.date"
            :slug="post.slug"
            :cover="post.cover"
            :tags="post.tags"
          />
        </div>
        <!-- 加载更多 -->
        <div class="mt-12 text-center">
          <UButton
            v-if="hasMore"
            color="gray"
            variant="soft"
            size="lg"
            :loading="loadingMore"
            @click="loadMore"
          >
            加载更多
          </UButton>
        </div>
      </div>
    </div>
    <div id="graph-2">
      <div class="max-w-7xl mx-auto px-4 text-center">
        <h3>关于</h3>
        <div class="about-content max-w-2xl mx-auto leading-relaxed">
          这里是关于部分的描述内容...
        </div>
      </div>
    </div>
    <div id="graph-3">
      <div class="max-w-7xl mx-auto px-4 text-center">
        <h3>标签</h3>
        <div class="tags-container">
          <span v-for="tag in ['Vue', 'Nuxt', 'TypeScript', 'CSS']" :key="tag">
            {{ tag }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import useFetch from '@nuxt/ui';
import { onMounted, onUnmounted, ref } from 'vue';
import BlogItemComponent from './BlogItemComponent.vue';
import AccountIconComponent from '../AccountIconComponent.vue';

// 导航相关状态
const isNavSticky = ref(false);
const isBgLoaded = ref(false);
const scrolling = ref(false);
const isLogedIn = ref(false);

const navBarRef = ref<HTMLElement | null>(null);
const navTriggerRef = ref<HTMLElement | null>(null);
// 博客数据
const page = ref(1);
const hasMore = ref(true);
const loadingMore = ref(false);

const { data: blogPosts, pending } = await useFetch('/api/posts', {
  default: () => ([
    {
      slug: 'nuxt-3-guide',
      title: 'Nuxt 3 完全指南',
      excerpt: '深入了解 Nuxt 3 的自动导入、渲染模式和服务器引擎...',
      date: '2026-03-20',
      cover: '/images/blog-1.jpg',
      tags: ['Nuxt', 'Vue']
    },
    {
      slug: 'typescript-tips',
      title: 'TypeScript 高级技巧',
      excerpt: '类型推断、泛型和类型守卫的最佳实践...',
      date: '2026-03-18',
      cover: '/images/blog-2.jpg',
      tags: ['TypeScript']
    },
    {
      slug: 'css-modern',
      title: '现代 CSS 特性一览',
      excerpt: '探索 Container Queries、:has() 选择器等新特性...',
      date: '2026-03-15',
      tags: ['CSS']
    }
  ])
});

// 滚动检测逻辑
const checkSticky = () => {
  if (!navTriggerRef.value) return;
  const targetRect = navTriggerRef.value.getBoundingClientRect();
  isNavSticky.value = targetRect.top <= 0;
  scrolling.value = false;
};

const handleScroll = () => {
  if (!scrolling.value) {
    scrolling.value = true;
    requestAnimationFrame(checkSticky);
  }
};

const init = () => {
  setTimeout(() => {
    isBgLoaded.value = true;
  }, 100);
  checkSticky();
};

const loadMore = async () => {
  loadingMore.value = true;
  page.value++;
  // 这里添加实际的数据获取逻辑
  await new Promise(resolve => setTimeout(resolve, 1000));
  loadingMore.value = false;
};

onMounted(() => {
  init();
  window.addEventListener('scroll', handleScroll, { passive: true });
});

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll);
});
</script>

<style scoped>
h1 {
  margin-bottom: 20px;
  font-weight: 400;
  font-size: 76px;
  color: #fff
}

h2 {
  margin-top: 25px;
  font-weight: 400;
  font-size: 15px;
  color: #fff
}

h2 span {
  color: rgba(255, 255, 255, .6)
}

h3 {
  margin-bottom: 20px;
  font-size: 30px;
  color: var(--txt-b-pure);
  transition: .25s
}

h4 {
  color: var(--txt-b-pure);
  font-size: 28px
}

h5 {
  color: var(--txt-b-pure);
  font-size: 24px;
  text-align: center
}

:root {
  --nav-height: 90px;
}

#nav-bar {
  position: absolute;
  top: 0;
  width: 100%;
  height: var(--nav-height);
  transition: .25s
}

#nav-bar.sticky {
  z-index: 99;
  position: fixed;
  --nav-height: 82px;
  background-color: var(--bg-w-pure);
  box-shadow: rgba(0, 0, 0, .1) 0 5px 20px
}

#nav-logo-container {
  position: absolute;
  top: 20px;
  left: 200px;
  font-size: 0;
  transition: .25s
}

#nav-logo-main, #nav-logo-text {
  display: inline-block;
  height: 50px;
  background-repeat: no-repeat;
  background-size: contain;
  background-position: left center;
  transition: .25s
}

#nav-logo-main {
  width: 50px;
  background-image: url(/images/nilarea.png);
}

#nav-logo-text {
  width: 100px;
}

#nav-bar.sticky #nav-logo-container {
  top: 16px
}

#nav-link-container {
  position: absolute;
  right: 170px;
  top: 30px;
  white-space: nowrap;
  transition: .25s
}

.nav-link {
  position: relative;
  cursor: pointer;
  transition: .25s
}

#nav-bar .nav-link {
  margin: 0 10px;
  padding: 10px 20px;
  border-radius: 5px;
  color: #fff;
  font-size: 15px;
}

#nav-bar .nav-link:before {
  position: absolute;
  left: 20px;
  bottom: 3px;
  width: calc(100% - 40px);
  height: 3px;
  content: '';
  background: var(--theme-color);
  border-radius: 2px;
  transition: .25s;
  transform: scale(0);
  opacity: 0
}


#nav-bar .nav-link:hover {
  background-color: var(--b-alpha-5)
}

#nav-bar .nav-link:hover:before {
  transform: scale(1);
  opacity: 1
}

#nav-bar .nav-link:active, #nav-bar .nav-link:active:before {
  opacity: .6
}

#nav-bar.sticky .nav-link {
  color: var(--txt-b-pure)
}

@media screen and (max-width: 1300px) {
  #nav-logo-container {
    left: 100px
  }

  #nav-link-container {
    right: 70px
  }
}

@media screen and (max-width: 900px) {
  #nav-logo-container {
    left: 70px
  }

  #nav-link-container {
    right: 40px
  }
}

@media screen and (max-width: 760px) {
  #nav-logo-container {
    left: 25px
  }

  #nav-link-container {
    display: none
  }
}

#top-bg {
  z-index: -100;
  position: fixed;
  width: 100%;
  height: 100%;
  background-repeat: no-repeat;
  background-size: cover;
  background-position: center;
  background-image: url(/images/fufu-bg.png);
  transition: transform 1.5s, opacity 1s;
  transform: scale(1.05);
  opacity: 0
}

#top-bg.loaded {
  transform: scale(1);
  opacity: 1
}

#headline-container {
  position: absolute;
  top: 30%;
  width: 100%;
  text-align: center;
  font-size: 0;
}

.heading-underline {
  display: inline-block;
  width: 70px;
  height: 4px;
  border-radius: 2px;
  background-color: var(--theme-color);
}

.heading-underline-1 {
  display: inline-block;
  width: 50px;
  height: 3px;
  border-radius: 2px;
  background-color: var(--theme-color);
}

#spacer {
  height: 30vh;
}

#graph-1, #graph-2, #graph-3 {
  position: relative;
  top: 100%;
  width: 100%;
  padding-top: 80px;
  padding-bottom: 100px;
  font-size: 0;
  text-align: center;
  transition: .25s
}

#graph-1, #graph-3 {
  background-color: var(--bg-w-pure)
}

#graph-1, #graph-3 {
  background-color: var(--bg-w-245)
}

:deep(.blog-card) {
  background: var(--bg-w-pure);
  border-radius: 8px;
  overflow: hidden;
  transition: transform 0.25s, box-shadow 0.25s;
}

:deep(.blog-card:hover) {
  transform: translateY(-4px);
  box-shadow: 0 10px 30px rgba(0,0,0,0.1);
}

.tags-container {
  margin-top: 25px;
  font-size: small
}

.tags-container span {
  display: inline-block;
  margin: 3px;
  padding: 8px 15px;
  border-radius: 5px;
  background-color: var(--b-alpha-5);
  color: var(--b-alpha-60);
  transition: .25s
}

.tags-container span:hover {
  background-color: var(--b-alpha-10);
  color: var(--txt-b-pure);
  -webkit-box-shadow: rgba(0, 0, 0, .1) 0 5px 10px;
  box-shadow: rgba(0, 0, 0, .1) 0 5px 10px
}

.about-content {
  position: relative;
  margin-top: 25px;
  width: 100%;
  color: var(--b-alpha-80);
  font-size: 14px
}
</style>
