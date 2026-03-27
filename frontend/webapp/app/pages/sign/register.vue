<template>
  <UPageCard>
    <template #body>
      <UAuthForm
        :schema="schema"
        title="注册"
        description="输入您的凭据以创建您的帐户"
        icon="i-lucide-user"
        :fields="fields"
        @submit="onSubmit"
      />
    </template>
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
  </UPageCard>
</template>

<script lang="ts" setup>
import type { FormSubmitEvent, AuthFormField } from '@nuxt/ui'
import type { RegisterRequest } from '~/types'
import * as v from 'valibot'

const toast = useToast()
const fields: AuthFormField[] = [{
  name: 'username',
  defaultValue: '',
  type: 'text',
  label: '用户名',
  placeholder: '输入您的用户名',
  required: true
}, {
  name: 'email',
  defaultValue: '',
  type: 'email',
  label: '邮箱',
  placeholder: 'your@email.com',
  required: true
}, {
  name: 'password',
  defaultValue: '',
  label: '密码',
  type: 'password',
  placeholder: '输入您的密码',
  required: true
}, {
  name: 'confirmPassword',
  defaultValue: '',
  label: '再次输入密码',
  type: 'password',
  placeholder: '确认您的密码',
  required: true
}]
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

const loading = ref(false)

async function onSubmit(event: FormSubmitEvent<Schema>) {
  toast.add({ title: 'Success', description: 'The form has been submitted.', color: 'success' })
  const state = event.data
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
