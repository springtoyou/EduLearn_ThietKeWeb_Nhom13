<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="EduLearn.Admin.Dashboard" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Quản trị tổng quan | EduLearn</title>
    <link rel="stylesheet"
        href="<%= ResolveUrl("~/Content/CSS/admin-dashboard.css") %>" />
    <link rel="stylesheet"
        href="<%= ResolveUrl("~/Content/CSS/admin-popups.css") %>?v=2" />
</head>
<body class="admin-dashboard-body">
    <form id="form1" runat="server">
        <div class="admin-layout">
            <aside class="admin-sidebar">
                <a class="admin-brand" href="#overview">
                    <span class="admin-brand-mark">E</span>
                    <span><strong>EduLearn</strong><small>LMS Portal</small></span>
                </a>
                <nav class="admin-navigation" aria-label="Điều hướng quản trị">
                    <a class="is-active" href="#overview" aria-label="Tổng quan">
                        <span aria-hidden="true">▦</span><span class="admin-nav-label">Tổng quan</span>
                    </a>
                    <a href="#user-management" aria-label="Quản lý tài khoản">
                        <span aria-hidden="true">♙</span><span class="admin-nav-label">Quản lý tài khoản</span>
                    </a>
                    <a href="#course-management" aria-label="Quản lý môn học">
                        <span aria-hidden="true">▤</span><span class="admin-nav-label">Quản lý môn học</span>
                    </a>
                </nav>
                <div class="admin-sidebar-footer">
                    <span class="admin-sidebar-icon" aria-hidden="true">⚙</span>
                    <span><strong>EduLearn Admin</strong></span>
                </div>
            </aside>

            <div class="admin-main">
                <header class="admin-header">
                    <label class="admin-search">
                        <span aria-hidden="true">⌕</span>
                        <input id="adminSearch" type="search"
                            placeholder="Tìm tài khoản, môn học..."
                            aria-label="Tìm tài khoản hoặc môn học" />
                    </label>
                    <div class="admin-header-actions">
                        <div class="admin-identity">
                            <span class="admin-avatar">
                                <asp:Literal ID="litAdminInitials" runat="server" />
                            </span>
                            <span class="admin-identity-copy">
                                <strong><asp:Literal ID="litAdminName" runat="server" /></strong>
                                <small>Quản trị viên</small>
                            </span>
                        </div>
                        <asp:Button ID="btnLogout" runat="server"
                            CssClass="admin-logout-button"
                            Text="Đăng xuất"
                            OnClick="btnLogout_Click" />
                    </div>
                </header>

                <main class="admin-content" id="overview">
                    <asp:Panel ID="pnlAdminFeedback" runat="server"
                        Visible="false" CssClass="admin-feedback-banner">
                        <asp:Literal ID="litAdminFeedback" runat="server" />
                    </asp:Panel>

                    <section class="admin-welcome-card">
                        <div>
                            <div class="admin-welcome-title">
                                <h1>Xin chào, <asp:Literal ID="litWelcomeName" runat="server" />!</h1>
                                <span class="admin-system-state">
                                    <i aria-hidden="true"></i>
                                    <asp:Literal ID="litSystemStatus" runat="server" />
                                </span>
                            </div>
                            <p>Theo dõi tài khoản, môn học và hoạt động trên EduLearn.</p>
                            <span class="admin-date-pill">
                                <span aria-hidden="true">▣</span>
                                <asp:Literal ID="litToday" runat="server" />
                            </span>
                        </div>
                        <div class="admin-welcome-actions">
                            <button type="button" class="admin-button admin-button-primary"
                                onclick="openAddUserModal()">
                                + Thêm tài khoản mới
                            </button>
                            <button type="button" class="admin-button admin-button-soft"
                                onclick="openAddCourseModal()">
                                + Thêm môn học mới
                            </button>
                        </div>
                    </section>

                    <section class="admin-stat-grid" aria-label="Thống kê hệ thống">
                        <button type="button" class="admin-stat-card admin-stat-btn"
                            onclick="openRoleAccountsPopup('All')">
                            <div class="admin-stat-top">
                                <span>Tổng số tài khoản</span>
                                <span class="admin-stat-icon icon-blue" aria-hidden="true">♙</span>
                            </div>
                            <strong class="admin-stat-value"><asp:Literal ID="litTotalUsers" runat="server" /></strong>
                            <small><asp:Literal ID="litActiveUsers" runat="server" /> tài khoản đang hoạt động</small>
                        </button>
                        <button type="button" class="admin-stat-card admin-stat-btn"
                            onclick="openRoleAccountsPopup('Student')">
                            <div class="admin-stat-top">
                                <span>Sinh viên</span>
                                <span class="admin-stat-icon icon-green" aria-hidden="true">◇</span>
                            </div>
                            <strong class="admin-stat-value"><asp:Literal ID="litStudentCount" runat="server" /></strong>
                            <small>Tài khoản có vai trò Sinh viên</small>
                        </button>
                        <button type="button" class="admin-stat-card admin-stat-btn"
                            onclick="openRoleAccountsPopup('Instructor')">
                            <div class="admin-stat-top">
                                <span>Giảng viên</span>
                                <span class="admin-stat-icon icon-blue" aria-hidden="true">▣</span>
                            </div>
                            <strong class="admin-stat-value"><asp:Literal ID="litInstructorCount" runat="server" /></strong>
                            <small>Tài khoản có vai trò Giảng viên</small>
                        </button>
                        <button type="button" class="admin-stat-card admin-stat-btn"
                            onclick="openCoursesPopup()">
                            <div class="admin-stat-top">
                                <span>Tổng số môn học</span>
                                <span class="admin-stat-icon icon-amber" aria-hidden="true">▤</span>
                            </div>
                            <strong class="admin-stat-value"><asp:Literal ID="litCourseCount" runat="server" /></strong>
                            <small><asp:Literal ID="litEnrollmentCount" runat="server" /> lượt đăng ký</small>
                        </button>
                    </section>

                    <div class="admin-dashboard-grid">
                        <div class="admin-primary-column">
                            <section class="admin-panel" id="user-management">
                                <div class="admin-panel-heading">
                                    <div>
                                        <h2>Quản lý tài khoản</h2>
                                        <p><asp:Literal ID="litUserCount" runat="server" /> tài khoản trong dữ liệu</p>
                                    </div>
                                    <button type="button" class="admin-button admin-button-primary"
                                        onclick="openAddUserModal()">
                                        + Thêm tài khoản
                                    </button>
                                </div>
                                <div class="admin-filter-bar">
                                    <label class="admin-table-search">
                                        <span aria-hidden="true">⌕</span>
                                        <input id="adminUserSearch" type="search"
                                            placeholder="Tìm theo tên, email, mã số..."
                                            aria-label="Tìm tài khoản" />
                                    </label>
                                    <select id="adminRoleFilter" aria-label="Lọc theo vai trò">
                                        <option value="">Tất cả vai trò</option>
                                        <option value="admin">Quản trị viên</option>
                                        <option value="instructor">Giảng viên</option>
                                        <option value="student">Sinh viên</option>
                                    </select>
                                    <select id="adminStatusFilter" aria-label="Lọc theo trạng thái">
                                        <option value="">Tất cả trạng thái</option>
                                        <option value="active">Đang hoạt động</option>
                                        <option value="inactive">Không hoạt động</option>
                                    </select>
                                </div>
                                <div class="admin-table-scroll">
                                    <table class="admin-data-table" id="adminUserTable">
                                        <thead>
                                            <tr>
                                                <th>STT</th>
                                                <th>Họ và tên</th>
                                                <th>Email</th>
                                                <th>Vai trò</th>
                                                <th>Trạng thái</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Repeater ID="rptUsers" runat="server"
                                                OnItemCommand="rptUsers_ItemCommand">
                                                <ItemTemplate>
                                                    <tr data-admin-user="true"
                                                        data-role='<%#: Eval("RoleKey") %>'
                                                        data-status='<%#: Eval("StatusKey") %>'
                                                        data-search='<%#: Eval("SearchText") %>'>
                                                        <td><%#: Eval("Number") %></td>
                                                        <td>
                                                            <div class="admin-person-cell">
                                                                <span class="admin-person-avatar"><%#: Eval("Initials") %></span>
                                                                <span><strong><%#: Eval("FullName") %></strong><small><%#: Eval("UserId") %></small></span>
                                                            </div>
                                                        </td>
                                                        <td class="admin-email-cell"><%#: Eval("Email") %></td>
                                                        <td><span class='<%# "admin-role-pill " + Eval("RoleCss") %>'><%#: Eval("RoleLabel") %></span></td>
                                                        <td>
                                                            <asp:LinkButton ID="btnToggleUserStatus" runat="server"
                                                                CommandName="ToggleUserStatus"
                                                                CommandArgument='<%# Eval("UserId") %>'
                                                                CausesValidation="false"
                                                                ToolTip="Nhấn để đổi trạng thái hoạt động"
                                                                CssClass='<%# "admin-status-pill admin-status-toggle " + Eval("StatusCss") %>'>
                                                                <%#: Eval("StatusLabel") %>
                                                            </asp:LinkButton>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>
                                <asp:Panel ID="pnlNoUsers" runat="server"
                                    CssClass="admin-empty-message" Visible="false">
                                    Chưa có tài khoản trong dữ liệu.
                                </asp:Panel>
                                <p id="adminUserEmptyFilter" class="admin-empty-message"
                                    hidden="hidden">Không tìm thấy tài khoản phù hợp.</p>
                                <div class="admin-table-footer">
                                    <span id="adminVisibleUserCount">
                                        Hiển thị <asp:Literal ID="litUserCountFooter" runat="server" />
                                    </span>
                                    <a href="#user-management">Danh sách tài khoản trong XML</a>
                                </div>
                            </section>

                            <section class="admin-panel admin-course-panel"
                                id="course-management">
                                <div class="admin-panel-heading">
                                    <div>
                                        <h2>Môn học &amp; nội dung</h2>
                                        <p><asp:Literal ID="litCourseCountText" runat="server" /> môn học</p>
                                    </div>
                                    <button type="button" class="admin-button admin-button-primary"
                                        onclick="openAddCourseModal()">
                                        + Thêm môn học mới
                                    </button>
                                </div>
                                <div class="admin-table-scroll">
                                    <table class="admin-data-table admin-course-table"
                                        id="adminCourseTable">
                                        <thead>
                                            <tr>
                                                <th>Mã &amp; tên môn học</th>
                                                <th>Giảng viên</th>
                                                <th>Tuần / bài giảng</th>
                                                <th>Đăng ký</th>
                                                <th>Trạng thái</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Repeater ID="rptCourses" runat="server"
                                                OnItemCommand="rptCourses_ItemCommand">
                                                <ItemTemplate>
                                                    <tr data-admin-course="true"
                                                        data-search='<%#: Eval("SearchText") %>'>
                                                        <td>
                                                            <strong class="admin-course-name"><%#: Eval("CourseName") %></strong>
                                                            <small class="admin-course-code"><%#: Eval("CourseCode") %> · <%#: Eval("Credits") %> tín chỉ</small>
                                                        </td>
                                                        <td><%#: Eval("InstructorName") %></td>
                                                        <td><%#: Eval("WeekCount") %> tuần · <%#: Eval("LessonCount") %> bài</td>
                                                        <td><%#: Eval("EnrollmentCount") %></td>
                                                        <td>
                                                            <asp:LinkButton ID="btnToggleCourseStatus" runat="server"
                                                                CommandName="ToggleCourseStatus"
                                                                CommandArgument='<%# Eval("CourseId") %>'
                                                                CausesValidation="false"
                                                                ToolTip="Nhấn để đổi trạng thái môn học"
                                                                CssClass='<%# "admin-status-pill admin-status-toggle " + Eval("StatusCss") %>'>
                                                                <%#: Eval("StatusLabel") %>
                                                            </asp:LinkButton>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>
                                <asp:Panel ID="pnlNoCourses" runat="server"
                                    CssClass="admin-empty-message" Visible="false">
                                    Chưa có môn học trong dữ liệu.
                                </asp:Panel>
                                <p id="adminCourseEmptyFilter" class="admin-empty-message"
                                    hidden="hidden">Không tìm thấy môn học phù hợp.</p>
                            </section>
                        </div>

                        <aside class="admin-secondary-column">
                            <section class="admin-panel admin-side-panel">
                                <div class="admin-panel-heading">
                                    <div>
                                        <h2>Vai trò tài khoản</h2>
                                        <p>Phân bố theo dữ liệu hiện có</p>
                                    </div>
                                    <span class="admin-side-heading-icon" aria-hidden="true">♙</span>
                                </div>
                                <div class="admin-role-count-list">
                                    <asp:Repeater ID="rptRoleCounts" runat="server">
                                        <ItemTemplate>
                                            <div class="admin-role-count">
                                                <span class='<%# "admin-role-dot " + Eval("RoleCss") %>'></span>
                                                <span><strong><%#: Eval("RoleLabel") %></strong><small><%#: Eval("Description") %></small></span>
                                                <b><%#: Eval("Count") %></b>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </section>
                        </aside>
                    </div>
                    <footer class="admin-page-footer">
                        <span>EduLearn · Bảng điều khiển quản trị</span>
                        <span>Copyright © 2026 EduLearn. All rights reserved.</span>
                    </footer>
                </main>
            </div>
        </div>

        <div id="adminToast" class="admin-toast" role="status" aria-live="polite"
            hidden="hidden"></div>

        <div id="rolePopupModal" class="edu-admin-modal" hidden="hidden" role="dialog" aria-modal="true">
            <div class="edu-admin-modal-card">
                <div class="edu-admin-modal-head">
                    <h3 id="rolePopupTitle">Danh sách tài khoản</h3>
                    <button type="button" class="edu-admin-close" onclick="closeAdminModal('rolePopupModal')">✕</button>
                </div>
                <div class="edu-admin-modal-body">
                    <input type="search" id="rolePopupSearch" class="edu-admin-search"
                        placeholder="Tìm theo tên hoặc trạng thái hoạt động..." oninput="renderRolePopup()" />
                    <table class="edu-admin-table">
                        <thead>
                            <tr>
                                <th>Họ và tên</th>
                                <th>Trạng thái hoạt động</th>
                            </tr>
                        </thead>
                        <tbody id="rolePopupBody"></tbody>
                    </table>
                </div>
            </div>
        </div>

        <div id="coursesPopupModal" class="edu-admin-modal" hidden="hidden" role="dialog" aria-modal="true">
            <div class="edu-admin-modal-card is-wide">
                <div class="edu-admin-modal-head">
                    <h3>Danh sách môn học</h3>
                    <button type="button" class="edu-admin-close" onclick="closeAdminModal('coursesPopupModal')">✕</button>
                </div>
                <div class="edu-admin-modal-body">
                    <input type="search" id="coursesPopupSearch" class="edu-admin-search"
                        placeholder="Tìm theo môn học, mã môn hoặc giảng viên..." oninput="renderCoursesPopup()" />
                    <table class="edu-admin-table">
                        <thead>
                            <tr>
                                <th>Môn học</th>
                                <th>Giảng viên</th>
                                <th>Số người đăng ký</th>
                            </tr>
                        </thead>
                        <tbody id="coursesPopupBody"></tbody>
                    </table>
                </div>
            </div>
        </div>

        <div id="enrollmentsPopupModal" class="edu-admin-modal" hidden="hidden" role="dialog" aria-modal="true">
            <div class="edu-admin-modal-card is-wide">
                <div class="edu-admin-modal-head">
                    <div>
                        <button type="button" class="edu-admin-back" onclick="backToCoursesPopup()">← Danh sách môn học</button>
                        <h3 id="enrollmentsPopupTitle">Danh sách người đăng ký</h3>
                    </div>
                    <button type="button" class="edu-admin-close" onclick="closeAdminModal('enrollmentsPopupModal')">✕</button>
                </div>
                <div class="edu-admin-modal-body">
                    <input type="search" id="enrollmentsPopupSearch" class="edu-admin-search"
                        placeholder="Tìm người đăng ký theo tên, email hoặc trạng thái..." oninput="renderEnrollmentsPopup()" />
                    <table class="edu-admin-table">
                        <thead>
                            <tr>
                                <th>Họ và tên</th>
                                <th>Email</th>
                                <th>Trạng thái</th>
                            </tr>
                        </thead>
                        <tbody id="enrollmentsPopupBody"></tbody>
                    </table>
                </div>
            </div>
        </div>

        <div id="addUserModal" class="edu-admin-modal" hidden="hidden" role="dialog" aria-modal="true">
            <div class="edu-admin-modal-card">
                <div class="edu-admin-modal-head">
                    <h3>Thêm tài khoản mới</h3>
                    <button type="button" class="edu-admin-close" onclick="closeAdminModal('addUserModal')">✕</button>
                </div>
                <div class="edu-admin-modal-body">
                    <div class="edu-admin-grid">
                        <div class="edu-admin-field">
                            <label for="<%= txtNewUsername.ClientID %>">Tên đăng nhập *</label>
                            <asp:TextBox ID="txtNewUsername" runat="server" CssClass="edu-admin-input" />
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= txtNewFullName.ClientID %>">Họ và tên *</label>
                            <asp:TextBox ID="txtNewFullName" runat="server" CssClass="edu-admin-input" />
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= txtNewEmail.ClientID %>">Email *</label>
                            <asp:TextBox ID="txtNewEmail" runat="server" CssClass="edu-admin-input" TextMode="Email" />
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= txtNewPassword.ClientID %>">Mật khẩu *</label>
                            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="edu-admin-input" TextMode="Password" />
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= ddlNewRole.ClientID %>">Vai trò *</label>
                            <asp:DropDownList ID="ddlNewRole" runat="server" CssClass="edu-admin-input">
                                <asp:ListItem Value="Student">Sinh viên (Student)</asp:ListItem>
                                <asp:ListItem Value="Instructor">Giảng viên (Instructor)</asp:ListItem>
                                <asp:ListItem Value="Admin">Quản trị viên (Admin)</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= ddlNewUserStatus.ClientID %>">Trạng thái *</label>
                            <asp:DropDownList ID="ddlNewUserStatus" runat="server" CssClass="edu-admin-input">
                                <asp:ListItem Value="Active">Đang hoạt động (Active)</asp:ListItem>
                                <asp:ListItem Value="Inactive">Ngừng hoạt động (Inactive)</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= txtNewPhone.ClientID %>">Số điện thoại</label>
                            <asp:TextBox ID="txtNewPhone" runat="server" CssClass="edu-admin-input" />
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= ddlNewGender.ClientID %>">Giới tính</label>
                            <asp:DropDownList ID="ddlNewGender" runat="server" CssClass="edu-admin-input">
                                <asp:ListItem Value="">Chưa chọn</asp:ListItem>
                                <asp:ListItem Value="Nữ">Nữ</asp:ListItem>
                                <asp:ListItem Value="Nam">Nam</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="edu-admin-field is-full">
                            <label for="<%= txtNewDateOfBirth.ClientID %>">Ngày sinh</label>
                            <asp:TextBox ID="txtNewDateOfBirth" runat="server" CssClass="edu-admin-input" placeholder="YYYY-MM-DD" />
                        </div>
                    </div>
                    <div class="edu-admin-modal-foot">
                        <button type="button" class="edu-admin-back" onclick="closeAdminModal('addUserModal')">Hủy</button>
                        <asp:Button ID="btnCreateUser" runat="server"
                            Text="Lưu tài khoản"
                            CssClass="admin-button admin-button-primary"
                            OnClick="btnCreateUser_Click" />
                    </div>
                </div>
            </div>
        </div>

        <div id="addCourseModal" class="edu-admin-modal" hidden="hidden" role="dialog" aria-modal="true">
            <div class="edu-admin-modal-card is-wide">
                <div class="edu-admin-modal-head">
                    <h3>Thêm môn học mới</h3>
                    <button type="button" class="edu-admin-close" onclick="closeAdminModal('addCourseModal')">✕</button>
                </div>
                <div class="edu-admin-modal-body">
                    <div class="edu-admin-grid">
                        <div class="edu-admin-field">
                            <label for="<%= txtNewCourseCode.ClientID %>">Mã môn học *</label>
                            <asp:TextBox ID="txtNewCourseCode" runat="server" CssClass="edu-admin-input" placeholder="VD: CS301" />
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= txtNewCourseName.ClientID %>">Tên môn học *</label>
                            <asp:TextBox ID="txtNewCourseName" runat="server" CssClass="edu-admin-input" placeholder="VD: Lập trình mạng" />
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= ddlNewCourseInstructor.ClientID %>">Giảng viên phụ trách *</label>
                            <asp:DropDownList ID="ddlNewCourseInstructor" runat="server" CssClass="edu-admin-input" />
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= ddlNewCourseCategoryId.ClientID %>">Mã danh mục (CategoryId)</label>
                            <asp:DropDownList ID="ddlNewCourseCategoryId" runat="server" CssClass="edu-admin-input">
                                <asp:ListItem Value="CTA001">Lập trình</asp:ListItem>
                                <asp:ListItem Value="CTA002">Cơ sở dữ liệu</asp:ListItem>
                                <asp:ListItem Value="CTA003">Công nghệ phần mềm</asp:ListItem>
                                <asp:ListItem Value="CTA004">An toàn thông tin</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= txtNewCourseCredits.ClientID %>">Số tín chỉ *</label>
                            <asp:TextBox ID="txtNewCourseCredits" runat="server" CssClass="edu-admin-input" TextMode="Number" Text="3" />
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= txtNewCourseTotalWeeks.ClientID %>">Số tuần học *</label>
                            <asp:TextBox ID="txtNewCourseTotalWeeks" runat="server" CssClass="edu-admin-input" TextMode="Number" Text="8" />
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= ddlNewCourseLevel.ClientID %>">Trình độ</label>
                            <asp:DropDownList ID="ddlNewCourseLevel" runat="server" CssClass="edu-admin-input">
                                <asp:ListItem Value="Cơ bản">Cơ bản</asp:ListItem>
                                <asp:ListItem Value="Trung bình">Trung bình</asp:ListItem>
                                <asp:ListItem Value="Nâng cao">Nâng cao</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= ddlNewCourseStatus.ClientID %>">Trạng thái</label>
                            <asp:DropDownList ID="ddlNewCourseStatus" runat="server" CssClass="edu-admin-input">
                                <asp:ListItem Value="Active">Đang hoạt động (Active)</asp:ListItem>
                                <asp:ListItem Value="Inactive">Ngừng hoạt động (Inactive)</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= txtNewCourseSemester.ClientID %>">Học kỳ</label>
                            <asp:TextBox ID="txtNewCourseSemester" runat="server" CssClass="edu-admin-input" Text="1" />
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= txtNewCourseAcademicYear.ClientID %>">Năm học</label>
                            <asp:TextBox ID="txtNewCourseAcademicYear" runat="server" CssClass="edu-admin-input" Text="2026-2027" />
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= txtNewCourseThumbnail.ClientID %>">Ảnh đại diện (Thumbnail)</label>
                            <asp:TextBox ID="txtNewCourseThumbnail" runat="server" CssClass="edu-admin-input" placeholder="VD: lap-trinh-web.jpg" />
                        </div>
                        <div class="edu-admin-field edu-admin-field-bottom">
                            <label class="edu-admin-checkbox-label">
                                <asp:CheckBox ID="chkNewCourseEnrollmentOpen" runat="server" Checked="true" />
                                <span>Mở đăng ký (EnrollmentOpen)</span>
                            </label>
                        </div>
                        <div class="edu-admin-field is-full">
                            <label for="<%= txtNewCourseShortDesc.ClientID %>">Mô tả ngắn *</label>
                            <asp:TextBox ID="txtNewCourseShortDesc" runat="server" CssClass="edu-admin-input" TextMode="MultiLine" Rows="2" />
                        </div>
                        <div class="edu-admin-field is-full">
                            <label for="<%= txtNewCourseDescription.ClientID %>">Mô tả chi tiết</label>
                            <asp:TextBox ID="txtNewCourseDescription" runat="server" CssClass="edu-admin-input" TextMode="MultiLine" Rows="3" />
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= txtNewCourseObjectives.ClientID %>">Mục tiêu học tập (mỗi dòng 1 mục)</label>
                            <asp:TextBox ID="txtNewCourseObjectives" runat="server" CssClass="edu-admin-input" TextMode="MultiLine" Rows="3"
                                placeholder="Hiểu kiến trúc và nguyên lý..." />
                        </div>
                        <div class="edu-admin-field">
                            <label for="<%= txtNewCoursePrerequisites.ClientID %>">Điều kiện tiên quyết (mỗi dòng 1 mục)</label>
                            <asp:TextBox ID="txtNewCoursePrerequisites" runat="server" CssClass="edu-admin-input" TextMode="MultiLine" Rows="3"
                                placeholder="Kiến thức lập trình cơ bản." />
                        </div>
                    </div>
                    <div class="edu-admin-modal-foot">
                        <button type="button" class="edu-admin-back" onclick="closeAdminModal('addCourseModal')">Hủy</button>
                        <asp:Button ID="btnCreateCourse" runat="server"
                            Text="Lưu môn học"
                            CssClass="admin-button admin-button-primary"
                            OnClick="btnCreateCourse_Click" />
                    </div>
                </div>
            </div>
        </div>

        <asp:Literal ID="litAdminPopupsJson" runat="server" />
    </form>

    <script type="text/javascript">
