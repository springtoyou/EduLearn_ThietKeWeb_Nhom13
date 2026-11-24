<%@ Page Title="Trang chủ"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="EduLearn.Student.Dashboard" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet"
        href="<%= ResolveUrl("~/Content/CSS/dashboard.css") %>" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="dashboard">

        <section class="welcome-banner">
            <div class="welcome-content">
                <span class="welcome-label">Không gian học tập của bạn</span>

                <h2>
                    Xin chào,
                    <asp:Literal ID="litWelcomeName" runat="server" />
                    <span class="welcome-wave" aria-hidden="true">👋</span>
                </h2>

                <p>
                    Chào mừng bạn quay trở lại EduLearn.<br />
                    Hãy tiếp tục hành trình học tập hôm nay nhé!
                </p>
            </div>

            <div class="welcome-actions">
                <a href="<%= ResolveUrl("~/Student/MyCourses.aspx") %>" class="welcome-button">
                    <span aria-hidden="true">◎</span>
                    Môn học của tôi
                </a>
                <a href="<%= ResolveUrl("~/Student/Profile.aspx") %>" class="profile-button">
                    Hồ sơ cá nhân
                </a>
                <asp:LinkButton ID="btnLogout" runat="server"
                    CssClass="logout-button"
                    CausesValidation="false"
                    OnClick="btnLogout_Click">
                    Đăng xuất
                </asp:LinkButton>
            </div>
        </section>

        <section class="statistics-grid">

            <a class="stat-card" href="#my-courses">
                <div class="stat-icon stat-blue" aria-hidden="true">
                    <svg viewBox="0 0 24 24">
                        <rect x="4" y="4" width="7" height="7" rx="1.5" />
                        <rect x="13" y="4" width="7" height="7" rx="1.5" />
                        <rect x="4" y="13" width="7" height="7" rx="1.5" />
                        <rect x="13" y="13" width="7" height="7" rx="1.5" />
                    </svg>
                </div>
                <div class="stat-information">
                    <span class="stat-label">Môn đã đăng ký</span>
                    <asp:Label ID="lblTotalCourses"
                        runat="server"
                        CssClass="stat-value"
                        Text="0" />
                    <span class="stat-description">Tổng số môn học</span>
                </div>
            </a>

            <a class="stat-card" href="#my-courses">
                <div class="stat-icon stat-purple" aria-hidden="true">
                    <svg viewBox="0 0 24 24">
                        <circle cx="12" cy="12" r="8" />
                        <path d="M12 8v4l3 2" />
                    </svg>
                </div>
                <div class="stat-information">
                    <span class="stat-label">Đang học</span>
                    <asp:Label ID="lblActiveCourses"
                        runat="server"
                        CssClass="stat-value"
                        Text="0" />
                    <span class="stat-description">Môn đang theo học</span>
                </div>
            </a>

            <a class="stat-card" href="#my-courses">
                <div class="stat-icon stat-green" aria-hidden="true">
                    <svg viewBox="0 0 24 24">
                        <circle cx="12" cy="12" r="8" />
                        <path d="m8.5 12.5 2.2 2.2 4.8-5" />
                    </svg>
                </div>
                <div class="stat-information">
                    <span class="stat-label">Đã hoàn thành</span>
                    <asp:Label ID="lblCompletedCourses"
                        runat="server"
                        CssClass="stat-value"
                        Text="0" />
                    <span class="stat-description">Môn đã hoàn thành</span>
                </div>
            </a>

            <a class="stat-card" href="#recent-lessons">
                <div class="stat-icon stat-orange" aria-hidden="true">
                    <svg viewBox="0 0 24 24">
                        <path d="M6 4.5h9.5A2.5 2.5 0 0 1 18 7v13l-6-3-6 3V7" />
                        <path d="M9 9h6" />
                    </svg>
                </div>
                <div class="stat-information">
                    <span class="stat-label">Bài giảng hoàn thành</span>
                    <asp:Label ID="lblCompletedLessons"
                        runat="server"
                        CssClass="stat-value"
                        Text="0" />
                    <span class="stat-description">Bài học đã hoàn thành</span>
                </div>
            </a>

        </section>

        <section class="dashboard-section" id="my-courses">

            <div class="section-heading">
                <div>
                    <h3>Môn học của tôi</h3>
                    <p>Tiếp tục học tập từ nơi bạn đã dừng lại</p>
                </div>

                <a href="<%= ResolveUrl("~/Student/MyCourses.aspx") %>" class="view-all-link">
                    Xem tất cả <span aria-hidden="true">→</span>
                </a>
            </div>

            <asp:Panel ID="pnlNoCourses"
                runat="server"
                CssClass="empty-state"
                Visible="false">

                <div class="empty-icon" aria-hidden="true">
                    <svg viewBox="0 0 24 24">
                        <path d="M4 5.5A2.5 2.5 0 0 1 6.5 3H20v16H6.5A2.5 2.5 0 0 0 4 21.5Z" />
                        <path d="M8 8h8" />
                        <path d="M8 12h6" />
                    </svg>
                </div>
                <h4>Bạn chưa đăng ký môn học nào</h4>
                <p>Danh sách môn học bạn đăng ký sẽ xuất hiện tại đây.</p>

            </asp:Panel>

            <div class="course-grid">

                <asp:Repeater ID="rptCourses" runat="server">
                    <ItemTemplate>

                        <article class="course-card">

                            <div class="course-card-body">

                                <span class="course-category">
                                    <%#: Eval("CategoryName") %>
                                </span>

                                <h4><%#: Eval("CourseName") %></h4>

                                <div class="course-instructor">
                                    <svg viewBox="0 0 24 24" aria-hidden="true">
                                        <circle cx="12" cy="8" r="4" />
                                        <path d="M4 21v-2a8 8 0 0 1 16 0v2" />
                                    </svg>
                                    <span>Giảng viên: <%#: Eval("InstructorName") %></span>
                                </div>

                                <div class="course-progress-heading">
                                    <span>Tiến độ học tập</span>
                                    <strong><%#: Eval("ProgressText") %></strong>
                                </div>

                                <div class="progress-track">
                                    <div class="progress-fill"
                                        style='<%# "width:" + Eval("ProgressPercent") + "%" %>'>
                                    </div>
                                </div>

                                <div class="course-card-footer">
                                    <span><%#: Eval("ProgressSummary") %></span>
                                </div>

                                <a href='<%# ResolveUrl("~/Student/CourseLearning.aspx?courseId=" + Server.UrlEncode(Convert.ToString(Eval("CourseId")))) %>' class="course-action">
                                    <span aria-hidden="true">▷</span>
                                    Mở bài giảng theo tuần
                                </a>

                            </div>

                        </article>

                    </ItemTemplate>
                </asp:Repeater>

            </div>
            <p id="dashboardSearchEmpty" class="search-empty-state" hidden="hidden">
                Không tìm thấy nội dung phù hợp.
            </p>

        </section>

        <div class="dashboard-bottom">

            <section class="dashboard-panel" id="recent-lessons">

                <div class="section-heading">
                    <div>
                        <h3>Bài giảng gần đây</h3>
                        <p>Những bài học bạn đã truy cập</p>
                    </div>
                </div>

                <asp:Panel ID="pnlNoRecentLessons"
                    runat="server"
                    CssClass="small-empty-state"
                    Visible="false">
                    Chưa có lịch sử học tập.
                </asp:Panel>

                <div class="recent-list">
                    <asp:Repeater ID="rptRecentLessons" runat="server">
                        <ItemTemplate>

                            <div class="recent-lesson">

                                <div class="recent-lesson-icon" aria-hidden="true">
                                    <svg viewBox="0 0 24 24">
                                        <path d="m9 7 8 5-8 5V7Z" />
                                    </svg>
                                </div>

                                <div class="recent-lesson-info">
                                    <strong><%#: Eval("LessonTitle") %></strong>
                                    <span><%#: Eval("CourseName") %></span>
                                </div>

                                <span class='<%# "lesson-status " + Eval("StatusCss") %>'>
                                    <%#: Eval("StatusText") %>
                                </span>

                            </div>

                        </ItemTemplate>
                    </asp:Repeater>
                </div>

            </section>

            <section class="dashboard-panel" id="notifications">

                <div class="section-heading">
                    <div>
                        <h3>Thông báo mới</h3>
                        <p>Cập nhật từ hệ thống EduLearn</p>
                    </div>
                </div>

                <asp:Panel ID="pnlNoNotifications"
                    runat="server"
                    CssClass="small-empty-state"
                    Visible="false">
                    Bạn chưa có thông báo mới.
                </asp:Panel>

                <div class="notification-list">
                    <asp:Repeater ID="rptNotifications" runat="server">
                        <ItemTemplate>

                            <div class="notification-item">

                                <div class="notification-indicator" aria-hidden="true"></div>

                                <div class="notification-info">
                                    <strong><%#: Eval("Title") %></strong>
                                    <p><%#: Eval("Content") %></p>
                                    <span><%#: Eval("CreatedAt") %></span>
                                </div>

                            </div>

                        </ItemTemplate>
                    </asp:Repeater>
                </div>

            </section>

        </div>

    </div>

    <script type="text/javascript">
        (function () {
            var search = document.getElementById("txtGlobalSearch");
            if (!search) {
                return;
            }

            var searchableItems = document.querySelectorAll(
                ".course-card, .recent-lesson, .notification-item");
            var searchEmpty = document.getElementById("dashboardSearchEmpty");

            function filterDashboard() {
                var query = search.value.trim().toLocaleLowerCase();
                var matchCount = 0;
                for (var i = 0; i < searchableItems.length; i++) {
                    var text = searchableItems[i].textContent.toLocaleLowerCase();
                    var isMatch = !query || text.indexOf(query) !== -1;
                    searchableItems[i].style.display = isMatch ? "" : "none";
                    if (query && isMatch) {
                        matchCount++;
                    }
                }
                searchEmpty.hidden = !query || matchCount > 0;
            }

            search.addEventListener("input", filterDashboard);
            filterDashboard();
        }());
    </script>

</asp:Content>
