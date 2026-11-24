using System;
using System.Collections.Generic;
using System.Linq;
using EduLearn.Instructor.Data;
using EduLearn.Services;

namespace EduLearn.Instructor
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Instructor")) return;

            if (!IsPostBack)
            {
                LoadProfileData();
            }
        }

        private void LoadProfileData()
        {
            string key = InstructorStore.CurrentKey();
            GvProfile p = InstructorStore.GetProfile(key, InstructorStore.CurrentFullName());

            string[] parts = p.FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            litAvatarBig.Text = parts.Length > 0 ? parts[parts.Length - 1].Substring(0, 1).ToUpperInvariant() : "G";
            litProfileName.Text = p.FullName;
            litProfileDegree.Text = p.Degree;
            litProfileDept.Text = p.Department;

            txtFullName.Text = p.FullName;
            txtEmail.Text = p.Email;
            txtPhone.Text = p.Phone;
            txtDegree.Text = p.Degree;
            txtDepartment.Text = p.Department;
            txtBio.Text = p.Bio;

            List<CourseItem> courses = InstructorStore.GetCourses(key);
            litStatCourses.Text = courses.Count.ToString();
            litStatStudents.Text = courses.Sum(c => c.StudentCount).ToString();
            litStatLectures.Text = courses.Sum(c => c.LectureCount).ToString();
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            Page.Validate("ProfileGroup");
            if (!Page.IsValid) return;

            string key = InstructorStore.CurrentKey();
            InstructorStore.SaveProfile(key, new GvProfile
            {
                Username = key,
                FullName = txtFullName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Degree = txtDegree.Text.Trim(),
                Department = txtDepartment.Text.Trim(),
                Bio = txtBio.Text.Trim()
            });

            lblProfileAlert.Text = "Đã cập nhật hồ sơ cá nhân thành công.";
            lblProfileAlert.Visible = true;
            LoadProfileData();
        }

        protected void btnChangePass_Click(object sender, EventArgs e)
        {
            string oldP = txtOldPass.Text;
            string newP = txtNewPass.Text;
            string confirmP = txtConfirmPass.Text;

            if (string.IsNullOrEmpty(oldP) || string.IsNullOrEmpty(newP))
            {
                lblPassAlert.CssClass = "alert-banner alert-banner--error";
                lblPassAlert.Text = "Vui lòng nhập đầy đủ mật khẩu hiện tại và mật khẩu mới.";
                lblPassAlert.Visible = true;
                return;
            }
            if (newP.Length < 6)
            {
                lblPassAlert.CssClass = "alert-banner alert-banner--error";
                lblPassAlert.Text = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                lblPassAlert.Visible = true;
                return;
            }
            if (newP != confirmP)
            {
                lblPassAlert.CssClass = "alert-banner alert-banner--error";
                lblPassAlert.Text = "Mật khẩu xác nhận không khớp.";
                lblPassAlert.Visible = true;
                return;
            }

            string err;
            if (InstructorStore.ChangePassword(InstructorStore.CurrentKey(), oldP, newP, out err))
            {
                lblPassAlert.CssClass = "alert-banner alert-banner--success";
                lblPassAlert.Text = "Đổi mật khẩu thành công.";
            }
            else
            {
                lblPassAlert.CssClass = "alert-banner alert-banner--error";
                lblPassAlert.Text = err;
            }
            lblPassAlert.Visible = true;
        }
    }
}