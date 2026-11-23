<script setup>
import AppHeader from './components/AppHeader.vue'
import AppFooter from './components/AppFooter.vue'
</script>

<template>
  <div class="app-layout">
    <!-- 網站共用頂部導覽列 -->
    <AppHeader />

    <!-- 各路由頁面的主要內容 -->
    <main class="main-content">
      <RouterView />
    </main>

    <!-- 網站共用底部頁尾 -->
    <AppFooter />
  </div>
</template>

<style>
.app-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-content {
  flex: 1;
  width: 100%;
}
</style>