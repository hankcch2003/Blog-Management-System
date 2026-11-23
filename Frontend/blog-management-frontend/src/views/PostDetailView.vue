<script setup>
import { ref, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { getPostById } from '../services/postService'

const route = useRoute()

const post = ref(null)
const isLoading = ref(false)
const errorMessage = ref('')

const loadPost = async () => {
  const postId = Number(route.params.id)

  post.value = null
  errorMessage.value = ''

  if (!Number.isInteger(postId) || postId <= 0) {
    errorMessage.value = '文章編號格式不正確。'
    return
  }

  isLoading.value = true

  try {
    const response = await getPostById(postId)

    if (!response || typeof response !== 'object') {
      throw new Error('文章資料格式不正確。')
    }

    if (response.isPublished !== true) {
      errorMessage.value = '找不到這篇文章。'
      return
    }

    post.value = response
  } catch (error) {
    console.error('取得文章失敗：', error)

    if (error.response?.status === 404) {
      errorMessage.value = '找不到這篇文章，可能已經被刪除。'
    } else if (error.response) {
      errorMessage.value =
        `文章載入失敗，伺服器回應 HTTP ${error.response.status}。`
    } else if (error.request) {
      errorMessage.value =
        '無法連接後端 API，請確認後端服務是否啟動。'
    } else {
      errorMessage.value =
        error.message || '載入文章時發生未知錯誤。'
    }
  } finally {
    isLoading.value = false
  }
}

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
    month: 'long',
    day: '2-digit'
  }).format(parsedDate)
}

onMounted(() => {
  loadPost()
})

watch(
  () => route.params.id,
  () => {
    loadPost()
  }
)
</script>

<template>
  <main class="detail-page">
    <div class="detail-container">

      <!-- Page Navigation -->
      <nav class="page-navigation">
        <RouterLink to="/" class="navigation-link">
          首頁
        </RouterLink>

        <span class="navigation-separator">/</span>

        <RouterLink to="/posts" class="navigation-link">
          文章列表
        </RouterLink>

        <span class="navigation-separator">/</span>

        <span class="current-page">文章詳細內容</span>
      </nav>

      <!-- Loading State -->
      <div v-if="isLoading" class="loading-state">
        <p>正在載入文章內容，請稍候...</p>
      </div>

      <!-- Error State -->
      <div v-else-if="errorMessage" class="error-state">
        <h2>無法顯示文章</h2>
        <p>{{ errorMessage }}</p>

        <div class="error-actions">
          <button
            type="button"
            class="retry-button"
            @click="loadPost"
          >
            重新載入
          </button>

          <RouterLink to="/posts" class="home-button">
            返回文章列表
          </RouterLink>
        </div>
      </div>

      <!-- Article Content -->
      <article v-else-if="post" class="article-detail">

        <header class="article-header">
          <span class="article-category">
            {{ post.categoryName || '未分類' }}
          </span>

          <h1>{{ post.title }}</h1>

          <div class="article-meta">
            <span>
              發布日期：{{ formatDate(post.publishedAt) }}
            </span>

            <span>
              分類：{{ post.categoryName || '未分類' }}
            </span>
          </div>

          <p
            v-if="post.summary"
            class="article-summary"
          >
            {{ post.summary }}
          </p>
        </header>

        <div class="article-divider"></div>

        <section class="article-content">
          <h2>文章內容</h2>

          <div class="content-text">{{ post.content }}</div>
        </section>

        <footer class="article-footer">
          <RouterLink to="/posts" class="back-list-link">
            ← 返回文章列表
          </RouterLink>

          <RouterLink to="/" class="back-home-link">
            返回首頁
          </RouterLink>
        </footer>

      </article>

    </div>
  </main>
</template>

<style scoped>
.detail-page {
  width: 100%;
  min-height: 60vh;
  padding: 48px 24px 80px;
  background-color: #f8fafc;
}

.detail-container {
  max-width: 900px;
  margin: 0 auto;
}

/* Page Navigation */
.page-navigation {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 10px;
  margin-bottom: 28px;
  font-size: 14px;
}

.navigation-link {
  color: #2563eb;
  font-weight: 700;
  text-decoration: none;
}

.navigation-link:hover {
  color: #1e40af;
  text-decoration: underline;
}

.navigation-separator {
  color: #94a3b8;
}

.current-page {
  color: #64748b;
}

/* Article */
.article-detail {
  padding: 48px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  background-color: #ffffff;
  box-shadow: 0 4px 16px rgb(15 23 42 / 5%);
}

.article-header {
  margin-bottom: 32px;
}

.article-category {
  display: inline-block;
  margin-bottom: 20px;
  padding: 6px 12px;
  border-radius: 20px;
  background-color: #dbeafe;
  color: #1d4ed8;
  font-size: 13px;
  font-weight: 700;
}

.article-header h1 {
  margin-bottom: 20px;
  color: #0f172a;
  font-size: 36px;
  line-height: 1.4;
}

.article-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 12px 24px;
  color: #64748b;
  font-size: 13px;
}

.article-summary {
  margin-top: 28px;
  margin-bottom: 0;
  padding: 20px;
  border-left: 4px solid #3b82f6;
  border-radius: 4px;
  background-color: #eff6ff;
  color: #334155;
  font-size: 16px;
  line-height: 1.8;
}

.article-divider {
  height: 1px;
  margin: 32px 0;
  background-color: #e2e8f0;
}

/* Article Content */
.article-content h2 {
  margin-bottom: 24px;
  font-size: 23px;
}

.content-text {
  color: #334155;
  font-size: 16px;
  line-height: 2;
  white-space: pre-wrap;
  overflow-wrap: anywhere;
}

/* Article Footer */
.article-footer {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
  margin-top: 48px;
  padding-top: 24px;
  border-top: 1px solid #e2e8f0;
}

.back-list-link,
.back-home-link {
  color: #2563eb;
  font-size: 14px;
  font-weight: 700;
  text-decoration: none;
}

.back-list-link:hover,
.back-home-link:hover {
  color: #1e40af;
  text-decoration: underline;
}

/* Loading */
.loading-state {
  padding: 64px 24px;
  border-radius: 12px;
  background-color: #ffffff;
  color: #64748b;
  text-align: center;
}

/* Error */
.error-state {
  padding: 64px 24px;
  border: 1px solid #fecaca;
  border-radius: 12px;
  background-color: #fef2f2;
  text-align: center;
}

.error-state h2 {
  color: #b91c1c;
}

.error-state p {
  margin-bottom: 24px;
  color: #991b1b;
}

.error-actions {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 12px;
}

.retry-button,
.home-button {
  display: inline-block;
  padding: 10px 22px;
  border: none;
  border-radius: 8px;
  background-color: #1d4ed8;
  color: #ffffff;
  font-size: 14px;
  font-weight: 700;
  text-decoration: none;
  cursor: pointer;
}

.retry-button:hover,
.home-button:hover {
  background-color: #1e40af;
}

/* Responsive */
@media (max-width: 600px) {
  .detail-page {
    padding: 32px 16px 56px;
  }

  .article-detail {
    padding: 24px 20px;
  }

  .article-header h1 {
    font-size: 27px;
  }

  .article-summary {
    font-size: 14px;
  }

  .content-text {
    font-size: 15px;
  }

  .article-footer {
    flex-direction: column;
    gap: 16px;
  }
}
</style>