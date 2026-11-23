<script setup>
import { ref, computed, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import {
  getPosts,
  getPostById,
  createPost,
  updatePost,
  deletePost
} from '../services/postService'
import { getCategories } from '../services/categoryService'

// 文章及分類資料
const posts = ref([])
const categories = ref([])

// 畫面狀態
const isLoading = ref(false)
const isSubmitting = ref(false)
const isUpdating = ref(false)
const isLoadingEdit = ref(false)
const isDeleting = ref(false)
const deletingPostId = ref(null)

const errorMessage = ref('')
const successMessage = ref('')
const submitErrorMessage = ref('')
const deleteSuccessMessage = ref('')
const deleteErrorMessage = ref('')

// 新增文章表單
const postForm = ref({
  title: '',
  content: '',
  summary: '',
  categoryId: '',
  isPublished: false
})

// 新增文章欄位驗證錯誤
const validationErrors = ref({})

// 編輯文章狀態
const editingPostId = ref(null)
const editForm = ref({
  title: '',
  content: '',
  summary: '',
  categoryId: '',
  isPublished: false
})
const editValidationErrors = ref({})
const editSuccessMessage = ref('')
const editErrorMessage = ref('')

// 篩選條件
const searchKeyword = ref('')
const selectedCategoryId = ref('all')
const selectedStatus = ref('all')

// 取得文章及分類資料
const loadPosts = async () => {
  isLoading.value = true
  errorMessage.value = ''

  try {
    const [postsResponse, categoriesResponse] = await Promise.all([
      getPosts(),
      getCategories()
    ])

    if (!Array.isArray(postsResponse)) {
      throw new Error('API 回傳的文章資料格式不正確。')
    }

    if (!Array.isArray(categoriesResponse)) {
      throw new Error('API 回傳的分類資料格式不正確。')
    }

    posts.value = [...postsResponse].sort((a, b) => {
      const dateA = new Date(a.createdAt || 0).getTime()
      const dateB = new Date(b.createdAt || 0).getTime()
      return dateB - dateA
    })

    categories.value = [...categoriesResponse].sort(
      (a, b) => a.id - b.id
    )
  } catch (error) {
    console.error('載入文章管理資料失敗：', error)

    if (error.response) {
      errorMessage.value =
        `資料載入失敗，伺服器回應 HTTP ${error.response.status}。`
    } else if (error.request) {
      errorMessage.value =
        '無法連接後端 API，請確認後端服務是否啟動。'
    } else {
      errorMessage.value =
        error.message || '載入資料時發生未知錯誤。'
    }
  } finally {
    isLoading.value = false
  }
}

// 驗證新增文章表單
const validatePostForm = () => {
  const errors = {}

  if (!postForm.value.title.trim()) {
    errors.title = '請輸入文章標題。'
  } else if (postForm.value.title.trim().length > 200) {
    errors.title = '文章標題不可超過 200 個字元。'
  }

  if (!postForm.value.content.trim()) {
    errors.content = '請輸入文章內容。'
  }

  if (postForm.value.summary.length > 500) {
    errors.summary = '文章摘要不可超過 500 個字元。'
  }

  if (!postForm.value.categoryId) {
    errors.categoryId = '請選擇文章分類。'
  }

  validationErrors.value = errors

  return Object.keys(errors).length === 0
}

// 重設新增文章表單
const resetPostForm = () => {
  postForm.value = {
    title: '',
    content: '',
    summary: '',
    categoryId: '',
    isPublished: false
  }

  validationErrors.value = {}
  submitErrorMessage.value = ''
}

// 新增文章
const handleCreatePost = async () => {
  successMessage.value = ''
  submitErrorMessage.value = ''

  if (!validatePostForm()) {
    submitErrorMessage.value = '請檢查表單欄位是否填寫正確。'
    return
  }

  if (isSubmitting.value || isDeleting.value) {
    return
  }

  isSubmitting.value = true

  try {
    const postData = {
      title: postForm.value.title.trim(),
      content: postForm.value.content.trim(),
      summary: postForm.value.summary.trim() || null,
      categoryId: Number(postForm.value.categoryId),
      isPublished: postForm.value.isPublished
    }

    await createPost(postData)

    successMessage.value =
      postData.isPublished
        ? '文章已成功新增並發布！'
        : '文章已成功新增為草稿！'

    resetPostForm()
    await loadPosts()
  } catch (error) {
    console.error('新增文章失敗：', error)

    if (error.response) {
      const status = error.response.status

      if (status === 400) {
        submitErrorMessage.value =
          '新增失敗：資料驗證未通過，請檢查標題、內容及分類。'
      } else if (status === 404) {
        submitErrorMessage.value =
          '新增失敗：指定的文章分類不存在，請重新選擇分類。'
      } else {
        submitErrorMessage.value =
          `新增文章失敗，伺服器回應 HTTP ${status}。`
      }
    } else if (error.request) {
      submitErrorMessage.value =
        '無法連接後端 API，請確認後端服務是否正常。'
    } else {
      submitErrorMessage.value =
        error.message || '新增文章時發生未知錯誤。'
    }
  } finally {
    isSubmitting.value = false
  }
}

// 開始編輯文章：先從 API 取得最新資料
const handleEditPost = async (post) => {
  if (isLoadingEdit.value || isUpdating.value || isDeleting.value) {
    return
  }

  editingPostId.value = post.id
  editSuccessMessage.value = ''
  editErrorMessage.value = ''
  editValidationErrors.value = {}

  editForm.value = {
    title: '',
    content: '',
    summary: '',
    categoryId: '',
    isPublished: false
  }

  isLoadingEdit.value = true

  try {
    const response = await getPostById(post.id)

    if (!response || typeof response !== 'object') {
      throw new Error('API 回傳的文章資料格式不正確。')
    }

    if (editingPostId.value !== post.id) {
      return
    }

    editForm.value = {
      title: response.title || '',
      content: response.content || '',
      summary: response.summary || '',
      categoryId: response.categoryId
        ? String(response.categoryId)
        : '',
      isPublished: response.isPublished === true
    }
  } catch (error) {
    console.error('取得文章編輯資料失敗：', error)

    if (error.response) {
      const status = error.response.status

      if (status === 404) {
        editErrorMessage.value =
          '找不到這篇文章，可能已經被刪除，請重新載入文章列表。'
      } else {
        editErrorMessage.value =
          `取得文章資料失敗，伺服器回應 HTTP ${status}。`
      }
    } else if (error.request) {
      editErrorMessage.value =
        '無法連接後端 API，請確認後端服務是否正常。'
    } else {
      editErrorMessage.value =
        error.message || '載入文章編輯資料時發生未知錯誤。'
    }
  } finally {
    isLoadingEdit.value = false
  }
}

// 驗證編輯文章表單
const validateEditForm = () => {
  const errors = {}

  if (!editForm.value.title.trim()) {
    errors.title = '請輸入文章標題。'
  } else if (editForm.value.title.trim().length > 200) {
    errors.title = '文章標題不可超過 200 個字元。'
  }

  if (!editForm.value.content.trim()) {
    errors.content = '請輸入文章內容。'
  }

  if (editForm.value.summary.length > 500) {
    errors.summary = '文章摘要不可超過 500 個字元。'
  }

  if (!editForm.value.categoryId) {
    errors.categoryId = '請選擇文章分類。'
  }

  editValidationErrors.value = errors

  return Object.keys(errors).length === 0
}

// 取消編輯
const cancelEditPost = () => {
  if (isUpdating.value || isDeleting.value) {
    return
  }

  editingPostId.value = null
  editForm.value = {
    title: '',
    content: '',
    summary: '',
    categoryId: '',
    isPublished: false
  }

  editValidationErrors.value = {}
  editSuccessMessage.value = ''
  editErrorMessage.value = ''
}

// 儲存文章修改
const handleUpdatePost = async () => {
  editSuccessMessage.value = ''
  editErrorMessage.value = ''

  if (!editingPostId.value) {
    editErrorMessage.value = '目前沒有選取要編輯的文章。'
    return
  }

  if (!validateEditForm()) {
    editErrorMessage.value = '請檢查編輯表單欄位是否填寫正確。'
    return
  }

  if (isUpdating.value || isDeleting.value) {
    return
  }

  isUpdating.value = true

  try {
    const postData = {
      title: editForm.value.title.trim(),
      content: editForm.value.content.trim(),
      summary: editForm.value.summary.trim() || null,
      categoryId: Number(editForm.value.categoryId),
      isPublished: editForm.value.isPublished
    }

    await updatePost(editingPostId.value, postData)

    successMessage.value = '文章修改成功！'
    editingPostId.value = null

    editForm.value = {
      title: '',
      content: '',
      summary: '',
      categoryId: '',
      isPublished: false
    }

    editValidationErrors.value = {}
    editSuccessMessage.value = ''

    await loadPosts()
  } catch (error) {
    console.error('修改文章失敗：', error)

    if (error.response) {
      const status = error.response.status

      if (status === 400) {
        editErrorMessage.value =
          '修改失敗：資料驗證未通過，請檢查標題、內容及分類。'
      } else if (status === 404) {
        editErrorMessage.value =
          '修改失敗：文章或指定的分類不存在，請重新載入資料。'
      } else {
        editErrorMessage.value =
          `修改文章失敗，伺服器回應 HTTP ${status}。`
      }
    } else if (error.request) {
      editErrorMessage.value =
        '無法連接後端 API，請確認後端服務是否正常。'
    } else {
      editErrorMessage.value =
        error.message || '修改文章時發生未知錯誤。'
    }
  } finally {
    isUpdating.value = false
  }
}

// 刪除文章
const handleDeletePost = async (post) => {
  if (
    isDeleting.value ||
    isUpdating.value ||
    isLoadingEdit.value ||
    isSubmitting.value
  ) {
    return
  }

  // 刪除前確認，避免誤刪
  const confirmed = window.confirm(
    `確定要刪除這篇文章嗎？\n\n` +
    `文章 ID：${post.id}\n` +
    `文章標題：${post.title}\n\n` +
    '刪除後將無法復原。'
  )

  if (!confirmed) {
    return
  }

  isDeleting.value = true
  deletingPostId.value = post.id
  deleteSuccessMessage.value = ''
  deleteErrorMessage.value = ''

  try {
    await deletePost(post.id)

    // 如果正在編輯同一篇文章，關閉編輯表單
    if (editingPostId.value === post.id) {
      editingPostId.value = null
      editForm.value = {
        title: '',
        content: '',
        summary: '',
        categoryId: '',
        isPublished: false
      }
      editValidationErrors.value = {}
      editErrorMessage.value = ''
    }

    deleteSuccessMessage.value =
      `文章「${post.title}」已成功刪除。`

    // 重新取得文章列表及統計資料
    await loadPosts()
  } catch (error) {
    console.error('刪除文章失敗：', error)

    if (error.response) {
      const status = error.response.status

      if (status === 404) {
        deleteErrorMessage.value =
          '刪除失敗：找不到這篇文章，可能已經被刪除。請重新載入文章列表。'
      } else if (status === 400) {
        deleteErrorMessage.value =
          '刪除失敗：伺服器拒絕此項刪除請求，請檢查文章資料。'
      } else {
        deleteErrorMessage.value =
          `刪除文章失敗，伺服器回應 HTTP ${status}。`
      }
    } else if (error.request) {
      deleteErrorMessage.value =
        '無法連接後端 API，刪除未能完成。請確認後端服務是否正常。'
    } else {
      deleteErrorMessage.value =
        error.message || '刪除文章時發生未知錯誤。'
    }
  } finally {
    isDeleting.value = false
    deletingPostId.value = null
  }
}

// 篩選文章
const filteredPosts = computed(() => {
  let result = [...posts.value]

  const keyword = searchKeyword.value.trim().toLowerCase()

  if (keyword) {
    result = result.filter((post) => {
      const title = (post.title || '').toLowerCase()
      const summary = (post.summary || '').toLowerCase()

      return title.includes(keyword) || summary.includes(keyword)
    })
  }

  if (selectedCategoryId.value !== 'all') {
    result = result.filter(
      (post) => post.categoryId === Number(selectedCategoryId.value)
    )
  }

  if (selectedStatus.value === 'published') {
    result = result.filter((post) => post.isPublished === true)
  } else if (selectedStatus.value === 'draft') {
    result = result.filter((post) => post.isPublished === false)
  }

  return result
})

// 清除所有篩選條件
const clearFilters = () => {
  searchKeyword.value = ''
  selectedCategoryId.value = 'all'
  selectedStatus.value = 'all'
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
    month: 'long',
    day: '2-digit'
  }).format(parsedDate)
}

