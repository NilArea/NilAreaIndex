<template>
  <USeparator
    label="NilArea"
  />
  <UFooter>
    <template #top>
      <UContainer
        fluid
        class="w-full gap-4"
      >
        <div class="flex justify-center gap-6 flex-wrap">
          <div class="flex flex-col items-center gap-2">
            <p class="text-sm font-medium w-auto">
              与我联系
            </p>
            <ULink
              to="mailto:qianxilee232@163.com"
              class="text-sm hover:text-primary transition-colors block"
            >
              我的邮箱
            </ULink>
            <ULink
              to="https://space.bilibili.com/569316920/"
              target="_blank"
              class="text-sm hover:text-primary transition-colors block"
            >
              我的 Bilibili 频道
            </ULink>
            <ULink
              to="https://ifdian.net/a/QianXi_Lee"
              target="_blank"
              class="text-sm hover:text-primary transition-colors block"
            >
              我的 爱发电
            </ULink>
          </div>
          <div class="flex flex-col items-center gap-2">
            <p class="text-sm font-medium">
              © 2026 浅析
            </p>
            <ULink
              class="text-sm hover:text-primary transition-colors block"
            >
              版权声明
            </ULink>
            <ULink
              target="_blank"
              class="text-sm hover:text-primary transition-colors block"
            >
              XICP备xxxxxxxx号-x
            </ULink>
            <ULink
              target="_blank"
              class="text-sm hover:text-primary transition-colors block"
            >
              X公网安备 xxxxxxxxxxxxx号
            </ULink>
          </div>
        </div>
      </UContainer>
    </template>
    <template #left>
      <p class="text-muted text-sm">
        Copyright © {{ new Date().getFullYear() }}
      </p>
    </template>
    <ClientOnly>
      <UNavigationMenu :items="items" />
    </ClientOnly>
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
    </template>
  </UFooter>
</template>

<script setup lang="ts">
import type { NavigationMenuItem } from '@nuxt/ui'

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
