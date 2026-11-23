# Blog Management System (部落格管理系統)

使用 Vue 3、ASP.NET Core Web API、Entity Framework Core 與 Microsoft SQL Server 建置的全端部落格管理系統。

本專案為個人全端網頁開發作品，主要實作文章與分類管理功能，透過前端網頁、後端 RESTful API 與關聯式資料庫的整合，練習完整的全端開發流程。

開發過程包含資料庫設計、API 開發、前端頁面實作、前後端串接、資料驗證、錯誤處理及測試，並使用 Git 與 GitHub 管理專案程式碼。

---

## 一、專案簡介

本專案以部落格文章管理為核心，提供文章（Posts）及文章分類（Categories）的管理功能。

使用者可以透過前端網頁瀏覽已發布的文章、搜尋文章、依分類篩選內容，也可以進入管理介面新增、查詢、修改及刪除文章與分類。

前端使用 Vue 3 建置，透過 Axios 呼叫 ASP.NET Core Web API；後端使用 Entity Framework Core 操作 Microsoft SQL Server，完成資料新增、查詢、修改及刪除。

除了基本 CRUD 功能，也處理資料驗證、錯誤回應、資料表關聯、跨來源資源分享（CORS）、響應式網頁設計（RWD）及前後端整合測試。

### 專案目標

- 建立前端、後端與資料庫整合架構。
- 使用 Vue 3 建立文章瀏覽及管理介面。
- 使用 ASP.NET Core Web API 開發 RESTful API。
- 使用 Controller、Service、DTO 整理後端程式架構。
- 使用 Entity Framework Core 操作 SQL Server。
- 實作文章與分類的 CRUD 功能。
- 處理 API 輸入驗證及常見錯誤回應。
- 使用 Swagger、Postman 與 SSMS 進行測試。
- 使用 Git 與 GitHub 管理專案程式碼。

---

## 二、使用技術與開發工具

| 類別 | 技術或工具 |
|---|---|
| 後端語言 | C# |
| 後端框架 | ASP.NET Core Web API (.NET 10) |
| ORM | Entity Framework Core |
| 資料庫 | Microsoft SQL Server |
| 資料庫驗證 | Windows Authentication |
| 前端 | HTML5、CSS3、JavaScript |
| 前端框架 | Vue 3 |
| 前端建置工具 | Vite |
| 前端路由 | Vue Router |
| HTTP 請求 | Axios |
| API 文件及測試 | Swagger / OpenAPI |
| API 自動化測試 | Postman |
| 資料庫管理 | SQL Server Management Studio (SSMS) |
| 版本控制 | Git |
| 原始碼管理 | GitHub |
| 開發環境 | Visual Studio、Visual Studio Code |

以上為本專案使用的主要技術與工具。

---

## 三、系統功能

### 1. 首頁與文章瀏覽

- 顯示已發布的部落格文章。
- 顯示文章標題、摘要、分類及日期。
- 依文章標題或摘要進行搜尋。
- 依文章分類篩選。
- 提供文章排序功能。
- 處理載入中、查無資料及 API 錯誤狀態。

### 2. 文章管理（Posts Management）

提供文章完整 CRUD 功能：

| 功能 | HTTP Method | API Endpoint |
|---|---|---|
| 查詢所有文章 | GET | `/api/posts` |
| 新增文章 | POST | `/api/posts` |
| 查詢單筆文章 | GET | `/api/posts/{id}` |
| 修改文章 | PUT | `/api/posts/{id}` |
| 刪除文章 | DELETE | `/api/posts/{id}` |

文章資料包含：

- Title：文章標題
- Content：文章內容
- Summary：文章摘要
- IsPublished：是否發布
- CreatedAt：建立時間
- UpdatedAt：更新時間
- PublishedAt：發布時間
- CategoryId：文章分類編號

文章管理功能包含：

- 新增文章。
- 查詢文章。
- 修改文章內容。
- 刪除文章。
- 設定文章發布狀態。
- 依據文章發布狀態管理 PublishedAt。
- 指定文章所屬分類。
- 驗證必填欄位及分類編號。

### 3. 分類管理（Categories Management）

提供分類完整 CRUD 功能：

