<template>
  <div id="app-container">
    <main>
      <router-view v-slot="{ Component }">
        <transition mode="out-in" name="fade">
          <component :is="Component" />
        </transition>
      </router-view>
    </main>
    <FooterComponent v-if="showFooter" />
  </div>
</template>

<script lang="ts" setup>
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import FooterComponent from './components/FooterComponent.vue';

const route = useRoute();

// 根据路由决定是否显示导航和页脚
//const showNavigation = computed(() => !route.meta.hideNavigation)
const showFooter = computed(() => !route.meta.hideFooter);
//const isAuthenticated = computed(() => localStorage.getItem('token') !== null)
</script>

<style scoped>
#app-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}
</style>
