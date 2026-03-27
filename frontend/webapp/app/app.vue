<template>
  <UApp :locale="zh_cn">
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </UApp>
</template>

<script setup lang="ts">
import { zh_cn } from '@nuxt/ui/locale'

const title = 'ツ箫声断丶何处莫凭栏 | 浅析'
const description = '这里是浅析的个人网页'
useHead({
  meta: [
    { name: 'viewport', content: 'width=device-width, initial-scale=1' }
  ],
  link: [
    { rel: 'icon', href: '/nilarea.ico' }
  ],
  htmlAttrs: {
    lang: 'zh-CN'
  }
})
useSeoMeta({
  title,
  description,
  ogTitle: title,
  ogDescription: description
})
</script>
