<%@ Page Language="C#" MasterPageFile="~/Instructor/Instructor.master" AutoEventWireup="true" CodeBehind="CourseManage.aspx.cs" Inherits="EduLearn.Instructor.CourseManage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">Quản lý học phần</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">
    <asp:Panel ID="pnlNotFound" runat="server" CssClass="card empty" Visible="false">
        <strong>Không tìm thấy môn học</strong>
        Môn học không tồn tại hoặc không thuộc quyền phụ trách của bạn.
        <div style="margin-top:12px;"><a class="btn btn--primary" href="Courses.aspx">Quay lại danh sách môn học</a></div>
    </asp:Panel>

    <asp:Panel ID="pnlContent" runat="server">
        <asp:Label ID="lblAlert" runat="server" CssClass="alert-banner alert-banner--success" Visible="false" />

        <!-- Thông tin & cập nhật tiến độ -->
        <section class="card form-card" style="margin-bottom:24px;">
            <div class="section__head">
                <div class="section__title">
                    <h2><asp:Literal ID="litCourseName" runat="server" Mode="Encode" /></h2>
                    <span class="pill"><asp:Literal ID="litCourseCategory" runat="server" Mode="Encode" /></span>
                </div>
                <div class="action-group">
                    <a class="btn btn--white" href="Courses.aspx">&larr; Danh sách môn</a>
                    <asp:HyperLink ID="lnkAddLecture" runat="server" CssClass="btn btn--primary">+ Thêm bài giảng</asp:HyperLink>
                </div>
            </div>

            <div class="form-grid-2">
                <div class="form-group">
                    <label class="form-label">Trạng thái học phần</label>
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="ctrl-select">
                        <asp:ListItem Value="Đang diễn ra" Text="Đang diễn ra" />
                        <asp:ListItem Value="Sắp diễn ra" Text="Sắp diễn ra" />
                        <asp:ListItem Value="Hoàn thành" Text="Hoàn thành" />
                    </asp:DropDownList>
                </div>
                <div class="form-group">
                    <label class="form-label">Tiến độ giảng dạy kỳ (%)</label>
                    <asp:TextBox ID="txtProgress" runat="server" CssClass="ctrl-input" TextMode="Number" min="0" max="100" />
                </div>
            </div>

            <div class="form-group">
                <label class="form-label">Mô tả học phần</label>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="ctrl-textarea" TextMode="MultiLine" />
            </div>

            <div>
                <asp:Button ID="btnSaveCourse" runat="server" Text="Lưu thay đổi môn học" CssClass="btn btn--primary" OnClick="btnSaveCourse_Click" />
            </div>
        </section>

        <!-- Danh sách bài giảng của môn -->
        <section class="card" style="margin-bottom:24px;">
            <div style="padding:18px 20px; border-bottom:1px solid #E2E8F0; display:flex; justify-content:space-between; align-items:center;">
                <h3>Bài giảng thuộc môn học</h3>
                <span class="sub"><asp:Literal ID="litLectureSummary" runat="server" /></span>
            </div>

            <asp:Panel ID="pnlNoLecture" runat="server" CssClass="empty" Visible="false">
                Chưa có bài giảng nào cho môn học này.
            </asp:Panel>

            <div class="data-table-wrap">
                <table class="data-table">
                    <thead>
                        <tr>
                            <th>Tuần</th>
                            <th>Tiêu đề bài giảng</th>
                            <th>Loại</th>
                            <th>Thời lượng</th>
                            <th>Trạng thái</th>
                            <th>Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptLectures" runat="server" OnItemCommand="rptLectures_ItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td><strong>Tuần <%# Eval("Week") %></strong></td>
                                    <td><%#: Eval("Title") %></td>
                                    <td><%#: Eval("Type") %></td>
                                    <td><%#: Eval("Duration") %></td>
                                    <td>
                                        <span class='<%# Convert.ToString(Eval("Status")) == "Đã xuất bản" ? "chip chip--public" : "chip chip--draft" %>'>
                                            <%#: Eval("Status") %>
                                        </span>
                                    </td>
                                    <td>
                                        <div class="action-group">
                                            <asp:LinkButton runat="server" CommandName="TogglePublish" CommandArgument='<%# Eval("Id") %>' CssClass="btn-sm btn-sm--green">
                                                <%# Convert.ToString(Eval("Status")) == "Đã xuất bản" ? "Gỡ xuất bản" : "Xuất bản" %>
                                            </asp:LinkButton>
                                            <a class="btn-sm btn-sm--blue" href='<%# "LectureEdit.aspx?id=" + Eval("Id") %>'>Sửa</a>
                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </section>

        <!-- Danh sách sinh viên lớp học phần -->
        <section class="card">
            <div style="padding:18px 20px; border-bottom:1px solid #E2E8F0;">
                <h3>Danh sách sinh viên tiêu biểu trong lớp</h3>
            </div>
            <div class="data-table-wrap">
                <table class="data-table">
                    <thead>
                        <tr>
                            <th>Mã SV</th>
                            <th>Họ và tên</th>
                            <th>Email</th>
                            <th>Tiến độ học tập</th>
                            <th>Trạng thái</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptStudents" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><strong><%#: Eval("StudentCode") %></strong></td>
                                    <td><%#: Eval("FullName") %></td>
                                    <td><%#: Eval("Email") %></td>
                                    <td><strong><%# Eval("CompletedPercent") %>%</strong></td>
                                    <td><span class="chip chip--official"><%#: Eval("Status") %></span></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </section>
    </asp:Panel>
</asp:Content>