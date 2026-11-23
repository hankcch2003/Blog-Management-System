<template>
  <main class="not-found">
    <h1>404</h1>
    <h2>找不到此頁面</h2>
    <p>很抱歉，您所瀏覽的頁面不存在。</p>

    <RouterLink to="/">
      返回首頁
    </RouterLink>
  </main>
</template>

<style scoped>
.not-found {
  padding: 60px 20px;
  text-align: center;
}

.not-found h1 {
  font-size: 64px;
  margin-bottom: 10px;
}

.not-found h2 {
  margin-bottom: 16px;
}

.not-found p {
  margin-bottom: 24px;
}

.not-found a {
  color: #2563eb;
  text-decoration: none;
}

.not-found a:hover {
  text-decoration: underline;
}
</style>