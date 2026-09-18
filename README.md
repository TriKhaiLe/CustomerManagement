# Customer Management System (Blazor WebAssembly & ASP.NET Core Web API)

Hệ thống quản lý khách hàng (Customer Management) được xây dựng theo kiến trúc Clean Architecture với Backend sử dụng **ASP.NET Core Web API (.NET 8)** và Frontend sử dụng **Blazor WebAssembly (.NET 8)** kết hợp thư viện giao diện **MudBlazor**.

---

## 📋 Mục lục
1. [Yêu cầu hệ thống](#1-yêu-cầu-hệ-thống)
2. [Khởi chạy Database (Docker SQL Server)](#2-khởi-chạy-database-docker-sql-server)
3. [Cấu hình chuỗi kết nối (Connection String)](#3-cấu-hình-chuỗi-kết-nối-connection-string)
4. [Chạy Database Migration](#4-chạy-database-migration)
5. [Cấu hình chạy đồng thời BE và FE trên Visual Studio](#5-cấu-hình-chạy-đồng-thời-be-và-fe-trên-visual-studio)
6. [Thông tin đăng nhập & Tài khoản mặc định](#6-thông-tin-đăng-nhập--tài-khoản-mặc-định)
7. [Các cổng truy cập & URL mặc định](#7-các-cổng-truy-cập--url-mặc-định)
8. [Xử lý lỗi thường gặp (Troubleshooting)](#8-xử-lý-lỗi-thường-gặp-troubleshooting)

---

## 1. Yêu cầu hệ thống

Trước khi bắt đầu, hãy đảm bảo máy tính của bạn đã cài đặt:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (phiên bản 17.8 trở lên) có kèm workload:
  - **ASP.NET and web development**
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (nếu chạy SQL Server qua Docker container).

---

## 2. Khởi chạy Database (Docker SQL Server)

Dự án sử dụng cơ sở dữ liệu Microsoft SQL Server. Bạn có thể sử dụng Docker để tạo và chạy container SQL Server 2022 cục bộ nhanh chóng bằng lệnh sau trong Terminal (CMD / PowerShell / Bash):

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MyStrong@Password123" \
   -p 1433:1433 --name sql_server_dev \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

> [!TIP]
> - Nếu bạn đã tắt máy hoặc khởi động lại máy tính, bạn chỉ cần khởi động lại container đã tạo bằng lệnh:
>   ```bash
>   docker start sql_server_dev
>   ```
> - Kiểm tra container có đang chạy không: `docker ps`

---

## 3. Cấu hình chuỗi kết nối (Connection String)

Cấu hình kết nối Database nằm tại file:
📁 `Server.Api/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=CustomerManagementDb;User Id=sa;Password=MyStrong@Password123;TrustServerCertificate=True;"
  }
}
```

### Giải thích các thông số:
| Tham số | Giá trị mặc định | Mô tả |
| :--- | :--- | :--- |
| **Server** | `localhost,1433` | Host và port máy chủ SQL Server đang lắng nghe |
| **Database** | `CustomerManagementDb` | Tên database sẽ được tạo tự động khi chạy migration |
| **User Id** | `sa` | Tài khoản quản trị SQL Server |
| **Password** | `MyStrong@Password123` | Mật khẩu SA tương ứng với lệnh Docker ở bước 2 |
| **TrustServerCertificate** | `True` | Bỏ qua xác thực chứng chỉ SSL local để kết nối không bị từ chối |

> [!NOTE]
> Nếu bạn dùng SQL Server cục bộ (LocalDB hoặc SQL Server Express có sẵn trên máy), hãy đổi `DefaultConnection` cho phù hợp, ví dụ:
> `"Server=(localdb)\\mssqllocaldb;Database=CustomerManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"`

---

## 4. Chạy Database Migration

Dự án đã có sẵn migration khởi tạo schema bảng `Customers` và `AppUsers` tại project `Server.Infrastructure/Migrations/`. Bạn chỉ cần thực thi migration để tạo Database và các bảng tương ứng.

Bạn có thể chọn **Cách 1 (Visual Studio)** hoặc **Cách 2 (.NET CLI)**:

### Cách 1: Sử dụng Visual Studio (Package Manager Console)
1. Mở Visual Studio và mở solution `CustomerManagement.sln`.
2. Mở cửa sổ **Package Manager Console**:
   - Menu: **Tools** ➔ **NuGet Package Manager** ➔ **Package Manager Console**.
3. Tại ô **Default project** (ở góc trên cửa sổ Package Manager Console), chọn:
   - 👉 `Server.Infrastructure`
4. Đảm bảo project khởi động (Startup Project) trên Solution Explorer là `Server.Api`.
5. Chạy lệnh sau trong Package Manager Console:
   ```powershell
   Update-Database
   ```

---

### Cách 2: Sử dụng .NET CLI (Terminal / Command Prompt / PowerShell)
Mở Terminal tại thư mục gốc của repository (nơi chứa file `CustomerManagement.sln`) và chạy:

```bash
# 1. Cài đặt công cụ dotnet-ef nếu máy chưa có (chỉ cần chạy 1 lần duy nhất)
dotnet tool install --global dotnet-ef

# 2. Cập nhật cơ sở dữ liệu từ migration đã có
dotnet ef database update --project Server.Infrastructure --startup-project Server.Api
```

*(Tùy chọn: Nếu sau này bạn thay đổi Entity và muốn thêm migration mới:)*
- **Package Manager Console:** `Add-Migration Ten_Migration_Moi` (Default project: `Server.Infrastructure`)
- **.NET CLI:** `dotnet ef migrations add Ten_Migration_Moi --project Server.Infrastructure --startup-project Server.Api`

---

## 5. Cấu hình chạy đồng thời BE và FE trên Visual Studio

Solution gồm 2 project độc lập cần chạy cùng lúc:
- `Server.Api` (Backend Web API)
- `CustomerManagement.Client` (Frontend Blazor WebAssembly)

Dự án đã được cấu hình sẵn profile chạy đa dự án trong `CustomerManagement.slnLaunch.user`. Bạn có thể thiết lập theo các bước dưới đây:

### Bước 1: Thiết lập Multiple Startup Projects
1. Trong cửa sổ **Solution Explorer**, nhấp chuột phải vào Solution `'CustomerManagement'`.
2. Chọn **Configure Startup Projects...** (hoặc **Properties**).
3. Chọn tùy chọn **Common Properties** ➔ **Startup Project**.
4. Tích chọn **Multiple startup projects**.
5. Cấu hình danh sách như sau:
   - **`Server.Api`** ➔ Action: **Start**
   - **`CustomerManagement.Client`** ➔ Action: **Start**
   - *(Các project khác như `Server.Application`, `Server.Domain`, `Server.Infrastructure`, `CustomerManagement.Shared` giữ nguyên là **None**)*.
6. Nhấn nút mũi tên lên/xuống để đưa `Server.Api` lên trên `CustomerManagement.Client` (giúp API sẵn sàng trước khi Client tải).
7. Nhấn **Apply** ➔ **OK**.


### Bước 2: Khởi chạy dự án
- Nhấn **F5** (chạy kèm Debug) hoặc **Ctrl + F5** (chạy không Debug - nhanh và nhẹ hơn).
- Hai cửa sổ trình duyệt sẽ tự động bật lên:
  1. Trình duyệt mở **Swagger UI** của Backend.
  2. Trình duyệt mở **Ứng dụng Blazor WebAssembly** của Frontend.

---

## 6. Thông tin đăng nhập & Tài khoản mặc định

Hệ thống sử dụng JWT Authentication với tài khoản quản trị viên (Admin) được cấu hình tại `Server.Api/appsettings.Development.json`:

- **Username:** `admin`
- **Password:** `Admin@123`

Đăng nhập bằng tài khoản trên tại màn hình Login của ứng dụng Frontend để quản lý danh sách khách hàng.

---

## 7. Các cổng truy cập & URL mặc định

| Ứng dụng | Giao thức | URL |
| :--- | :--- | :--- |
| **Backend Swagger UI** | HTTPS | [https://localhost:7295/swagger](https://localhost:7295/swagger) |
| **Backend Swagger UI** | HTTP | [http://localhost:5001/swagger](http://localhost:5001/swagger) |
| **Frontend Blazor Web** | HTTPS | [https://localhost:7212](https://localhost:7212) |
| **Frontend Blazor Web** | HTTP | [http://localhost:5166](http://localhost:5166) |

> [!NOTE]
> - Backend đã cấu hình CORS cho phép `https://localhost:7212` và `http://localhost:5166`.
> - Frontend cấu hình trỏ tới `ApiBaseUrl` là `https://localhost:7295/` trong `CustomerManagement.Client/wwwroot/appsettings.json`.

---

## 8. Xử lý lỗi thường gặp (Troubleshooting)

### 1. Lỗi chứng chỉ SSL (`Untrusted Certificate` / `ERR_CERT_AUTHORITY_INVALID`)
Nếu trình duyệt hoặc Blazor gọi API bị từ chối do chứng chỉ HTTPS dev của .NET chưa được tin cậy, chạy lệnh sau trong Terminal:
```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

### 2. Lỗi `A network-related or instance-specific error occurred while establishing a connection to SQL Server`
- Đảm bảo Docker container đang chạy: `docker ps`.
- Nếu container đã dừng, khởi động lại: `docker start sql_server_dev`.
- Đảm bảo port `1433` trên máy tính chưa bị chiếm dụng bởi phiên bản SQL Server khác đang cài trực tiếp trên máy.

### 3. Lỗi `401 Unauthorized` hoặc CORS khi gọi API
- Đảm bảo Backend `Server.Api` đang chạy ở đúng port HTTPS `7295` (hoặc HTTP `5001`).
- Kiểm tra file `CustomerManagement.Client/wwwroot/appsettings.json` xem giá trị `ApiBaseUrl` có đúng port đang chạy của API hay không.