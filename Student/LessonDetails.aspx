<%@ Page Title="Chi tiết bài giảng"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="LessonDetails.aspx.cs"
    Inherits="EduLearn.Student.LessonDetails" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet"
        href="<%= ResolveUrl("~/Content/CSS/lesson-details.css") %>" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="lesson-detail-page">
        <asp:Panel ID="pnlLessonNotFound" runat="server"
            CssClass="lesson-empty-state" Visible="false">
            <h2>Không tìm thấy bài giảng</h2>
            <p>Bài giảng không tồn tại hoặc đã bị gỡ khỏi chương trình.</p>
            <a class="lesson-button lesson-button-primary"
                href="<%: ResolveUrl("~/Student/MyCourses.aspx") %>">
                Quay lại Môn học của tôi
            </a>
        </asp:Panel>

        <asp:Panel ID="pnlLessonAccessDenied" runat="server"
            CssClass="lesson-empty-state" Visible="false">
            <h2><asp:Literal ID="litAccessHeading" runat="server" /></h2>
            <p><asp:Literal ID="litAccessMessage" runat="server" /></p>
            <asp:HyperLink ID="lnkAccessDestination" runat="server"
                CssClass="lesson-button lesson-button-primary">
                Xem nội dung môn học
            </asp:HyperLink>
        </asp:Panel>

        <asp:Panel ID="pnlLessonView" runat="server" Visible="false">
            <nav class="lesson-breadcrumb" aria-label="Điều hướng">
                <asp:HyperLink ID="lnkBreadcrumbCourses" runat="server">
                    Môn học của tôi
                </asp:HyperLink>
                <span aria-hidden="true">›</span>
                <asp:HyperLink ID="lnkBreadcrumbCourse" runat="server">
                    <asp:Literal ID="litBreadcrumbCourse" runat="server" />
                </asp:HyperLink>
                <span aria-hidden="true">›</span>
                <span><asp:Literal ID="litBreadcrumbWeek" runat="server" /></span>
                <span aria-hidden="true">›</span>
                <strong><asp:Literal ID="litBreadcrumbLesson" runat="server" /></strong>
            </nav>

            <section class="lesson-intro-card">
                <div class="lesson-intro-copy">
                    <div class="lesson-badges">
                        <span class="lesson-badge lesson-badge-week">
                            <asp:Literal ID="litWeekBadge" runat="server" />
                        </span>
                        <span class="lesson-badge lesson-badge-status">
                            <asp:Literal ID="litLessonStatus" runat="server" />
                        </span>
                        <span class="lesson-badge lesson-badge-code">
                            Mã bài: <asp:Literal ID="litLessonCode" runat="server" />
                        </span>
                    </div>
                    <h2><asp:Literal ID="litLessonTitle" runat="server" /></h2>
                    <p class="lesson-intro-description">
                        <asp:Literal ID="litLessonDescription" runat="server" />
                    </p>
                    <div class="lesson-intro-meta">
                        <span>Giảng viên: <strong><asp:Literal ID="litInstructorName" runat="server" /></strong></span>
                        <span>Thời lượng: <strong><asp:Literal ID="litDuration" runat="server" /></strong></span>
                        <span>Loại bài học: <strong><asp:Literal ID="litLessonType" runat="server" /></strong></span>
                        <span><asp:Literal ID="litTerm" runat="server" /></span>
                    </div>
                </div>
                <div class="lesson-intro-actions">
                    <button type="button" class="lesson-button lesson-button-subtle"
                        id="shareLessonButton">
                        <span aria-hidden="true">↗</span> Chia sẻ
                    </button>
                </div>
            </section>

            <div class="lesson-layout">
                <main class="lesson-main-column">
                    <section class="lesson-player-card" aria-label="Video bài giảng">
                        <asp:Panel ID="pnlVideoPlayer" runat="server"
                            CssClass="lesson-video-wrap" Visible="false">
                            <video id="lessonVideo" runat="server"
                                class="lesson-video" controls preload="metadata">
                                <source id="lessonVideoSource" runat="server"
                                    type="video/mp4" />
                            </video>
                        </asp:Panel>
                        <asp:Panel ID="pnlVideoUnavailable" runat="server"
                            CssClass="lesson-video-placeholder" Visible="false">
                            <span class="lesson-video-mark" aria-hidden="true">▶</span>
                            <strong><asp:Literal ID="litVideoPlaceholderTitle" runat="server" /></strong>
                            <span><asp:Literal ID="litVideoPlaceholderMessage" runat="server" /></span>
                        </asp:Panel>
                    </section>

                    <section class="lesson-controls-card" aria-label="Điều khiển bài học">
                        <asp:HyperLink ID="lnkPreviousLesson" runat="server"
                            CssClass="lesson-button lesson-button-subtle"
                            Visible="false">
                            ← Bài trước
                        </asp:HyperLink>
                        <div class="lesson-progress-control">
                            <span>Tiến độ bài học: <strong><asp:Literal ID="litProgressPercent" runat="server" />%</strong></span>
                            <div id="lessonProgressTrack" runat="server"
                                class="lesson-progress-track" role="progressbar"
                                aria-valuemin="0" aria-valuemax="100">
                                <span id="lessonProgressBar" runat="server"
                                    class="lesson-progress-fill"></span>
                            </div>
                        </div>
                        <div class="lesson-control-actions">
                            <asp:Label ID="lblCompletionState" runat="server"
                                CssClass="lesson-complete-state" Visible="false" />
                            <asp:Button ID="btnMarkComplete" runat="server"
                                CssClass="lesson-button lesson-button-complete"
                                Text="✓  Đánh dấu hoàn thành"
                                OnClick="btnMarkComplete_Click" />
                            <asp:HyperLink ID="lnkNextLesson" runat="server"
                                CssClass="lesson-button lesson-button-primary"
                                Visible="false">
                                Bài tiếp theo →
                            </asp:HyperLink>
                        </div>
                        <asp:Panel ID="pnlActionMessage" runat="server"
                            CssClass="lesson-action-message" Visible="false">
                            <asp:Literal ID="litActionMessage" runat="server" />
                        </asp:Panel>
                    </section>

                    <section class="lesson-content-card">
                        <div class="lesson-content-heading">
                            <h3><span aria-hidden="true">▤</span> Nội dung bài giảng</h3>
                            <span><asp:Literal ID="litContentSectionCount" runat="server" /></span>
                        </div>
                        <asp:Panel ID="pnlNoLessonSections" runat="server"
                            CssClass="lesson-content-empty" Visible="false">
                            Bài giảng chưa có nội dung văn bản được cập nhật.
                        </asp:Panel>
                        <asp:Repeater ID="rptLessonSections" runat="server">
                            <ItemTemplate>
                                <article class="lesson-section-card">
                                    <h4>
                                        <span class="lesson-section-number"><%#: Eval("Number") %></span>
                                        <%#: Eval("Title") %>
                                    </h4>
                                    <asp:PlaceHolder runat="server"
                                        Visible='<%# (bool)Eval("IsCode") %>'>
                                        <pre class="lesson-code"><code><%#: Eval("Body") %></code></pre>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder runat="server"
                                        Visible='<%# !(bool)Eval("IsCode") %>'>
                                        <p><%#: Eval("Body") %></p>
                                    </asp:PlaceHolder>
                                </article>
                            </ItemTemplate>
                        </asp:Repeater>

                        <asp:Panel ID="pnlLessonResources" runat="server"
                            CssClass="lesson-resources-card" Visible="false">
                            <div>
                                <strong><asp:Literal ID="litResourceName" runat="server" /></strong>
                                <span><asp:Literal ID="litResourceMetadata" runat="server" /></span>
                            </div>
                            <asp:HyperLink ID="lnkDownloadResource" runat="server"
                                CssClass="lesson-button lesson-button-subtle"
                                Target="_blank">
                                ↓ Tải tài liệu
                            </asp:HyperLink>
                        </asp:Panel>
                    </section>
                </main>

                <aside class="lesson-aside">
                    <section class="lesson-course-outline">
                        <div class="lesson-outline-heading">
                            <h3>Danh sách bài học</h3>
                            <span class="lesson-week-count">
                                <asp:Literal ID="litCurrentWeekProgress" runat="server" />
                            </span>
                        </div>
                        <div class="lesson-course-progress">
                            <div class="lesson-course-progress-label">
                                <span>Tiến độ cả môn học:</span>
                                <strong><asp:Literal ID="litCourseProgressPercent" runat="server" />% hoàn thành</strong>
                            </div>
                            <div class="lesson-progress-track" aria-hidden="true">
                                <span id="courseProgressBar" runat="server"
                                    class="lesson-progress-fill"></span>
                            </div>
                            <small><asp:Literal ID="litCourseCompletionCount" runat="server" /></small>
                        </div>
                        <div class="lesson-outline-list">
                            <asp:Repeater ID="rptCourseWeeks" runat="server">
                                <ItemTemplate>
                                    <section class='<%# "lesson-week-group " + Eval("CurrentCss") %>'>
                                        <div class="lesson-week-heading">
                                            <span>
                                                <strong><%#: Eval("WeekTitle") %></strong>
                                                <small><%#: Eval("ProgressSummary") %></small>
                                            </span>
                                            <b class='<%# "lesson-week-state " + Eval("StatusCss") %>'>
                                                <%#: Eval("StatusText") %>
                                            </b>
                                        </div>
                                        <asp:PlaceHolder runat="server"
                                            Visible='<%# (bool)Eval("IsAccessible") %>'>
                                            <div class="lesson-outline-items">
                                                <asp:Repeater ID="rptWeekLessons" runat="server"
                                                    DataSource='<%# Eval("Lessons") %>'>
                                                    <ItemTemplate>
                                                        <a class='<%# "lesson-outline-item " + Eval("CurrentCss") %>'
                                                            href='<%#: Eval("Url") %>'
                                                            data-lesson-search='<%#: Eval("SearchText") %>'>
                                                            <span class="lesson-outline-icon" aria-hidden="true">
                                                                <%#: Eval("Icon") %>
                                                            </span>
                                                            <span class="lesson-outline-copy">
                                                                <strong><%#: Eval("DisplayTitle") %></strong>
                                                                <small><%#: Eval("Meta") %></small>
                                                            </span>
                                                            <span class="lesson-outline-status"><%#: Eval("ProgressText") %></span>
                                                        </a>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <asp:Panel runat="server"
                                                    CssClass="lesson-week-empty"
                                                    Visible='<%# !(bool)Eval("HasLessons") %>'>
                                                    Chưa có bài giảng được phát hành.
                                                </asp:Panel>
                                            </div>
                                        </asp:PlaceHolder>
                                        <asp:Panel runat="server"
                                            CssClass="lesson-week-locked"
                                            Visible='<%# !(bool)Eval("IsAccessible") %>'>
                                            Chưa mở · Mở nội dung tuần này để xem bài giảng
                                        </asp:Panel>
                                    </section>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <asp:HyperLink ID="lnkCourseLearning" runat="server"
                            CssClass="lesson-outline-link">
                            Xem trang môn học →
                        </asp:HyperLink>
                    </section>

                    <section class="lesson-help-card">
                        <span class="lesson-help-icon" aria-hidden="true">?</span>
                        <div>
                            <strong>Cần trợ giúp bài học?</strong>
                            <span>Liên hệ giảng viên phụ trách môn học.</span>
                        </div>
                        <asp:HyperLink ID="lnkInstructorEmail" runat="server"
                            CssClass="lesson-help-link" Visible="false">
                            Hỏi
                        </asp:HyperLink>
                    </section>
                </aside>
            </div>
            <p id="lessonShareStatus" class="lesson-share-status"
                role="status" aria-live="polite"></p>
        </asp:Panel>
    </div>

    <script type="text/javascript">
        (function () {
            var shareButton = document.getElementById("shareLessonButton");
            var shareStatus = document.getElementById("lessonShareStatus");
            var search = document.getElementById("txtGlobalSearch");

            if (shareButton && shareStatus) {
                shareButton.addEventListener("click", function () {
                    var shareData = {
                        title: document.title,
                        url: window.location.href
                    };

                    if (navigator.share) {
                        navigator.share(shareData).catch(function (error) {
                            if (error.name !== "AbortError") {
                                shareStatus.textContent =
                                    "Không thể mở chức năng chia sẻ trên trình duyệt này.";
                            }
                        });
                        return;
                    }

                    if (navigator.clipboard &&
                        window.isSecureContext &&
                        navigator.clipboard.writeText) {
                        navigator.clipboard.writeText(shareData.url)
                            .then(function () {
                                shareStatus.textContent = "Đã sao chép liên kết bài giảng.";
                            })
                            .catch(function () {
                                shareStatus.textContent = "Không thể sao chép liên kết trên trình duyệt này.";
                            });
                        return;
                    }

                    shareStatus.textContent = "Liên kết bài giảng: " + shareData.url;
                });
            }

            if (search) {
                var lessonItems = Array.prototype.slice.call(
                    document.querySelectorAll("[data-lesson-search]"));
                search.addEventListener("input", function () {
                    var query = search.value.trim().toLocaleLowerCase();
                    lessonItems.forEach(function (item) {
                        var matches = !query ||
                            item.getAttribute("data-lesson-search")
                                .toLocaleLowerCase().indexOf(query) !== -1;
                        item.style.display = matches ? "" : "none";
                    });
                });
            }
        }());
    </script>
</asp:Content>
