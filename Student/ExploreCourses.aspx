<%@ Page Title="Khám phá môn học"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="ExploreCourses.aspx.cs"
    Inherits="EduLearn.Student.ExploreCourses" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet"
        href="<%= ResolveUrl("~/Content/CSS/explore-courses.css") %>" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="explore-page">
        <header class="explore-intro">
            <h2>Khám phá môn học</h2>
            <p>Tìm kiếm và đăng ký những môn học phù hợp với hành trình học tập của bạn.</p>
        </header>

        <asp:Panel ID="pnlActionMessage" runat="server"
            CssClass="course-alert" Visible="false" role="status">
            <asp:Literal ID="litActionMessage" runat="server" />
        </asp:Panel>

        <section class="explore-toolbar" aria-label="Tìm và lọc môn học">
            <label class="explore-search">
                <span aria-hidden="true">⌕</span>
                <asp:TextBox ID="txtCourseSearch" runat="server"
                    ClientIDMode="Static"
                    CssClass="explore-search-input"
                    TextMode="Search"
                    placeholder="Tìm kiếm tên môn học..."
                    aria-label="Tìm kiếm tên môn học" />
            </label>

            <div class="explore-toolbar-controls">
                <div class="explore-category-filters" role="group"
                    aria-label="Lọc theo danh mục">
                    <button type="button" class="is-selected"
                        data-category-filter="all" aria-pressed="true">Tất cả</button>
                    <asp:Repeater ID="rptCategories" runat="server">
                        <ItemTemplate>
                            <button type="button" data-category-filter='<%# Eval("CategoryId") %>'
                                aria-pressed="false"><%#: Eval("CategoryName") %></button>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <label class="explore-sort">
                    <span class="visually-hidden">Sắp xếp môn học</span>
                    <select id="courseSort">
                        <option value="newest">Mới nhất</option>
                        <option value="oldest">Cũ nhất</option>
                        <option value="name">Tên môn học</option>
                    </select>
                </label>
            </div>
        </section>

        <section class="explore-course-section" id="course-list">
            <div class="explore-list-heading">
                <h3>Tất cả môn học</h3>
                <span id="courseCount">
                    Hiển thị <asp:Literal ID="litCourseCount" runat="server" /> môn học
                </span>
            </div>

            <asp:Panel ID="pnlNoCourses" runat="server"
                CssClass="explore-empty-state" Visible="false">
                <span aria-hidden="true">▤</span>
                <h4>Chưa có môn học để khám phá</h4>
                <p>Các môn học đang mở sẽ được hiển thị tại đây.</p>
            </asp:Panel>

            <div class="explore-course-grid" id="courseGrid">
                <asp:Repeater ID="rptCourses" runat="server"
                    OnItemCommand="rptCourses_ItemCommand">
                    <ItemTemplate>
                        <details class="explore-course-card"
                            data-category-id='<%# Eval("CategoryId") %>'
                            data-sort-date='<%# Eval("CreatedAtSort") %>'
                            data-sort-name='<%#: Eval("CourseName") %>'>
                            <summary class="explore-course-summary">
                                <div class="explore-course-cover">
                                    <asp:PlaceHolder runat="server"
                                        Visible='<%# (bool)Eval("HasThumbnail") %>'>
                                        <img src='<%# ResolveUrl(Convert.ToString(Eval("ThumbnailUrl"))) %>'
                                            alt='<%#: Eval("CourseName") %>' />
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder runat="server"
                                        Visible='<%# !(bool)Eval("HasThumbnail") %>'>
                                        <div class="explore-cover-fallback" aria-hidden="true">
                                            <span><%#: Eval("CourseCode") %></span>
                                        </div>
                                    </asp:PlaceHolder>
                                    <span class="explore-category-badge"><%#: Eval("CategoryName") %></span>
                                    <asp:PlaceHolder runat="server"
                                        Visible='<%# (bool)Eval("IsEnrolled") %>'>
                                        <span class="explore-enrolled-badge">Đã đăng ký</span>
                                    </asp:PlaceHolder>
                                </div>

                                <div class="explore-course-body">
                                    <h4><%#: Eval("CourseName") %></h4>
                                    <p class="explore-course-description"><%#: Eval("ShortDescription") %></p>
                                    <div class="explore-course-meta">
                                        <span><span aria-hidden="true">♙</span>
                                            Giảng viên: <strong><%#: Eval("InstructorName") %></strong></span>
                                        <span><span aria-hidden="true">▣</span>
                                            <%#: Eval("LessonCountText") %></span>
                                    </div>
                                    <span class="explore-detail-action">
                                        Xem chi tiết <span aria-hidden="true">→</span>
                                    </span>
                                </div>
                            </summary>

                            <div class="explore-course-details">
                                <p><%#: Eval("Description") %></p>
                                <dl class="explore-course-facts">
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
                                </dl>
                                <asp:PlaceHolder runat="server"
                                    Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("ObjectivesText"))) %>'>
                                    <div class="explore-detail-copy">
                                        <h5>Mục tiêu học tập</h5>
                                        <p><%#: Eval("ObjectivesText") %></p>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server"
                                    Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("PrerequisitesText"))) %>'>
                                    <div class="explore-detail-copy">
                                        <h5>Điều kiện tiên quyết</h5>
                                        <p><%#: Eval("PrerequisitesText") %></p>
                                    </div>
                                </asp:PlaceHolder>
                                <a class="explore-enroll-action"
                                    href='<%# ResolveUrl("~/Student/CourseDetails.aspx?courseId=" + Server.UrlEncode(Convert.ToString(Eval("CourseId")))) %>'>
                                    Xem chi tiết đầy đủ
                                </a>

                                <asp:PlaceHolder runat="server"
                                    Visible='<%# (bool)Eval("IsEnrolled") %>'>
                                    <a class="explore-enroll-action is-enrolled"
                                        href='<%# ResolveUrl("~/Student/CourseLearning.aspx?courseId=" + Server.UrlEncode(Convert.ToString(Eval("CourseId")))) %>'>
                                        Đã đăng ký · Mở bài giảng
                                    </a>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server"
                                    Visible='<%# (bool)Eval("CanEnroll") %>'>
                                    <asp:LinkButton ID="btnEnroll" runat="server"
                                        CssClass="explore-enroll-action"
                                        CommandName="Enroll"
                                        CommandArgument='<%# Eval("CourseId") %>'
                                        CausesValidation="false">
                                        Đăng ký môn học
                                    </asp:LinkButton>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server"
                                    Visible='<%# !(bool)Eval("IsEnrolled") && !(bool)Eval("CanEnroll") %>'>
                                    <span class="explore-enroll-action is-closed">
                                        Môn học hiện không mở đăng ký
                                    </span>
                                </asp:PlaceHolder>
                            </div>
                        </details>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <p id="noCourseResults" class="explore-no-results" hidden="hidden">
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
                grid.querySelectorAll(".explore-course-card"));
            var activeCategory = "all";
            var filters = document.querySelectorAll("[data-category-filter]");

            function applyFilters() {
                var query = search.value.trim().toLocaleLowerCase();
                var sortMode = sort.value;
                var visibleCount = 0;

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
                    var matchesCategory = activeCategory === "all" ||
                        cards[i].getAttribute("data-category-id") === activeCategory;
                    var matchesText = cards[i].textContent
                        .toLocaleLowerCase().indexOf(query) !== -1;
                    var visible = matchesCategory && matchesText;

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

            for (var i = 0; i < filters.length; i++) {
                filters[i].addEventListener("click", function () {
                    activeCategory = this.getAttribute("data-category-filter");
                    for (var j = 0; j < filters.length; j++) {
                        var selected = filters[j] === this;
                        filters[j].classList.toggle("is-selected", selected);
                        filters[j].setAttribute(
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
