<%@ Page Language="C#" MasterPageFile="~/Instructor/Instructor.master" AutoEventWireup="true" CodeBehind="Courses.aspx.cs" Inherits="EduLearn.Instructor.Courses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">Môn học của tôi</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">
    <section class="card toolbar">
        <div class="toolbar__filters">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="ctrl-input" placeholder="Tìm theo tên hoặc mã môn học..." />
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="ctrl-select">
                <asp:ListItem Value="" Text="Tất cả trạng thái" />
                <asp:ListItem Value="Đang diễn ra" Text="Đang diễn ra" />
                <asp:ListItem Value="Sắp diễn ra" Text="Sắp diễn ra" />
                <asp:ListItem Value="Hoàn thành" Text="Hoàn thành" />
            </asp:DropDownList>
            <asp:Button ID="btnFilter" runat="server" Text="Lọc môn học" CssClass="btn btn--primary" OnClick="btnFilter_Click" />
            <asp:Button ID="btnReset" runat="server" Text="Đặt lại" CssClass="btn btn--white" OnClick="btnReset_Click" />
        </div>
        <span class="pill"><asp:Literal ID="litResultCount" runat="server" /></span>
    </section>

    <asp:Panel ID="pnlEmpty" runat="server" CssClass="card empty" Visible="false">
        <strong>Không tìm thấy môn học phù hợp</strong>
        Thử thay đổi từ khóa hoặc bộ lọc trạng thái để xem các học phần khác.
    </asp:Panel>

    <div class="course-grid">
        <asp:Repeater ID="rptCourses" runat="server">
            <ItemTemplate>
                <article class="course">
                    <div class="course__thumb">
                        <asp:Image runat="server" CssClass="course__img"
                            ImageUrl='<%# Eval("ThumbnailUrl") %>' AlternateText='<%# Eval("Name") %>'
                            Visible='<%# !string.IsNullOrEmpty(Convert.ToString(Eval("ThumbnailUrl"))) %>' />
                        <span class="tag tag--cat"><%#: Eval("Category") %></span>
                        <span class='<%# Convert.ToString(Eval("Status")) == "Đang diễn ra" ? "tag tag--live" : "tag tag--muted" %>'><%#: Eval("Status") %></span>
                    </div>

                    <div class="course__body">
                        <div>
                            <h3><%#: Eval("Name") %> (<%#: Eval("Id") %>)</h3>
                            <p class="course__desc"><%#: Eval("Description") %></p>
                        </div>

                        <div class="course__stats">
                            <div><strong><%# Eval("StudentCount") %></strong><span>Sinh viên</span></div>
                            <div><strong><%# Eval("Weeks") %> tuần</strong><span>Thời lượng</span></div>
                            <div><strong><%# Eval("LectureCount") %> bài</strong><span><%# Eval("PublishedCount") %> đã xuất bản</span></div>
                        </div>

                        <div class="course__foot">
                            <div class="progress">
                                <div class="progress__head"><span>Tiến độ kỳ</span><strong><%# Eval("Progress") %>%</strong></div>
                                <div class="progress__track"><div class="progress__fill" style='width:<%# Eval("Progress") %>%'></div></div>
                            </div>
                            <a class="btn-manage" href='<%# ResolveUrl("~/Instructor/CourseManage.aspx") + "?id=" + Server.UrlEncode(Convert.ToString(Eval("Id"))) %>'>Quản lý →</a>
                        </div>
                    </div>
                </article>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>