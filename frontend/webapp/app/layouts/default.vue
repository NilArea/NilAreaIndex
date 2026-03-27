<template>
  <div
    :class="background.class"
    :style="background.style"
    class="min-h-screen"
  >
    <NilAreaHeader
      :class="sectionClass"
    />
    <UMain>
      <slot />
    </UMain>
    <NilAreaFooter
      v-if="!route.meta.hideFooter"
      :class="sectionClass"
    />
  </div>
</template>

<script setup>
import NilAreaFooter from '~/components/default/NilAreaFooter.vue'
import NilAreaHeader from '~/components/default/NilAreaHeader.vue'

const sectionClass = 'bg-default/75 backdrop-blur border-b/80 border-default/80'

const route = useRoute()
const background = computed(() => {
  // 背景类（默认白色/暗黑模式深色）
  const bgClass = route.meta.background || 'bg-white dark:bg-gray-950'

  // 背景样式（如果有背景图片）
  const bgStyle = route.meta.backgroundImage
    ? {
        backgroundImage: `url(${route.meta.backgroundImage})`,
        backgroundSize: 'cover',
        backgroundPosition: 'center',
        backgroundAttachment: 'fixed'
      }
    : {}

  return {
    class: bgClass,
    style: bgStyle
  }
})
</script>
