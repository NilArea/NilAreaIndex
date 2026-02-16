<template>
  <div>
    <div
      id="top-bg"
      :class="{ loaded: isBgLoaded}"
    />
    <div id="sign-container">
      <UCard>
        <template #header>
          <button>aa</button>
        </template>
        <template #footer>
          <button>aa</button>
        </template>
        <SignIn
          v-if="isSignIn"
        />
        <SignUp
          v-else
        />
      </UCard>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { onMounted, ref } from 'vue';
import { SignIn, SignUp } from '../../components/Sign';

const isBgLoaded = ref(false);
const isSignIn = ref(true);

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
  justify-content: center;
  align-items: center;
  overflow: auto;
}

.sign-card {
  background-color: #fff;
  border-radius: 16px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.12);
  padding: 16px;
}
</style>
