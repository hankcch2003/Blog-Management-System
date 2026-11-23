import api from './api'

// 取得所有文章
export const getPosts = async () => {
  const response = await api.get('/posts')
  return response.data
}

// 取得指定 ID 的文章
export const getPostById = async (id) => {
  const response = await api.get(`/posts/${id}`)
  return response.data
}

// 新增文章
export const createPost = async (postData) => {
  const response = await api.post('/posts', postData)
  return response.data
}

// 修改指定 ID 的文章
export const updatePost = async (id, postData) => {
  const response = await api.put(`/posts/${id}`, postData)
  return response.data
}

// 刪除指定 ID 的文章
export const deletePost = async (id) => {
  await api.delete(`/posts/${id}`)
}