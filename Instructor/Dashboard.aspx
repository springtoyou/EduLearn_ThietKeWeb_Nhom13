<%@ Page Language="C#" MasterPageFile="~/Instructor/Instructor.master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="EduLearn.Instructor.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">Tổng quan giảng viên</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">
    <!-- 1. Lời chào -->
    <section class="card welcome">
        <div class="welcome__text">
            <h1>Xin chào, <asp:Literal ID="litName" runat="server" Mode="Encode" />!</h1>
            <span class="badge-online"><i></i>Đang trực tuyến</span>
            <p>Chào mừng bạn quay lại hệ thống quản lý giảng dạy EduLearn. Chúc bạn một ngày làm việc hiệu quả!</p>
        </div>
        <div class="welcome__actions">
            <span class="date-chip">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
                <asp:Literal ID="litDateChip" runat="server" Mode="Encode" />
            </span>
            <a class="btn btn--primary" href="<%= ResolveUrl("~/Instructor/LectureEdit.aspx") %>">
                <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.6"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
                Thêm bài giảng mới
            </a>
            <a class="btn btn--white" href="<%= ResolveUrl("~/Instructor/Notifications.aspx") %>?new=1">
                <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M3 11l18-8v18L3 13z"/><path d="M11.6 16.8a3 3 0 1 1-5.8-1.6"/></svg>
                Tạo thông báo
            </a>
        </div>
    </section>

    <!-- 2. Thống kê -->
    <section class="stats">
        <article class="card stat">
            <div class="stat__body">
                <span class="stat__label">Tổng số môn học</span>
                <strong class="stat__value"><asp:Literal ID="litTotalCourses" runat="server" /></strong>
                <span class="stat__note">Đang giảng dạy kỳ này</span>
            </div>
            <span class="stat__icon ic-blue">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"/><path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"/></svg>
            </span>
        </article>

        <article class="card stat">
            <div class="stat__body">
                <span class="stat__label">Tổng số sinh viên</span>
                <strong class="stat__value"><asp:Literal ID="litTotalStudents" runat="server" /></strong>
                <span class="stat__note"><asp:Literal ID="litClassCount" runat="server" /> lớp học phần</span>
            </div>
            <span class="stat__icon ic-green">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>
            </span>
        </article>

        <article class="card stat">
            <div class="stat__body">
                <span class="stat__label">Tổng số bài giảng</span>
                <strong class="stat__value"><asp:Literal ID="litTotalLectures" runat="server" /></strong>
                <span class="stat__note">Bao gồm video &amp; bài tập</span>
            </div>
            <span class="stat__icon ic-orange">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="3" width="18" height="18" rx="2"/><polygon points="10 8 16 12 10 16 10 8"/></svg>
            </span>
        </article>

        <article class="card stat">
            <div class="stat__body">
                <span class="stat__label">Bài giảng đã xuất bản</span>
                <strong class="stat__value"><asp:Literal ID="litPublished" runat="server" /></strong>
                <span class="stat__note stat__note--green">Tỉ lệ hoàn thiện <asp:Literal ID="litRate" runat="server" />%</span>
            </div>
            <span class="stat__icon ic-sky">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>
            </span>
        </article>
    </section>

    <!-- 3. Môn học đang phụ trách -->
    <section class="section">
        <div class="section__head">
            <div class="section__title">
                <h2>Môn học đang phụ trách</h2>
                <span class="pill"><asp:Literal ID="litCourseBadge" runat="server" /></span>
                <span class="sub"><asp:Literal ID="litCourseShown" runat="server" /></span>
            </div>
            <a class="link" href="<%= ResolveUrl("~/Instructor/Courses.aspx") %>">Xem tất cả môn học →</a>
        </div>

        <asp:Panel ID="pnlNoCourse" runat="server" CssClass="card empty" Visible="false">
            <strong>Chưa có môn học nào</strong>
            Bạn chưa được phân công môn học trong học kỳ này.
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
                            <span class='<%# GetCourseBadgeClass(Eval("Status")) %>'><%#: Eval("Status") %></span>
                        </div>

                        <div class="course__body">
                            <div>
                                <h3><%#: Eval("Name") %></h3>
                                <p class="course__desc"><%#: Eval("Description") %></p>
                            </div>

                            <div class="course__stats">
                                <div>
                                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/></svg>
                                    <strong><%# Eval("StudentCount") %></strong><span>Sinh viên</span>
                                </div>
                                <div>
                                    <svg class="c-orange" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/></svg>
                                    <strong><%# Eval("Weeks") %> tuần</strong><span>Thời lượng</span>
                                </div>
                                <div>
                                    <svg class="c-green" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><polygon points="10 8 16 12 10 16 10 8"/></svg>
                                    <strong><%# Eval("LectureCount") %> bài</strong><span><%# Eval("PublishedCount") %> đã xuất bản</span>
                                </div>
                            </div>

                            <div class="course__foot">
                                <div class="progress">
                                    <div class="progress__head"><span>Tiến độ kỳ</span><strong><%# Eval("Progress") %>%</strong></div>
                                    <div class="progress__track"><div class="progress__fill" style='width:<%# Eval("Progress") %>%'></div></div>
                                </div>
                                <a class="btn-manage"
                                   href='<%# ResolveUrl("~/Instructor/CourseManage.aspx") + "?id=" + Server.UrlEncode(Convert.ToString(Eval("Id"))) %>'>Quản lý →</a>
                            </div>
                        </div>
                    </article>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </section>

    <!-- 4. Hoạt động gần đây -->
    <section class="section">
        <div>
            <h2>Hoạt động gần đây</h2>
            <p class="sub">Lịch sử cập nhật bài giảng và phân công chuyên môn gần nhất</p>
        </div>

        <asp:Panel ID="pnlNoActivity" runat="server" CssClass="card empty" Visible="false">
            <strong>Chưa có hoạt động nào</strong>
            Các cập nhật bài giảng và phân công của bạn sẽ hiển thị tại đây.
        </asp:Panel>

        <div class="card activities">
            <asp:Repeater ID="rptActivities" runat="server">
                <ItemTemplate>
                    <div class="activity">
                        <div class="activity__left">
                            <span class='<%# GetActivityIconClass(Eval("Type")) %>'><%# GetActivityIcon(Eval("Type")) %></span>
                            <div class="activity__info">
                                <strong><%#: Eval("Title") %></strong>
                                <small><%#: Eval("Meta") %></small>
                            </div>
                        </div>
                        <span class='<%# GetStatusClass(Eval("Status")) %>'><%#: Eval("Status") %></span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </section>
</asp:Content>