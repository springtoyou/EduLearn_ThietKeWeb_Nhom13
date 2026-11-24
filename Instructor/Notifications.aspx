<%@ Page Language="C#" MasterPageFile="~/Instructor/Instructor.master" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="EduLearn.Instructor.Notifications" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">Thông báo giảng dạy</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">
    <asp:Label ID="lblAlert" runat="server" CssClass="alert-banner alert-banner--success" Visible="false" />

    <!-- Khung tạo thông báo mới -->
    <section class="card form-card">
        <div class="section__head">
            <h2>Gửi thông báo mới cho sinh viên</h2>
            <asp:Button ID="btnMarkAllRead" runat="server" Text="Đánh dấu tất cả đã đọc" CssClass="btn btn--white" CausesValidation="false" OnClick="btnMarkAllRead_Click" />
        </div>

        <div class="form-grid-2">
            <div class="form-group">
                <label class="form-label">Đối tượng nhận thông báo</label>
                <asp:DropDownList ID="ddlTargetCourse" runat="server" CssClass="ctrl-select" />
            </div>
            <div class="form-group">
                <label class="form-label">Tiêu đề thông báo *</label>
                <asp:TextBox ID="txtNotiTitle" runat="server" CssClass="ctrl-input" placeholder="Ví dụ: Nhắc nhở lịch kiểm tra giữa kỳ" />
                <asp:RequiredFieldValidator ID="rfvNotiTitle" runat="server" ControlToValidate="txtNotiTitle"
                    ValidationGroup="NotiGroup" CssClass="val-err" Display="Dynamic" ErrorMessage="Vui lòng nhập tiêu đề." />
            </div>
        </div>

        <div class="form-group">
            <label class="form-label">Nội dung thông báo *</label>
            <asp:TextBox ID="txtNotiMessage" runat="server" CssClass="ctrl-textarea" TextMode="MultiLine" placeholder="Nhập nội dung thông báo gửi tới lớp học phần..." />
            <asp:RequiredFieldValidator ID="rfvNotiMsg" runat="server" ControlToValidate="txtNotiMessage"
                ValidationGroup="NotiGroup" CssClass="val-err" Display="Dynamic" ErrorMessage="Vui lòng nhập nội dung thông báo." />
        </div>

        <div>
            <asp:Button ID="btnSendNoti" runat="server" Text="Phát hành thông báo" CssClass="btn btn--primary"
                ValidationGroup="NotiGroup" OnClick="btnSendNoti_Click" />
        </div>
    </section>

    <!-- Danh sách thông báo -->
    <section class="section">
        <div class="section__head">
            <h2>Danh sách thông báo</h2>
            <span class="pill"><asp:Literal ID="litUnreadSummary" runat="server" /></span>
        </div>

        <asp:Panel ID="pnlEmpty" runat="server" CssClass="card empty" Visible="false">
            Chưa có thông báo nào.
        </asp:Panel>

        <div class="activity-list" style="display:flex; flex-direction:column; gap:12px;">
            <asp:Repeater ID="rptNotifications" runat="server" OnItemCommand="rptNotifications_ItemCommand">
                <ItemTemplate>
                    <article class='<%# (bool)Eval("IsRead") ? "card noti-card" : "card noti-card noti-card--unread" %>'>
                        <div>
                            <div style="display:flex; align-items:center; gap:8px; margin-bottom:6px;">
                                <span class="pill"><%#: Eval("CourseName") %></span>
                                <small style="color:#64748B;"><%# Eval("CreatedAt", "{0:dd/MM/yyyy HH:mm}") %></small>
                            </div>
                            <h3 style="font-size:15px; margin-bottom:4px;"><%#: Eval("Title") %></h3>
                            <p style="font-size:13px; color:#434655; line-height:1.5;"><%#: Eval("Message") %></p>
                        </div>

                        <div class="action-group">
                            <asp:LinkButton runat="server" CommandName="MarkRead" CommandArgument='<%# Eval("Id") %>'
                                CssClass="btn-sm btn-sm--blue" Visible='<%# !(bool)Eval("IsRead") %>'>Đã đọc</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="DeleteNoti" CommandArgument='<%# Eval("Id") %>'
                                CssClass="btn-sm btn-sm--danger" OnClientClick="return confirm('Xóa thông báo này?');">Xóa</asp:LinkButton>
                        </div>
                    </article>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </section>
</asp:Content>