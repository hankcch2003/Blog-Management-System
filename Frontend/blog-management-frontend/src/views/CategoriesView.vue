<script setup>
import { ref, onMounted } from 'vue'
import {
  getCategories,
  createCategory,
  updateCategory,
  deleteCategory
} from '../services/categoryService'

const categories = ref([])

const isLoading = ref(false)
const isSaving = ref(false)
const deletingCategoryId = ref(null)

const errorMessage = ref('')
const formErrorMessage = ref('')
const successMessage = ref('')
const deleteErrorMessage = ref('')

const categoryForm = ref({
  name: '',
  description: ''
})

// 編輯狀態
const editingCategoryId = ref(null)

const editForm = ref({
  name: '',
  description: ''
})

const editErrorMessage = ref('')
const editSuccessMessage = ref('')

// 取得分類列表
const loadCategories = async () => {
  isLoading.value = true
  errorMessage.value = ''

  try {
    const response = await getCategories()

    if (!Array.isArray(response)) {
      throw new Error('API 回傳的分類資料格式不正確。')
    }

    categories.value = response.sort((a, b) => a.id - b.id)
  } catch (error) {
    console.error('取得分類失敗：', error)

    if (error.response) {
      errorMessage.value =
        `分類載入失敗，伺服器回應 HTTP ${error.response.status}。`
    } else if (error.request) {
      errorMessage.value =
        '無法連接後端 API，請確認後端服務是否啟動。'
    } else {
      errorMessage.value =
        error.message || '載入分類時發生未知錯誤。'
    }
  } finally {
    isLoading.value = false
  }
}

// 新增分類
const handleCreateCategory = async () => {
  formErrorMessage.value = ''
  successMessage.value = ''

  const name = categoryForm.value.name.trim()
  const description = categoryForm.value.description.trim()

  if (!name) {
    formErrorMessage.value = '請輸入分類名稱。'
    return
  }

  if (name.length > 100) {
    formErrorMessage.value = '分類名稱不可超過 100 個字元。'
    return
  }

  if (description.length > 500) {
    formErrorMessage.value = '分類描述不可超過 500 個字元。'
    return
  }

  isSaving.value = true

  try {
    await createCategory({
      name,
      description: description || null
    })

    successMessage.value = '分類新增成功！'

    categoryForm.value = {
      name: '',
      description: ''
    }

    await loadCategories()
  } catch (error) {
    console.error('新增分類失敗：', error)

    if (error.response?.status === 400) {
      formErrorMessage.value =
        '新增失敗，請確認分類名稱及描述是否符合欄位規則。'
    } else if (error.response) {
      formErrorMessage.value =
        `新增分類失敗，伺服器回應 HTTP ${error.response.status}。`
    } else if (error.request) {
      formErrorMessage.value =
        '無法連接後端 API，請確認後端服務是否啟動。'
    } else {
      formErrorMessage.value =
        error.message || '新增分類時發生未知錯誤。'
    }
  } finally {
    isSaving.value = false
  }
}

// 開始編輯分類
const startEditCategory = (category) => {
  editingCategoryId.value = category.id

  editForm.value = {
    name: category.name || '',
    description: category.description || ''
  }

  editErrorMessage.value = ''
  editSuccessMessage.value = ''
}

// 取消編輯
const cancelEdit = () => {
  editingCategoryId.value = null

  editForm.value = {
    name: '',
    description: ''
  }

  editErrorMessage.value = ''
}

// 更新分類
const handleUpdateCategory = async (categoryId) => {
  editErrorMessage.value = ''
  editSuccessMessage.value = ''

  const name = editForm.value.name.trim()
  const description = editForm.value.description.trim()

  if (!name) {
    editErrorMessage.value = '請輸入分類名稱。'
    return
  }

  if (name.length > 100) {
    editErrorMessage.value = '分類名稱不可超過 100 個字元。'
    return
  }

  if (description.length > 500) {
    editErrorMessage.value = '分類描述不可超過 500 個字元。'
    return
  }

  isSaving.value = true

  try {
    await updateCategory(categoryId, {
      name,
      description: description || null
    })

    editSuccessMessage.value = '分類更新成功！'

    await loadCategories()

    editingCategoryId.value = null

    editForm.value = {
      name: '',
      description: ''
    }
  } catch (error) {
    console.error('更新分類失敗：', error)

    if (error.response?.status === 400) {
      editErrorMessage.value =
        '更新失敗，請確認分類名稱及描述是否符合欄位規則。'
    } else if (error.response?.status === 404) {
      editErrorMessage.value =
        '找不到此分類，可能已經被刪除。請重新載入分類列表。'
    } else if (error.response) {
      editErrorMessage.value =
        `更新分類失敗，伺服器回應 HTTP ${error.response.status}。`
    } else if (error.request) {
      editErrorMessage.value =
        '無法連接後端 API，請確認後端服務是否啟動。'
    } else {
      editErrorMessage.value =
        error.message || '更新分類時發生未知錯誤。'
    }
  } finally {
    isSaving.value = false
  }
}

