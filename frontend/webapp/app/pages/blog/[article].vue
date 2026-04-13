<template>
  <UPage>
    <div class="py-8">
      <!-- Loading State -->
      <div
        v-if="isLoading"
        class="flex justify-center items-center py-32"
      >
        <UProgress
          :size="60"
          :thickness="4"
        />
      </div>

      <!-- Article Content -->
      <template v-else>
        <!-- Article Header -->
        <div class="bg-gray-50 py-12 mb-8">
          <UContainer>
            <div class="max-w-3xl mx-auto">
              <div class="flex items-center text-sm text-gray-500 mb-4">
                <UBadge
                  variant="outline"
                  color="primary"
                  class="mr-3"
                >
                  {{ articleData.category }}
                </UBadge>
                <span class="mx-3">•</span>
                <span>{{ articleData.date }}</span>
                <span class="mx-3">•</span>
                <span>{{ articleData.readTime }} min read</span>
              </div>
              <h1 class="text-4xl md:text-5xl font-bold text-gray-900 mb-6">
                {{ articleData.title }}
              </h1>
              <div class="flex items-center">
                <UAvatar
                  :src="articleData.authorAvatar"
                  :alt="articleData.authorName"
                  class="w-12 h-12 mr-4"
                />
                <div>
                  <h3 class="font-semibold text-gray-900">
                    {{ articleData.authorName }}
                  </h3>
                  <p class="text-sm text-gray-500">
                    {{ articleData.authorBio }}
                  </p>
                </div>
              </div>
            </div>
          </UContainer>
        </div>

        <!-- Article Content -->
        <UContainer>
          <div class="grid grid-cols-1 lg:grid-cols-12 gap-12">
            <!-- Main Content -->
            <div class="lg:col-span-8">
              <div class="prose prose-lg max-w-none">
                <!-- Featured Image -->
                <div class="mb-8">
                  <img
                    :src="articleData.imageUrl"
                    alt="Article featured image"
                    class="w-full h-96 object-cover rounded-lg shadow-lg"
                  >
                </div>

                <!-- Article Body -->
                <div
                  class="prose prose-lg max-w-none"
                  v-html="articleData.content"
                />

                <!-- Tags -->
                <div class="mt-10 pt-6 border-t border-gray-200">
                  <h3 class="text-sm font-semibold text-gray-500 uppercase mb-3">
                    Tags
                  </h3>
                  <div class="flex flex-wrap gap-2">
                    <UBadge
                      v-for="tag in articleData.tags"
                      :key="tag"
                      variant="outline"
                      color="gray"
                    >
                      {{ tag }}
                    </UBadge>
                  </div>
                </div>

                <!-- Author Card -->
                <UCard class="mt-10">
                  <div class="flex flex-col md:flex-row items-center p-6">
                    <UAvatar
                      :src="articleData.authorAvatar"
                      :alt="articleData.authorName"
                      class="w-20 h-20 mr-6 mb-4 md:mb-0"
                    />
                    <div class="flex-1">
                      <h3 class="text-xl font-bold text-gray-900 mb-2">
                        {{ articleData.authorName }}
                      </h3>
                      <p class="text-gray-600 mb-4">
                        {{ articleData.authorBio }}
                      </p>
                      <div class="flex space-x-4">
                        <UButton
                          variant="ghost"
                          size="sm"
                          class="text-gray-600 hover:text-primary"
                        >
                          <UIcon
                            name="twitter"
                            class="w-5 h-5"
                          />
                        </UButton>
                        <UButton
                          variant="ghost"
                          size="sm"
                          class="text-gray-600 hover:text-primary"
                        >
                          <UIcon
                            name="github"
                            class="w-5 h-5"
                          />
                        </UButton>
                        <UButton
                          variant="ghost"
                          size="sm"
                          class="text-gray-600 hover:text-primary"
                        >
                          <UIcon
                            name="linkedin"
                            class="w-5 h-5"
                          />
                        </UButton>
                      </div>
                    </div>
                  </div>
                </UCard>

                <!-- Comments Section -->
                <div class="mt-10">
                  <h2 class="text-2xl font-bold text-gray-900 mb-6">
                    Comments ({{ articleData.comments.length }})
                  </h2>

                  <!-- Comment Form -->
                  <UCard class="mb-8">
                    <div class="p-6">
                      <h3 class="text-lg font-semibold text-gray-900 mb-4">
                        Leave a comment
                      </h3>
                      <form>
                        <div class="mb-4">
                          <UTextarea
                            placeholder="Write your comment here..."
                            rows="4"
                            class="w-full"
                          />
                        </div>
                        <UButton
                          type="submit"
                          color="primary"
                        >
                          Post Comment
                        </UButton>
                      </form>
                    </div>
                  </UCard>

                  <!-- Comments List -->
                  <div class="space-y-6">
                    <UCard
                      v-for="comment in articleData.comments"
                      :key="comment.id"
                    >
                      <div class="p-4">
                        <div class="flex items-start">
                          <UAvatar
                            :src="comment.avatar"
                            :alt="comment.name"
                            class="w-10 h-10 mr-4"
                          />
                          <div class="flex-1">
                            <div class="flex items-center justify-between mb-2">
                              <h4 class="font-semibold text-gray-900">
                                {{ comment.name }}
                              </h4>
                              <span class="text-sm text-gray-500">{{ comment.date }}</span>
                            </div>
                            <p class="text-gray-600">
                              {{ comment.content }}
                            </p>
                          </div>
                        </div>
                      </div>
                    </UCard>
                  </div>
                </div>
              </div>
            </div>

            <!-- Sidebar -->
            <div class="lg:col-span-4">
              <!-- Related Articles -->
              <div class="mb-10">
                <h2 class="text-xl font-bold text-gray-900 mb-6">
                  Related Articles
                </h2>
                <div class="space-y-6">
                  <UCard
                    v-for="relatedPost in relatedArticles"
                    :key="relatedPost.slug"
                    variant="outline"
                  >
                    <div class="flex space-x-4 p-4">
                      <img
                        :src="relatedPost.imageUrl"
                        alt="Related article thumbnail"
                        class="w-24 h-24 object-cover rounded-lg shrink-0"
                      >
                      <div class="flex-1">
                        <h3 class="font-semibold text-gray-900 hover:text-primary transition-colors mb-1">
                          <router-link :to="`/blog/${relatedPost.slug}`">{{ relatedPost.title }}</router-link>
                        </h3>
                        <p class="text-sm text-gray-500">
                          {{ relatedPost.date }}
                        </p>
                      </div>
                    </div>
                  </UCard>
                </div>
              </div>

              <!-- Categories -->
              <div class="mb-10">
                <h2 class="text-xl font-bold text-gray-900 mb-4">
                  Categories
                </h2>
                <UList>
                  <UListItem
                    v-for="category in categories"
                    :key="category.name"
                    class="py-2"
                  >
                    <template #default>
                      <a
                        href="#"
                        class="flex items-center justify-between w-full text-gray-700 hover:text-primary transition-colors"
                      >
                        <span>{{ category.name }}</span>
                        <UBadge
                          variant="outline"
                          color="gray"
                          class="ml-2"
                        >
                          {{ category.count }}
                        </UBadge>
                      </a>
                    </template>
                  </UListItem>
                </UList>
              </div>

              <!-- Popular Tags -->
              <div>
                <h2 class="text-xl font-bold text-gray-900 mb-4">
                  Popular Tags
                </h2>
                <div class="flex flex-wrap gap-2">
                  <UBadge
                    v-for="tag in popularTags"
                    :key="tag"
                    variant="outline"
                    color="gray"
                    class="hover:bg-primary hover:text-white hover:border-primary transition-colors"
                  >
                    {{ tag }}
                  </UBadge>
                </div>
              </div>
            </div>
          </div>
        </UContainer>
      </template>
    </div>
  </UPage>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const articleSlug = route.params.article as string

