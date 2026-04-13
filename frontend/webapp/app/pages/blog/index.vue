<template>
  <UPage>
    <div class="py-10">
      <UContainer>
        <div class="text-center mb-12">
          <h1 class="text-4xl font-bold text-gray-900 mb-4">
            Blog
          </h1>
          <p class="text-lg text-gray-600 max-w-2xl mx-auto">
            Explore our latest articles and insights on technology, design, and development.
          </p>
          <div class="mt-8">
            <UForm>
              <UInput
                placeholder="Search articles..."
                class="w-full max-w-md mx-auto"
              />
              <template #append>
                <UButton color="primary">
                  <UIcon name="search" />
                </UButton>
              </template>
            </UForm>
          </div>
        </div>

        <div class="mb-8">
          <USelect
            v-model="selectedCategory"
            :options="categories"
            placeholder="Filter by category"
            class="w-full max-w-md"
            @update:model-value="handleCategoryChange"
          />
        </div>

        <div
          v-if="isLoading"
          class="flex justify-center items-center py-20"
        >
          <UProgress
            :size="40"
            :thickness="4"
          />
        </div>
        <div
          v-else
          class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8"
        >
          <BlogCard
            v-for="post in filteredPosts"
            :key="post.slug"
            :title="post.title"
            :description="post.description"
            :category="post.category"
            :date="post.date"
            :read-time="post.readTime"
            :author-name="post.authorName"
            :author-avatar="post.authorAvatar"
            :image-url="post.imageUrl"
            :slug="post.slug"
          />
        </div>

        <div class="mt-12">
          <UPagination
            v-model:page="page"
            class="w-full flex justify-center"
            color="primary"
            variant="outline"
            :total="100"
            @update:model-value="handlePageChange"
          />
        </div>
      </UContainer>
    </div>
  </UPage>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import BlogCard from '~/components/blog/BlogCard.vue'

const page = ref(1)
const selectedCategory = ref('')

// API data
const blogPosts = ref([] as {
  title: string
  description: string
  category: string
  date: string
  readTime: number
  authorName: string
  authorAvatar: string
  imageUrl: string
  slug: string
}[])

// Loading state
const isLoading = ref(true)

// Fetch blog posts from API
const fetchBlogPosts = async () => {
  try {
    isLoading.value = true
    // Replace with actual API call
    // const response = await fetch(`/api/articles?page=${page.value}&category=${selectedCategory.value}`)
    // const data = await response.json()

    // Simulate API response for now
    setTimeout(() => {
      blogPosts.value = [
        {
          title: 'Getting Started with Nuxt 3',
          description: 'Learn the basics of Nuxt 3 and how to build modern web applications with this comprehensive guide.',
          category: 'Technology',
          date: '2024-04-10',
          readTime: 8,
          authorName: 'John Doe',
          authorAvatar: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=professional%20male%20avatar%20portrait&image_size=square',
          imageUrl: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=nuxt%203%20web%20development%20coding&image_size=landscape_16_9',
          slug: 'getting-started-with-nuxt-3'
        },
        {
          title: 'The Future of AI in Web Development',
          description: 'Explore how artificial intelligence is transforming the way we build and maintain web applications.',
          category: 'AI',
          date: '2024-04-05',
          readTime: 10,
          authorName: 'Jane Smith',
          authorAvatar: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=professional%20female%20avatar%20portrait&image_size=square',
          imageUrl: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=ai%20web%20development%20future%20technology&image_size=landscape_16_9',
          slug: 'future-of-ai-in-web-development'
        },
        {
          title: 'Responsive Design Best Practices',
          description: 'Master the art of creating responsive websites that look great on all devices with these best practices.',
          category: 'Design',
          date: '2024-03-28',
          readTime: 6,
          authorName: 'Alex Johnson',
          authorAvatar: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=professional%20avatar%20portrait%20neutral&image_size=square',
          imageUrl: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=responsive%20web%20design%20mobile%20desktop&image_size=landscape_16_9',
          slug: 'responsive-design-best-practices'
        },
        {
          title: 'Performance Optimization Techniques',
          description: 'Learn how to optimize your web applications for speed and performance with these proven techniques.',
          category: 'Development',
          date: '2024-03-20',
          readTime: 9,
          authorName: 'Mike Chen',
          authorAvatar: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=professional%20asian%20male%20avatar%20portrait&image_size=square',
          imageUrl: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=web%20performance%20optimization%20speed&image_size=landscape_16_9',
          slug: 'performance-optimization-techniques'
        },
        {
          title: 'Getting Started with Tailwind CSS',
          description: 'A comprehensive guide to using Tailwind CSS for rapid UI development and styling.',
          category: 'CSS',
          date: '2024-03-15',
          readTime: 7,
          authorName: 'Sarah Lee',
          authorAvatar: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=professional%20asian%20female%20avatar%20portrait&image_size=square',
          imageUrl: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=tailwind%20css%20styling%20web%20development&image_size=landscape_16_9',
          slug: 'getting-started-with-tailwind-css'
        },
        {
          title: 'The Importance of Accessibility in Web Design',
          description: 'Understand why accessibility matters and how to create inclusive web experiences for all users.',
          category: 'Accessibility',
          date: '2024-03-10',
          readTime: 8,
          authorName: 'David Wilson',
          authorAvatar: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=professional%20male%20avatar%20portrait%20glasses&image_size=square',
          imageUrl: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=web%20accessibility%20inclusive%20design&image_size=landscape_16_9',
          slug: 'importance-of-accessibility-in-web-design'
        }
      ]
      isLoading.value = false
    }, 500)
  } catch (error) {
    console.error('Error fetching blog posts:', error)
    isLoading.value = false
  }
}

// Categories for filter
const categories = computed(() => {
  const uniqueCategories = [...new Set(blogPosts.value.map(post => post.category))]
  return uniqueCategories.map(category => ({ value: category, label: category }))
})

// Filtered posts based on selected category
const filteredPosts = computed(() => {
  if (!selectedCategory.value) {
    return blogPosts.value
  }
  return blogPosts.value.filter(post => post.category === selectedCategory.value)
})

// Fetch data on mount
onMounted(() => {
  fetchBlogPosts()
})

// Fetch data when category changes
const handleCategoryChange = () => {
  page.value = 1
  fetchBlogPosts()
}

// Fetch data when page changes
const handlePageChange = () => {
  fetchBlogPosts()
}
</script>