// 刪除分類
const handleDeleteCategory = async (category) => {
  deleteErrorMessage.value = ''
  successMessage.value = ''

  const confirmed = window.confirm(
    `確定要刪除分類「${category.name}」嗎？\n\n此操作無法復原。`
  )

  if (!confirmed) {
    return
  }

  deletingCategoryId.value = category.id

  try {
    await deleteCategory(category.id)

    successMessage.value =
      `分類「${category.name}」已成功刪除！`

    // 若正在編輯被刪除的分類，清除編輯狀態
    if (editingCategoryId.value === category.id) {
      cancelEdit()
    }

    await loadCategories()
  } catch (error) {
    console.error('刪除分類失敗：', error)

    if (error.response?.status === 409) {
      deleteErrorMessage.value =
        `無法刪除分類「${category.name}」，因為仍有文章使用此分類。請先處理相關文章後再試。`
    } else if (error.response?.status === 404) {
      deleteErrorMessage.value =
        '找不到此分類，可能已經被刪除。正在重新載入分類列表。'

      await loadCategories()
    } else if (error.response?.status === 400) {
      deleteErrorMessage.value =
        '刪除失敗，請確認分類資料是否正確。'
    } else if (error.response) {
      deleteErrorMessage.value =
        `刪除分類失敗，伺服器回應 HTTP ${error.response.status}。`
    } else if (error.request) {
      deleteErrorMessage.value =
        '無法連接後端 API，請確認後端服務是否啟動。'
    } else {
      deleteErrorMessage.value =
        error.message || '刪除分類時發生未知錯誤。'
    }
  } finally {
    deletingCategoryId.value = null
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
    month: 'long',
    day: '2-digit'
  }).format(parsedDate)
}

onMounted(() => {
  loadCategories()
})
</script>

