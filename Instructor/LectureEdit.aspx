<%@ Page Language="C#" MasterPageFile="~/Instructor/Instructor.master" AutoEventWireup="true" CodeBehind="LectureEdit.aspx.cs" Inherits="EduLearn.Instructor.LectureEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">Biên soạn bài giảng</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">
    <section class="card form-card">
        <div class="section__head">
            <h2><asp:Literal ID="litFormTitle" runat="server" Text="Thêm bài giảng mới" /></h2>
            <a class="btn btn--white" href="Lectures.aspx">&larr; Danh sách bài giảng</a>
        </div>

        <asp:Label ID="lblAlert" runat="server" CssClass="alert-banner alert-banner--error" Visible="false" />

        <div class="form-grid-2">
            <div class="form-group">
                <label class="form-label">Môn học phụ trách *</label>
                <asp:DropDownList ID="ddlCourse" runat="server" CssClass="ctrl-select" />
            </div>
            <div class="form-group">
                <label class="form-label">Tuần học *</label>
                <asp:TextBox ID="txtWeek" runat="server" CssClass="ctrl-input" TextMode="Number" Text="1" min="1" max="20" />
            </div>
        </div>

        <div class="form-group">
            <label class="form-label">Tiêu đề bài giảng *</label>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="ctrl-input" placeholder="Ví dụ: Tuần 5: Kết nối dữ liệu XML với ASP.NET" />
            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle"
                CssClass="val-err" Display="Dynamic" ErrorMessage="Vui lòng nhập tiêu đề bài giảng." />
        </div>

        <div class="form-grid-2">
            <div class="form-group">
                <label class="form-label">Loại bài giảng</label>
                <asp:DropDownList ID="ddlType" runat="server" CssClass="ctrl-select">
                    <asp:ListItem Value="Video" Text="Video bài giảng" />
                    <asp:ListItem Value="Tài liệu" Text="Tài liệu đọc / Slide" />
                    <asp:ListItem Value="Bài tập" Text="Bài tập thực hành" />
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label class="form-label">Thời lượng dự kiến</label>
                <asp:TextBox ID="txtDuration" runat="server" CssClass="ctrl-input" Text="45 phút" />
            </div>
        </div>

        <div class="form-grid-2">
            <div class="form-group">
                <label class="form-label">Liên kết Video / Tài liệu (URL)</label>
                <asp:TextBox ID="txtVideoUrl" runat="server" CssClass="ctrl-input" placeholder="https://..." />
            </div>
            <div class="form-group">
                <label class="form-label">Trạng thái phát hành</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="ctrl-select">
                    <asp:ListItem Value="Đã xuất bản" Text="Đã xuất bản (Công khai cho sinh viên)" />
                    <asp:ListItem Value="Bản nháp" Text="Lưu bản nháp" />
                </asp:DropDownList>
            </div>
        </div>

        <div class="form-group">
            <label class="form-label">Nội dung tóm tắt / Hướng dẫn học tập</label>
            <asp:TextBox ID="txtContent" runat="server" CssClass="ctrl-textarea" TextMode="MultiLine" placeholder="Nhập nội dung chi tiết bài giảng..." />
        </div>

        <div class="action-group">
            <asp:Button ID="btnSave" runat="server" Text="Lưu bài giảng" CssClass="btn btn--primary" OnClick="btnSave_Click" />
            <a class="btn btn--white" href="Lectures.aspx">Hủy bỏ</a>
        </div>
    </section>
</asp:Content>