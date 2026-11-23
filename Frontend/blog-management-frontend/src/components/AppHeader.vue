<template>
  <header class="app-header">
    <div class="header-container">
      <!-- 網站名稱 -->
      <RouterLink to="/" class="brand">
        Blog Management System
      </RouterLink>

      <!-- 網站導覽列 -->
      <nav class="main-navigation">
        <RouterLink
          to="/"
          class="nav-link"
          exact-active-class="active"
        >
          首頁
        </RouterLink>

        <RouterLink
          to="/posts"
          class="nav-link"
          active-class="active"
        >
          文章管理
        </RouterLink>

        <RouterLink
          to="/categories"
          class="nav-link"
          active-class="active"
        >
          分類管理
        </RouterLink>
      </nav>
    </div>
  </header>
</template>

<style scoped>
.app-header {
  width: 100%;
  background-color: #172554;
  color: #ffffff;
}

.header-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 18px 24px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 24px;
}

.brand {
  color: #ffffff;
  font-size: 20px;
  font-weight: 700;
  text-decoration: none;
}

.main-navigation {
  display: flex;
  align-items: center;
  gap: 24px;
}

.nav-link {
  color: #cbd5e1;
  text-decoration: none;
  font-size: 15px;
  transition: color 0.2s ease;
}

.nav-link:hover,
.nav-link.active {
  color: #ffffff;
}

.nav-link.active {
  font-weight: 700;
}

@media (max-width: 600px) {
  .header-container {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }

  .main-navigation {
    flex-wrap: wrap;
    gap: 16px;
  }
}
</style>