| 功能 | HTTP Method | API Endpoint |
|---|---|---|
| 查詢所有分類 | GET | `/api/categories` |
| 新增分類 | POST | `/api/categories` |
| 查詢單筆分類 | GET | `/api/categories/{id}` |
| 修改分類 | PUT | `/api/categories/{id}` |
| 刪除分類 | DELETE | `/api/categories/{id}` |

分類資料包含：

- Id：分類編號
- Name：分類名稱
- Description：分類描述
- CreatedAt：建立時間

分類管理功能包含：

- 新增文章分類。
- 查詢分類資料。
- 修改分類名稱與描述。
- 刪除分類。
- 驗證分類名稱及欄位長度。
- 防止刪除仍有文章關聯的分類。

### 4. 前端與使用者操作

前端使用 Vue 3 與 Vue Router 建立不同頁面：

| 頁面 | 路由 | 功能 |
|---|---|---|
| 首頁 | `/` | 顯示文章及搜尋功能 |
| 文章列表 | `/posts` | 瀏覽文章 |
| 文章詳細 | `/posts/:id` | 顯示單篇文章 |
| 分類管理 | `/categories` | 管理文章分類 |
| 錯誤頁面 | `/:pathMatch(.*)*` | 處理不存在的路由 |

其他前端功能：

- 共用 Header 與 Footer 元件。
- 使用 Axios 集中管理 API 請求。
- 使用 Vue Router 管理頁面路由。
- 使用元件化方式整理程式碼。
- 提供載入中、錯誤及空資料提示。
- 支援桌面、平板及手機等不同螢幕尺寸。

---

## 四、系統架構

本系統採用前後端分離（Frontend / Backend Separation）架構。

```text
                 使用者
                   |
                   v
          Vue 3 Frontend
          HTML / CSS / JavaScript
                   |
                   v
                Axios
                   |
                   | HTTP Request / JSON
                   |
                   v
        ASP.NET Core Web API
                   |
                   v
              Controllers
                   |
                   v
                  DTOs
                   |
                   v
                Services
                   |
                   v
       Entity Framework Core
                   |
                   v
         Microsoft SQL Server
                   |
                   v
                Database
```

### 前端架構

- Vue 3：建立互動式使用者介面。
- Vue Router：管理前端路由。
- Axios：呼叫後端 API。
- CSS3：負責版面與 RWD。
- Vite：提供前端開發伺服器及正式建置。

### 後端架構

採用 Controller、Service、DTO 與 Data 分層方式：

- Controllers：接收 HTTP 請求並回傳 HTTP 回應。
- DTOs：定義 API 接收及回傳的資料格式，並處理輸入驗證。
- Services：處理文章及分類的商業邏輯。
- Data：管理 Entity Framework Core DbContext。
- Models：定義資料庫實體。
- Migrations：管理資料庫結構變更。

### 資料庫關聯

本系統主要包含兩個資料表：

**Categories**

儲存文章分類資料。

**Posts**

儲存文章內容，並透過 CategoryId 連結 Categories。

```text
Categories
-----------
Id (PK)
Name
Description
CreatedAt
      |
      | 1
      |
      | N
Posts
-----------
Id (PK)
Title
Content
Summary
IsPublished
CreatedAt
UpdatedAt
PublishedAt
CategoryId (FK)
```

資料表關聯：

- 一個 Category 可以對應多篇 Post。
- 每篇 Post 透過 CategoryId 關聯至一個 Category。
- Posts.CategoryId 為外鍵。
- 刪除仍有文章關聯的分類時，API 會回傳 HTTP 409 Conflict，避免破壞既有資料關聯。

---

## 五、API 設計與錯誤處理

本專案使用 RESTful API 設計方式，透過 HTTP Method 表達不同資料操作。

### HTTP Status Code

| Status Code | 說明 | 使用情境 |
|---|---|---|
| 200 OK | 請求成功 | 查詢、修改成功 |
| 201 Created | 成功建立資源 | 新增文章或分類 |
| 204 No Content | 成功處理且無回傳內容 | 刪除成功 |
| 400 Bad Request | 請求資料不正確 | 欄位驗證失敗、分類編號無效 |
| 404 Not Found | 找不到指定資源 | 查詢不存在的文章或分類 |
| 409 Conflict | 資料關聯衝突 | 刪除仍有文章使用的分類 |

### 輸入驗證