// API data
const articleData = ref({
  title: '',
  description: '',
  category: '',
  date: '',
  readTime: 0,
  authorName: '',
  authorAvatar: '',
  authorBio: '',
  imageUrl: '',
  content: '',
  tags: [] as string[],
  comments: [] as {
    id: number
    name: string
    avatar: string
    date: string
    content: string
  }[]
})

// Related articles
const relatedArticles = ref([] as {
  title: string
  date: string
  imageUrl: string
  slug: string
}[])

// Categories
const categories = ref([] as {
  name: string
  count: number
}[])

// Popular tags
const popularTags = ref([] as string[])

// Loading state
const isLoading = ref(true)

// Fetch article data from API
const fetchArticleData = async () => {
  try {
    isLoading.value = true
    // Replace with actual API call
    // const response = await fetch(`/api/articles/${articleSlug}`)
    // const data = await response.json()

    // Simulate API response for now
    setTimeout(() => {
      articleData.value = {
        title: 'Getting Started with Nuxt 3',
        description: 'Learn the basics of Nuxt 3 and how to build modern web applications with this comprehensive guide.',
        category: 'Technology',
        date: '2024-04-10',
        readTime: 8,
        authorName: 'John Doe',
        authorAvatar: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=professional%20male%20avatar%20portrait&image_size=square',
        authorBio: 'Full-stack developer with over 10 years of experience building modern web applications.',
        imageUrl: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=nuxt%203%20web%20development%20coding&image_size=landscape_16_9',
        content: `
          <p class="mb-6">Nuxt 3 is the latest version of the popular Vue.js framework that provides a powerful and intuitive way to build modern web applications. In this guide, we'll cover the basics of Nuxt 3 and show you how to get started with your first project.</p>

          <h2 class="text-2xl font-bold mb-4">What is Nuxt 3?</h2>
          <p class="mb-6">Nuxt 3 is a full-stack framework for building modern web applications. It's built on top of Vue 3 and provides a number of features that make it easier to build and deploy high-quality web applications.</p>

          <h2 class="text-2xl font-bold mb-4">Key Features</h2>
          <ul class="list-disc pl-6 mb-6 space-y-2">
            <li>Server-side rendering (SSR)</li>
            <li>Static site generation (SSG)</li>
            <li>Client-side routing</li>
            <li>Automatic code splitting</li>
            <li>Built-in state management</li>
            <li>Module system for extending functionality</li>
          </ul>

          <h2 class="text-2xl font-bold mb-4">Getting Started</h2>
          <p class="mb-4">To get started with Nuxt 3, you'll need to have Node.js installed on your machine. Once you have Node.js installed, you can create a new Nuxt 3 project using the following command:</p>

          <pre class="bg-gray-100 p-4 rounded-lg mb-6"><code>npx nuxi init my-nuxt3-app</code></pre>

          <p class="mb-6">This will create a new Nuxt 3 project in a directory called <code>my-nuxt3-app</code>. Once the project is created, you can navigate to the directory and install the dependencies:</p>

          <pre class="bg-gray-100 p-4 rounded-lg mb-6"><code>cd my-nuxt3-app
npm install</code></pre>

          <h2 class="text-2xl font-bold mb-4">Project Structure</h2>
          <p class="mb-4">A typical Nuxt 3 project has the following structure:</p>

          <pre class="bg-gray-100 p-4 rounded-lg mb-6"><code>my-nuxt3-app/
├── app/
│   ├── components/
│   ├── composables/
│   ├── pages/
│   └── plugins/
├── public/
├── nuxt.config.ts
├── package.json
└── tsconfig.json</code></pre>

          <h2 class="text-2xl font-bold mb-4">Creating Pages</h2>
          <p class="mb-6">In Nuxt 3, pages are created in the <code>app/pages</code> directory. Each file in this directory corresponds to a route in your application. For example, if you create a file called <code>about.vue</code> in the <code>app/pages</code> directory, it will be accessible at <code>/about</code>.</p>

          <h2 class="text-2xl font-bold mb-4">Running the Development Server</h2>
          <p class="mb-4">To run the development server, use the following command:</p>

          <pre class="bg-gray-100 p-4 rounded-lg mb-6"><code>npm run dev</code></pre>

          <p class="mb-6">This will start the development server and you can access your application at <code>http://localhost:3000</code>.</p>

          <h2 class="text-2xl font-bold mb-4">Building for Production</h2>
          <p class="mb-4">To build your application for production, use the following command:</p>

          <pre class="bg-gray-100 p-4 rounded-lg mb-6"><code>npm run build</code></pre>

          <p class="mb-6">This will create a production build of your application in the <code>.output</code> directory.</p>

          <h2 class="text-2xl font-bold mb-4">Conclusion</h2>
          <p class="mb-6">Nuxt 3 is a powerful framework that makes it easy to build modern web applications. With its built-in features and intuitive API, you can create high-quality applications in less time. Whether you're building a small personal project or a large enterprise application, Nuxt 3 has everything you need to get started.</p>
        `,
        tags: ['nuxt', 'vue', 'web development', 'javascript', 'framework'],
        comments: [
          {
            id: 1,
            name: 'Jane Smith',
            avatar: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=professional%20female%20avatar%20portrait&image_size=square',
            date: '2024-04-11',
            content: 'Great article! I just started learning Nuxt 3 and this guide was very helpful.'
          },
          {
            id: 2,
            name: 'Mike Chen',
            avatar: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=professional%20asian%20male%20avatar%20portrait&image_size=square',
            date: '2024-04-12',
            content: 'Thanks for the detailed explanation. The project structure section was particularly helpful.'
          }
        ]
      }

      relatedArticles.value = [
        {
          title: 'The Future of AI in Web Development',
          date: '2024-04-05',
          imageUrl: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=ai%20web%20development%20future%20technology&image_size=landscape_16_9',
          slug: 'future-of-ai-in-web-development'
        },
        {
          title: 'Performance Optimization Techniques',
          date: '2024-03-20',
          imageUrl: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=web%20performance%20optimization%20speed&image_size=landscape_16_9',
          slug: 'performance-optimization-techniques'
        },
        {
          title: 'Getting Started with Tailwind CSS',
          date: '2024-03-15',
          imageUrl: 'https://a0ai.marscode.cn/api/ide/v1/text_to_image?prompt=tailwind%20css%20styling%20web%20development&image_size=landscape_16_9',
          slug: 'getting-started-with-tailwind-css'
        }
      ]

      categories.value = [
        { name: 'Technology', count: 12 },
        { name: 'AI', count: 8 },
        { name: 'Design', count: 6 },
        { name: 'Development', count: 15 },
        { name: 'CSS', count: 5 },
        { name: 'Accessibility', count: 4 }
      ]

      popularTags.value = ['nuxt', 'vue', 'ai', 'tailwind', 'performance', 'accessibility', 'responsive', 'javascript']

      isLoading.value = false
    }, 500)
  } catch (error) {
    console.error('Error fetching article data:', error)
    isLoading.value = false
  }
}

// Fetch data on mount
onMounted(() => {
  fetchArticleData()
})
</script>
