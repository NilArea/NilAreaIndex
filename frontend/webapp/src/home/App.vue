<template>
  <div id="home">
    <div id="nav-bar">
    <span id="nav-logo-container">
        <span id="nav-logo-main"></span>
    </span>
      <div id="nav-link-container">
        <a class="nav-link" href="/">首页</a>
        <a class="nav-link" href="/home">博客</a>
      </div>
    </div>
    <div id="top-bg"></div>
    <div id="graph-1" nav-trigger>
    </div>
    <FooterComponent />
  </div>
</template>

<script setup>
import FooterComponent from '../components/FooterComponent.vue';
import { navScroll } from '../assets/scroll.ts';

$(function(){
  $('#top-bg').addClass('loaded');

  const $nav = $('#nav-bar');
  const $target = $('[nav-trigger]');
  let scrolling = false;

  function checkSticky() {
    if ($target[0] === undefined) {
      return;
    }
    const targetRect = $target[0].getBoundingClientRect();
    if (targetRect.top <= 0) {
      $nav.addClass('sticky');
    } else {
      $nav.removeClass('sticky');
    }
    scrolling = false;
  }

  $(window).on('scroll', function() {
    if (!scrolling) {
      scrolling = true;
      requestAnimationFrame(checkSticky);
    }
  });
  checkSticky();
  navScroll($('.nav-link[data-target]'))
});
</script>

<style scoped>
#home {
  height: 30%;
}

h1 {
  margin-bottom: 20px;
  font-weight: 400;
  font-size: 76px;
  color: #fff
}

h2 {
  margin-top: 25px;
  font-weight: 400;
  font-size: 15px;
  color: #fff
}

h2 span {
  color: rgba(255, 255, 255, .6)
}

h3 {
  margin-bottom: 20px;
  font-size: 30px;
  color: var(--txt-b-pure);
  transition: .25s
}

h4 {
  color: var(--txt-b-pure);
  font-size: 28px
}

h5 {
  color: var(--txt-b-pure);
  font-size: 24px;
  text-align: center
}

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
  background-image: url(/nilarea.png);
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
  background-image: url(/fufu-bg.png);
  transition: transform 1.5s, opacity 1s;
  transform: scale(1.05);
  opacity: 0
}

#top-bg.loaded {
  transform: scale(1);
  opacity: 1
}

#headline-container {
  position: absolute;
  top: 30%;
  width: 100%;
  text-align: center;
  font-size: 0;
}

.heading-underline {
  display: inline-block;
  width: 70px;
  height: 4px;
  border-radius: 2px;
  background-color: var(--theme-color);
}

.heading-underline-1 {
  display: inline-block;
  width: 50px;
  height: 3px;
  border-radius: 2px;
  background-color: var(--theme-color);
}

#graph-1, #graph-2, #graph-3 {
  position: relative;
  top: 100%;
  width: 100%;
  padding-top: 80px;
  padding-bottom: 100px;
  font-size: 0;
  text-align: center;
  transition: .25s
}

#graph-1, #graph-3 {
  background-color: var(--bg-w-pure)
}

#graph-1 {
  padding-left: 5%;
  padding-right: 5%
}

.tags-container {
  margin-top: 25px;
  font-size: small
}

.tags-container span {
  display: inline-block;
  margin: 3px;
  padding: 8px 15px;
  border-radius: 5px;
  background-color: var(--b-alpha-5);
  color: var(--b-alpha-60);
  transition: .25s
}

.tags-container span:hover {
  background-color: var(--b-alpha-10);
  color: var(--txt-b-pure);
  -webkit-box-shadow: rgba(0, 0, 0, .1) 0 5px 10px;
  box-shadow: rgba(0, 0, 0, .1) 0 5px 10px
}

.about-content {
  position: relative;
  margin-top: 25px;
  width: 100%;
  color: var(--b-alpha-80);
  font-size: 14px
}
</style>
