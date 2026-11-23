<script setup>
import { ref, computed, onMounted } from 'vue'
import { getPosts } from '../services/postService'

// 文章資料
const posts = ref([])

// API 狀態
const isLoading = ref(false)
const errorMessage = ref('')

// 搜尋與分類篩選條件
const searchKeyword = ref('')
const selectedCategory = ref('')

// 取得所有文章分類名稱
const categories = computed(() => {
  return [
    ...new Set(
      posts.value
        .map(post => post.categoryName)
        .filter(name => name && name.trim() !== '')
    )
  ].sort((a, b) => a.localeCompare(b, 'zh-TW'))
})

// 根據搜尋文字及分類篩選文章
const filteredPosts = computed(() => {
  const keyword = searchKeyword.value.trim().toLowerCase()

  return posts.value.filter(post => {
    const matchesKeyword =
      !keyword ||
      (post.title || '').toLowerCase().includes(keyword) ||
      (post.summary || '').toLowerCase().includes(keyword)

    const matchesCategory =
      !selectedCategory.value ||
      post.categoryName === selectedCategory.value

    return matchesKeyword && matchesCategory
  })
})

// 是否正在使用搜尋或分類條件
const hasActiveFilters = computed(() => {
  return (
    searchKeyword.value.trim() !== '' ||
    selectedCategory.value !== ''
  )
})

// 清除搜尋條件
const clearFilters = () => {
  searchKeyword.value = ''
  selectedCategory.value = ''
}

// 載入文章
const loadPosts = async () => {
  isLoading.value = true
  errorMessage.value = ''

  try {
    const response = await getPosts()

    if (!Array.isArray(response)) {
      throw new Error('API 回傳的文章資料格式不正確。')
    }

    posts.value = response
      .filter(post => post.isPublished === true)
      .sort((a, b) => {
        const dateA = a.publishedAt
          ? new Date(a.publishedAt).getTime()
          : 0

        const dateB = b.publishedAt
          ? new Date(b.publishedAt).getTime()
          : 0

        return dateB - dateA
      })
  } catch (error) {
    console.error('載入文章失敗：', error)

    if (error.response) {
      errorMessage.value =
        `文章載入失敗，伺服器回應 HTTP ${error.response.status}。`
    } else if (error.request) {
      errorMessage.value =
        '無法連接後端 API，請確認後端服務是否啟動及連線設定是否正確。'
    } else {
      errorMessage.value =
        error.message || '載入文章時發生未知錯誤。'
    }
  } finally {
    isLoading.value = false
  }
}