<template>
  <main class="categories-page">
    <div class="categories-container">

      <!-- Page Header -->
      <header class="page-header">
        <div>
          <span class="page-label">CATEGORY MANAGEMENT</span>
          <h1>分類管理</h1>
          <p>管理 Blog Management System 的文章分類。</p>
        </div>
      </header>

      <!-- Category Summary -->
      <section class="summary-section">
        <div class="summary-card">
          <span class="summary-label">分類總數</span>
          <strong>{{ categories.length }}</strong>
          <span class="summary-description">
            目前系統中的文章分類
          </span>
        </div>
      </section>

      <!-- Create Category Form -->
      <section class="form-section">
        <div class="section-header">
          <h2>新增文章分類</h2>
          <span class="form-hint">請填寫分類資料</span>
        </div>

        <form
          class="category-form"
          @submit.prevent="handleCreateCategory"
        >
          <div class="form-group">
            <label for="categoryName">
              分類名稱 <span class="required-mark">*</span>
            </label>

            <input
              id="categoryName"
              v-model="categoryForm.name"
              type="text"
              maxlength="100"
              placeholder="例如：Java 程式設計與開發"
              required
            />

            <small>
              {{ categoryForm.name.length }}/100 個字元
            </small>
          </div>

          <div class="form-group">
            <label for="categoryDescription">
              分類描述
            </label>

            <textarea
              id="categoryDescription"
              v-model="categoryForm.description"
              rows="4"
              maxlength="500"
              placeholder="請輸入分類描述（選填）"
            ></textarea>

            <small>
              {{ categoryForm.description.length }}/500 個字元
            </small>
          </div>

          <p v-if="formErrorMessage" class="form-error" role="alert">
            {{ formErrorMessage }}
          </p>

          <p v-if="successMessage" class="form-success" role="status">
            {{ successMessage }}
          </p>

          <button
            type="submit"
            class="submit-button"
            :disabled="isSaving || deletingCategoryId !== null"
          >
            {{ isSaving ? '正在新增...' : '新增分類' }}
          </button>
        </form>
      </section>

      <!-- Loading State -->
      <div v-if="isLoading" class="loading-state">
        <p>正在載入分類資料，請稍候...</p>
      </div>

      <!-- Error State -->
      <div v-else-if="errorMessage" class="error-state">
        <h2>分類載入失敗</h2>
        <p>{{ errorMessage }}</p>

        <button
          type="button"
          class="retry-button"
          @click="loadCategories"
        >
          重新載入
        </button>
      </div>

      <!-- Empty State -->
      <section
        v-else-if="categories.length === 0"
        class="empty-state"
      >
        <h2>目前沒有文章分類</h2>
        <p>可以使用上方表單建立第一個文章分類。</p>
      </section>

      <!-- Category List -->
      <section v-else class="category-section">
        <div class="section-header">
          <h2>所有文章分類</h2>
          <span class="category-count">
            共 {{ categories.length }} 個分類
          </span>
        </div>

        <p
          v-if="deleteErrorMessage"
          class="delete-error"
          role="alert"
        >
          {{ deleteErrorMessage }}
        </p>

        <div class="category-grid">
          <article
            v-for="category in categories"
            :key="category.id"
            class="category-card"
          >
            <template v-if="editingCategoryId !== category.id">
              <div class="category-card-header">
                <span class="category-icon">#</span>

                <span class="category-id">
                  ID: {{ category.id }}
                </span>
              </div>

              <h3>{{ category.name }}</h3>

              <p class="category-description">
                {{ category.description || '尚未提供分類描述。' }}
              </p>

              <div class="category-footer">
                <span>建立日期</span>
                <time>
                  {{ formatDate(category.createdAt) }}
                </time>
              </div>

              <div class="category-actions">
                <button
                  type="button"
                  class="edit-button"
                  :disabled="deletingCategoryId !== null"
                  @click="startEditCategory(category)"
                >
                  編輯分類
                </button>

                <button
                  type="button"
                  class="delete-button"
                  :disabled="
                    deletingCategoryId !== null ||
                    isSaving
                  "
                  @click="handleDeleteCategory(category)"
                >
                  {{
                    deletingCategoryId === category.id
                      ? '正在刪除...'
                      : '刪除分類'
                  }}
                </button>
              </div>
            </template>

            <!-- Edit Form -->
            <form
              v-else
              class="edit-form"
              @submit.prevent="handleUpdateCategory(category.id)"
            >
              <div class="edit-form-header">
                <h3>編輯分類</h3>
                <span class="category-id">
                  ID: {{ category.id }}
                </span>
              </div>

              <div class="form-group">
                <label :for="`editName-${category.id}`">
                  分類名稱
                </label>

                <input
                  :id="`editName-${category.id}`"
                  v-model="editForm.name"
                  type="text"
                  maxlength="100"
                  required
                />

                <small>
                  {{ editForm.name.length }}/100 個字元
                </small>
              </div>

              <div class="form-group">
                <label :for="`editDescription-${category.id}`">
                  分類描述
                </label>

                <textarea
                  :id="`editDescription-${category.id}`"
                  v-model="editForm.description"
                  rows="4"
                  maxlength="500"
                ></textarea>

                <small>
                  {{ editForm.description.length }}/500 個字元
                </small>
              </div>

              <p
                v-if="editErrorMessage"
                class="form-error"
                role="alert"
              >
                {{ editErrorMessage }}
              </p>

              <div class="edit-actions">
                <button
                  type="submit"
                  class="submit-button"
                  :disabled="isSaving || deletingCategoryId !== null"
                >
                  {{ isSaving ? '正在儲存...' : '儲存修改' }}
                </button>

                <button
                  type="button"
                  class="cancel-button"
                  :disabled="isSaving || deletingCategoryId !== null"
                  @click="cancelEdit"
                >
                  取消
                </button>
              </div>
            </form>
          </article>
        </div>
      </section>

      <p v-if="editSuccessMessage" class="global-success" role="status">
        {{ editSuccessMessage }}
      </p>

    </div>
  </main>
</template>

<style scoped>
.categories-page {
  min-height: 60vh;
  padding: 48px 24px 80px;
  background-color: #f8fafc;
}

.categories-container {
  max-width: 1100px;
  margin: 0 auto;
}

