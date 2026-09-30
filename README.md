# PRN222 Assignment 02 - FUNewsManagementSystem (Razor Pages & SignalR)

## 📌 Thông tin sinh viên
- **Họ và tên:** Nguyễn Văn Điệp
- **Mã số sinh viên:** QE180203
- **Lớp:** SE19C.NET
- **Môn học:** PRN222 - C# and .NET Programming

---

## 🏗️ Kiến trúc dự án (3-Layers Architecture)
Solution: `NguyenVanDiep_SE19C.NET_A02.sln`

- **BusinessObjects:** Chứa các thực thể dữ liệu (`Category`, `NewsArticle`, `NewsTag`, `Tag`, `SystemAccount`) với DataAnnotations Validation.
- **DataAccessObjects:** `FUNewsManagementContext` (EF Core) và các DAO áp dụng **Singleton Pattern**.
- **Repositories:** Triển khai **Repository Pattern** trung gian giữa Data Access và Business Services.
- **Services:** Tầng xử lý nghiệp vụ trung gian theo kiến trúc 3 lớp chuẩn.
- **NguyenVanDiepRazorPages:** Ứng dụng Web ASP.NET Core Razor Pages tích hợp **SignalR Real-time communication**.

---

## ⚡ Tính năng nổi bật
1. **SignalR Real-time:** Tự động phát sóng (`NewsHub`) khi Staff Thực hiện CRUD tin tức (Tạo, Sửa, Xóa). Toàn bộ người dùng đang mở trang tin tức nhận thông báo Toast tức thì và dữ liệu tự động đồng bộ thời gian thực không cần tải lại trang.
2. **Popup Modal & Confirm Dialog:** Thao tác Create/Update hiển thị dạng Bootstrap Popup Modal; Thao tác Xóa tích hợp SweetAlert2 xác nhận an toàn.
3. **Phân quyền người dùng (Role-based Authorization):**
   - **Khách (Guest):** Xem danh sách tin tức Active, tìm kiếm và xem chi tiết bài viết.
   - **Lecturer:** Đăng nhập, xem tin tức Active.
   - **Staff:** Quản lý danh mục (không cho xóa danh mục có bài viết), quản lý bài viết + thẻ Tags (Real-time), xem lịch sử bài viết của mình, quản lý hồ sơ cá nhân.
   - **Admin:** Quản lý tài khoản người dùng (`SystemAccount`), báo cáo thống kê theo khoảng thời gian (`StartDate` đến `EndDate`), sắp xếp giảm dần theo ngày tạo.

---

## 🚀 Hướng dẫn chạy dự án
1. Chạy script SQL `FUNewsManagement.sql` trên SQL Server Management Studio (SSMS).
2. Kiểm tra chuỗi kết nối trong `NguyenVanDiepRazorPages/appsettings.json`.
3. Mở Solution `NguyenVanDiep_SE19C.NET_A02.sln` trong Visual Studio hoặc chạy dòng lệnh:
```bash
dotnet run --project NguyenVanDiepRazorPages
```
4. Truy cập trình duyệt: `http://localhost:5136` (mặc định mở trang Đăng Nhập).

---

## 🔑 Tài khoản kiểm thử nhanh
- **Admin (appsettings.json):** `admin@FUNewsManagementSystem.org` / `@@abc123@@`
- **Staff (Database):** `IsabellaDavid@FUNewsManagement.org` / `@1`
- **Lecturer (Database):** `EmmaWilliam@FUNewsManagement.org` / `@1`
