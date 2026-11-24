<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="EduLearn.Login" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Đăng nhập - EduLearn</title>

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700&display=swap" rel="stylesheet" />
    <link href="~/Content/CSS/Login.css" rel="stylesheet" runat="server" />
</head>
<body>
    <form id="form1" runat="server" class="page">

        <!-- ================= CỘT TRÁI: THƯƠNG HIỆU ================= -->
        <section class="brand-panel">

            <!-- Logo -->
            <div class="brand-logo">
                <div class="brand-logo__icon">E</div>
                <div class="brand-logo__text">
                    <span class="brand-logo__name">EduLearn</span>
                    <span class="brand-logo__sub">Hệ thống quản lý bài giảng</span>
                </div>
            </div>

            <!-- Nội dung giữa -->
            <div class="brand-center">
                <div class="brand-center__inner">

                    <span class="badge">
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"/><path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"/></svg>
                        Nền tảng đào tạo trực tuyến chuẩn hóa
                    </span>

                    <h1 class="brand-title">Chào mừng bạn đến với <span>EduLearn</span></h1>
                    <p class="brand-desc">
                        Nền tảng học tập trực tuyến giúp bạn tiếp cận bài giảng và kiến thức mọi lúc, mọi nơi với trải nghiệm tối ưu.
                    </p>

                    <!-- Slogan -->
                    <div class="slogan-card">
                        <div class="slogan-card__icon">
                            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"/></svg>
                        </div>
                        <div>
                            <div class="slogan-card__label">Slogan</div>
                            <div class="slogan-card__text">“Học tập dễ dàng – Kiến thức không giới hạn”</div>
                        </div>
                    </div>

                    <!-- Hình minh họa -->
                    <div class="showcase">
                       <img class="showcase__img" src="~/anhimg/anhminhhoahoctap.png" runat="server"
                            alt="Minh họa học tập trực tuyến EduLearn" />
                        <div class="showcase__bar">
                            <span class="showcase__dot"></span>
                            <span class="showcase__roles">3 Vai trò: Sinh viên • Giảng viên • Quản trị viên</span>
                            <a class="showcase__link" href="#">Cổng đăng nhập dùng chung</a>
                        </div>
                    </div>

                </div>
            </div>

            <!-- Điểm nổi bật -->
            <ul class="feature-list">
                <li><span class="feature-list__check">&#10003;</span>Theo dõi tiến độ tuần</li>
                <li><span class="feature-list__check">&#10003;</span>Quản lý bài giảng video</li>
                <li><span class="feature-list__check">&#10003;</span>Phân quyền tự động</li>
            </ul>
        </section>

        <!-- ================= CỘT PHẢI: FORM ĐĂNG NHẬP ================= -->
        <section class="form-panel">
           <div class="form-wrap">
                <!-- ==================== KHUNG 1: ĐĂNG NHẬP ==================== -->
                <asp:Panel ID="pnlLogin" runat="server" CssClass="login-card">

                    <header class="login-header">
                        <h2>Đăng nhập</h2>
                        <p>Vui lòng đăng nhập để tiếp tục sử dụng <strong>EduLearn</strong></p>
                    </header>

                    <!-- Thông báo điều hướng vai trò -->
                    <div class="notice">
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/></svg>
                        <p>
                            Hệ thống tự động nhận diện và điều hướng chính xác đến
                            <strong>Dashboard Sinh viên</strong>, <strong>Giảng viên</strong> hoặc
                            <strong>Quản trị viên</strong> dựa trên quyền tài khoản đã cấp.
                        </p>
                    </div>

                    <!-- Thông báo lỗi (server) -->
                    <asp:Label ID="lblMessage" runat="server" CssClass="alert-error" Visible="false" EnableViewState="false" />

                    <!-- Tên đăng nhập -->
                    <div class="field">
                        <label class="field__label" for="txtUsername">Email hoặc tên đăng nhập <span class="req">*</span></label>
                        <div class="input-wrap">
                            <span class="input-icon">
                                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="4"/><path d="M16 8v5a3 3 0 0 0 6 0v-1a10 10 0 1 0-3.92 7.94"/></svg>
                            </span>
                            <asp:TextBox ID="txtUsername" runat="server" ClientIDMode="Static"
                                CssClass="input" placeholder="Nhập email hoặc tên đăng nhập" MaxLength="100" autocomplete="username" />
                        </div>
                        <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername"
                            ValidationGroup="LoginGroup" CssClass="field__error" Display="Dynamic"
                            ErrorMessage="Vui lòng nhập email hoặc tên đăng nhập." />
                    </div>

                    <!-- Mật khẩu -->
                    <div class="field">
                        <div class="field__row">
                            <label class="field__label" for="txtPassword">Mật khẩu <span class="req">*</span></label>
                            <asp:LinkButton ID="lnkForgotPassword" runat="server" CssClass="link"
                                CausesValidation="false" OnClick="lnkForgotPassword_Click">Quên mật khẩu?</asp:LinkButton>
                        </div>
                        <div class="input-wrap">
                            <span class="input-icon">
                                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="11" width="18" height="11" rx="2" ry="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/></svg>
                            </span>
                            <asp:TextBox ID="txtPassword" runat="server" ClientIDMode="Static" TextMode="Password"
                                CssClass="input input--password" placeholder="Nhập mật khẩu" MaxLength="100" autocomplete="current-password" />
                            <button type="button" id="btnTogglePassword" class="input-toggle" aria-label="Hiện/ẩn mật khẩu">
                                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>
                            </button>
                        </div>
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                            ValidationGroup="LoginGroup" CssClass="field__error" Display="Dynamic"
                            ErrorMessage="Vui lòng nhập mật khẩu." />
                    </div>

                    <!-- Ghi nhớ -->
                    <label class="remember">
                        <asp:CheckBox ID="chkRemember" runat="server" />
                        <span>Ghi nhớ đăng nhập trên thiết bị này</span>
                    </label>

                    <!-- Nút đăng nhập -->
                    <asp:LinkButton ID="btnLogin" runat="server" CssClass="btn-primary"
                        ValidationGroup="LoginGroup" OnClick="btnLogin_Click">
                        <span>Đăng nhập</span>
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="5" y1="12" x2="19" y2="12"/><polyline points="12 5 19 12 12 19"/></svg>
                    </asp:LinkButton>

                    <footer class="login-footer">
                        Bạn chưa có tài khoản? <strong>Liên hệ quản trị viên nhà trường để được cấp tài khoản.</strong>
                    </footer>
                </asp:Panel>

                <!-- ==================== KHUNG 2: QUÊN MẬT KHẨU ==================== -->
                <asp:Panel ID="pnlForgotPassword" runat="server" CssClass="login-card" Visible="false">

                    <header class="login-header">
                        <h2>Quên mật khẩu</h2>
                        <p>Nhập địa chỉ Gmail đã đăng ký để nhận mật khẩu mới từ <strong>EduLearn</strong></p>
                    </header>

                    <!-- Thông báo kết quả gửi mã -->
                    <asp:Label ID="lblForgotMessage" runat="server" CssClass="alert-error" Visible="false" EnableViewState="false" />

                    <!-- Ô nhập Gmail -->
                    <div class="field">
                        <label class="field__label" for="txtForgotEmail">Địa chỉ Gmail <span class="req">*</span></label>
                        <div class="input-wrap">
                            <span class="input-icon">
                                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="2" y="4" width="20" height="16" rx="2"/><path d="m22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7"/></svg>
                            </span>
                            <asp:TextBox ID="txtForgotEmail" runat="server" ClientIDMode="Static"
                                CssClass="input" placeholder="Nhập địa chỉ Gmail của bạn" MaxLength="100" autocomplete="email" />
                        </div>
                        <asp:RequiredFieldValidator ID="rfvForgotEmail" runat="server" ControlToValidate="txtForgotEmail"
                            ValidationGroup="ForgotGroup" CssClass="field__error" Display="Dynamic"
                            ErrorMessage="Vui lòng nhập địa chỉ Gmail." />
                        <asp:RegularExpressionValidator ID="revForgotEmail" runat="server" ControlToValidate="txtForgotEmail"
                            ValidationGroup="ForgotGroup" CssClass="field__error" Display="Dynamic"
                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                            ErrorMessage="Định dạng email không hợp lệ." />
                    </div>

                    <!-- Nút Gửi mã -->
                    <asp:LinkButton ID="btnSendCode" runat="server" CssClass="btn-primary"
                        ValidationGroup="ForgotGroup" OnClick="btnSendCode_Click">
                        <span>Gửi mã</span>
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="22" y1="2" x2="11" y2="13"/><polygon points="22 2 15 22 11 13 2 9 22 2"/></svg>
                    </asp:LinkButton>

                    <footer class="login-footer">
                        <asp:LinkButton ID="lnkBackToLogin" runat="server" CssClass="link"
                            CausesValidation="false" OnClick="lnkBackToLogin_Click">&larr; Quay lại đăng nhập</asp:LinkButton>
                    </footer>
                </asp:Panel>
            </div>
        </section>
    </form>

    <script>
        // Ẩn / hiện mật khẩu
        (function () {
            var btn = document.getElementById('btnTogglePassword');
            var txt = document.getElementById('txtPassword');
            if (!btn || !txt) return;
            btn.addEventListener('click', function () {
                txt.type = (txt.type === 'password') ? 'text' : 'password';
            });
        })();
    </script>
</body>
</html>
