<%@ Page Title="Hồ sơ cá nhân"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Profile.aspx.cs"
    Inherits="EduLearn.Student.Profile" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet"
        href="<%= ResolveUrl("~/Content/CSS/profile.css") %>" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="student-profile">
        <nav class="profile-breadcrumb" aria-label="Điều hướng">
            <a href="<%: ResolveUrl("~/Student/Dashboard.aspx") %>">Trang chủ</a>
            <span aria-hidden="true">›</span>
            <span>Hồ sơ cá nhân</span>
        </nav>

        <asp:Panel ID="pnlProfileMessage" runat="server"
            CssClass="profile-message" Visible="false" role="status">
            <asp:Label ID="lblProfileMessage" runat="server" />
        </asp:Panel>

        <section class="profile-intro">
            <div class="profile-avatar" aria-hidden="true">
                <asp:Literal ID="litProfileInitials" runat="server" />
            </div>
            <div class="profile-intro-copy">
                <span class="profile-eyebrow">Tài khoản sinh viên</span>
                <h2><asp:Literal ID="litProfileName" runat="server" /></h2>
                <p>Quản lý thông tin cá nhân và thông tin tài khoản EduLearn.</p>
            </div>
            <span class="profile-status">
                <asp:Literal ID="litAccountStatus" runat="server" />
            </span>
        </section>

        <div class="profile-layout">
            <aside class="profile-summary-card">
                <div class="profile-card-heading">
                    <span class="profile-card-icon" aria-hidden="true">◎</span>
                    <div>
                        <h3>Thông tin tài khoản</h3>
                        <p>Thông tin định danh EduLearn</p>
                    </div>
                </div>

                <dl class="profile-account-list">
                    <div>
                        <dt>Mã sinh viên</dt>
                        <dd><asp:Literal ID="litStudentId" runat="server" /></dd>
                    </div>
                    <div>
                        <dt>Tên đăng nhập</dt>
                        <dd><asp:Literal ID="litUsername" runat="server" /></dd>
                    </div>
                    <div>
                        <dt>Email</dt>
                        <dd><asp:Literal ID="litEmail" runat="server" /></dd>
                    </div>
                    <div>
                        <dt>Ngày tham gia</dt>
                        <dd><asp:Literal ID="litCreatedAt" runat="server" /></dd>
                    </div>
                    <div>
                        <dt>Cập nhật lần cuối</dt>
                        <dd><asp:Literal ID="litUpdatedAt" runat="server" /></dd>
                    </div>
                </dl>

                <a class="profile-dashboard-link"
                    href="<%: ResolveUrl("~/Student/Dashboard.aspx") %>">
                    <span aria-hidden="true">←</span> Quay lại trang chủ
                </a>
            </aside>

            <section class="profile-edit-card" aria-labelledby="profileFormTitle">
                <div class="profile-card-heading profile-edit-heading">
                    <span class="profile-card-icon profile-edit-icon" aria-hidden="true">✎</span>
                    <div>
                        <h3 id="profileFormTitle">Thông tin cá nhân</h3>
                        <p>Cập nhật các thông tin bạn muốn hiển thị trên hồ sơ.</p>
                    </div>
                </div>

                <div class="profile-form-grid">
                    <div class="profile-field profile-field-wide">
                        <label for="txtFullName">Họ và tên <span aria-hidden="true">*</span></label>
                        <asp:TextBox ID="txtFullName" runat="server"
                            ClientIDMode="Static" MaxLength="100"
                            CssClass="profile-input" />
                    </div>

                    <div class="profile-field">
                        <label for="txtPhone">Số điện thoại</label>
                        <asp:TextBox ID="txtPhone" runat="server"
                            ClientIDMode="Static" MaxLength="32"
                            CssClass="profile-input"
                            placeholder="Nhập số điện thoại" />
                    </div>

                    <div class="profile-field">
                        <label for="ddlGender">Giới tính</label>
                        <asp:DropDownList ID="ddlGender" runat="server"
                            ClientIDMode="Static" CssClass="profile-input">
                            <asp:ListItem Text="Chưa cập nhật" Value="" />
                            <asp:ListItem Text="Nam" Value="Nam" />
                            <asp:ListItem Text="Nữ" Value="Nữ" />
                            <asp:ListItem Text="Khác" Value="Khác" />
                            <asp:ListItem Text="Không muốn chia sẻ"
                                Value="Không muốn chia sẻ" />
                        </asp:DropDownList>
                    </div>

                    <div class="profile-field">
                        <label for="txtDateOfBirth">Ngày sinh</label>
                        <asp:TextBox ID="txtDateOfBirth" runat="server"
                            ClientIDMode="Static" TextMode="Date"
                            CssClass="profile-input" />
                    </div>

                    <p class="profile-form-note profile-field-wide">
                        Email, tên đăng nhập và mã sinh viên được quản lý bởi hệ thống
                        và không thể thay đổi tại đây.
                    </p>
                </div>

                <div class="profile-form-actions">
                    <a class="profile-cancel-button"
                        href="<%: ResolveUrl("~/Student/Dashboard.aspx") %>">
                        Hủy
                    </a>
                    <asp:Button ID="btnSaveProfile" runat="server"
                        Text="Lưu thay đổi"
                        CssClass="profile-save-button"
                        OnClick="btnSaveProfile_Click" />
                </div>
            </section>
        </div>
    </div>
</asp:Content>