// 格式化日期
const formatDate = (date) => {
  if (!date) {
    return '日期未提供'
  }

  const parsedDate = new Date(date)

  if (Number.isNaN(parsedDate.getTime())) {
    return '日期格式錯誤'
  }

  return new Intl.DateTimeFormat('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  }).format(parsedDate)
}

// 頁面載入時取得文章
onMounted(() => {
  loadPosts()
})
</script>

<template>
  <div class="home-page">
    <!-- Hero Section -->
    <section class="hero-section">
      <div class="hero-container">
        <p class="hero-label">WELCOME TO MY BLOG</p>

        <h1>
          分享技術知識<br />
          探索程式開發
        </h1>

        <p class="hero-description">
          記錄程式學習歷程，分享 Java、後端開發與軟體工程相關知識。
        </p>

        <RouterLink to="/posts" class="hero-button">
          瀏覽文章
        </RouterLink>
      </div>
    </section>

    <!-- Article List Section -->
    <section class="articles-section">
      <div class="articles-container">
        <div class="section-heading">
          <div>
            <p class="section-label">LATEST ARTICLES</p>
            <h2>最新文章</h2>
            <p class="section-description">
              探索最新的技術文章與學習紀錄。
            </p>
          </div>

          <RouterLink to="/posts" class="view-all-link">
            查看全部文章 →
          </RouterLink>
        </div>

        <!-- Loading State -->
        <div v-if="isLoading" class="loading-state">
          <p>正在載入文章，請稍候...</p>
        </div>

        <!-- Error State -->
        <div v-else-if="errorMessage" class="error-state">
          <h3>文章載入失敗</h3>
          <p>{{ errorMessage }}</p>

          <button
            type="button"
            class="retry-button"
            @click="loadPosts"
          >
            重新載入
          </button>
        </div>

        <!-- Article Content -->
        <template v-else>
          <!-- Search and Filter -->
          <div v-if="posts.length > 0" class="filter-panel">
            <div class="search-control">
              <label for="article-search">搜尋文章</label>
              <input
                id="article-search"
                v-model="searchKeyword"
                type="search"
                placeholder="輸入文章標題或摘要關鍵字..."
              />
            </div>

            <div class="category-control">
              <label for="category-filter">文章分類</label>
              <select
                id="category-filter"
                v-model="selectedCategory"
              >
                <option value="">全部分類</option>
                <option
                  v-for="category in categories"
                  :key="category"
                  :value="category"
                >
                  {{ category }}
                </option>
              </select>
            </div>

            <button
              v-if="hasActiveFilters"
              type="button"
              class="clear-filter-button"
              @click="clearFilters"
            >
              清除條件
            </button>
          </div>

          <!-- Result Count -->
          <div v-if="posts.length > 0" class="result-count">
            共找到
            <strong>{{ filteredPosts.length }}</strong>
            篇文章
          </div>

          <!-- Article Cards -->
          <div
            v-if="filteredPosts.length > 0"
            class="article-grid"
          >
            <article
              v-for="post in filteredPosts"
              :key="post.id"
              class="article-card"
            >
              <div class="article-card-content">
                <span class="article-category">
                  {{ post.categoryName || '未分類' }}
                </span>

                <h3>{{ post.title }}</h3>

                <p class="article-summary">
                  {{ post.summary || '目前沒有文章摘要。' }}
                </p>

                <div class="article-meta">
                  <span>{{ formatDate(post.publishedAt) }}</span>

                  <span class="published-label">
                    已發布
                  </span>
                </div>

                <RouterLink
                  :to="`/posts/${post.id}`"
                  class="read-more-link"
                >
                  閱讀全文 →
                </RouterLink>
              </div>
            </article>
          </div>

          <!-- No Published Articles -->
          <div
            v-else-if="posts.length === 0"
            class="empty-state"
          >
            <h3>目前尚無已發布文章</h3>
            <p>敬請期待後續更新的文章內容。</p>
          </div>

          <!-- No Search Results -->
          <div v-else class="empty-state">
            <h3>找不到符合條件的文章</h3>
            <p>請嘗試其他關鍵字或選擇不同分類。</p>

            <button
              type="button"
              class="clear-filter-button"
              @click="clearFilters"
            >
              清除搜尋條件
            </button>
          </div>
        </template>
      </div>
    </section>
  </div>
</template>

<style scoped>
.home-page {
  width: 100%;
}

/* Hero Section */
.hero-section {
  width: 100%;
  background: linear-gradient(135deg, #172554, #1e40af);
  color: #ffffff;
}

.hero-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 90px 24px;
}

.hero-label {
  margin-bottom: 16px;
  color: #bfdbfe;
  font-size: 13px;
  font-weight: 700;
  letter-spacing: 2px;
}

.hero-container h1 {
  margin-bottom: 24px;
  color: #ffffff;
  font-size: 44px;
  font-weight: 800;
  line-height: 1.4;
}

.hero-description {
  max-width: 600px;
  margin-bottom: 32px;
  color: #dbeafe;
  font-size: 17px;
}

.hero-button {
  display: inline-block;
  padding: 12px 28px;
  border-radius: 8px;
  background-color: #ffffff;
  color: #1e3a8a;
  font-weight: 700;
  text-decoration: none;
  transition: background-color 0.2s ease;
}

.hero-button:hover {
  background-color: #dbeafe;
}

/* Articles Section */
.articles-section {
  padding: 64px 24px 80px;
  background-color: #f8fafc;
}

.articles-container {
  max-width: 1200px;
  margin: 0 auto;
}

.section-heading {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: 24px;
  margin-bottom: 32px;
}

.section-label {
  margin-bottom: 8px;
  color: #2563eb;
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 1.5px;
}

.section-heading h2 {
  margin-bottom: 8px;
  font-size: 30px;
}

.section-description {
  margin-bottom: 0;
  color: #64748b;
}

.view-all-link {
  color: #2563eb;
  font-weight: 700;
  text-decoration: none;
  white-space: nowrap;
}

.view-all-link:hover {
  color: #1e40af;
}

/* Search and Filter */
.filter-panel {
  display: flex;
  align-items: flex-end;
  gap: 16px;
  margin-bottom: 20px;
  padding: 20px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  background-color: #ffffff;
}

.search-control,
.category-control {
  display: flex;
  flex: 1;
  flex-direction: column;
  gap: 8px;
  min-width: 0;
}

.search-control label,
.category-control label {
  color: #334155;
  font-size: 14px;
  font-weight: 700;
}

.search-control input,
.category-control select {
  width: 100%;
  min-height: 44px;
  padding: 10px 12px;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  background-color: #ffffff;
  color: #1e293b;
}

.search-control input:focus,
.category-control select:focus {
  border-color: #2563eb;
  outline: 2px solid #bfdbfe;
}

.clear-filter-button {
  min-height: 44px;
  padding: 10px 18px;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  background-color: #ffffff;
  color: #334155;
  font-weight: 700;
}

.clear-filter-button:hover {
  background-color: #f1f5f9;
}

.result-count {
  margin-bottom: 20px;
  color: #64748b;
  font-size: 14px;
}

.result-count strong {
  color: #1d4ed8;
}

/* Article Cards */
.article-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 24px;
}

