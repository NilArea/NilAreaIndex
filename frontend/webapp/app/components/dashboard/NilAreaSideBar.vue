<template>
  <UDashboardSidebar
    :ui="{ footer: 'border-t border-default' }"
    collapsible
    resizable
  >
    <template #header="{ collapsed }">
      <NuxtLink
        v-if="collapsed"
        to="/"
      >
        <NilAreaLogo
          :collapsed="collapsed"
          class="w-auto h-8 shrink-0"
        />
      </NuxtLink>
      <NuxtLink
        v-else
        to="/"
      >
        NilArea
      </NuxtLink>
    </template>
    <template #default="{ collapsed }">
      <UButton
        :label="collapsed ? undefined : 'Search...'"
        :square="collapsed"
        block
        color="neutral"
        icon="i-lucide-search"
        variant="outline"
      >
        <template
          v-if="!collapsed"
          #trailing
        >
          <div class="flex items-center gap-0.5 ms-auto">
            <UKbd
              value="meta"
              variant="subtle"
            />
            <UKbd
              value="K"
              variant="subtle"
            />
          </div>
        </template>
      </UButton>

      <UNavigationMenu
        :collapsed="collapsed"
        :items="items[0]"
        orientation="vertical"
      />

      <UNavigationMenu
        :collapsed="collapsed"
        :items="items[1]"
        class="mt-auto"
        orientation="vertical"
      />
    </template>

    <template #footer="{ collapsed }">
      <UButton
        :avatar="{
          src: 'https://github.com/benjamincanac.png',
          loading: 'lazy' as const
        }"
        :block="collapsed"
        :label="collapsed ? undefined : 'Benjamin'"
        class="w-full"
        color="neutral"
        variant="ghost"
      />
    </template>
  </UDashboardSidebar>
</template>

<script lang="ts" setup>
import type { NavigationMenuItem } from '@nuxt/ui'
import NilAreaLogo from '~/components/NilAreaLogo.vue'

const items: NavigationMenuItem[][] = [
  [
    {
      label: 'Home',
      to: '/dashboard/home',
      icon: 'i-lucide-house'
    },
    {
      label: 'Settings',
      to: '/dashboard/settings',
      icon: 'i-lucide-settings',
      children: [
        {
          label: 'General',
          to: '/dashboard/settings/general'
        },
        {
          label: 'Notifications',
          to: '/dashboard/settings/notification'
        }]
    }
  ],
  [
  ]]
</script>
