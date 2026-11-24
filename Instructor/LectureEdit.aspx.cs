using System;
using System.Web.UI.WebControls;
using EduLearn.Instructor.Data;
using EduLearn.Services;

namespace EduLearn.Instructor
{
    public partial class LectureEdit : System.Web.UI.Page
    {
        private string EditId { get { return (Request.QueryString["id"] ?? string.Empty).Trim(); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Instructor")) return;

            if (!IsPostBack)
            {
                string key = InstructorStore.CurrentKey();
                foreach (CourseItem c in InstructorStore.GetCourses(key))
                {
                    ddlCourse.Items.Add(new ListItem(c.Name + " (" + c.Id + ")", c.Id));
                }

                string preCourse = Request.QueryString["courseId"];
                if (!string.IsNullOrEmpty(preCourse) && ddlCourse.Items.FindByValue(preCourse) != null)
                    ddlCourse.SelectedValue = preCourse;

                if (!string.IsNullOrEmpty(EditId))
                {
                    LectureItem lec = InstructorStore.GetLectureById(key, EditId);
                    if (lec != null)
                    {
                        litFormTitle.Text = "Chỉnh sửa bài giảng";
                        if (ddlCourse.Items.FindByValue(lec.CourseId) != null) ddlCourse.SelectedValue = lec.CourseId;
                        txtWeek.Text = lec.Week.ToString();
                        txtTitle.Text = lec.Title;
                        if (ddlType.Items.FindByValue(lec.Type) != null) ddlType.SelectedValue = lec.Type;
                        txtDuration.Text = lec.Duration;
                        txtVideoUrl.Text = lec.VideoUrl;
                        txtContent.Text = lec.Content;
                        if (ddlStatus.Items.FindByValue(lec.Status) != null) ddlStatus.SelectedValue = lec.Status;
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid) return;

            if (ddlCourse.Items.Count == 0)
            {
                lblAlert.Text = "Bạn chưa được phân công môn học nào để tạo bài giảng.";
                lblAlert.Visible = true;
                return;
            }

            int week;
            if (!int.TryParse(txtWeek.Text, out week) || week < 1) week = 1;

            var item = new LectureItem
            {
                Id = EditId,
                CourseId = ddlCourse.SelectedValue,
                Week = week,
                Title = txtTitle.Text.Trim(),
                Type = ddlType.SelectedValue,
                Duration = string.IsNullOrWhiteSpace(txtDuration.Text) ? "45 phút" : txtDuration.Text.Trim(),
                VideoUrl = txtVideoUrl.Text.Trim(),
                Content = txtContent.Text.Trim(),
                Status = ddlStatus.SelectedValue
            };

            InstructorStore.SaveLecture(InstructorStore.CurrentKey(), item);
            Response.Redirect(ResolveUrl("~/Instructor/Lectures.aspx?saved=1"), false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}