// 取得文章狀態文字
const getStatusText = (isPublished) => {
  return isPublished ? '已發布' : '草稿'
}

onMounted(() => {
  loadPosts()
})
</script>

<template>
  <main class="posts-page">
    <div class="posts-container">

      <!-- Page Header -->
      <header class="page-header">
        <div>
          <span class="page-label">POST MANAGEMENT</span>
          <h1>文章管理</h1>
          <p>管理 Blog Management System 中的所有文章。</p>
        </div>

        <RouterLink to="/" class="create-button">
          返回首頁
        </RouterLink>
      </header>

      <!-- Create Post Form -->
      <section class="create-section">
        <div class="section-heading">
          <div>
            <h2>新增文章</h2>
            <p>填寫以下資料，建立一篇新的部落格文章。</p>
          </div>
        </div>

        <form class="create-form" @submit.prevent="handleCreatePost">

          <div class="form-group">
            <label for="postTitle">
              文章標題 <span class="required">*</span>
            </label>

            <input
              id="postTitle"
              v-model="postForm.title"
              type="text"
              maxlength="200"
              placeholder="請輸入文章標題"
              :disabled="isSubmitting || isDeleting"
            />

            <div class="field-footer">
              <span v-if="validationErrors.title" class="field-error">
                {{ validationErrors.title }}
              </span>
              <span class="character-count">
                {{ postForm.title.length }} / 200
              </span>
            </div>
          </div>

          <div class="form-group">
            <label for="postContent">
              文章內容 <span class="required">*</span>
            </label>

            <textarea
              id="postContent"
              v-model="postForm.content"
              rows="9"
              placeholder="請輸入完整文章內容..."
              :disabled="isSubmitting || isDeleting"
            ></textarea>

            <span v-if="validationErrors.content" class="field-error">
              {{ validationErrors.content }}
            </span>
          </div>

          <div class="form-group">
            <label for="postSummary">文章摘要</label>

            <textarea
              id="postSummary"
              v-model="postForm.summary"
              rows="3"
              maxlength="500"
              placeholder="請輸入文章摘要（選填，最多 500 字）"
              :disabled="isSubmitting || isDeleting"
            ></textarea>

            <div class="field-footer">
              <span v-if="validationErrors.summary" class="field-error">
                {{ validationErrors.summary }}
              </span>
              <span class="character-count">
                {{ postForm.summary.length }} / 500
              </span>
            </div>
          </div>

          <div class="form-group">
            <label for="postCategory">
              文章分類 <span class="required">*</span>
            </label>

            <select
              id="postCategory"
              v-model="postForm.categoryId"
              :disabled="isSubmitting || isDeleting"
            >
              <option value="">請選擇文章分類</option>
              <option
                v-for="category in categories"
                :key="category.id"
                :value="String(category.id)"
              >
                {{ category.name }}
              </option>
            </select>

            <span v-if="validationErrors.categoryId" class="field-error">
              {{ validationErrors.categoryId }}
            </span>
          </div>

          <div class="publish-option">
            <label class="checkbox-label">
              <input
                v-model="postForm.isPublished"
                type="checkbox"
                :disabled="isSubmitting || isDeleting"
              />
              <span>立即發布文章</span>
            </label>

            <p>
              勾選後文章會以已發布狀態建立；未勾選則儲存為草稿。
            </p>
          </div>

          <div v-if="submitErrorMessage" class="form-message error-message">
            {{ submitErrorMessage }}
          </div>

          <div v-if="successMessage" class="form-message success-message">
            {{ successMessage }}
          </div>

          <div class="form-actions">
            <button
              type="button"
              class="reset-button"
              :disabled="isSubmitting || isDeleting"
              @click="resetPostForm"
            >
              清空表單
            </button>

            <button
              type="submit"
              class="submit-button"
              :disabled="isSubmitting || isDeleting || categories.length === 0"
            >
              {{ isSubmitting ? '正在新增...' : '新增文章' }}
            </button>
          </div>

          <p v-if="categories.length === 0" class="category-warning">
            目前沒有可用分類，請先至分類管理建立文章分類。
          </p>
        </form>
      </section>

      <!-- Edit Post Form -->
      <section
        v-if="editingPostId !== null || editErrorMessage"
        class="edit-section"
      >
        <div class="section-heading">
          <div>
            <h2>編輯文章</h2>
            <p>正在編輯 POST #{{ editingPostId ?? '—' }}，修改完成後請儲存。</p>
          </div>
        </div>

        <div v-if="isLoadingEdit" class="loading-state edit-loading">
          正在取得文章資料，請稍候...
        </div>

        <div v-else-if="editErrorMessage" class="form-message error-message">
          {{ editErrorMessage }}
        </div>

        <form
          v-else
          class="create-form"
          @submit.prevent="handleUpdatePost"
        >
          <div class="form-group">
            <label for="editPostTitle">
              文章標題 <span class="required">*</span>
            </label>

            <input
              id="editPostTitle"
              v-model="editForm.title"
              type="text"
              maxlength="200"
              placeholder="請輸入文章標題"
              :disabled="isUpdating || isDeleting"
            />

            <div class="field-footer">
              <span v-if="editValidationErrors.title" class="field-error">
                {{ editValidationErrors.title }}
              </span>
              <span class="character-count">
                {{ editForm.title.length }} / 200
              </span>
            </div>
          </div>

          <div class="form-group">
            <label for="editPostContent">
              文章內容 <span class="required">*</span>
            </label>

            <textarea
              id="editPostContent"
              v-model="editForm.content"
              rows="9"
              placeholder="請輸入完整文章內容..."
              :disabled="isUpdating || isDeleting"
            ></textarea>

            <span v-if="editValidationErrors.content" class="field-error">
              {{ editValidationErrors.content }}
            </span>
          </div>

          <div class="form-group">
            <label for="editPostSummary">文章摘要</label>

            <textarea
              id="editPostSummary"
              v-model="editForm.summary"
              rows="3"
              maxlength="500"
              placeholder="請輸入文章摘要（選填，最多 500 字）"
              :disabled="isUpdating || isDeleting"
            ></textarea>

            <div class="field-footer">
              <span v-if="editValidationErrors.summary" class="field-error">
                {{ editValidationErrors.summary }}
              </span>
              <span class="character-count">
                {{ editForm.summary.length }} / 500
              </span>
            </div>
          </div>

          <div class="form-group">
            <label for="editPostCategory">
              文章分類 <span class="required">*</span>
            </label>

            <select
              id="editPostCategory"
              v-model="editForm.categoryId"
              :disabled="isUpdating || isDeleting"
            >
              <option value="">請選擇文章分類</option>
              <option
                v-for="category in categories"
                :key="category.id"
                :value="String(category.id)"
              >
                {{ category.name }}
              </option>
            </select>

            <span v-if="editValidationErrors.categoryId" class="field-error">
              {{ editValidationErrors.categoryId }}
            </span>
          </div>

          <div class="publish-option">
            <label class="checkbox-label">
              <input
                v-model="editForm.isPublished"
                type="checkbox"
                :disabled="isUpdating || isDeleting"
              />
              <span>發布文章</span>
            </label>

            <p>
              勾選代表文章為已發布狀態；取消勾選則將文章設為草稿。
            </p>
          </div>

          <div v-if="editErrorMessage" class="form-message error-message">
            {{ editErrorMessage }}
          </div>

          <div class="form-actions">
            <button
              type="button"
              class="reset-button"
              :disabled="isUpdating || isDeleting"
              @click="cancelEditPost"
            >
              取消編輯
            </button>

            <button
              type="submit"
              class="submit-button"
              :disabled="isUpdating || isDeleting || categories.length === 0"
            >
              {{ isUpdating ? '正在儲存...' : '儲存修改' }}
            </button>
          </div>
        </form>
      </section>

      <!-- Delete Result Messages -->
      <section
        v-if="deleteSuccessMessage || deleteErrorMessage"
        class="delete-message-section"
        aria-live="polite"
      >
        <div
          v-if="deleteSuccessMessage"
          class="form-message success-message"
          role="status"
        >
          {{ deleteSuccessMessage }}
        </div>

        <div
          v-if="deleteErrorMessage"
          class="form-message error-message"
          role="alert"
        >
          {{ deleteErrorMessage }}
        </div>
      </section>

      <!-- Summary -->
      <section class="summary-section">
        <div class="summary-card">
          <span class="summary-label">文章總數</span>
          <strong>{{ posts.length }}</strong>
          <span class="summary-description">系統目前儲存的文章數量</span>
        </div>

        <div class="summary-card">
          <span class="summary-label">已發布文章</span>
          <strong class="published-number">
            {{ posts.filter(post => post.isPublished).length }}
          </strong>
          <span class="summary-description">已公開發布的文章</span>
        </div>

        <div class="summary-card">
          <span class="summary-label">草稿文章</span>
          <strong class="draft-number">
            {{ posts.filter(post => !post.isPublished).length }}
          </strong>
          <span class="summary-description">尚未發布的文章</span>
        </div>
      </section>

      <!-- Filters -->
      <section class="filter-section">
        <div class="section-header">
          <div>
            <h2>文章列表</h2>
            <p>搜尋文章或依照分類、發布狀態篩選。</p>
          </div>

          <span class="result-count">
            顯示 {{ filteredPosts.length }} / {{ posts.length }} 篇
          </span>
        </div>

        <div class="filter-panel">
          <div class="filter-group search-group">
            <label for="postSearch">搜尋文章</label>
            <input
              id="postSearch"
              v-model="searchKeyword"
              type="search"
              placeholder="輸入文章標題或摘要..."
            />
          </div>

          <div class="filter-group">
            <label for="categoryFilter">文章分類</label>
            <select id="categoryFilter" v-model="selectedCategoryId">
              <option value="all">所有分類</option>
              <option
                v-for="category in categories"
                :key="category.id"
                :value="String(category.id)"
              >
                {{ category.name }}
              </option>
            </select>
          </div>

          <div class="filter-group">
            <label for="statusFilter">發布狀態</label>
            <select id="statusFilter" v-model="selectedStatus">
              <option value="all">全部狀態</option>
              <option value="published">已發布</option>
              <option value="draft">草稿</option>
            </select>
          </div>

          <button
            type="button"
            class="clear-button"
            @click="clearFilters"
          >
            清除篩選
          </button>
        </div>
      </section>

      <!-- Loading -->
      <section v-if="isLoading" class="loading-state">
        <p>正在載入文章資料，請稍候...</p>
      </section>

      <!-- Error -->
      <section v-else-if="errorMessage" class="error-state">
        <h2>文章資料載入失敗</h2>
        <p>{{ errorMessage }}</p>
        <button type="button" class="retry-button" @click="loadPosts">
          重新載入
        </button>
      </section>

      <!-- Empty -->
      <section v-else-if="posts.length === 0" class="empty-state">
        <h2>目前沒有文章</h2>
        <p>目前系統尚未建立任何文章。</p>
      </section>

      <!-- No Search Results -->
      <section v-else-if="filteredPosts.length === 0" class="empty-state">
        <h2>找不到符合條件的文章</h2>
        <p>請調整搜尋關鍵字或篩選條件。</p>
        <button
          type="button"
          class="retry-button"
          @click="clearFilters"
        >
          清除篩選條件
        </button>
      </section>

      <!-- Post List -->
      <section v-else class="post-list-section">
        <div class="post-list">
          <article
            v-for="post in filteredPosts"
            :key="post.id"
            class="post-card"
          >
            <div class="post-card-top">
              <span class="post-id">POST #{{ post.id }}</span>
              <span
                class="status-badge"
                :class="post.isPublished ? 'published' : 'draft'"
              >
                {{ getStatusText(post.isPublished) }}
              </span>
            </div>

            <div class="post-card-content">
              <div class="post-category">
                {{ post.categoryName || '未指定分類' }}
              </div>
              <h3>{{ post.title }}</h3>
              <p class="post-summary">
                {{ post.summary || '尚未提供文章摘要。' }}
              </p>
            </div>

            <div class="post-card-footer">
              <div class="post-dates">
                <span>建立日期：{{ formatDate(post.createdAt) }}</span>
                <span v-if="post.publishedAt">
                  發布日期：{{ formatDate(post.publishedAt) }}
                </span>
              </div>

              <div class="post-card-actions">
                <button
                  type="button"
                  class="edit-button"
                  :disabled="
                    isLoadingEdit ||
                    isUpdating ||
                    isDeleting ||
                    isSubmitting
                  "
                  @click="handleEditPost(post)"
                >
                  {{ editingPostId === post.id ? '正在編輯' : '編輯文章' }}
                </button>

                <button
                  type="button"
                  class="delete-button"
                  :disabled="
                    isLoadingEdit ||
                    isUpdating ||
                    isDeleting ||
                    isSubmitting
                  "
                  @click="handleDeletePost(post)"
                >
                  {{
                    deletingPostId === post.id
                      ? '正在刪除...'
                      : '刪除文章'
                  }}
                </button>

                <RouterLink
                  :to="`/posts/${post.id}`"
                  class="detail-link"
                >
                  查看文章
                </RouterLink>
              </div>
            </div>
          </article>
        </div>
      </section>

    </div>
  </main>
