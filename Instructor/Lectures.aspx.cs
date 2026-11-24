using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using EduLearn.Instructor.Data;
using EduLearn.Services;

namespace EduLearn.Instructor
{
    public partial class Lectures : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Instructor")) return;

            if (!IsPostBack)
            {
                if (Request.QueryString["saved"] == "1")
                {
                    lblAlert.Text = "Đã lưu bài giảng thành công.";
                    lblAlert.Visible = true;
                }

                string key = InstructorStore.CurrentKey();
                ddlCourse.Items.Add(new ListItem("Tất cả môn học", ""));
                foreach (CourseItem c in InstructorStore.GetCourses(key))
                {
                    ddlCourse.Items.Add(new ListItem(c.Name, c.Id));
                }

                string cid = Request.QueryString["courseId"];
                if (!string.IsNullOrEmpty(cid) && ddlCourse.Items.FindByValue(cid) != null)
                    ddlCourse.SelectedValue = cid;

                BindLectures();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            BindLectures();
        }

        private void BindLectures()
        {
            string key = InstructorStore.CurrentKey();
            List<LectureItem> list = InstructorStore.GetLectures(key, ddlCourse.SelectedValue);

            if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                list = list.Where(l => string.Equals(l.Status, ddlStatus.SelectedValue, StringComparison.OrdinalIgnoreCase)).ToList();

            string kw = txtKeyword.Text.Trim();
            if (!string.IsNullOrEmpty(kw))
                list = list.Where(l => l.Title.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            litCount.Text = list.Count.ToString();
            pnlEmpty.Visible = list.Count == 0;
            rptLectures.DataSource = list;
            rptLectures.DataBind();
        }

        protected void rptLectures_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string key = InstructorStore.CurrentKey();
            string id = Convert.ToString(e.CommandArgument);

            if (e.CommandName == "TogglePublish")
            {
                InstructorStore.ToggleLecturePublish(key, id);
                lblAlert.Text = "Đã cập nhật trạng thái xuất bản.";
                lblAlert.Visible = true;
            }
            else if (e.CommandName == "DeleteItem")
            {
                InstructorStore.DeleteLecture(key, id);
                lblAlert.Text = "Đã xóa bài giảng.";
                lblAlert.Visible = true;
            }

            BindLectures();
        }
    }
}