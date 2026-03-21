<template>
  <UApp>
    <main>
      <router-view v-slot="{ Component }">
        <transition mode="out-in" name="fade">
          <component :is="Component" />
        </transition>
      </router-view>
    </main>
    <FooterComponent v-if="showFooter" />
  </UApp>
</template>

<script lang="ts" setup>
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import FooterComponent from './views/FooterComponent.vue';

const route = useRoute();

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
