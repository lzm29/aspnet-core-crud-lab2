# Bài tập thực hành số 2 — ASP.NET Core Web API + Axios + EF Core

Dự án mẫu **Quản lý sản phẩm (Product Manager)** gồm backend ASP.NET Core 8 Web API theo mô hình **Controller**, frontend HTML/CSS/JavaScript dùng **Vite + Axios**, database **SQLite** dùng **Entity Framework Core**. Có đủ thao tác CRUD và migration đầu tiên.

## 1. Nội dung đã làm so với yêu cầu đề bài

| Yêu cầu | File / nơi minh họa |
|---|---|
| Tạo project API backend ASP.NET Core | `backend/Lab2.Api/Lab2.Api.csproj`, `Program.cs` |
| Tạo Controller GET, POST, PUT, DELETE | `backend/Lab2.Api/Controllers/ProductsController.cs` |
| Gọi backend bằng Axios GET, POST, PUT, DELETE | `frontend/src/main.js` |
| Cấu hình kết nối database | `backend/Lab2.Api/appsettings.json` và `Data/AppDbContext.cs` |
| Cài EF Core và tạo migration | `Lab2.Api.csproj`, `backend/Lab2.Api/Migrations/` |

### Cấu trúc thư mục

```text
ASP.NET-Core-CRUD-Lab2/
├─ backend/
│  └─ Lab2.Api/
│     ├─ Controllers/ProductsController.cs  # API CRUD
│     ├─ Data/AppDbContext.cs              # DbContext / ánh xạ SQLite
│     ├─ Models/Product.cs                 # Entity
│     ├─ Models/ProductInput.cs            # DTO + validation
│     ├─ Migrations/                       # InitialCreate + snapshot
│     ├─ Properties/launchSettings.json    # API chạy tại localhost:5050
│     ├─ Lab2.Api.csproj                   # EF Core dependencies
│     ├─ Lab2.Api.http                     # Test request GET/POST/PUT/DELETE
│     ├─ appsettings.json                  # ConnectionStrings:DefaultConnection
│     └─ Program.cs                        # DI, EF Core, CORS, controllers
├─ frontend/
│  ├─ index.html                           # Giao diện form & danh sách
│  ├─ src/main.js                          # Axios: get/post/put/delete
│  ├─ src/style.css                        # CSS giao diện responsive
│  ├─ .env.example
│  └─ package.json
├─ .config/dotnet-tools.json               # dotnet-ef version 8.0.32
├─ .devcontainer/devcontainer.json         # Codespaces .NET 8 + Node 22
├─ .github/workflows/build.yml            # GitHub Actions build
└─ README.md
```

## 2. Cài đặt trước khi chạy

- **.NET 8 SDK** (có thể dùng .NET SDK mới hơn, miễn hỗ trợ build `net8.0`): https://dotnet.microsoft.com/download/dotnet/8.0
- **Node.js** bản hỗ trợ Vite 6 (Node 20 hoặc 22): https://nodejs.org/
- **VS Code** (hoặc Visual Studio), Git.
- Kết nối internet lần đầu để NuGet và npm tải thư viện.

Kiểm tra cài đặt trong Terminal:

```bash
dotnet --version
node --version
npm --version
```

## 2a. Chạy online bằng GitHub Codespaces (KHÔNG phải cài .NET/Node trên Windows)

1. Upload source lên GitHub (xem Mục 6), sao cho `README.md`, `backend/`, `frontend/` nằm ngay ở thư mục **gốc repository**.
2. Vào repository → **Code → Codespaces → Create codespace on main**. GitHub đọc `.devcontainer/devcontainer.json` để chuẩn bị **.NET 8 SDK và Node.js 22 trong cloud**.
3. Sau khi VS Code trên trình duyệt hiện lên, chọn **Terminal → New Terminal**. Ở terminal thứ nhất chạy:

```bash
cd backend/Lab2.Api
dotnet restore
dotnet tool restore
dotnet ef database update
dotnet run --no-launch-profile --urls http://0.0.0.0:5050
```

4. Mở **terminal thứ hai**, nhập:

```bash
cd frontend
npm install
npm run dev
```

