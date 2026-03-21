<template>
  <UCard class="h-full hover:shadow-lg transition-shadow duration-300">
    <template #header v-if="cover">
      <div class="relative h-48 overflow-hidden rounded-t-lg -m-4 mb-4">
        <img
          :src="cover"
          :alt="title"
          class="w-full h-full object-cover transform hover:scale-105 transition-transform duration-500"
        />
      </div>
    </template>
    <div class="space-y-3">
      <div class="flex flex-wrap gap-2" v-if="tags?.length">
        <UBadge
          v-for="tag in tags"
          :key="tag"
          color="primary"
          variant="soft"
          size="sm"
        >
          {{ tag }}
        </UBadge>
      </div>
      <h3
        class="text-lg font-bold text-gray-900 dark:text-white line-clamp-2 hover:text-primary-500 transition-colors cursor-pointer"
        @click="handleClick"
      >
        {{ title }}
      </h3>

      <p class="text-gray-600 dark:text-gray-400 text-sm line-clamp-3 leading-relaxed">
        {{ excerpt }}
      </p>

      <div class="flex items-center justify-between pt-4 border-t border-gray-200 dark:border-gray-700">
        <div class="flex items-center gap-2 text-sm text-gray-500">
          <UIcon name="i-heroicons-calendar" class="w-4 h-4" />
          <span>{{ formatDate(date) }}</span>
        </div>
        <UButton
          color="primary"
          variant="ghost"
          size="sm"
          trailing-icon="i-heroicons-arrow-right"
          @click="handleClick"
        >
          阅读
        </UButton>
      </div>
    </div>
  </UCard>
</template>

<script lang="ts" setup>
import navigateTo from '@nuxt/ui'
const props = defineProps<{
  title: string
  excerpt: string
  date: string
  slug: string
  cover?: string
  tags?: string[]
}>()

// 添加类型注解（解决 TS7006）
const formatDate = (dateStr: string): string => {
  return new Date(dateStr).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

// 封装跳转逻辑
const handleClick = () => {
  await navigateTo(`/blog/${props.slug}`)
}
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