</template>

<style scoped>
.posts-page {
  min-height: 60vh;
  padding: 48px 24px 80px;
  background-color: #f8fafc;
}

.posts-container {
  max-width: 1100px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: 20px;
  margin-bottom: 36px;
}

.page-label {
  color: #2563eb;
  font-size: 12px;
  font-weight: 800;
  letter-spacing: 1.5px;
}

.page-header h1 {
  margin: 12px 0;
  color: #0f172a;
  font-size: 34px;
}

.page-header p {
  color: #64748b;
  font-size: 15px;
}

.create-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 12px 22px;
  border-radius: 8px;
  background-color: #1d4ed8;
  color: #ffffff;
  font-size: 14px;
  font-weight: 700;
  text-decoration: none;
}

.create-button:hover {
  background-color: #1e40af;
}

/* Create and Edit Forms */
.create-section,
.edit-section {
  margin-bottom: 40px;
  padding: 28px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  background-color: #ffffff;
}

.edit-section {
  border-color: #93c5fd;
  box-shadow: 0 3px 12px rgb(37 99 235 / 6%);
}

.section-heading {
  margin-bottom: 24px;
}

.section-heading h2 {
  margin: 0 0 8px;
  color: #0f172a;
  font-size: 22px;
}

.section-heading p {
  margin: 0;
  color: #64748b;
  font-size: 14px;
}

