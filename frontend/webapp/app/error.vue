<template>
  <UApp>
    <NilAreaHeader />

    <UMain>
      <NuxtLayout>
        <UError
          :clear="{
            color: 'neutral',
            size: 'xl',
            icon: 'i-lucide-arrow-left',
            class: 'rounded-full'
          }"
          :error="{
            statusCode: 404,
            statusMessage: 'Page not found',
            message: 'The page you are looking for does not exist.'
          }"
        />
      </NuxtLayout>
    </UMain>

    <USeparator label="NilArea" />
    <NilAreaFooter />
  </UApp>
</template>

<script setup lang="ts">
import NilAreaFooter from '~/components/NilAreaFooter.vue'
</script>