5. Mở tab **Ports** của Codespaces, tìm port **5173**, chọn **Open in Browser**. Đây là giao diện quản lý sản phẩm. Không dùng `localhost:5173` trên máy tính cá nhân: cần mở địa chỉ forwarded port của Codespaces. Nếu thấy lỗi 502 hoặc frontend báo mất kết nối, kiểm tra backend ở terminal thứ nhất và port 5050.
6. Thêm/sửa/xóa sản phẩm, mở Developer Tools → Network → Fetch/XHR để chụp các request Axios GET, POST, PUT, DELETE; ảnh có thể nộp cùng link GitHub.

**Ghi chú:** Codespaces tốn hạn mức máy ảo GitHub khi chạy; nhớ **Stop codespace** khi làm xong. Đừng xóa Codespace trước khi lưu ảnh hoặc dữ liệu cần thiết vì `lab2.db` chỉ nằm trong môi trường Codespaces và không được đẩy lên GitHub.

## 3. Chạy backend ASP.NET Core

Mở terminal ở thư mục project **ASP.NET-Core-CRUD-Lab2** rồi chạy:

```bash
cd backend/Lab2.Api
dotnet restore
dotnet tool restore
dotnet ef migrations list
dotnet ef database update
dotnet run --launch-profile http
```

> Chú ý: `dotnet tool restore` dùng cấu hình `.config/dotnet-tools.json` ở thư mục gốc để cài `dotnet-ef`. Nếu bạn chạy trên PowerShell, các lệnh `cd`, `dotnet` vẫn dùng tương tự. Không xóa terminal này trong lúc thao tác frontend.

API chạy tại **http://localhost:5050**.

- http://localhost:5050/ — kiểm tra backend.
- http://localhost:5050/api/products — xem JSON danh sách sản phẩm (`[]` khi chưa có dữ liệu).
- Sau `dotnet ef database update`, SQLite tạo file database `backend/Lab2.Api/lab2.db` trên máy bạn. File database được `.gitignore` để không đẩy dữ liệu cá nhân lên GitHub.

### Migration làm những gì?

1. `AppDbContext` mô tả bảng `Products`.
2. `Migrations/20261027130000_InitialCreate.cs` mô tả lệnh tạo bảng và xóa bảng khi rollback.
3. `dotnet ef database update` chạy migration và tạo database/tables.

**Để thực hành tự tạo migration với tên do bạn đặt**, hãy thêm trường mới vào `Models/Product.cs`, cập nhật `AppDbContext` hoặc model, sau đó chạy:

```bash
dotnet ef migrations add AddNewField
dotnet ef database update
```

**Không chạy lại** `dotnet ef migrations add InitialCreate` vì migration `InitialCreate` đã có sẵn trong source code. Khi sửa model, chọn tên migration mới.

## 4. Chạy frontend Axios

Mở **terminal thứ hai**, trở về thư mục gốc project, chạy:

```bash
cd frontend
npm install
npm run dev
```

Truy cập: **http://127.0.0.1:5173**.

Frontend gọi đường dẫn **`/api`** bằng Axios; Vite proxy chuyển đến `http://127.0.0.1:5050/api`. Cách này chạy được ở máy local và GitHub Codespaces mà không cần sửa CORS. Nếu API ở một máy chủ khác, có thể cấu hình `frontend/.env` theo `.env.example`.

## 5. Các endpoint API cần demo

| HTTP Method | Đường dẫn | Ý nghĩa | Kết quả thành công |
|---|---|---|---|
| GET | `/api/products` | Lấy tất cả sản phẩm | `200 OK` + mảng JSON |
| GET | `/api/products/{id}` | Lấy sản phẩm theo ID | `200 OK` |
| POST | `/api/products` | Thêm sản phẩm | `201 Created` |
| PUT | `/api/products/{id}` | Sửa sản phẩm | `204 No Content` |
| DELETE | `/api/products/{id}` | Xóa sản phẩm | `204 No Content` |

Ví dụ JSON body cho POST và PUT:

```json
{
  "name": "Tai nghe Bluetooth",
  "description": "Chống ồn chủ động",
  "price": 590000,
  "quantity": 12
}
```

