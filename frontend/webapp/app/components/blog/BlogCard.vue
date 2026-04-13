<template>
  <UCard class="overflow-hidden hover:shadow-lg transition-shadow duration-300">
    <div class="relative h-48 overflow-hidden">
      <img
        :src="imageUrl"
        alt="Blog post thumbnail"
        class="w-full h-full object-cover"
      >
      <UBadge
        class="absolute top-2 left-2"
        variant="solid"
        color="primary"
      >
        {{ category }}
      </UBadge>
    </div>
    <div class="p-6">
      <div class="flex items-center text-sm text-gray-500 mb-2">
        <span>{{ date }}</span>
        <span class="mx-2">•</span>
        <span>{{ readTime }} min read</span>
      </div>
      <h3 class="text-xl font-bold text-gray-900 mb-3 hover:text-primary transition-colors duration-200">
        {{ title }}
      </h3>
      <p class="text-gray-600 mb-4 line-clamp-2">
        {{ description }}
      </p>
      <div class="flex items-center justify-between">
        <div class="flex items-center">
          <UAvatar
            :src="authorAvatar"
            :alt="authorName"
            class="w-8 h-8 mr-2"
          />
          <span class="text-sm font-medium text-gray-700">{{ authorName }}</span>
        </div>
        <router-link
          :to="`/blog/${slug}`"
          class="text-primary font-medium hover:underline"
        >
          Read more
        </router-link>
      </div>
    </div>
  </UCard>
</template>

<script setup lang="ts">
defineProps({
  title: {
    type: String,
    required: true
  },
  description: {
    type: String,
    required: true
  },
  category: {
    type: String,
    default: 'Technology'
  },
  date: {
    type: String,
    default: new Date().toLocaleDateString()
  },
  readTime: {
    type: Number,
    default: 5
  },
  authorName: {
    type: String,
    default: 'Admin'
  },
  authorAvatar: {
    type: String,
    default: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=professional%20avatar%20portrait&image_size=square'
  },
  imageUrl: {
    type: String,
    default: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=blog%20post%20thumbnail%20technology&image_size=landscape_16_9'
  },
  slug: {
    type: String,
    required: true
  }
})
</script>