使用 DTO 與 Data Annotations 處理 API 輸入資料驗證。

Category：

- Name 為必填欄位，最大長度 100 字元。
- Description 最大長度 500 字元。

Post：

- Title 為必填欄位，最大長度 200 字元。
- Content 為必填欄位。
- Summary 最大長度 500 字元。
- CategoryId 必須為有效的正整數。

透過上述驗證，避免不符合欄位規則的資料進入後續處理流程。

### 資料完整性

- 使用外鍵限制文章與分類的關聯。
- 使用 Entity Framework Core 管理資料表關聯。
- 使用 Restrict 刪除行為避免誤刪仍有關聯的分類。
- 對不存在的資料回傳適當的 HTTP Status Code。

---

## 六、專案目錄結構

```text
Blog-Management-System/
│
├── Backend/
│   └── BlogManagement.Api/
│       ├── BlogManagement.Api.slnx
│       │
│       └── BlogManagement.Api/
│           ├── Controllers/
│           │   ├── CategoriesController.cs
│           │   └── PostsController.cs
│           │
│           ├── Data/
│           │   └── ApplicationDbContext.cs
│           │
│           ├── DTOs/
│           │   ├── Categories/
│           │   │   ├── CategoryResponseDto.cs
│           │   │   ├── CreateCategoryDto.cs
│           │   │   └── UpdateCategoryDto.cs
│           │   │
│           │   └── Posts/
│           │       ├── CreatePostDto.cs
│           │       ├── PostResponseDto.cs
│           │       └── UpdatePostDto.cs
│           │
│           ├── Migrations/
│           │   ├── InitialCreate.cs
│           │   ├── InitialCreate.Designer.cs
│           │   └── ApplicationDbContextModelSnapshot.cs
│           │
│           ├── Models/
│           │   ├── Category.cs
│           │   └── Post.cs
│           │
│           ├── Properties/
│           │   └── launchSettings.json
│           │
│           ├── Services/
│           │   ├── CategoryService.cs
│           │   ├── ICategoryService.cs
│           │   ├── IPostService.cs
│           │   └── PostService.cs
│           │
│           ├── Program.cs
│           └── BlogManagement.Api.csproj
│
├── Frontend/
│   └── blog-management-frontend/
│       ├── public/
│       ├── src/
│       │   ├── components/
│       │   │   ├── AppFooter.vue
│       │   │   └── AppHeader.vue
│       │   │
│       │   ├── router/
│       │   │   └── index.js
│       │   │
│       │   ├── services/
│       │   │   ├── api.js
│       │   │   ├── categoryService.js
│       │   │   └── postService.js
│       │   │
│       │   ├── views/
│       │   │   ├── CategoriesView.vue
│       │   │   ├── HomeView.vue
│       │   │   ├── NotFoundView.vue
│       │   │   ├── PostDetailView.vue
│       │   │   └── PostsView.vue
│       │   │
│       │   ├── App.vue
│       │   ├── main.js
│       │   └── style.css
│       │
│       ├── index.html
│       ├── package.json
│       └── vite.config.js
│
├── Screenshots/
│   ├── 01-Swagger-API-Overview.png
│   ├── ...
│   └── 42-Post-Delete_02.png
│
├── .gitignore
└── README.md
```

---

## 七、開發環境

### 必要工具

開始執行專案前，請先準備：

- Windows 開發環境。
- .NET 10 SDK。
- Visual Studio 或 Visual Studio Code。
- Node.js 與 npm。
- Microsoft SQL Server。
- SQL Server Management Studio（SSMS）。
- Git。
- Google Chrome、Microsoft Edge 或其他現代瀏覽器。

### 開發環境版本

本專案開發與測試使用：

- .NET 10
- Node.js v24.22.1
- npm 11.19.1
- Vue 3
- Vite
- SQL Server

實際執行環境請以本機安裝版本及專案設定為準。

---

## 八、本機安裝與執行方式

### 1. Clone 專案

開啟 PowerShell 或終端機：

```bash
git clone https://github.com/hankcch2003/Blog-Management-System.git
```

進入專案根目錄：

```bash
cd Blog-Management-System
```

### 2. 設定 SQL Server

啟動 SQL Server 服務，並使用 SSMS 確認可以正常連線。

本專案開發環境使用 Windows Authentication，不需要在 Repository 中公開資料庫帳號或密碼。

