<template>
  <footer>
    <div>
      <p>与我联系</p>
      <a id="link-contact-me">我的邮箱</a><br>
      <a href="https://space.bilibili.com/569316920/" target="_blank">我的 bilibili 频道</a><br>
      <a href="https://ifdian.net/a/QianXi_Lee" target="_blank">我的 爱发电</a><br>
    </div>
    <div id="copyright">
      <p>© 2026 浅析</p>
      <span class="legal-text-container">
            <a id="link-copyright">版权声明</a><br>
            <a target="_blank">XICP备xxxxxxxx号-x</a><br>
            <a target="_blank">X公网安备 xxxxxxxxxxxxx号</a>
        </span>
    </div>
  </footer>
</template>

<script setup>
$(function() {
  $('#link-contact-me').on('click', function(e) {
    e.preventDefault();
    location.href = 'mailto:qianxilee232@163.com';
  });
});
</script>

<style scoped>
footer {
  position: relative;
  top: 100%;
  width: 100%;
  padding: 100px 0 95px;
  font-size: 0;
  text-align: center;
  transition: .25s;
  background: rgba(0, 0, 0, .05);
}

footer div {
  display: inline-block;
  position: relative;
  text-align: left;
  vertical-align: top;
  padding: 0 50px
}

footer p {
  margin-top: 0;
  font-size: 15px;
  font-weight: 700;
  color: #fff
}

footer a {
  display: inline-block;
  margin-bottom: 5px;
  border-bottom: 1px solid transparent;
  color: #fff;
  cursor: pointer;
  font-size: 12px
}

footer a:hover {
  border-color: #fff
}

footer a:active {
  opacity: .6
}

.legal-text-container {
  position: relative;
  top: -6px;
}
</style>
