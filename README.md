# 🛒 Techshop Project

Hệ thống thương mại điện tử được xây dựng với **React**, **ASP.NET Core Web API** và **Microsoft SQL Server**.

---

## 🏗️ Công nghệ sử dụng

| Tầng | Công nghệ |
|------|-----------|
| Frontend | React (JavaScript), Vite |
| Backend | ASP.NET Core Web API (.NET 8) |
| Database | Microsoft SQL Server, Cloudinary | 
| ORM | Entity Framework Core |
| Authentication | JWT |
| Version Control | Git / GitHub |

---

## 📁 Cấu trúc project

```
E-commerce/
├── Frontend/E-commerce/     ← Frontend (React + Vite)
│   ├── src/
│   │   ├── components/      ← UI components
│   │   ├── pages/           ← Các trang
│   │   ├── services/        ← Gọi API
│   │   └── types/           ← TypeScript types
│   ├── public/
│   ├── index.html
│   └── package.json
│
├── Backend/E-commerce/      ← Backend (ASP.NET Core)
│   ├── Controllers/         ← API endpoints
│   ├── Models/              ← Database models
│   ├── DTOs/                ← Data transfer objects
│   ├── Services/            ← Business logic
│   ├── Repositories/        ← Database queries
│   ├── Middlewares/         ← Xử lý lỗi, auth
│   ├── Properties/
│   ├── appsettings.json
│   └── Program.cs
│
└── Database/                
    └── E-commerce.bak             
```

---

## ⚙️ Hướng dẫn cài đặt và chạy

### Yêu cầu
- [Node.js](https://nodejs.org) (v18+)
- [.NET SDK](https://dotnet.microsoft.com) (v8+)
- [SQL Server](https://www.microsoft.com/sql-server) (Express hoặc Developer)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/ssms)
- [Git](https://git-scm.com)

---

### 1. Clone project

```bash
git clone https://github.com/truongvan25/e-commerce.git
cd E-commerce
```

---

### 2. Tạo Database

Mở **SSMS** → kết nối SQL Server → restore file `Database/E-commerce.bak` 
---

### 3. Cấu hình Backend

Mở file `Backend/E-commerce/appsettings.json`, sửa connection string theo máy của bạn:

**Windows Authentication (không cần username/password):**
```json
"ConnectionStrings": {
  "DBConnection": "data source=.;initial catalog=ECommerceDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;encrypt=false"
}
```

**SQL Server Authentication (có username/password):**
```json
"ConnectionStrings": {
  "DBConnection": "data source=.;initial catalog=ECommerceDB;user id=sa;password=YOUR_PASSWORD;MultipleActiveResultSets=True;encrypt=false"
}
```

Thông tin Cloudinary đã cấu hình sẵn trong appsettings.json. Nếu muốn dùng account riêng, đổi 3 giá trị:

```json
"Cloudinary": {
  "CloudName": "your-cloud-name",
  "ApiKey": "your-api-key",
  "ApiSecret": "your-api-secret"
} 
```

---

### 4. Chạy Backend

```bash
cd Backend/E-commerce
dotnet restore
dotnet run
```

Backend chạy tại: `https://localhost:7020`

Swagger UI: `https://localhost:7020/swagger`

---

### 5. Chạy Frontend

Cấu hình biến môi trường

cd Frontend/E-commerce

copy nội dung file  .env.example vào file .env tạo mới
File .env mặc định:

VITE_API_URL=https://localhost:7270/api

Không cần thay đổi nếu backend chạy đúng port mặc định.

Cài dependencies và chạy

```bash
cd Frontend/E-commerce
npm install
npm run dev
```

Frontend chạy tại: `http://localhost:5173`

---

## 🌿 Quy tắc làm việc với Git

### Tạo branch mới trước khi code

```bash
git checkout -b feature/ten-chuc-nang
```

### Commit thường xuyên

```bash
git add .
git commit -m "Add: mô tả ngắn chức năng"
git push origin feature/ten-chuc-nang
```

### Tạo Pull Request để merge vào main

Vào GitHub → **Pull Requests** → **New Pull Request** → chọn branch của bạn → nhờ người khác review trước khi merge.

### Quy tắc đặt tên branch

| Loại | Ví dụ |
|------|-------|
| Tính năng mới | `feature/login` |
| Sửa lỗi | `fix/loi-dang-nhap` |
| Database | `db/them-bang-orders` |

---