由於 `appsettings.json` 已加入 `.gitignore`，下載專案後需要在以下目錄建立本機設定檔：

```text
Backend/BlogManagement.Api/BlogManagement.Api/appsettings.json
```

設定範例：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BlogManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

請依照自己的 SQL Server 執行個體名稱及資料庫設定調整 `Server` 與 `Database`。

`TrustServerCertificate=True` 僅為開發環境設定示例，正式部署時應依照實際憑證與安全需求設定。

### 3. 建立資料庫

進入後端專案目錄：

```bash
cd Backend/BlogManagement.Api/BlogManagement.Api
```

還原套件：

```bash
dotnet restore
```

套用 EF Core Migration：

```bash
dotnet ef database update
```

若尚未安裝 EF Core CLI 工具，請先依專案使用的 EF Core 版本安裝相容工具。

完成後可透過 SSMS 確認 Categories 與 Posts 資料表是否建立成功。

### 4. 啟動後端 API

在後端專案目錄執行：

```bash
dotnet build
```

啟動 API：

```bash
dotnet run
```

本專案開發測試使用的 HTTPS API 位址：

```text
https://localhost:7103
```

Swagger 文件：

```text
https://localhost:7103/swagger/index.html
```

請以實際終端機輸出及 `Properties/launchSettings.json` 為準。

### 5. 啟動前端

開啟另一個終端機，進入前端目錄：

```bash
cd Frontend/blog-management-frontend
```

安裝套件：

```bash
npm install
```

啟動 Vite 開發伺服器：

```bash
npm run dev
```

終端機會顯示前端網址，通常為：

```text
http://localhost:5173
```

使用瀏覽器開啟該網址，即可操作 Blog Management System。

### 6. 前後端連線設定

前端 API 基底網址設定於：

```text
Frontend/blog-management-frontend/src/services/api.js
```

請確認 API Base URL 與後端實際啟動網址一致。

若前端無法呼叫後端 API，請檢查：

- 後端 API 是否正常啟動。
- API Base URL 是否正確。
- CORS 是否允許前端開發伺服器來源。
- 瀏覽器 Console 與 Network 是否出現錯誤。

---

## 九、測試與驗證紀錄

本專案在開發過程中進行 API 測試、資料庫檢查及前後端整合測試，確認主要功能是否能正常運作。

### 1. Swagger API 測試

使用 Swagger 測試以下功能：

- Categories CRUD。
- Posts CRUD。
- API 回應格式。
- HTTP Status Code。
- 資料驗證。
- 不存在資料的錯誤處理。

### 2. Postman API 測試

建立 Postman Collection：

```text
Blog Management System API
├── Categories
│   ├── Get All Categories
│   ├── Create Category
│   ├── Get Category By ID
│   ├── Update Category
│   └── Delete Category
│
└── Posts
    ├── Get All Posts
    ├── Create Post
    ├── Get Post By ID
    ├── Update Post
    └── Delete Post
```

測試使用 Collection Variables 保存新增資料的動態 ID，讓後續 GET、PUT、DELETE 請求可以接續執行。

Postman Collection Runner 測試結果：

| 項目 | 結果 |
|---|---:|
| Request 數量 | 10 |
| Assertions | 55 |
| 通過 | 55 |
| 失敗 | 0 |

以上為本次 Postman Collection 的測試結果。

### 3. 錯誤處理與資料完整性測試

已驗證以下情境：

| 測試情境 | 預期結果 | 測試結果 |
|---|---|---|
| 查詢不存在的文章 | 404 Not Found | 通過 |
| 新增無效分類編號的文章 | 400 Bad Request | 通過 |
| 刪除仍有文章關聯的分類 | 409 Conflict | 通過 |
| 無效欄位資料 | 400 Bad Request | 通過 |
| 正常 CRUD 操作 | 正確完成資料操作 | 通過 |

並透過 SQL Server Management Studio 檢查資料表內容與外鍵關聯，確認資料關聯符合預期。

### 4. 前後端整合測試

- 前端可以呼叫後端 API。
- CORS 設定正常。
- 新增、查詢、修改及刪除功能可以正常操作。
- 文章發布狀態及分類資料正確。
- API 錯誤能在前端適當呈現。
- 資料庫內容與前端操作結果一致。

