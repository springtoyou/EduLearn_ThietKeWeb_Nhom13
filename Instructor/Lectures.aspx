<%@ Page Language="C#" MasterPageFile="~/Instructor/Instructor.master" AutoEventWireup="true" CodeBehind="Lectures.aspx.cs" Inherits="EduLearn.Instructor.Lectures" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">Quản lý bài giảng</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">
    <asp:Label ID="lblAlert" runat="server" CssClass="alert-banner alert-banner--success" Visible="false" />

    <section class="card toolbar">
        <div class="toolbar__filters">
            <asp:DropDownList ID="ddlCourse" runat="server" CssClass="ctrl-select" />
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="ctrl-select">
                <asp:ListItem Value="" Text="Tất cả trạng thái" />
                <asp:ListItem Value="Đã xuất bản" Text="Đã xuất bản" />
                <asp:ListItem Value="Bản nháp" Text="Bản nháp" />
            </asp:DropDownList>
            <asp:TextBox ID="txtKeyword" runat="server" CssClass="ctrl-input" placeholder="Tìm tiêu đề bài giảng..." />
            <asp:Button ID="btnFilter" runat="server" Text="Lọc" CssClass="btn btn--white" OnClick="btnFilter_Click" />
        </div>
        <a class="btn btn--primary" href="LectureEdit.aspx">+ Thêm bài giảng mới</a>
    </section>

    <section class="card">
        <div style="padding:16px 20px; border-bottom:1px solid #E2E8F0; display:flex; justify-content:space-between; align-items:center;">
            <h3>Danh sách bài giảng (<asp:Literal ID="litCount" runat="server" />)</h3>
        </div>

        <asp:Panel ID="pnlEmpty" runat="server" CssClass="empty" Visible="false">
            <strong>Chưa có bài giảng phù hợp</strong>
            Bấm “+ Thêm bài giảng mới” để tạo nội dung học tập đầu tiên.
        </asp:Panel>

        <div class="data-table-wrap">
            <table class="data-table">
                <thead>
                    <tr>
                        <th>Môn học</th>
                        <th>Tuần</th>
                        <th>Tiêu đề bài giảng</th>
                        <th>Loại</th>
                        <th>Cập nhật</th>
                        <th>Trạng thái</th>
                        <th>Thao tác</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptLectures" runat="server" OnItemCommand="rptLectures_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><strong><%#: Eval("CourseName") %></strong></td>
                                <td>Tuần <%# Eval("Week") %></td>
                                <td>
                                    <div style="font-weight:600;"><%#: Eval("Title") %></div>
                                    <small style="color:#64748B;"><%#: Eval("Duration") %></small>
                                </td>
                                <td><%#: Eval("Type") %></td>
                                <td><%# Eval("UpdatedAt", "{0:dd/MM/yyyy HH:mm}") %></td>
                                <td>
                                    <span class='<%# Convert.ToString(Eval("Status")) == "Đã xuất bản" ? "chip chip--public" : "chip chip--draft" %>'>
                                        <%#: Eval("Status") %>
                                    </span>
                                </td>
                                <td>
                                    <div class="action-group">
                                        <asp:LinkButton runat="server" CommandName="TogglePublish" CommandArgument='<%# Eval("Id") %>' CssClass="btn-sm btn-sm--green">
                                            <%# Convert.ToString(Eval("Status")) == "Đã xuất bản" ? "Ẩn nháp" : "Xuất bản" %>
                                        </asp:LinkButton>
                                        <a class="btn-sm btn-sm--blue" href='<%# "LectureEdit.aspx?id=" + Eval("Id") %>'>Sửa</a>
                                        <asp:LinkButton runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("Id") %>'
                                            CssClass="btn-sm btn-sm--danger" OnClientClick="return confirm('Xóa bài giảng này?');">Xóa</asp:LinkButton>
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </section>
</asp:Content>