/* Page Header */
.page-header {
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

/* Summary */
.summary-section {
  margin-bottom: 36px;
}

.summary-card {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  width: 230px;
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

.summary-description {
  color: #94a3b8;
  font-size: 12px;
}

/* Sections */
.form-section,
.category-section {
  margin-bottom: 40px;
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
  margin: 0;
  color: #0f172a;
  font-size: 22px;
}

.form-hint,
.category-count {
  color: #64748b;
  font-size: 13px;
}

/* Form */
.category-form,
.edit-form {
  padding: 24px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  background-color: #ffffff;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 22px;
}

.form-group label {
  color: #334155;
  font-size: 14px;
  font-weight: 700;
}

.required-mark {
  color: #dc2626;
}

.form-group input,
.form-group textarea {
  width: 100%;
  padding: 12px 14px;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  background-color: #ffffff;
  color: #1e293b;
  font: inherit;
}

.form-group textarea {
  resize: vertical;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: 2px solid #93c5fd;
  border-color: #2563eb;
}

.form-group small {
  color: #94a3b8;
  font-size: 12px;
  text-align: right;
}

/* Buttons */
.submit-button,
.retry-button,
.edit-button,
.delete-button {
  padding: 11px 24px;
  border: none;
  border-radius: 8px;
  color: #ffffff;
  font-weight: 700;
  cursor: pointer;
}

.submit-button,
.retry-button,
.edit-button {
  background-color: #1d4ed8;
}

.submit-button:hover,
.retry-button:hover,
.edit-button:hover {
  background-color: #1e40af;
}

.delete-button {
  background-color: #dc2626;
}

.delete-button:hover {
  background-color: #b91c1c;
}

.submit-button:disabled,
.cancel-button:disabled,
.edit-button:disabled,
.delete-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.cancel-button {
  padding: 11px 24px;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  background-color: #ffffff;
  color: #475569;
  font-weight: 700;
  cursor: pointer;
}

.cancel-button:hover {
  background-color: #f1f5f9;
}

.form-error {
  margin-bottom: 16px;
  color: #b91c1c;
  font-size: 14px;
}

.form-success,
.global-success {
  margin-bottom: 16px;
  color: #15803d;
  font-size: 14px;
}

.delete-error {
  margin-bottom: 20px;
  padding: 14px 16px;
  border: 1px solid #fecaca;
  border-radius: 8px;
  background-color: #fef2f2;
  color: #b91c1c;
  font-size: 14px;
  line-height: 1.7;
}

/* Category Grid */
.category-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 20px;
}

.category-card {
  padding: 24px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  background-color: #ffffff;
  box-shadow: 0 3px 12px rgb(15 23 42 / 4%);
}

.category-card-header,
.edit-form-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 20px;
}

.category-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 38px;
  height: 38px;
  border-radius: 10px;
  background-color: #dbeafe;
  color: #1d4ed8;
  font-size: 23px;
  font-weight: 800;
}

.category-id {
  color: #94a3b8;
  font-size: 12px;
}

.category-card h3,
.edit-form h3 {
  margin-bottom: 12px;
  color: #0f172a;
  font-size: 19px;
  line-height: 1.5;
}

.category-description {
  min-height: 60px;
  color: #64748b;
  font-size: 14px;
  line-height: 1.8;
}

.category-footer {
  display: flex;
  flex-direction: column;
  gap: 6px;
  margin-top: 20px;
  padding-top: 16px;
  border-top: 1px solid #e2e8f0;
  color: #64748b;
  font-size: 12px;
}

.category-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-top: 20px;
}

.edit-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

/* Loading and Error */
.loading-state {
  padding: 64px 24px;
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
  color: #991b1b;
}

/* Empty */
.empty-state {
  padding: 64px 24px;
  border: 1px dashed #cbd5e1;
  border-radius: 12px;
  background-color: #ffffff;
  text-align: center;
}

.empty-state h2 {
  color: #334155;
}

.empty-state p {
  color: #64748b;
}

/* Responsive */
@media (max-width: 900px) {
  .category-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 600px) {
  .categories-page {
    padding: 32px 16px 56px;
  }

  .page-header h1 {
    font-size: 28px;
  }

  .summary-card {
    width: 100%;
  }

  .category-form,
  .edit-form {
    padding: 20px;
  }

  .category-grid {
    grid-template-columns: 1fr;
  }

  .submit-button,
  .cancel-button,
  .edit-button,
  .delete-button {
    width: 100%;
  }

  .category-actions,
  .edit-actions {
    flex-direction: column;
  }
}
</style>