### 5. RWD 與跨瀏覽器測試

測試不同螢幕尺寸：

| 裝置類型 | 測試解析度 |
|---|---|
| Desktop | 1440 × 900 |
| Tablet | 768 × 1024 |
| Mobile | 375 × 812 |

跨瀏覽器測試：

- Google Chrome
- Microsoft Edge
- Mozilla Firefox

檢查首頁、文章列表、文章詳細頁及分類管理頁面，確認主要功能與不同尺寸的版面顯示。

### 6. 建置驗證

後端：

```bash
dotnet build -c Release
```

前端：

```bash
npm run build
```

兩者皆已完成建置驗證。

---

## 十、開發過程中的問題與解決方式

本節整理開發及整合過程中實際處理的問題，以及測試時驗證的錯誤情境。

### 1. 前後端跨來源請求（CORS）

**問題：**

前端 Vite 開發伺服器與 ASP.NET Core API 使用不同的來源位址，瀏覽器會依照跨來源安全政策檢查請求。

若後端沒有正確設定 CORS，前端可能無法正常取得 API 資料。

**處理方式：**

- 在 ASP.NET Core 設定 CORS Policy。
- 指定允許的前端開發來源。
- 在 HTTP Pipeline 中正確套用 CORS。
- 重新啟動後端並透過瀏覽器確認 API 請求。

**驗證結果：**

完成 CORS 設定後，透過瀏覽器確認前端可以正常呼叫後端 API。

### 2. 文章與分類的外鍵關聯

**問題：**

文章必須屬於一個分類，因此新增文章時必須確認 CategoryId 對應的分類存在。

另外，若刪除仍有文章使用的分類，可能造成資料關聯問題。

**處理方式：**

- 使用 Entity Framework Core 建立 Category 與 Post 的關聯。
- 設定 Posts.CategoryId 為外鍵。
- 使用 Restrict 刪除行為。
- 新增文章時驗證分類是否存在。
- 刪除仍有文章關聯的分類時回傳 409 Conflict。

**驗證結果：**

測試確認無效分類編號會被拒絕，且仍有文章關聯的分類無法直接刪除。

### 3. API 輸入驗證與錯誤回應

**問題：**

使用者可能傳入空白標題、超過欄位長度的資料、不存在的文章 ID 或無效分類編號。

若缺少驗證，可能導致不符合規則的資料進入後續處理流程，或讓 API 回傳不適當的錯誤。

**處理方式：**

- 使用 DTO 隔離 API 輸入及輸出資料。
- 使用 Data Annotations 設定必填與長度限制。
- 驗證 CategoryId 是否有效。
- 對不存在的資源回傳 404。
- 對無效資料回傳 400。
- 對資料關聯衝突回傳 409。

**驗證結果：**

透過 Swagger、Postman 與前端操作確認不同錯誤情境能依照預期回應。

### 4. 前端 API 請求與畫面狀態

**問題：**

前端呼叫 API 時，可能遇到資料尚未載入、查無資料、網路連線失敗或後端回傳錯誤等情況。

如果沒有適當的畫面狀態管理，使用者可能無法判斷目前發生什麼問題。

**處理方式：**

- 使用 Axios 集中管理 API 請求。
- 使用 categoryService 與 postService 整理資料操作。
- 在 Vue 頁面處理 loading、error 與 empty states。
- 對錯誤提供適當訊息及重新嘗試方式。

**驗證結果：**

針對載入中、查無資料及 API 錯誤等情況，調整前端頁面的顯示方式。

### 5. RWD 不同螢幕尺寸的版面調整

**問題：**

桌面版面在手機螢幕上可能出現內容超出畫面、元件擠壓或水平捲動等情況。

**處理方式：**

- 使用 CSS Media Queries 調整版面。
- 檢查導覽列、表單、文章卡片及管理介面。
- 分別在 Desktop、Tablet 與 Mobile 尺寸測試。

**驗證結果：**

針對 1440、768 及 375 像素寬度進行版面檢查，確認主要頁面的顯示情況。

### 6. Git 換行格式提示（LF / CRLF）

**問題：**

在 Windows 環境執行 Git Stage 時，Git 顯示：

```text
LF will be replaced by CRLF the next time Git touches it
```

**原因：**

