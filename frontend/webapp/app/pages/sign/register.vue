<template>
  <UCard>
    <template #header>
      <h3 class="text-xl font-semibold text-center">
        创建账号
      </h3>
    </template>
    <UForm
      :schema="schema"
      :state="state"
      class="space-y-4"
      @submit="onSubmit"
    >
      <UFormField
        label="用户名"
        name="username"
      >
        <UInput
          v-model="state.username"
          placeholder="您的用户名"
          icon="i-heroicons-user"
          required
        />
      </UFormField>
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
      <UFormField
        label="确认密码"
        name="confirmPassword"
      >
        <UInput
          v-model="state.confirmPassword"
          type="password"
          placeholder="••••••••"
          icon="i-heroicons-check-circle"
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
        注册
      </UButton>
    </UForm>
    <template #footer>
      <p class="text-sm text-center text-gray-500">
        已有账号？
        <UButton
          variant="link"
          color="primary"
          @click="useRouter().push('/sign/login')"
        >
          立即登录
        </UButton>
      </p>
    </template>
  </UCard>
</template>

<script lang="ts" setup>
import type { RegisterRequest } from '~/types'
import * as v from 'valibot'
import type { FormSubmitEvent } from '@nuxt/ui'

interface Model {
  username: string
  email: string
  password: string
  confirmPassword: string
}
const schema = v.pipe(
  v.object({
    username: v.pipe(
      v.string(),
      v.minLength(3, '用户名至少需要3个字符'),
      v.maxLength(20, '用户名不能超过20个字符')
    ),
    email: v.pipe(
      v.string(),
      v.email('请输入有效的邮箱地址')
    ),
    password: v.pipe(
      v.string(),
      v.minLength(8, '密码长度至少8位')
    ),
    confirmPassword: v.string()
  }),
  v.forward(
    v.partialCheck(
      [['password'], ['confirmPassword']],
      input => input.password === input.confirmPassword,
      '两次输入的密码不一致'
    ),
    ['confirmPassword']
  )
)
type Schema = v.InferOutput<typeof schema>
const state = reactive<Model>({
  username: '',
  email: '',
  password: '',
  confirmPassword: ''
})
const loading = ref(false)

async function onSubmit(event: FormSubmitEvent<Schema>) {
  // toast.add({ title: 'Success', description: 'The form has been submitted.', color: 'success' })
  const req: RegisterRequest = {
    username: state.username,
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
  title: 'ツ箫声断丶何处莫凭栏 | 注册'
})
</script>

<style scoped>

</style>