**Cách chứng minh Axios:** Mở trang frontend, nhấn `F12` → tab **Network** → lọc **Fetch/XHR** → thao tác lần lượt Tải lại, Thêm, Sửa, Xóa. Network sẽ xuất hiện các request GET, POST, PUT và DELETE.

**Cách test API độc lập:** cài extension VS Code "REST Client", mở `backend/Lab2.Api/Lab2.Api.http`, bấm **Send Request** phía trên từng request. Khi dùng GET/PUT/DELETE theo ID, nhớ lấy ID thực tế từ POST/GET.

## 6. Hướng dẫn đẩy lên GitHub

Nếu chưa có repository, tạo repository **Public** trên GitHub với tên `aspnet-core-crud-lab2`. Trong trình duyệt, vào repository → **Add file → Upload files** rồi kéo **các file và thư mục bên trong thư mục đã giải nén** (README.md, backend/, frontend/, .config/, .devcontainer/, .github/, .gitignore) lên; không chỉ upload mỗi ZIP, và không đặt cả project vào một thư mục con của repo. Chọn **Commit changes** để hoàn tất. Nếu đã tạo README bằng GitHub, bạn có thể thay nó bằng README của bài lab.

Mở terminal trong thư mục gốc `ASP.NET-Core-CRUD-Lab2`, chạy:

```bash
git init
git add .
git commit -m "Complete Lab 2 ASP.NET Core CRUD with Axios and EF Core"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/aspnet-core-crud-lab2.git
git push -u origin main
```

Thay `YOUR_USERNAME` bằng tên GitHub của bạn. Nếu đang dùng GitHub Desktop, cũng có thể chọn **Add existing repository** / **Publish repository** rồi publish Public. **Không đẩy** các thư mục `bin`, `obj`, `node_modules` hay file `lab2.db`.

> Lưu ý: GitHub là nơi chứa **source code**, không tự động chạy API hoặc frontend cho người chấm. Người chấm có thể clone, làm theo README rồi chạy trên máy.

## 7. Nộp trên Google Classroom

1. Vào bài "Bài tập thực hành số 2" trên Classroom.
2. Chọn **Thêm hoặc tạo** → **Đường liên kết**, dán link repo GitHub ở dạng `https://github.com/YOUR_USERNAME/aspnet-core-crud-lab2`.
3. Nên chụp ảnh giao diện frontend sau khi thêm sản phẩm và ảnh **Network** hiển thị các request GET/POST/PUT/DELETE. Có thể tải kèm ảnh minh chứng nếu giảng viên yêu cầu.
4. Bấm **Nộp / Turn in** để hoàn tất; chỉ dán liên kết nhưng chưa bấm Nộp có thể chưa tính là đã nộp bài.

Mẫu nhận xét khi nộp:

> Em đã hoàn thành lab số 2 gồm backend ASP.NET Core Web API, Controller CRUD với GET/POST/PUT/DELETE, frontend gọi API qua Axios, cấu hình SQLite bằng EF Core và migration InitialCreate. Mã nguồn: [dán link repo GitHub].

## 8. Xử lý lỗi thường gặp

- **`dotnet is not recognized`**: chưa cài .NET SDK / chưa khởi động lại terminal.
- **`No project was found`**: chưa `cd backend/Lab2.Api` khi chạy `dotnet ef`.
- **`Cannot connect` / `ERR_CONNECTION_REFUSED`**: API chưa chạy hoặc frontend/backend sai port.
- **`SQLite Error 1: no such table: Products`**: chưa chạy `dotnet ef database update` từ backend.
- **Lỗi CORS**: frontend cần chạy tại localhost/127.0.0.1 cổng 5173 như hướng dẫn; nếu đổi cổng frontend, cập nhật `AllowedOrigins` trong `appsettings.json`.
- **`npm` không tìm thấy**: cài Node.js rồi mở lại terminal.

## 9. Công nghệ sử dụng

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core 8 + SQLite
- Controllers + DataAnnotations validation + dependency injection
- Axios + Vite + HTML/CSS/JavaScript
- Git / GitHub