(function () {
    var searchInputs = [
        document.getElementById("adminSearch"),
        document.getElementById("adminUserSearch")
    ];
    var roleFilter = document.getElementById("adminRoleFilter");
    var statusFilter = document.getElementById("adminStatusFilter");
    var userRows = Array.prototype.slice.call(
        document.querySelectorAll("[data-admin-user]"));
    var courseRows = Array.prototype.slice.call(
        document.querySelectorAll("[data-admin-course]"));
    var visibleCount = document.getElementById("adminVisibleUserCount");
    var userEmpty = document.getElementById("adminUserEmptyFilter");
    var courseEmpty = document.getElementById("adminCourseEmptyFilter");
    var navigationLinks = Array.prototype.slice.call(
        document.querySelectorAll(".admin-navigation a"));
    var activeSection = {
        overview: "overview",
        "user-management": "user-management",
        "course-management": "course-management",
        "activity-log": "activity-log"
    };

    function updateActiveNavigation() {
        var section = window.location.hash.substring(1);
        var selectedSection = activeSection[section] || "overview";
        navigationLinks.forEach(function (link) {
            var selected = link.getAttribute("href") ===
                "#" + selectedSection;
            link.classList.toggle("is-active", selected);
            if (selected) {
                link.setAttribute("aria-current", "page");
            } else {
                link.removeAttribute("aria-current");
            }
        });
    }

    function applyFilters() {
        var globalQuery = searchInputs[0]
            ? searchInputs[0].value.trim().toLocaleLowerCase()
            : "";
        var userQuery = searchInputs[1]
            ? searchInputs[1].value.trim().toLocaleLowerCase()
            : "";
        var role = roleFilter ? roleFilter.value : "";
        var status = statusFilter ? statusFilter.value : "";
        var shownUsers = 0;
        var shownCourses = 0;

        userRows.forEach(function (row) {
            var searchText = (row.getAttribute("data-search") || "")
                .toLocaleLowerCase();
            var matches = (!globalQuery ||
                searchText.indexOf(globalQuery) !== -1) &&
                (!userQuery || searchText.indexOf(userQuery) !== -1) &&
                (!role || row.getAttribute("data-role") === role) &&
                (!status || row.getAttribute("data-status") === status);
            row.hidden = !matches;
            if (matches) { shownUsers++; }
        });

        courseRows.forEach(function (row) {
            var searchText = (row.getAttribute("data-search") || "")
                .toLocaleLowerCase();
            var matches = !globalQuery ||
                searchText.indexOf(globalQuery) !== -1;
            row.hidden = !matches;
            if (matches) { shownCourses++; }
        });

        if (visibleCount) {
            visibleCount.textContent = "Hiển thị " + shownUsers +
                " / " + userRows.length + " tài khoản";
        }
        if (userEmpty) {
            userEmpty.hidden =
                userRows.length === 0 || shownUsers !== 0;
        }
        if (courseEmpty) {
            courseEmpty.hidden =
                courseRows.length === 0 || shownCourses !== 0;
        }
    }

    searchInputs.forEach(function (input) {
        if (input) {
            input.addEventListener("input", applyFilters);
        }
    });
    if (roleFilter) { roleFilter.addEventListener("change", applyFilters); }
    if (statusFilter) { statusFilter.addEventListener("change", applyFilters); }
    window.addEventListener("hashchange", updateActiveNavigation);
    navigationLinks.forEach(function (link) {
        link.addEventListener("click", function () {
            var selected = link.getAttribute("href").substring(1);
            if (window.location.hash === "#" + selected) {
                updateActiveNavigation();
            }
        });
    });
    updateActiveNavigation();

    var payload = window.EDU_ADMIN_POPUPS || { accounts: [], courses: [] };
    var activeRole = "All";
    var activeStudents = [];

    function esc(v) {
        return String(v || "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;");
    }

    window.closeAdminModal = function (id) {
        var modal = document.getElementById(id);
        if (modal) { modal.hidden = true; }
    };

    window.openAddUserModal = function () {
        document.getElementById("addUserModal").hidden = false;
    };

    window.openAddCourseModal = function () {
        document.getElementById("addCourseModal").hidden = false;
    };

    window.openRoleAccountsPopup = function (role) {
        activeRole = role || "All";
        var titleEl = document.getElementById("rolePopupTitle");
        titleEl.textContent = activeRole === "Student"
            ? "Chi tiết tài khoản Sinh viên"
            : activeRole === "Instructor"
                ? "Chi tiết tài khoản Giảng viên"
                : "Danh sách tài khoản";
        var input = document.getElementById("rolePopupSearch");
        input.value = "";
        window.renderRolePopup();
        document.getElementById("rolePopupModal").hidden = false;
        input.focus();
    };

    window.renderRolePopup = function () {
        var q = (document.getElementById("rolePopupSearch").value || "").trim().toLowerCase();
        var tbody = document.getElementById("rolePopupBody");
        var list = (payload.accounts || []).filter(function (a) {
            var okRole = activeRole === "All" ||
                String(a.Role || "").toLowerCase() === activeRole.toLowerCase();
            var okText = !q ||
                String(a.FullName || "").toLowerCase().indexOf(q) !== -1 ||
                String(a.StatusText || "").toLowerCase().indexOf(q) !== -1;
            return okRole && okText;
        });

        if (!list.length) {
            tbody.innerHTML = "<tr><td colspan='2'>Không tìm thấy tài khoản phù hợp.</td></tr>";
            return;
        }

        tbody.innerHTML = list.map(function (a) {
            var cls = a.IsActive ? "admin-status-pill is-active" : "admin-status-pill is-neutral";
            return "<tr>" +
                "<td><strong>" + esc(a.FullName) + "</strong></td>" +
                "<td><span class='" + cls + "'>" + esc(a.StatusText) + "</span></td>" +
                "</tr>";
        }).join("");
    };

    window.openCoursesPopup = function () {
        var input = document.getElementById("coursesPopupSearch");
        input.value = "";
        window.renderCoursesPopup();
        document.getElementById("coursesPopupModal").hidden = false;
        input.focus();
    };

    window.renderCoursesPopup = function () {
        var q = (document.getElementById("coursesPopupSearch").value || "").trim().toLowerCase();
        var tbody = document.getElementById("coursesPopupBody");
        var courses = payload.courses || [];
        var rows = [];

        for (var i = 0; i < courses.length; i++) {
            var c = courses[i];
            var match = !q ||
                String(c.CourseName || "").toLowerCase().indexOf(q) !== -1 ||
                String(c.CourseCode || "").toLowerCase().indexOf(q) !== -1 ||
                String(c.InstructorName || "").toLowerCase().indexOf(q) !== -1;
            if (match) {
                rows.push(
                    "<tr>" +
                    "<td><strong>" + esc(c.CourseName) + "</strong>" +
                    "<small class='edu-admin-sub'>" + esc(c.CourseCode) + "</small></td>" +
                    "<td>" + esc(c.InstructorName) + "</td>" +
                    "<td><button type='button' class='edu-admin-count-btn' onclick='openCourseEnrollmentsPopup(" + i + ")'>" +
                    esc(c.EnrolledCount) + " người đăng ký</button></td>" +
                    "</tr>"
                );
            }
        }

        tbody.innerHTML = rows.length
            ? rows.join("")
            : "<tr><td colspan='3'>Không tìm thấy môn học phù hợp.</td></tr>";
    };

    window.openCourseEnrollmentsPopup = function (courseIdx) {
        var c = (payload.courses || [])[courseIdx];
        if (!c) { return; }
        activeStudents = c.Students || [];
        document.getElementById("enrollmentsPopupTitle").textContent =
            "Người đăng ký · " + c.CourseName;
        document.getElementById("coursesPopupModal").hidden = true;
        var input = document.getElementById("enrollmentsPopupSearch");
        input.value = "";
        window.renderEnrollmentsPopup();
        document.getElementById("enrollmentsPopupModal").hidden = false;
        input.focus();
    };

    window.backToCoursesPopup = function () {
        document.getElementById("enrollmentsPopupModal").hidden = true;
        document.getElementById("coursesPopupModal").hidden = false;
    };

    window.renderEnrollmentsPopup = function () {
        var q = (document.getElementById("enrollmentsPopupSearch").value || "").trim().toLowerCase();
        var tbody = document.getElementById("enrollmentsPopupBody");
        var list = activeStudents.filter(function (s) {
            return !q ||
                String(s.FullName || "").toLowerCase().indexOf(q) !== -1 ||
                String(s.Email || "").toLowerCase().indexOf(q) !== -1 ||
                String(s.Status || "").toLowerCase().indexOf(q) !== -1;
        });

        if (!list.length) {
            tbody.innerHTML = "<tr><td colspan='3'>Không tìm thấy người đăng ký phù hợp.</td></tr>";
            return;
        }

        tbody.innerHTML = list.map(function (s) {
            return "<tr>" +
                "<td><strong>" + esc(s.FullName) + "</strong></td>" +
                "<td>" + esc(s.Email) + "</td>" +
                "<td><span class='admin-status-pill is-active'>" + esc(s.Status) + "</span></td>" +
                "</tr>";
        }).join("");
    };

    var activity = payload.Activity || { AdminId: "", AlertKeys: [], Toast: "" };
    var alertKeys = activity.AlertKeys || [];
    var seenStorageKey = "eduAdminActivitySeen_" + (activity.AdminId || "admin");
    var latestAlertKey = alertKeys.reduce(function (max, key) {
        return key > max ? key : max;
    }, 0);
    var activityBadge = document.getElementById("activityBadge");
    var activityNav = document.getElementById("navActivityLog");
    var activityPanel = document.getElementById("activity-log");
    var activityItems = Array.prototype.slice.call(
        document.querySelectorAll(".admin-activity-item"));
    var seenKey = null;

    function readSeenKey() {
        try {
            var raw = window.localStorage.getItem(seenStorageKey);
            if (raw === null) { return null; }
            var value = Number(raw);
            return isNaN(value) ? null : value;
        } catch (e) {
            return null;
        }
    }

    function writeSeenKey(value) {
        try {
            window.localStorage.setItem(seenStorageKey, String(value));
        } catch (e) { /* trình duyệt chặn lưu trữ: bỏ qua */ }
    }

    function renderActivityBadge() {
        if (!activityBadge) { return; }
        var unread = alertKeys.filter(function (key) {
            return key > seenKey;
        }).length;
        activityBadge.textContent = unread > 9 ? "9+" : String(unread);
        activityBadge.hidden = unread === 0;
        if (activityNav) {
            activityNav.setAttribute("aria-label", unread > 0
                ? "Nhật ký hoạt động, " + unread + " thông báo mới"
                : "Nhật ký hoạt động");
        }
    }

    function markActivitiesSeen() {
        if (latestAlertKey > seenKey) {
            seenKey = latestAlertKey;
            writeSeenKey(seenKey);
        }
        renderActivityBadge();
    }

    seenKey = readSeenKey();
    if (seenKey === null) {
        seenKey = latestAlertKey;
        writeSeenKey(seenKey);
    }

    var seenAtLoad = seenKey;
    activityItems.forEach(function (item) {
        var isNew = item.getAttribute("data-alert") === "true" &&
            Number(item.getAttribute("data-sortkey")) > seenAtLoad;
        item.classList.toggle("is-new", isNew);
        var tag = item.querySelector(".admin-activity-new");
        if (tag) { tag.hidden = !isNew; }
    });

    renderActivityBadge();
    if (activityNav) { activityNav.addEventListener("click", markActivitiesSeen); }
    if (activityPanel) { activityPanel.addEventListener("click", markActivitiesSeen); }

    var toastTimer = null;
    window.showAdminToast = function (message) {
        var toast = document.getElementById("adminToast");
        if (!toast || !message) { return; }
        toast.textContent = "";
        var title = document.createElement("strong");
        title.textContent = "Nhật ký hoạt động";
        var body = document.createElement("span");
        body.textContent = message;
        toast.appendChild(title);
        toast.appendChild(body);
        toast.hidden = false;
        window.clearTimeout(toastTimer);
        toastTimer = window.setTimeout(function () { toast.hidden = true; }, 6000);
    };

    var toastEl = document.getElementById("adminToast");
    if (toastEl) {
        toastEl.addEventListener("click", function () { toastEl.hidden = true; });
    }
    if (activity.Toast) { window.showAdminToast(activity.Toast); }
}());
</script>
</body>
</html>
