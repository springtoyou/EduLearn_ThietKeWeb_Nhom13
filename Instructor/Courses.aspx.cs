using System;
using System.Collections.Generic;
using System.Linq;
using EduLearn.Instructor.Data;
using EduLearn.Services;

namespace EduLearn.Instructor
{
    public partial class Courses : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Instructor")) return;

            if (!IsPostBack)
            {
                string q = Request.QueryString["q"];
                if (!string.IsNullOrWhiteSpace(q)) txtSearch.Text = q.Trim();
                BindCourses();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            BindCourses();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            ddlStatus.SelectedIndex = 0;
            BindCourses();
        }

        private void BindCourses()
        {
            string key = InstructorStore.CurrentKey();
            List<CourseItem> list = InstructorStore.GetCourses(key);

            string kw = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(kw))
            {
                list = list.Where(c =>
                    c.Name.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    c.Id.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    c.Category.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            string status = ddlStatus.SelectedValue;
            if (!string.IsNullOrEmpty(status))
            {
                list = list.Where(c => string.Equals(c.Status, status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            litResultCount.Text = list.Count + " môn học";
            pnlEmpty.Visible = list.Count == 0;
            rptCourses.DataSource = list;
            rptCourses.DataBind();
        }
    }
}