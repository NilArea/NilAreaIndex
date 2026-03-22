<template>
  <UContainer>
    <div
      class="flex justify-center items-center"
      :style="{
        minHeight: 'calc(100vh - var(--ui-header-height) - 20px)'
      }"
    >
      <Transition
        mode="out-in"
        enter-active-class="transition duration-300 ease-out"
        enter-from-class="opacity-0 translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition duration-200 ease-in"
        leave-from-class="opacity-100 translate-y-0"
        leave-to-class="opacity-0 -translate-y-2"
        class="bg-default/85 backdrop-blur"
      >
        <NuxtPage />
      </Transition>
    </div>
  </UContainer>
</template>

<script setup lang="ts">
import CityBg from '~~/public/img/city-bg.png'

definePageMeta({
  backgroundImage: CityBg,
  hideFooter: true
})
</script>
