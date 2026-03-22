<template>
  <div
    :class="backgroundClass"
    :style="backgroundStyle"
    class="min-h-screen"
  >
    <slot />
  </div>
</template>

<script setup>
const route = useRoute()
const backgroundClass = computed(() => {
  // 从路由 meta 获取背景类
  return route.meta.background || 'bg-white dark:bg-gray-950'
})

const backgroundStyle = computed(() => {
  // 如果路由定义了背景图片，使用 style
  if (route.meta.backgroundImage) {
    return {
      backgroundImage: `url(${route.meta.backgroundImage})`,
      backgroundSize: 'cover',
      backgroundPosition: 'center',
      backgroundAttachment: 'fixed'
    }
  }
  return {}
})
</script>