.article-card {
  overflow: hidden;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  background-color: #ffffff;
  box-shadow: 0 4px 12px rgb(15 23 42 / 4%);
  transition:
    transform 0.2s ease,
    box-shadow 0.2s ease;
}

.article-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 10px 24px rgb(15 23 42 / 10%);
}

.article-card-content {
  padding: 24px;
}

.article-category {
  display: inline-block;
  margin-bottom: 16px;
  padding: 5px 10px;
  border-radius: 20px;
  background-color: #dbeafe;
  color: #1d4ed8;
  font-size: 12px;
  font-weight: 700;
}

.article-card h3 {
  margin-bottom: 12px;
  font-size: 20px;
}

.article-summary {
  min-height: 52px;
  margin-bottom: 20px;
  color: #64748b;
  font-size: 14px;
}

.article-meta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 20px;
  color: #94a3b8;
  font-size: 12px;
}

.published-label {
  color: #15803d;
  font-weight: 700;
}

.read-more-link {
  color: #2563eb;
  font-size: 14px;
  font-weight: 700;
  text-decoration: none;
}

.read-more-link:hover {
  color: #1e40af;
}

/* Loading State */
.loading-state {
  padding: 48px 24px;
  border-radius: 12px;
  background-color: #ffffff;
  color: #64748b;
  text-align: center;
}

/* Error State */
.error-state {
  padding: 48px 24px;
  border: 1px solid #fecaca;
  border-radius: 12px;
  background-color: #fef2f2;
  text-align: center;
}

.error-state h3 {
  color: #b91c1c;
}

.error-state p {
  color: #991b1b;
}

.retry-button {
  padding: 10px 20px;
  border: none;
  border-radius: 8px;
  background-color: #1d4ed8;
  color: #ffffff;
  font-weight: 700;
}

.retry-button:hover {
  background-color: #1e40af;
}

/* Empty State */
.empty-state {
  padding: 64px 24px;
  border: 1px dashed #cbd5e1;
  border-radius: 12px;
  background-color: #ffffff;
  text-align: center;
}

.empty-state h3 {
  margin-bottom: 8px;
}

.empty-state p {
  margin-bottom: 20px;
  color: #64748b;
}

/* Responsive */
@media (max-width: 900px) {
  .article-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 600px) {
  .hero-container {
    padding: 64px 20px;
  }

  .hero-container h1 {
    font-size: 32px;
  }

  .hero-description {
    font-size: 15px;
  }

  .articles-section {
    padding: 48px 20px 60px;
  }

  .section-heading {
    align-items: flex-start;
    flex-direction: column;
  }

  .section-heading h2 {
    font-size: 26px;
  }

  .filter-panel {
    align-items: stretch;
    flex-direction: column;
  }

  .article-grid {
    grid-template-columns: 1fr;
  }
}
</style>