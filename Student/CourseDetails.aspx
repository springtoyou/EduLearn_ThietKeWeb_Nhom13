<%@ Page Title="Chi tiết môn học"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="CourseDetails.aspx.cs"
    Inherits="EduLearn.Student.CourseDetails" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet"
        href="<%= ResolveUrl("~/Content/CSS/course-pages.css") %>" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="course-detail-page">
        <asp:Panel ID="pnlCourseNotFound" runat="server"
            CssClass="course-page-empty" Visible="false">
            <h2>Không tìm thấy môn học</h2>
            <p>Môn học này không tồn tại hoặc đã bị gỡ khỏi danh mục.</p>
            <a class="course-primary-link" href="<%: ResolveUrl("~/Student/ExploreCourses.aspx") %>">
                Quay lại Khám phá môn học
            </a>
        </asp:Panel>

        <asp:Panel ID="pnlCourseDetails" runat="server" Visible="false">
            <nav class="course-breadcrumb" aria-label="Điều hướng">
                <a href="<%: ResolveUrl("~/Student/Dashboard.aspx") %>">Trang chủ</a>
                <span aria-hidden="true">›</span>
                <a href="<%: ResolveUrl("~/Student/ExploreCourses.aspx") %>">Khám phá môn học</a>
                <span aria-hidden="true">›</span>
                <span><asp:Literal ID="litBreadcrumbCourse" runat="server" /></span>
            </nav>

            <asp:Panel ID="pnlRegistrationMessage" runat="server"
                CssClass="course-alert is-error" Visible="false" role="status">
                <asp:Literal ID="litRegistrationMessage" runat="server" />
            </asp:Panel>

            <div class="course-detail-layout">
                <main class="course-detail-main">
                    <section class="course-overview-card">
                        <div class="course-badges">
                            <span class="course-category-pill">
                                <asp:Literal ID="litCategory" runat="server" />
                            </span>
                            <asp:PlaceHolder ID="phTermBadge" runat="server">
                                <span class="course-term-pill">
                                    <asp:Literal ID="litTermBadge" runat="server" />
                                </span>
                            </asp:PlaceHolder>
                        </div>
                        <h2><asp:Literal ID="litCourseName" runat="server" /></h2>
                        <p class="course-short-description">
                            <asp:Literal ID="litShortDescription" runat="server" />
                        </p>
                        <div class="course-overview-meta">
                            <span><b aria-hidden="true">♙</b> Giảng viên:
                                <strong><asp:Literal ID="litInstructorName" runat="server" /></strong></span>
                            <span><b aria-hidden="true">▦</b> Tổng số tuần:
                                <strong><asp:Literal ID="litWeekCount" runat="server" /></strong></span>
                            <span><b aria-hidden="true">▣</b> Quy mô:
                                <strong><asp:Literal ID="litLessonCount" runat="server" /></strong></span>
                        </div>
                    </section>

                    <figure class="course-hero-image">
                        <asp:Image ID="imgCourseCover" runat="server"
                            AlternateText="Ảnh minh họa môn học" />
                        <figcaption>
                            <span aria-hidden="true">◉</span>
                            <asp:Literal ID="litCourseCode" runat="server" />
                            <span class="hero-caption-divider">·</span>
                            <asp:Literal ID="litLevel" runat="server" />
                        </figcaption>
                    </figure>

                    <section class="course-info-card">
                        <h3>Giới thiệu môn học</h3>
                        <p><asp:Literal ID="litDescription" runat="server" /></p>
                    </section>

                    <section class="course-info-card">
                        <h3>Mục tiêu học tập</h3>
                        <p class="course-section-lead">
                            Sau khi hoàn thành học phần, sinh viên có thể:
                        </p>
                        <asp:Panel ID="pnlNoObjectives" runat="server"
                            CssClass="course-muted-copy" Visible="false">
                            Mục tiêu học tập chưa được cập nhật.
                        </asp:Panel>
                        <ul class="course-objective-grid">
                            <asp:Repeater ID="rptObjectives" runat="server">
                                <ItemTemplate>
                                    <li><span aria-hidden="true">✓</span>
                                        <%#: Container.DataItem %></li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </section>

                    <section class="course-info-card course-curriculum-card">
                        <div class="course-section-heading">
                            <h3>Nội dung chương trình học</h3>
                            <span><asp:Literal ID="litCurriculumSummary" runat="server" /></span>
                        </div>
                        <asp:Panel ID="pnlNoWeeks" runat="server"
                            CssClass="course-muted-copy" Visible="false">
                            Chương trình học chưa có dữ liệu tuần.
                        </asp:Panel>
                        <div class="course-preview-weeks">
                            <asp:Repeater ID="rptWeeks" runat="server">
                                <ItemTemplate>
                                    <details class="course-preview-week"
                                        data-current='<%# Eval("IsCurrent") %>'>
                                        <summary>
                                            <span class="week-number"><%#: Eval("WeekNumberText") %></span>
                                            <span class="week-summary-text">
                                                <strong><%#: Eval("WeekTitle") %></strong>
                                                <small><%#: Eval("LessonSummary") %></small>
                                            </span>
                                            <span class='<%# "week-status " + Eval("StatusCss") %>'>
                                                <%#: Eval("StatusText") %>
                                            </span>
                                            <span class="week-chevron" aria-hidden="true">⌄</span>
                                        </summary>
                                        <div class="course-preview-week-content">
                                            <p><%#: Eval("Description") %></p>
                                            <asp:Repeater ID="rptPreviewLessons" runat="server"
                                                DataSource='<%# Eval("Lessons") %>'>
                                                <ItemTemplate>
                                                    <div class="preview-lesson-row">
                                                        <span class="preview-lesson-icon" aria-hidden="true">
                                                            <%#: Eval("TypeIcon") %>
                                                        </span>
                                                        <span class="preview-lesson-title">
                                                            <%#: Eval("LessonTitle") %>
                                                        </span>
                                                        <span class="preview-lesson-meta">
                                                            <%#: Eval("TypeText") %> · <%#: Eval("DurationText") %>
                                                        </span>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <asp:Panel ID="pnlNoPreviewLessons" runat="server"
                                                CssClass="course-muted-copy"
                                                Visible='<%# !(bool)Eval("HasLessons") %>'>
                                                Chưa có bài giảng trong tuần này.
                                            </asp:Panel>
                                        </div>
                                    </details>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </section>
                </main>

                <aside class="course-detail-aside">
                    <section class="course-aside-card registration-card">
                        <div class="registration-card-heading">
                            <h3>Thông tin đăng ký</h3>
                            <asp:Label ID="lblEnrollmentStatus" runat="server"
                                CssClass="course-status-badge" />
                        </div>
                        <dl class="course-registration-facts">
                            <div>
                                <dt><span aria-hidden="true">▦</span> Số tuần học</dt>
                                <dd><asp:Literal ID="litAsideWeekCount" runat="server" /></dd>
                            </div>
                            <div>
                                <dt><span aria-hidden="true">▣</span> Số bài giảng</dt>
                                <dd><asp:Literal ID="litAsideLessonCount" runat="server" /></dd>
                            </div>
                            <div>
                                <dt><span aria-hidden="true">▱</span> Tín chỉ</dt>
                                <dd><asp:Literal ID="litCredits" runat="server" /></dd>
                            </div>
                            <div>
                                <dt><span aria-hidden="true">◎</span> Trình độ</dt>
                                <dd><asp:Literal ID="litAsideLevel" runat="server" /></dd>
                            </div>
                        </dl>
                        <asp:Panel ID="pnlCanEnroll" runat="server" Visible="false">
                            <asp:LinkButton ID="btnEnroll" runat="server"
                                CssClass="course-primary-link"
                                OnClick="btnEnroll_Click"
                                CausesValidation="false">
                                Đăng ký môn học
                            </asp:LinkButton>
                            <p class="registration-note">Đăng ký được lưu vào hồ sơ môn học của bạn.</p>
                        </asp:Panel>
                        <asp:Panel ID="pnlAlreadyEnrolled" runat="server" Visible="false">
                            <asp:HyperLink ID="lnkCourseLearning" runat="server"
                                CssClass="course-primary-link">
                                Mở bài giảng theo tuần
                            </asp:HyperLink>
                        </asp:Panel>
                        <asp:Panel ID="pnlRegistrationClosed" runat="server"
                            CssClass="registration-closed" Visible="false">
                            Môn học hiện chưa mở đăng ký.
                        </asp:Panel>
                    </section>

                    <section class="course-aside-card instructor-card">
                        <h3>Thông tin giảng viên</h3>
                        <div class="instructor-identity">
                            <span class="instructor-initials">
                                <asp:Literal ID="litInstructorInitials" runat="server" />
                            </span>
                            <div>
                                <strong><asp:Literal ID="litInstructorFullName" runat="server" /></strong>
                                <span><asp:Literal ID="litDepartment" runat="server" /></span>
                            </div>
                        </div>
                        <p><asp:Literal ID="litInstructorBiography" runat="server" /></p>
                        <asp:HyperLink ID="lnkInstructorEmail" runat="server"
                            CssClass="instructor-email" Visible="false">
                        </asp:HyperLink>
                    </section>

                    <section class="course-aside-card">
                        <h3>Yêu cầu môn học</h3>
                        <asp:Panel ID="pnlNoPrerequisites" runat="server"
                            CssClass="course-muted-copy" Visible="false">
                            Chưa có điều kiện tiên quyết.
                        </asp:Panel>
                        <ul class="course-prerequisites">
                            <asp:Repeater ID="rptPrerequisites" runat="server">
                                <ItemTemplate>
                                    <li><span aria-hidden="true">→</span>
                                        <%#: Container.DataItem %></li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </section>
                </aside>
            </div>
        </asp:Panel>
    </div>

    <script type="text/javascript">
        (function () {
            var weeks = document.querySelectorAll(".course-preview-week");
            for (var i = 0; i < weeks.length; i++) {
                if (weeks[i].getAttribute("data-current") === "True") {
                    weeks[i].open = true;
                    break;
                }
            }
        }());
    </script>
</asp:Content>
