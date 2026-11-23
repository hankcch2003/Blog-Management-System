import './style.css'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

// 建立 Vue 應用程式
const app = createApp(App)

// 註冊 Vue Router
app.use(router)

// 掛載 Vue 應用程式
app.mount('#app')