.create-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group > label {
  color: #334155;
  font-size: 14px;
  font-weight: 700;
}

.required {
  color: #dc2626;
}

.form-group input,
.form-group textarea,
.form-group select {
  width: 100%;
  min-width: 0;
  padding: 12px 14px;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  background-color: #ffffff;
  color: #1e293b;
  font: inherit;
}

.form-group textarea {
  resize: vertical;
  line-height: 1.7;
}

.form-group input:focus,
.form-group textarea:focus,
.form-group select:focus {
  outline: 2px solid #93c5fd;
  border-color: #2563eb;
}

.form-group input:disabled,
.form-group textarea:disabled,
.form-group select:disabled {
  background-color: #f1f5f9;
  cursor: not-allowed;
}

.field-footer {
  display: flex;
  justify-content: space-between;
  gap: 12px;
}

.field-error {
  color: #dc2626;
  font-size: 13px;
}

.character-count {
  margin-left: auto;
  color: #94a3b8;
  font-size: 12px;
}

.publish-option {
  padding: 16px;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  background-color: #f8fafc;
}

.checkbox-label {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  color: #334155;
  font-size: 14px;
  font-weight: 700;
  cursor: pointer;
}

.checkbox-label input {
  width: 17px;
  height: 17px;
  accent-color: #2563eb;
}

