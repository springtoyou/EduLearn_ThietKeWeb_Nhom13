<%@ Page Language="C#" MasterPageFile="~/Instructor/Instructor.master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="EduLearn.Instructor.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">Hồ sơ cá nhân</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">
    <div class="profile-layout">
        <!-- Cột trái: Thẻ tóm tắt -->
        <aside class="card" style="padding:24px; text-align:center;">
            <div class="avatar-xl"><asp:Literal ID="litAvatarBig" runat="server" /></div>
            <h2 style="font-size:18px; margin-bottom:4px;"><asp:Literal ID="litProfileName" runat="server" Mode="Encode" /></h2>
            <p style="font-size:13px; color:#2563EB; font-weight:600; margin-bottom:4px;"><asp:Literal ID="litProfileDegree" runat="server" Mode="Encode" /></p>
            <p style="font-size:13px; color:#64748B; margin-bottom:18px;"><asp:Literal ID="litProfileDept" runat="server" Mode="Encode" /></p>

            <div class="course__stats" style="margin-top:8px;">
                <div><strong><asp:Literal ID="litStatCourses" runat="server" /></strong><span>Môn học</span></div>
                <div><strong><asp:Literal ID="litStatStudents" runat="server" /></strong><span>Sinh viên</span></div>
                <div><strong><asp:Literal ID="litStatLectures" runat="server" /></strong><span>Bài giảng</span></div>
            </div>
        </aside>

        <!-- Cột phải: Cập nhật thông tin & Đổi mật khẩu -->
        <div style="display:flex; flex-direction:column; gap:24px;">
            <section class="card form-card">
                <h2>Thông tin giảng viên</h2>
                <asp:Label ID="lblProfileAlert" runat="server" CssClass="alert-banner alert-banner--success" Visible="false" />

                <div class="form-grid-2">
                    <div class="form-group">
                        <label class="form-label">Họ và tên *</label>
                        <asp:TextBox ID="txtFullName" runat="server" CssClass="ctrl-input" />
                        <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtFullName"
                            ValidationGroup="ProfileGroup" CssClass="val-err" Display="Dynamic" ErrorMessage="Vui lòng nhập họ tên." />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Email công vụ</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="ctrl-input" ReadOnly="true" style="background:#F8FAFC;" />
                    </div>
                </div>

                <div class="form-grid-2">
                    <div class="form-group">
                        <label class="form-label">Số điện thoại</label>
                        <asp:TextBox ID="txtPhone" runat="server" CssClass="ctrl-input" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Học vị / Chức danh</label>
                        <asp:TextBox ID="txtDegree" runat="server" CssClass="ctrl-input" />
                    </div>
                </div>

                <div class="form-group">
                    <label class="form-label">Khoa / Bộ môn trực thuộc</label>
                    <asp:TextBox ID="txtDepartment" runat="server" CssClass="ctrl-input" />
                </div>

                <div class="form-group">
                    <label class="form-label">Giới thiệu chuyên môn</label>
                    <asp:TextBox ID="txtBio" runat="server" CssClass="ctrl-textarea" TextMode="MultiLine" />
                </div>

                <div>
                    <asp:Button ID="btnSaveProfile" runat="server" Text="Lưu thông tin cá nhân" CssClass="btn btn--primary"
                        ValidationGroup="ProfileGroup" OnClick="btnSaveProfile_Click" />
                </div>
            </section>

            <section class="card form-card">
                <h2>Đổi mật khẩu đăng nhập</h2>
                <asp:Label ID="lblPassAlert" runat="server" CssClass="alert-banner alert-banner--error" Visible="false" />

                <div class="form-group">
                    <label class="form-label">Mật khẩu hiện tại *</label>
                    <asp:TextBox ID="txtOldPass" runat="server" CssClass="ctrl-input" TextMode="Password" />
                </div>

                <div class="form-grid-2">
                    <div class="form-group">
                        <label class="form-label">Mật khẩu mới (tối thiểu 6 ký tự) *</label>
                        <asp:TextBox ID="txtNewPass" runat="server" CssClass="ctrl-input" TextMode="Password" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Xác nhận mật khẩu mới *</label>
                        <asp:TextBox ID="txtConfirmPass" runat="server" CssClass="ctrl-input" TextMode="Password" />
                    </div>
                </div>

                <div>
                    <asp:Button ID="btnChangePass" runat="server" Text="Cập nhật mật khẩu" CssClass="btn btn--white" OnClick="btnChangePass_Click" />
                </div>
            </section>
        </div>
    </div>
</asp:Content>