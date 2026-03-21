<template>
  <UApp>
    <UHeader>
      <template #left>
        <NuxtLink to="/">
          <AppLogo class="w-auto h-6 shrink-0" />
        </NuxtLink>
      </template>

      <template #right>
        <NuxtLink
          to="/"
          class="text-sm hover:text-primary transition-colors block"
        >
          首页
        </NuxtLink>
        <NuxtLink
          to="/blog"
          class="text-sm hover:text-primary transition-colors block"
        >
          博客
        </NuxtLink>

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
    </UHeader>

    <UMain>
      <NuxtPage />
    </UMain>

    <USeparator icon="i-simple-icons-nuxtdotjs" />

    <UFooter>
      <template #top>
        <div class="w-full flex justify-center py-4">
          <div class="flex flex-col sm:flex-row gap-8 sm:gap-16">
            <!-- 左侧：联系方式 -->
            <div class="text-center space-y-3">
              <p class="text-sm font-medium text-gray-900 dark:text-white">
                与我联系
              </p>
              <div class="flex flex-col gap-2">
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
            </div>

            <!-- 右侧：版权与备案 -->
            <div class="text-center space-y-3">
              <p class="text-sm font-medium text-gray-900 dark:text-white">
                © 2026 浅析
              </p>
              <div class="flex flex-col gap-2">
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
          </div>
        </div>
      </template>
      <template #left>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          Built with Nuxt UI
        </p>
      </template>
      <template #right>
        <UButton
          to="https://github.com/NilArea/NilAreaIndex"
          target="_blank"
          icon="i-simple-icons-github"
          aria-label="GitHub"
          color="neutral"
          variant="ghost"
          size="sm"
        />
      </template>
    </UFooter>
  </UApp>
</template>

<script setup>
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

const title = 'ツ箫声断丶何处莫凭栏 | 浅析'
const description = '这里是浅析的个人网页'

useSeoMeta({
  title,
  description,
  ogTitle: title,
  ogDescription: description
})
</script>