.publish-option p {
  margin: 8px 0 0 27px;
  color: #64748b;
  font-size: 12px;
}

.form-message {
  padding: 13px 16px;
  border-radius: 8px;
  font-size: 14px;
}

.error-message {
  border: 1px solid #fecaca;
  background-color: #fef2f2;
  color: #b91c1c;
}

.success-message {
  border: 1px solid #bbf7d0;
  background-color: #f0fdf4;
  color: #15803d;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.reset-button,
.submit-button {
  padding: 12px 22px;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 700;
  cursor: pointer;
}

.reset-button {
  background-color: #e2e8f0;
  color: #334155;
}

.reset-button:hover {
  background-color: #cbd5e1;
}

.submit-button {
  background-color: #1d4ed8;
  color: #ffffff;
}

.submit-button:hover {
  background-color: #1e40af;
}

.reset-button:disabled,
.submit-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.category-warning {
  margin: 0;
  color: #b45309;
  font-size: 13px;
}

.edit-loading {
  padding: 32px 16px;
  border-radius: 8px;
  background-color: #f8fafc;
  color: #64748b;
  text-align: center;
}

/* Delete Result Messages */
.delete-message-section {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 28px;
}

/* Summary */
.summary-section {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 20px;
  margin-bottom: 40px;
}

.summary-card {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 24px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  background-color: #ffffff;
}

.summary-label {
  color: #64748b;
  font-size: 14px;
}

.summary-card strong {
  margin: 12px 0;
  color: #1d4ed8;
  font-size: 36px;
}

.summary-card .published-number {
  color: #15803d;
}

.summary-card .draft-number {
  color: #b45309;
}

.summary-description {
  color: #94a3b8;
  font-size: 12px;
}

/* Sections and Filters */
.filter-section {
  margin-bottom: 32px;
}

.section-header {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 20px;
}

.section-header h2 {
  margin: 0 0 8px;
  color: #0f172a;
  font-size: 22px;
}

.section-header p {
  margin: 0;
  color: #64748b;
  font-size: 14px;
}

.result-count {
  color: #64748b;
  font-size: 13px;
}

.filter-panel {
  display: grid;
  grid-template-columns: minmax(200px, 2fr) minmax(160px, 1fr) minmax(140px, 1fr) auto;
  align-items: end;
  gap: 16px;
  padding: 24px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  background-color: #ffffff;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.filter-group label {
  color: #334155;
  font-size: 13px;
  font-weight: 700;
}

.filter-group input,
.filter-group select {
  width: 100%;
  min-width: 0;
  padding: 12px;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  background-color: #ffffff;
  color: #1e293b;
  font: inherit;
}

.filter-group input:focus,
.filter-group select:focus {
  outline: 2px solid #93c5fd;
  border-color: #2563eb;
}

.clear-button,
.retry-button {
  padding: 12px 18px;
  border: none;
  border-radius: 8px;
  background-color: #e2e8f0;
  color: #334155;
  font-weight: 700;
  cursor: pointer;
}

.clear-button:hover {
  background-color: #cbd5e1;
}

.retry-button {
  background-color: #1d4ed8;
  color: #ffffff;
}

.retry-button:hover {
  background-color: #1e40af;
}

/* Post Cards */
.post-list {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 20px;
}

.post-card {
  display: flex;
  flex-direction: column;
  min-width: 0;
  padding: 24px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  background-color: #ffffff;
  box-shadow: 0 3px 12px rgb(15 23 42 / 4%);
}

.post-card-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 20px;
}

