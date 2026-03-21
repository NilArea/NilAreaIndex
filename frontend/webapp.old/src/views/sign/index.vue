<template>
  <div>
    <div
      id="top-bg"
      :class="{ loaded: isBgLoaded}"
    />
    <div id="sign-container">
      <Transition
        mode="out-in"
        enter-active-class="transition duration-300 ease-out"
        enter-from-class="opacity-0 translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition duration-200 ease-in"
        leave-from-class="opacity-100 translate-y-0"
        leave-to-class="opacity-0 -translate-y-2"
      >
        <SignIn
          v-if="isLogin"
          @switch="isLogin = false"
          @success="handleAuth"
        />
        <SignUp
          v-else
          @switch="isLogin = true"
          @success="handleAuth"
        />
      </Transition>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { onMounted, ref } from 'vue';
import SignIn from './SignIn.vue';
import SignUp from './SignUp.vue';

const isBgLoaded = ref(false);
const isLogin = ref(true);

const handleAuth = () =>{
  location.href = '/blog';
}

const init = () => {
  setTimeout(() => {
    isBgLoaded.value = true;
  }, 100);
};

onMounted(() => {
  init();
});
</script>

<style scoped>
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

#sign-container {
  width: 100%;
  height: 100vh;
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  overflow: auto;
}
</style>
