<%@ Page Title="Bài giảng môn học"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="CourseLearning.aspx.cs"
    Inherits="EduLearn.Student.CourseLearning" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet"
        href="<%= ResolveUrl("~/Content/CSS/course-pages.css") %>" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="course-learning-page">
        <asp:Panel ID="pnlCourseNotFound" runat="server"
            CssClass="course-page-empty" Visible="false">
            <h2>Không tìm thấy môn học</h2>
            <p>Môn học này không tồn tại hoặc đã bị gỡ khỏi danh mục.</p>
            <a class="course-primary-link" href="<%: ResolveUrl("~/Student/MyCourses.aspx") %>">
                Quay lại Môn học của tôi
            </a>
        </asp:Panel>

        <asp:Panel ID="pnlNotEnrolled" runat="server"
            CssClass="course-page-empty" Visible="false">
            <h2>Bạn chưa đăng ký môn học này</h2>
            <p>Hãy xem thông tin môn học và trạng thái đăng ký trước khi truy cập bài giảng.</p>
            <asp:HyperLink ID="lnkCourseDetailsNotEnrolled" runat="server"
                CssClass="course-primary-link">
                Xem chi tiết môn học
            </asp:HyperLink>
        </asp:Panel>

        <asp:Panel ID="pnlCourseLearning" runat="server" Visible="false">
            <nav class="course-breadcrumb learning-breadcrumb" aria-label="Điều hướng">
                <a href="<%: ResolveUrl("~/Student/MyCourses.aspx") %>">Môn học của tôi</a>
                <span aria-hidden="true">›</span>
                <span><asp:Literal ID="litBreadcrumbCourse" runat="server" /></span>
            </nav>

            <section class="course-learning-hero">
                <div>
                    <div class="learning-hero-tags">
                        <span class="course-category-pill">
                            <asp:Literal ID="litCategory" runat="server" />
                        </span>
                        <asp:PlaceHolder ID="phTermBadge" runat="server">
                            <span class="course-term-pill">
                                <asp:Literal ID="litTerm" runat="server" />
                            </span>
                        </asp:PlaceHolder>
                        <span class="course-enrolled-pill">
                            <asp:Literal ID="litEnrollmentStatus" runat="server" />
                        </span>
                    </div>
                    <h2><asp:Literal ID="litCourseName" runat="server" /></h2>
                    <p><asp:Literal ID="litShortDescription" runat="server" /></p>
                    <div class="learning-hero-meta">
                        <span>Giảng viên: <strong>
                            <asp:Literal ID="litInstructorName" runat="server" /></strong></span>
                        <span>Đơn vị: <strong>
                            <asp:Literal ID="litDepartment" runat="server" /></strong></span>
                        <span>Mã môn: <strong>
                            <asp:Literal ID="litCourseCode" runat="server" /></strong></span>
                    </div>
                </div>
                <div class="learning-hero-actions">
                    <asp:HyperLink ID="lnkContinueLearning" runat="server">
                        Tiếp tục học
                    </asp:HyperLink>
                    <asp:HyperLink ID="lnkCourseDetailsHero" runat="server"
                        CssClass="secondary">
                        Chi tiết môn học
                    </asp:HyperLink>
                </div>
            </section>

            <section class="learning-stats-grid" aria-label="Tiến độ môn học">
                <article class="learning-stat-card">
                    <span>Tổng số tuần</span>
                    <div class="learning-stat-value">
                        <strong><asp:Literal ID="litTotalWeeks" runat="server" /></strong>
                        <small>tuần học</small>
                    </div>
                    <div class="learning-stat-note">
                        <asp:Literal ID="litTotalDuration" runat="server" />
                    </div>
                </article>
                <article class="learning-stat-card">
                    <span>Tổng số bài giảng</span>
                    <div class="learning-stat-value">
                        <strong><asp:Literal ID="litTotalLessons" runat="server" /></strong>
                        <small>bài học</small>
                    </div>
                    <div class="learning-stat-note">
                        <asp:Literal ID="litLessonCompletion" runat="server" />
                    </div>
                </article>
                <article class="learning-stat-card">
                    <span>Tiến độ môn học</span>
                    <div class="learning-progress-header">
                        <strong><asp:Literal ID="litProgressPercent" runat="server" />%</strong>
                        <span><asp:Literal ID="litProgressSummary" runat="server" /></span>
                    </div>
                    <div id="courseProgressTrack" runat="server"
                        class="learning-progress-track" role="progressbar"
                        aria-valuemin="0" aria-valuemax="100"
                        aria-label="Tiến độ hoàn thành môn học">
                        <span id="courseProgressBar" runat="server"
                            class="learning-progress-fill"></span>
                    </div>
                </article>
            </section>

            <asp:Panel ID="pnlUpdates" runat="server"
                CssClass="learning-updates" Visible="false">
                <div class="learning-updates-heading">
                    <h3>Thông tin cập nhật môn học</h3>
                    <span>Thông báo liên quan</span>
                </div>
                <div class="learning-updates-list">
                    <asp:Repeater ID="rptUpdates" runat="server">
                        <ItemTemplate>
                            <article class="learning-update-card">
                                <strong><%#: Eval("Title") %></strong>
                                <p><%#: Eval("Content") %></p>
                                <time><%#: Eval("CreatedAtText") %></time>
                            </article>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <section id="learning-program">
                <div class="learning-program-heading">
                    <div>
                        <h3>Nội dung bài giảng theo tuần</h3>
                        <p><asp:Literal ID="litWeeklySummary" runat="server" /></p>
                    </div>
                    <div class="learning-program-actions">
                        <button type="button" id="expandWeeks">Mở rộng tất cả</button>
                        <button type="button" id="collapseWeeks">Thu gọn tất cả</button>
                    </div>
                </div>

                <asp:Panel ID="pnlNoWeeks" runat="server"
                    CssClass="learning-program-card learning-empty-state"
                    Visible="false">
                    Môn học chưa có dữ liệu chương trình theo tuần.
                </asp:Panel>

                <div class="learning-week-list">
                    <asp:Repeater ID="rptWeeks" runat="server">
                        <ItemTemplate>
                            <details id='<%# "week-" + Eval("WeekId") %>'
                                class='<%# "learning-week " + Eval("CurrentCss") %>'
                                data-current='<%# Eval("IsCurrent") %>'
                                data-accessible='<%# Eval("IsAccessible") %>'>
                                <summary>
                                    <span class='<%# "learning-week-icon " + Eval("WeekIconCss") %>'
                                        aria-hidden="true"><%#: Eval("WeekIcon") %></span>
                                    <span class="learning-week-title">
                                        <strong><%#: Eval("WeekTitle") %></strong>
                                        <small><%#: Eval("LessonSummary") %></small>
                                    </span>
                                    <span class="learning-week-progress">
                                        <span><%#: Eval("ProgressSummary") %></span>
                                        <span class="learning-week-progress-track">
                                            <span class="learning-week-progress-fill"
                                                style='<%# "width:" + Eval("ProgressPercent") + "%" %>'></span>
                                        </span>
                                    </span>
                                    <span class='<%# "learning-week-status " + Eval("StatusCss") %>'>
                                        <%#: Eval("StatusText") %>
                                    </span>
                                    <span class="learning-week-chevron" aria-hidden="true">⌄</span>
                                </summary>

                                <asp:PlaceHolder runat="server"
                                    Visible='<%# (bool)Eval("IsAccessible") %>'>
                                    <div class="learning-lesson-list">
                                        <asp:Repeater ID="rptLessons" runat="server"
                                            DataSource='<%# Eval("Lessons") %>'>
                                            <ItemTemplate>
                                                <details id='<%# "lesson-" + Eval("LessonId") %>'
                                                    class="learning-lesson">
                                                    <summary>
                                                        <span class='<%# "learning-lesson-icon " + Eval("ProgressIconCss") %>'
                                                            aria-hidden="true"><%#: Eval("LessonIcon") %></span>
                                                        <span class="learning-lesson-name">
                                                            <strong><%#: Eval("LessonTitle") %></strong>
                                                            <small><%#: Eval("LessonTypeText") %> · <%#: Eval("DurationText") %>
                                                                <asp:PlaceHolder runat="server"
                                                                    Visible='<%# (bool)Eval("IsRequired") %>'>
                                                                    · Bắt buộc
                                                                </asp:PlaceHolder>
                                                            </small>
                                                        </span>
                                                        <span class='<%# "learning-lesson-state " + Eval("ProgressCss") %>'>
                                                            <%#: Eval("ProgressText") %>
                                                        </span>
                                                        <span class="learning-lesson-chevron" aria-hidden="true">⌄</span>
                                                    </summary>
                                                    <div class="learning-lesson-content">
                                                        <p class="learning-lesson-description">
                                                            <%#: Eval("ShortDescription") %>
                                                        </p>
                                                        <a class="learning-open-lesson"
                                                            href='<%# ResolveUrl("~/Student/LessonDetails.aspx?lessonId=" + Server.UrlEncode(Convert.ToString(Eval("LessonId")))) %>'>
                                                            Mở chi tiết bài giảng
                                                        </a>
                                                        <asp:Panel runat="server"
                                                            Visible='<%# (bool)Eval("HasSections") %>'>
                                                            <h5>Nội dung bài giảng</h5>
                                                            <div class="learning-lesson-sections">
                                                                <asp:Repeater ID="rptSections" runat="server"
                                                                    DataSource='<%# Eval("Sections") %>'>
                                                                    <ItemTemplate>
                                                                        <section class="learning-lesson-section">
                                                                            <h5><%#: Eval("Title") %></h5>
                                                                            <asp:PlaceHolder runat="server"
                                                                                Visible='<%# (bool)Eval("IsCode") %>'>
                                                                                <pre><code><%#: Eval("Body") %></code></pre>
                                                                            </asp:PlaceHolder>
                                                                            <asp:PlaceHolder runat="server"
                                                                                Visible='<%# !(bool)Eval("IsCode") %>'>
                                                                                <p><%#: Eval("Body") %></p>
                                                                            </asp:PlaceHolder>
                                                                        </section>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </div>
                                                        </asp:Panel>
                                                        <asp:PlaceHolder runat="server"
                                                            Visible='<%# (bool)Eval("HasResources") %>'>
                                                            <h5>Tài nguyên</h5>
                                                            <div class="learning-resources">
                                                                <asp:Repeater ID="rptResources" runat="server"
                                                                    DataSource='<%# Eval("Resources") %>'>
                                                                    <ItemTemplate>
                                                                        <a href='<%#: ResolveUrl(Convert.ToString(Eval("Url"))) %>'
                                                                            target="_blank" rel="noopener">
                                                                            <span aria-hidden="true">↗</span>
                                                                            <%#: Eval("Name") %>
                                                                        </a>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </div>
                                                        </asp:PlaceHolder>
                                                        <asp:Panel runat="server"
                                                            CssClass="learning-no-content"
                                                            Visible='<%# !(bool)Eval("HasSections") && !(bool)Eval("HasResources") %>'>
                                                            Chưa có nội dung hoặc tài nguyên được tải lên cho bài giảng này.
                                                        </asp:Panel>
                                                    </div>
                                                </details>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <asp:Panel runat="server"
                                            CssClass="learning-empty-state"
                                            Visible='<%# !(bool)Eval("HasLessons") %>'>
                                            Tuần này chưa có bài giảng được xuất bản.
                                        </asp:Panel>
                                    </div>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder runat="server"
                                    Visible='<%# !(bool)Eval("IsAccessible") %>'>
                                    <div class="learning-locked-message">
                                        Nội dung tuần này chưa được mở. Bạn sẽ truy cập được khi giảng viên phát hành.
                                    </div>
                                </asp:PlaceHolder>
                            </details>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </section>
        </asp:Panel>
    </div>

    <script type="text/javascript">
        (function () {
            var weeks = Array.prototype.slice.call(
                document.querySelectorAll(".learning-week"));
            var search = document.getElementById("txtGlobalSearch");

            for (var i = 0; i < weeks.length; i++) {
                if (weeks[i].getAttribute("data-current") === "True" &&
                    weeks[i].getAttribute("data-accessible") === "True") {
                    weeks[i].open = true;
                    break;
                }
            }

            var expandButton = document.getElementById("expandWeeks");
            if (expandButton) {
                expandButton.addEventListener("click", function () {
                    weeks.forEach(function (week) {
                        if (week.getAttribute("data-accessible") === "True") {
                            week.open = true;
                        }
                    });
                });
            }

            var collapseButton = document.getElementById("collapseWeeks");
            if (collapseButton) {
                collapseButton.addEventListener("click", function () {
                    weeks.forEach(function (week) {
                        week.open = false;
                    });
                });
            }

            if (search) {
                search.addEventListener("input", function () {
                    var query = search.value.trim().toLocaleLowerCase();
                    weeks.forEach(function (week) {
                        var lessonRows = Array.prototype.slice.call(
                            week.querySelectorAll(".learning-lesson"));
                        var weekMatches = week.querySelector("summary")
                            .textContent.toLocaleLowerCase().indexOf(query) !== -1;
                        var lessonMatches = 0;

                        lessonRows.forEach(function (lesson) {
                            var matches = query === "" || weekMatches ||
                                lesson.textContent.toLocaleLowerCase()
                                    .indexOf(query) !== -1;
                            lesson.style.display = matches ? "" : "none";
                            if (matches) {
                                lessonMatches++;
                            }
                        });

                        var showWeek = query === "" || weekMatches ||
                            lessonMatches > 0;
                        week.style.display = showWeek ? "" : "none";
                        if (query && showWeek &&
                            week.getAttribute("data-accessible") === "True") {
                            week.open = true;
                        }
                    });
                });
            }
        }());
    </script>
</asp:Content>
