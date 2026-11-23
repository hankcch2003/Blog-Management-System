import axios from 'axios'

// 建立共用 Axios API 實例
const api = axios.create({
  // ASP.NET Core Web API 基本網址
  baseURL: 'https://localhost:7103/api',

  // 請求逾時時間：10 秒
  timeout: 10000,

  // 預設 HTTP 請求標頭
  headers: {
    'Content-Type': 'application/json',
    Accept: 'application/json'
  }
})

// 匯出 API 實例，供其他 Service 使用
export default api