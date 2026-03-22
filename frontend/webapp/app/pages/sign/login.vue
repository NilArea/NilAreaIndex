<template>
  <UCard>
    <template #header>
      <h3 class="text-xl font-semibold text-center">
        欢迎回来
      </h3>
    </template>
    <UForm
      :schema="schema"
      :state="state"
      class="space-y-4"
      @submit="onSubmit"
    >
      <UFormField
        label="邮箱"
        name="email"
      >
        <UInput
          v-model="state.email"
          type="email"
          placeholder="your@email.com"
          icon="i-heroicons-envelope"
          required
        />
      </UFormField>
      <UFormField
        label="密码"
        name="password"
      >
        <UInput
          v-model="state.password"
          type="password"
          placeholder="••••••••"
          icon="i-heroicons-lock-closed"
          required
        />
      </UFormField>
      <UButton
        type="submit"
        color="primary"
        block
        :loading="loading"
        size="lg"
      >
        登录
      </UButton>
    </UForm>
    <template #footer>
      <p class="text-sm text-center text-gray-500">
        还没有账号？
        <UButton
          variant="link"
          color="primary"
          @click="useRouter().push('/sign/register')"
        >
          立即注册
        </UButton>
      </p>
    </template>
  </UCard>
</template>

<script lang="ts" setup>
import type { LoginRequest } from '~/types'
import * as v from 'valibot'
import type { FormSubmitEvent } from '@nuxt/ui'

interface Model {
  email: string
  password: string
}
const schema = v.object({
  email: v.pipe(v.string(), v.email('请输入有效的邮箱地址')),
  password: v.pipe(v.string(), v.minLength(8, '密码长度至少8位'))
})
type Schema = v.InferOutput<typeof schema>
const state = reactive<Model>({
  email: '',
  password: ''
})

const loading = ref(false)

async function onSubmit(event: FormSubmitEvent<Schema>) {
  // toast.add({ title: 'Success', description: 'The form has been submitted.', color: 'success' })
  const req: LoginRequest = {
    email: state.email,
    password: state.password
  }
  console.log(req)
  console.log(event)
  loading.value = true
  setTimeout(() => {
    handleAuth()
  }, 1000)
}

const handleAuth = () => {
  useRouter().push('/blog')
}

useHead({
  title: 'ツ箫声断丶何处莫凭栏 | 登录'
})
</script>
