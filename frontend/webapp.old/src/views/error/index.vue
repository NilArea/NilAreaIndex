<template>
  <div>
    <div
      id="nav-bar"
      ref="navBarRef"
      :class="{ sticky: isNavSticky }"
    >
      <span id="nav-logo-container">
        <span id="nav-logo-main"></span>
      </span>
      <div id="nav-link-container">
        <router-link class="nav-link" to="/">首页</router-link>
        <router-link class="nav-link" to="/blog">博客</router-link>
        <account-icon-component v-if="false" />
        <router-link v-else class="nav-link" to="/sign">登录</router-link>
      </div>
    </div>
    <div
      id="top-bg"
      :class="{ loaded: isBgLoaded}"
    ></div>
    <div id="spacer"></div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue';
import AccountIconComponent from '../AccountIconComponent.vue';

const isNavSticky = ref(false);
const isBgLoaded = ref(false);
const scrolling = ref(false);

const navBarRef = ref<HTMLElement | null>(null);
const navTriggerRef = ref<HTMLElement | null>(null);

const checkSticky = () => {
  if (!navTriggerRef.value) return;

  const targetRect = navTriggerRef.value.getBoundingClientRect();

  isNavSticky.value = targetRect.top <= 0;

  scrolling.value = false;
};

const handleScroll = () => {
  if (!scrolling.value) {
    scrolling.value = true;
    requestAnimationFrame(checkSticky);
  }
};

const init = () => {
  setTimeout(() => {
    isBgLoaded.value = true;
  }, 100);
  checkSticky();
};

onMounted(() => {
  init();
  window.addEventListener('scroll', handleScroll, { passive: true });
});

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll);
});
</script>

<style scoped>

:root {
  --nav-height: 90px;
}

#nav-bar {
  position: absolute;
  top: 0;
  width: 100%;
  height: var(--nav-height);
  transition: .25s
}

#nav-bar.sticky {
  z-index: 99;
  position: fixed;
  --nav-height: 82px;
  background-color: var(--bg-w-pure);
  box-shadow: rgba(0, 0, 0, .1) 0 5px 20px
}

#nav-logo-container {
  position: absolute;
  top: 20px;
  left: 200px;
  font-size: 0;
  transition: .25s
}

#nav-logo-main, #nav-logo-text {
  display: inline-block;
  height: 50px;
  background-repeat: no-repeat;
  background-size: contain;
  background-position: left center;
  transition: .25s
}

#nav-logo-main {
  width: 50px;
  background-image: url(/images/nilarea.png);
}

#nav-logo-text {
  width: 100px;
}

#nav-bar.sticky #nav-logo-container {
  top: 16px
}

#nav-link-container {
  position: absolute;
  right: 170px;
  top: 30px;
  white-space: nowrap;
  transition: .25s
}

.nav-link {
  position: relative;
  cursor: pointer;
  transition: .25s
}

#nav-bar .nav-link {
  margin: 0 10px;
  padding: 10px 20px;
  border-radius: 5px;
  color: #fff;
  font-size: 15px;
}

#nav-bar .nav-link:before {
  position: absolute;
  left: 20px;
  bottom: 3px;
  width: calc(100% - 40px);
  height: 3px;
  content: '';
  background: var(--theme-color);
  border-radius: 2px;
  transition: .25s;
  transform: scale(0);
  opacity: 0
}


#nav-bar .nav-link:hover {
  background-color: var(--b-alpha-5)
}

#nav-bar .nav-link:hover:before {
  transform: scale(1);
  opacity: 1
}

#nav-bar .nav-link:active, #nav-bar .nav-link:active:before {
  opacity: .6
}

#nav-bar.sticky .nav-link {
  color: var(--txt-b-pure)
}

@media screen and (max-width: 1300px) {
  #nav-logo-container {
    left: 100px
  }

  #nav-link-container {
    right: 70px
  }
}

@media screen and (max-width: 900px) {
  #nav-logo-container {
    left: 70px
  }

  #nav-link-container {
    right: 40px
  }
}

@media screen and (max-width: 760px) {
  #nav-logo-container {
    left: 25px
  }

  #nav-link-container {
    display: none
  }
}

#top-bg {
  z-index: -100;
  position: fixed;
  width: 100%;
  height: 100%;
  background-repeat: no-repeat;
  background-size: cover;
  background-position: center;
  background-image: url(/images/shop-bg.png);
  transition: transform 1.5s, opacity 1s;
  transform: scale(1.05);
  opacity: 0
}

#top-bg.loaded {
  transform: scale(1);
  opacity: 1
}

#spacer {
  height: 100vh;
}
</style>