.post-id {
  color: #94a3b8;
  font-size: 12px;
  font-weight: 700;
}

.status-badge {
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 700;
}

.status-badge.published {
  background-color: #dcfce7;
  color: #15803d;
}

.status-badge.draft {
  background-color: #fef3c7;
  color: #92400e;
}

.post-card-content {
  flex: 1;
}

.post-category {
  margin-bottom: 10px;
  color: #2563eb;
  font-size: 13px;
  font-weight: 700;
}

.post-card h3 {
  margin: 0 0 12px;
  color: #0f172a;
  font-size: 20px;
  line-height: 1.5;
  overflow-wrap: anywhere;
}

.post-summary {
  margin: 0;
  color: #64748b;
  font-size: 14px;
  line-height: 1.8;
  overflow-wrap: anywhere;
}

.post-card-footer {
  display: flex;
  flex-wrap: wrap;
  align-items: flex-end;
  justify-content: space-between;
  gap: 16px;
  margin-top: 24px;
  padding-top: 18px;
  border-top: 1px solid #e2e8f0;
}

.post-dates {
  display: flex;
  flex-direction: column;
  gap: 6px;
  color: #94a3b8;
  font-size: 12px;
}

.post-card-actions {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 12px;
}

.edit-button,
.delete-button {
  padding: 8px 14px;
  border-radius: 7px;
  font-size: 13px;
  font-weight: 700;
  cursor: pointer;
}

