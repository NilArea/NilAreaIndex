<template>
  <UPageCard>
    <template #body>
      <UAuthForm
        :schema="schema"
        title="登录"
        description="输入您的凭据以访问您的帐户"
        icon="i-lucide-user"
        :fields="fields"
        @submit="onSubmit"
      />
    </template>
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
  </UPageCard>
</template>

<script lang="ts" setup>
import type { FormSubmitEvent, AuthFormField } from '@nuxt/ui'
import type { LoginRequest } from '~/types'
import * as v from 'valibot'

const toast = useToast()
const fields: AuthFormField[] = [{
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
  name: 'remember',
  label: '记住我',
  type: 'checkbox'
}]
const schema = v.object({
  email: v.pipe(v.string(), v.email('请输入有效的邮箱地址')),
  password: v.pipe(v.string(), v.minLength(8, '密码长度至少8位'))
})
type Schema = v.InferOutput<typeof schema>

const loading = ref(false)

async function onSubmit(event: FormSubmitEvent<Schema>) {
  toast.add({ title: 'Success', description: 'The form has been submitted.', color: 'success' })
  const state = event.data
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
