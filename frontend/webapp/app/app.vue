<template>
  <UApp>
    <NuxtLayout>
      <NilAreaHeader
        class="bg-default/60 backdrop-blur border-b/75 border-default/75"
      />
      <UMain>
        <NuxtPage />
      </UMain>
    </NuxtLayout>
    <NilAreaFooter
      v-if="!route.meta.hideFooter"
    />
  </UApp>
</template>

<script setup lang="ts">
import NilAreaFooter from '~/components/NilAreaFooter.vue'
import NilareaIco from '~~/public/nilarea.ico'

const title = 'ツ箫声断丶何处莫凭栏 | 浅析'
const description = '这里是浅析的个人网页'
useHead({
  meta: [
    { name: 'viewport', content: 'width=device-width, initial-scale=1' }
  ],
  link: [
    { rel: 'icon', href: NilareaIco }
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
const route = useRoute()
</script>
