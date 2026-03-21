<template>
  <a
    :href="href"
    @click.prevent="handleClick"
  >
    <slot />
  </a>
</template>

<script lang="ts" setup>
import { computed, type ComputedRef } from 'vue';

const easingFunctions = {
  linear: (t: number) => t,
  easeInQuad: (t: number) => t * t,
  easeOutQuad: (t: number) => t * (2 - t),
  easeInOutQuad: (t: number) => t < 0.5 ? 2 * t * t : -1 + (4 - 2 * t) * t,
  easeInCubic: (t: number) => t * t * t,
  easeOutCubic: (t: number) => (--t) * t * t + 1,
  easeInOutCubic: (t: number) => t < 0.5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1,
  easeInQuart: (t: number) => t * t * t * t,
  easeOutQuart: (t: number) => 1 - (--t) * t * t * t,
  easeInOutQuart: (t: number) => t < 0.5 ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t,
  easeInQuint: (t: number) => t * t * t * t * t,
  easeOutQuint: (t: number) => 1 + (--t) * t * t * t * t,
  easeInOutQuint: (t: number) => t < 0.5 ? 16 * t * t * t * t * t : 1 + 16 * (--t) * t * t * t * t
};

export interface Props {
  // 目标可以是 DOM id 或绝对偏移量
  to: string | number;
  // 滚动时长（毫秒）
  duration?: number;
  // 偏移量（用于固定导航栏等场景）
  offset?: number;
  // 缓动函数
  easing?: keyof typeof easingFunctions;
  // 是否阻止默认事件
  preventDefault?: boolean;
  // 点击回调
  onClick?: () => void;
}

const props = withDefaults(defineProps<Props>(), {
  duration: 500,
  offset: 0,
  easing: 'easeInOutCubic',
  preventDefault: true,
  onClick: undefined
});

const emit = defineEmits<{
  click: [event: MouseEvent]
  scrollStart: []
  scrollComplete: []
}>();

const href: ComputedRef<string> = computed(() => {
  const off = Number.parseInt(props.to as string);
  return Number.isNaN(off) ? `#${props.to}` : off.toString();
});

// 获取目标位置
const getTargetPosition = (): number => {
  const off = Number.parseInt(props.to as string);
  if (!Number.isNaN(off)) return off - props.offset;
  const element = document.getElementById(props.to as string);
  if (!element) {
    console.warn(`Element with id "${props.to}" not found`);
    return 0;
  }
  return window.pageYOffset + element.getBoundingClientRect().top - props.offset;
};

// 平滑滚动
const smoothScroll = (targetPosition: number) => {
  const startPosition = window.pageYOffset;
  const distance = targetPosition - startPosition;
  const startTime = performance.now();
  const duration = props.duration;

  // 如果距离为0，直接触发完成事件
  if (distance === 0) {
    emit('scrollComplete');
    return;
  }

  const easingFunc = easingFunctions[props.easing];

  const scrollAnimation = (currentTime: number) => {
    const elapsedTime = currentTime - startTime;
    const progress = Math.min(elapsedTime / duration, 1);

    // 应用缓动函数
    const easedProgress = easingFunc(progress);

    // 计算当前滚动位置
    const currentPosition = startPosition + distance * easedProgress;

    window.scrollTo(0, currentPosition);

    if (progress < 1) {
      requestAnimationFrame(scrollAnimation);
    } else {
      emit('scrollComplete');
    }
  };

  emit('scrollStart');
  requestAnimationFrame(scrollAnimation);
};

// 处理点击事件
const handleClick = (event: MouseEvent) => {
  emit('click', event);

  // 执行自定义点击回调
  if (props.onClick) {
    props.onClick();
  }

  // 等待下一个tick确保DOM更新（如果有）
  setTimeout(() => {
    const targetPosition = getTargetPosition();
    smoothScroll(targetPosition);
  }, 10);
};

// 暴露方法给父组件
defineExpose({
  scrollTo: () => {
    const targetPosition = getTargetPosition();
    smoothScroll(targetPosition);
  }
});
</script>