不同作業系統使用的文字檔換行格式可能不同。Git 會依照設定處理 LF 與 CRLF 的轉換。

**處理方式：**

- 確認訊息屬於換行格式警告，而不是 Git 操作失敗。
- 執行 `git diff --cached --check` 檢查暫存內容。
- 確認沒有實際空白字元錯誤後繼續操作。

**驗證結果：**

Git Stage、Commit 及 Push 均成功完成。

### 7. GitHub 上傳前的設定檔與建置產物排除

**問題：**

專案中包含本機資料庫連線設定及建置產物，這些檔案不應直接上傳到公開 Repository。

**處理方式：**

更新 `.gitignore`，排除：

```gitignore
appsettings.json
appsettings.*.json
bin/
obj/
node_modules/
dist/
```

並透過以下指令確認：

```bash
git check-ignore -v Backend/BlogManagement.Api/BlogManagement.Api/appsettings.json
```

**驗證結果：**

確認本機 `appsettings.json`、.NET 建置產物、前端 `node_modules` 與 `dist` 未加入本次 Commit。

---

## 十一、Git 與 GitHub 版本控制流程

本專案使用 Git 管理程式碼，並透過 GitHub 公開 Repository 保存開發成果。

### 1. 確認 Git 狀態

```bash
git status
```

確認目前分支及尚未提交的檔案。

### 2. 檢查修改內容

```bash
git diff
```

檢查尚未加入暫存區的修改。

### 3. 加入 Git 暫存區

```bash
git add README.md .gitignore Backend/ Frontend/ Screenshots/
```

明確指定要加入暫存區的專案資料夾及檔案。

### 4. 檢查暫存內容

```bash
git status --short
git diff --cached --stat
git diff --cached --check
```

確認檔案數量、修改內容及格式。

### 5. 建立 Commit

```bash
git commit -m "Add full-stack blog management system"
```

本次首次完整專案上傳的 Commit：

```text
35a888f
Add full-stack blog management system
```

### 6. 推送至 GitHub

```bash
git push origin main
```

### 7. 確認遠端同步

```bash
git status
```

確認：

```text
Your branch is up to date with 'origin/main'.

nothing to commit, working tree clean
```

完成專案程式碼的 Commit 與 Push，並確認本機分支與 GitHub 遠端 Repository 同步。

---

## 十二、專案成果與學習心得

本專案完成了一套以文章及分類管理為主要功能的全端網頁系統，包含 Vue 3 前端、ASP.NET Core Web API 後端，以及 Microsoft SQL Server 資料庫。

在開發過程中，實際進行了以下工作：

1. 使用 Vue 3 建立前端頁面及共用元件。
2. 使用 Vue Router 管理前端頁面路由。
3. 使用 Axios 串接後端 API。
4. 使用 ASP.NET Core Web API 開發文章及分類管理功能。
5. 使用 Controller、Service 與 DTO 整理後端程式。
6. 使用 Entity Framework Core 操作 SQL Server。
7. 使用 Migration 建立及管理資料庫結構。
8. 設定文章與分類之間的外鍵關聯。
9. 處理 API 欄位驗證及錯誤回應。
10. 設定 CORS 並測試前後端連線。
11. 使用 Swagger 與 Postman 測試 API。
12. 使用 SSMS 檢查資料庫資料及關聯。
13. 調整前端 RWD 版面。
14. 使用 Git 與 GitHub 管理專案程式碼。

透過這次開發，對前端、後端及資料庫之間的資料傳遞方式有了更具體的了解，也實際遇到並處理了 API 驗證、跨來源請求及資料表關聯等問題。

目前系統以文章及分類管理為主要範圍，後續仍可依需求增加其他功能。

---

## 十三、專案畫面與測試截圖

本專案將開發及測試過程中的畫面整理於：

```text
Screenshots/
```

目前共保存 42 張專案開發與測試截圖，內容包含：

- Swagger API 操作與測試。
- Categories CRUD。
- Posts CRUD。
- API 錯誤回應。
- Postman Collection 與測試結果。
- Vue 3 前端首頁。
- 文章列表及詳細內容。
- 分類管理。
- 文章新增、查詢、修改及刪除。

可透過以下連結瀏覽：

[View Project Screenshots](https://github.com/hankcch2003/Blog-Management-System/tree/main/Screenshots)