<%@ Page Title="Môn học của tôi"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="MyCourses.aspx.cs"
    Inherits="EduLearn.Student.MyCourses" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet"
        href="<%= ResolveUrl("~/Content/CSS/my-courses.css") %>" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="my-courses-page">
        <header class="courses-intro">
            <div>
                <h2>Môn học của tôi</h2>
                <p>Quản lý và theo dõi các môn học bạn đã đăng ký trên EduLearn.</p>
            </div>
        </header>

        <section class="course-statistics" aria-label="Thống kê môn học">
            <button type="button" class="course-stat-card"
                data-course-filter="all" aria-label="Hiển thị tất cả môn học">
                <span class="course-stat-label">Tổng số môn học</span>
                <strong><asp:Literal ID="litTotalCourses" runat="server" /></strong>
                <span class="course-stat-note">Môn học đã đăng ký</span>
                <span class="course-stat-icon icon-blue" aria-hidden="true">▤</span>
            </button>

            <button type="button" class="course-stat-card"
                data-course-filter="Active" aria-label="Lọc môn đang học">
                <span class="course-stat-label">Đang học</span>
                <strong><asp:Literal ID="litActiveCourses" runat="server" /></strong>
                <span class="course-stat-note">Tiến trình đang hoạt động</span>
                <span class="course-stat-icon icon-blue" aria-hidden="true">◷</span>
            </button>

            <button type="button" class="course-stat-card"
                data-course-filter="Completed" aria-label="Lọc môn đã hoàn thành">
                <span class="course-stat-label">Đã hoàn thành</span>
                <strong><asp:Literal ID="litCompletedCourses" runat="server" /></strong>
                <span class="course-stat-note">Môn học đã hoàn tất</span>
                <span class="course-stat-icon icon-green" aria-hidden="true">✓</span>
            </button>
        </section>

        <section class="course-toolbar" aria-label="Tìm và lọc môn học">
            <label class="course-search">
                <span aria-hidden="true">⌕</span>
                <asp:TextBox ID="txtCourseSearch" runat="server"
                    ClientIDMode="Static"
                    CssClass="course-search-input"
                    TextMode="Search"
                    placeholder="Tìm kiếm môn học..."
                    aria-label="Tìm kiếm môn học" />
            </label>

            <div class="course-filter-tabs" role="group" aria-label="Lọc theo trạng thái">
                <button type="button" class="is-selected" data-course-filter="all"
                    aria-pressed="true">Tất cả</button>
                <button type="button" data-course-filter="Active"
                    aria-pressed="false">Đang học</button>
                <button type="button" data-course-filter="Completed"
                    aria-pressed="false">Đã hoàn thành</button>
            </div>

            <label class="course-sort">
                <span aria-hidden="true">☷</span>
                <span class="visually-hidden">Sắp xếp môn học</span>
                <select id="courseSort">
                    <option value="newest">Sắp xếp: Mới nhất</option>
                    <option value="oldest">Sắp xếp: Cũ nhất</option>
                    <option value="name">Sắp xếp: Tên môn học</option>
                </select>
            </label>
        </section>

        <section class="course-list-section" id="course-list">
            <div class="course-list-heading">
                <h3>Danh sách môn học</h3>
                <span id="courseCount">
                    Hiển thị <asp:Literal ID="litCourseCount" runat="server" /> môn học
                </span>
            </div>

            <asp:Panel ID="pnlNoCourses" runat="server"
                CssClass="courses-empty-state" Visible="false">
                <span class="empty-book" aria-hidden="true">▤</span>
                <h4>Bạn chưa đăng ký môn học nào</h4>
                <p>Các môn học bạn đăng ký sẽ xuất hiện tại đây.</p>
            </asp:Panel>

            <div class="course-grid" id="courseGrid">
                <asp:Repeater ID="rptCourses" runat="server">
                    <ItemTemplate>
                        <details class="course-card"
                            data-course-status='<%# Eval("StatusKey") %>'
                            data-sort-date='<%# Eval("EnrollmentDateSort") %>'
                            data-sort-name='<%#: Eval("CourseName") %>'>
                            <summary class="course-card-summary">
                                <div class='<%# (bool)Eval("HasThumbnail") ? "course-cover has-thumbnail" : "course-cover" %>'>
                                    <asp:PlaceHolder runat="server"
                                        Visible='<%# (bool)Eval("HasThumbnail") %>'>
                                        <img src='<%# ResolveUrl(Convert.ToString(Eval("ThumbnailUrl"))) %>'
                                            alt='<%#: Eval("CourseName") %>' />
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder runat="server"
                                        Visible='<%# !(bool)Eval("HasThumbnail") %>'>
                                        <div class="course-cover-fallback" aria-hidden="true">
                                            <span><%#: Eval("CourseCode") %></span>
                                        </div>
                                    </asp:PlaceHolder>
                                    <span class="course-category"><%#: Eval("CategoryName") %></span>
                                    <span class='<%# "course-status " + Eval("StatusCss") %>'>
                                        <%#: Eval("StatusText") %>
                                    </span>
                                </div>

                                <div class="course-card-body">
                                    <h4><%#: Eval("CourseName") %></h4>
                                    <div class="course-instructor">
                                        <span aria-hidden="true">♙</span>
                                        <span>Giảng viên: <%#: Eval("InstructorName") %></span>
                                    </div>
                                    <div class="course-progress-heading">
                                        <span>Tiến độ khóa học</span>
                                        <strong class='<%# (int)Eval("ProgressPercent") == 100 ? "progress-complete" : string.Empty %>'>
                                            <%#: Eval("ProgressText") %>
                                        </strong>
                                    </div>
                                    <div class="progress-track" role="progressbar"
                                        aria-valuemin="0" aria-valuemax="100"
                                        aria-valuenow='<%# Eval("ProgressPercent") %>'
                                        aria-label='<%# "Tiến độ " + Eval("CourseName") %>'>
                                        <span class='<%# (int)Eval("ProgressPercent") == 100 ? "progress-fill is-complete" : "progress-fill" %>'
                                            style='<%# "width:" + Eval("ProgressPercent") + "%" %>'></span>
                                    </div>
                                    <span class="course-action">
                                        <span class="action-open">Xem chi tiết <span aria-hidden="true">→</span></span>
                                        <span class="action-close">Thu gọn chi tiết <span aria-hidden="true">↑</span></span>
                                    </span>
                                </div>
                            </summary>

                            <div class="course-details">
                                <p class="course-description"><%#: Eval("Description") %></p>
                                <dl class="course-facts">
                                    <div>
                                        <dt>Mã môn học</dt>
                                        <dd><%#: Eval("CourseCode") %></dd>
                                    </div>
                                    <div>
                                        <dt>Tín chỉ</dt>
                                        <dd><%#: Eval("CreditsText") %></dd>
                                    </div>
                                    <div>
                                        <dt>Thời lượng</dt>
                                        <dd><%#: Eval("WeeksText") %></dd>
                                    </div>
                                    <div>
                                        <dt>Trình độ</dt>
                                        <dd><%#: Eval("LevelText") %></dd>
                                    </div>
                                    <div>
                                        <dt>Học kỳ</dt>
                                        <dd><%#: Eval("TermText") %></dd>
                                    </div>
                                    <div>
                                        <dt>Bài học hoàn thành</dt>
                                        <dd><%#: Eval("LessonSummary") %></dd>
                                    </div>
                                </dl>
                                <asp:PlaceHolder runat="server"
                                    Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("ObjectivesText"))) %>'>
                                    <div class="course-detail-copy">
                                        <h5>Mục tiêu học tập</h5>
                                        <p><%#: Eval("ObjectivesText") %></p>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server"
                                    Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("PrerequisitesText"))) %>'>
                                    <div class="course-detail-copy">
                                        <h5>Điều kiện tiên quyết</h5>
                                        <p><%#: Eval("PrerequisitesText") %></p>
                                    </div>
                                </asp:PlaceHolder>
                                <a class="course-open-link"
                                    href='<%# ResolveUrl("~/Student/CourseLearning.aspx?courseId=" + Server.UrlEncode(Convert.ToString(Eval("CourseId")))) %>'>
                                    Mở bài giảng theo tuần <span aria-hidden="true">→</span>
                                </a>
                            </div>
                        </details>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <p id="noCourseResults" class="no-course-results" hidden="hidden">
                Không tìm thấy môn học phù hợp.
            </p>
        </section>
    </div>

    <script type="text/javascript">
        (function () {
            var grid = document.getElementById("courseGrid");
            var count = document.getElementById("courseCount");
            var noResults = document.getElementById("noCourseResults");
            var search = document.getElementById("txtCourseSearch");
            var globalSearch = document.getElementById("txtGlobalSearch");
            var sort = document.getElementById("courseSort");
            var cards = Array.prototype.slice.call(
                grid.querySelectorAll(".course-card"));
            var activeStatus = "all";

            function applyFilters() {
                var query = search.value.trim().toLocaleLowerCase();
                var visibleCount = 0;
                var sortMode = sort.value;

                cards.sort(function (a, b) {
                    if (sortMode === "name") {
                        return a.getAttribute("data-sort-name").localeCompare(
                            b.getAttribute("data-sort-name"), "vi");
                    }

                    var dateA = a.getAttribute("data-sort-date");
                    var dateB = b.getAttribute("data-sort-date");
                    return sortMode === "oldest"
                        ? dateA.localeCompare(dateB)
                        : dateB.localeCompare(dateA);
                });

                for (var i = 0; i < cards.length; i++) {
                    var matchesStatus = activeStatus === "all" ||
                        cards[i].getAttribute("data-course-status") === activeStatus;
                    var matchesText = cards[i].textContent
                        .toLocaleLowerCase().indexOf(query) !== -1;
                    var visible = matchesStatus && matchesText;

                    cards[i].style.display = visible ? "" : "none";
                    grid.appendChild(cards[i]);
                    if (visible) {
                        visibleCount++;
                    }
                }

                count.textContent = "Hiển thị " + visibleCount +
                    " / " + cards.length + " môn học";
                noResults.hidden = visibleCount > 0 || cards.length === 0;
            }

            function setSearch(value) {
                search.value = value;
                if (globalSearch) {
                    globalSearch.value = value;
                }
                applyFilters();
            }

            search.addEventListener("input", function () {
                setSearch(search.value);
            });
            if (globalSearch) {
                globalSearch.addEventListener("input", function () {
                    setSearch(globalSearch.value);
                });
            }

            var filterButtons = document.querySelectorAll(
                "[data-course-filter]");
            for (var i = 0; i < filterButtons.length; i++) {
                filterButtons[i].addEventListener("click", function () {
                    activeStatus = this.getAttribute("data-course-filter");
                    for (var j = 0; j < filterButtons.length; j++) {
                        var selected = filterButtons[j] === this;
                        filterButtons[j].classList.toggle("is-selected", selected);
                        filterButtons[j].setAttribute(
                            "aria-pressed", selected ? "true" : "false");
                    }
                    applyFilters();
                    document.getElementById("course-list").scrollIntoView({
                        behavior: "smooth",
                        block: "start"
                    });
                });
            }

            sort.addEventListener("change", applyFilters);
            applyFilters();
        }());
    </script>
</asp:Content>
