import api from './api'

// 取得所有文章分類
export const getCategories = async () => {
  const response = await api.get('/categories')
  return response.data
}

// 取得指定 ID 的文章分類
export const getCategoryById = async (id) => {
  const response = await api.get(`/categories/${id}`)
  return response.data
}

// 新增文章分類
export const createCategory = async (categoryData) => {
  const response = await api.post('/categories', categoryData)
  return response.data
}

// 修改指定 ID 的文章分類
export const updateCategory = async (id, categoryData) => {
  const response = await api.put(`/categories/${id}`, categoryData)
  return response.data
}

// 刪除指定 ID 的文章分類
export const deleteCategory = async (id) => {
  await api.delete(`/categories/${id}`)
}