.edit-button {
  border: 1px solid #bfdbfe;
  background-color: #eff6ff;
  color: #1d4ed8;
}

.edit-button:hover {
  background-color: #dbeafe;
}

.delete-button {
  border: 1px solid #fecaca;
  background-color: #fef2f2;
  color: #b91c1c;
}

.delete-button:hover {
  background-color: #fee2e2;
}

.edit-button:disabled,
.delete-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.detail-link {
  color: #1d4ed8;
  font-size: 14px;
  font-weight: 700;
  text-decoration: none;
}

.detail-link:hover {
  color: #1e40af;
  text-decoration: underline;
}

/* Loading / Error / Empty */
.loading-state,
.empty-state {
  padding: 64px 24px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  background-color: #ffffff;
  color: #64748b;
  text-align: center;
}

.error-state {
  padding: 48px 24px;
  border: 1px solid #fecaca;
  border-radius: 12px;
  background-color: #fef2f2;
  text-align: center;
}

.error-state h2 {
  color: #b91c1c;
}

.error-state p {
  margin-bottom: 20px;
  color: #991b1b;
}

.empty-state h2 {
  color: #334155;
}

.empty-state p {
  margin-bottom: 20px;
  color: #64748b;
}

/* Responsive */
@media (max-width: 900px) {
  .filter-panel {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  .post-list {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 600px) {
  .posts-page {
    padding: 32px 16px 56px;
  }

  .page-header h1 {
    font-size: 28px;
  }

  .page-header > a {
    width: 100%;
  }

  .create-section,
  .edit-section {
    padding: 20px;
  }

  .summary-section {
    grid-template-columns: 1fr;
  }

  .filter-panel {
    grid-template-columns: 1fr;
    padding: 20px;
  }

  .clear-button {
    width: 100%;
  }

  .post-card {
    padding: 20px;
  }

  .post-card-footer {
    align-items: stretch;
    flex-direction: column;
  }

  .post-card-actions {
    align-items: stretch;
    flex-direction: column;
  }

  .post-card-actions > * {
    text-align: center;
  }

  .form-actions {
    flex-direction: column-reverse;
  }

  .form-actions button {
    width: 100%;
  }
}
</style>