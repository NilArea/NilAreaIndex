<template>
  <UHeader title="NilArea">
    <ClientOnly>
      <UNavigationMenu :items="items" />
    </ClientOnly>
    <template #body>
      <UNavigationMenu
        :items="items"
        orientation="vertical"
      />
      <USeparator class="my-6" />

      <UButton
        v-if="route.path.startsWith('/blog')"
        label="Sign in"
        to="/sign/login"
        color="neutral"
        variant="subtle"
        block
        class="mb-3"
      />
      <UButton
        v-if="route.path.startsWith('/blog')"
        label="Sign up"
        to="/sign/register"
        color="neutral"
        block
      />
    </template>
    <template #right>
      <UColorModeButton />
      <UButton
        to="https://github.com/LeeQianXi"
        target="_blank"
        icon="i-simple-icons-github"
        aria-label="GitHub"
        color="neutral"
        variant="ghost"
      />
      <UButton
        v-if="route.path.startsWith('/blog')"
        label="Sign in"
        to="/sign/login"
        color="neutral"
        variant="outline"
        class="hidden lg:inline-flex"
      />
      <UButton
        v-if="route.path.startsWith('/blog')"
        label="Sign up"
        to="/sign/register"
        color="neutral"
        trailing-icon="i-lucide-arrow-right"
        class="hidden lg:inline-flex"
      />
    </template>
  </UHeader>
</template>

<script setup lang="ts">
import type { NavigationMenuItem } from '@nuxt/ui'

const route = useRoute()
const items = computed<NavigationMenuItem[]>(() => [
  {
    label: 'Home',
    to: '/'
  },
  {
    label: 'Blog',
    to: '/blog'
  }
])
</script>
