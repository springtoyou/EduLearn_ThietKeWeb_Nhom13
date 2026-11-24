using System;
using System.Collections.Generic;
using EduLearn.Instructor.Data;
using EduLearn.Services;

namespace EduLearn.Instructor
{
    public partial class CourseManage : System.Web.UI.Page
    {
        private string CourseId
        {
            get { return (Request.QueryString["id"] ?? string.Empty).Trim(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Instructor")) return;

            if (!IsPostBack)
            {
                LoadCourseData();
            }
        }

        private void LoadCourseData()
        {
            string key = InstructorStore.CurrentKey();
            CourseItem course = InstructorStore.GetCourseById(key, CourseId);
            if (course == null)
            {
                pnlNotFound.Visible = true;
                pnlContent.Visible = false;
                return;
            }

            pnlNotFound.Visible = false;
            pnlContent.Visible = true;

            litCourseName.Text = course.Name + " (" + course.Id + ")";
            litCourseCategory.Text = course.Category;
            txtDescription.Text = course.Description;
            txtProgress.Text = course.Progress.ToString();
            if (ddlStatus.Items.FindByValue(course.Status) != null)
                ddlStatus.SelectedValue = course.Status;

            lnkAddLecture.NavigateUrl = "~/Instructor/LectureEdit.aspx?courseId=" + Server.UrlEncode(course.Id);

            List<LectureItem> lectures = InstructorStore.GetLectures(key, course.Id);
            litLectureSummary.Text = lectures.Count + " bài giảng";
            pnlNoLecture.Visible = lectures.Count == 0;
            rptLectures.DataSource = lectures;
            rptLectures.DataBind();

            rptStudents.DataSource = InstructorStore.GetSampleStudentsForCourse(course.Id);
            rptStudents.DataBind();
        }

        protected void btnSaveCourse_Click(object sender, EventArgs e)
        {
            int progress;
            if (!int.TryParse(txtProgress.Text, out progress)) progress = 0;

            InstructorStore.UpdateCourse(InstructorStore.CurrentKey(), CourseId, txtDescription.Text.Trim(), ddlStatus.SelectedValue, progress);
            lblAlert.Text = "Đã lưu thông tin môn học thành công.";
            lblAlert.Visible = true;
            LoadCourseData();
        }

        protected void rptLectures_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "TogglePublish")
            {
                InstructorStore.ToggleLecturePublish(InstructorStore.CurrentKey(), Convert.ToString(e.CommandArgument));
                lblAlert.Text = "Đã cập nhật trạng thái xuất bản bài giảng.";
                lblAlert.Visible = true;
                LoadCourseData();
            }
        }
    }
}