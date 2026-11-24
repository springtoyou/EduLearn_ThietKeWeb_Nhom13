using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using EduLearn.Models;
using EduLearn.Services;

namespace EduLearn.Student
{
    public partial class Profile : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Student"))
            {
                return;
            }

            txtDateOfBirth.Attributes["max"] =
                DateTime.Today.ToString("yyyy-MM-dd");

            if (!IsPostBack)
            {
                BindProfile(AuthSession.GetCurrentUser());
            }
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Student"))
            {
                return;
            }

            User currentUser = AuthSession.GetCurrentUser();
            if (currentUser == null)
            {
                return;
            }

            try
            {
                UserService.UpdateStudentProfile(
                    currentUser.UserId,
                    txtFullName.Text,
                    txtPhone.Text,
                    ddlGender.SelectedValue,
                    txtDateOfBirth.Text);

                User updatedUser =
                    UserService.GetUserById(currentUser.UserId);
                if (updatedUser == null)
                {
                    throw new InvalidOperationException(
                        "Không thể tải lại thông tin hồ sơ vừa cập nhật.");
                }

                currentUser.FullName = updatedUser.FullName;
                currentUser.Phone = updatedUser.Phone;
                currentUser.Gender = updatedUser.Gender;
                currentUser.DateOfBirth = updatedUser.DateOfBirth;
                currentUser.UpdatedAt = updatedUser.UpdatedAt;
                AuthSession.SetCurrentUser(currentUser);

                BindProfile(updatedUser);
                UpdateMasterUserName(currentUser.FullName);
                ShowMessage("Hồ sơ đã được cập nhật.", false);
            }
            catch (ArgumentException exception)
            {
                ShowMessage(exception.Message, true);
            }
            catch (InvalidOperationException exception)
            {
                ShowMessage(exception.Message, true);
            }
        }

        private void BindProfile(User sessionUser)
        {
            User user = sessionUser == null
                ? null
                : UserService.GetUserById(sessionUser.UserId);

            if (user == null)
            {
                btnSaveProfile.Enabled = false;
                ShowMessage(
                    "Không tìm thấy thông tin tài khoản sinh viên.",
                    true);
                return;
            }

            string fullName = string.IsNullOrWhiteSpace(user.FullName)
                ? "Sinh viên"
                : user.FullName.Trim();

            litProfileName.Text = Server.HtmlEncode(fullName);
            litProfileInitials.Text =
                Server.HtmlEncode(GetInitials(fullName));
            litAccountStatus.Text =
                Server.HtmlEncode(GetStatusText(user.Status));
            litStudentId.Text = Server.HtmlEncode(user.UserId);
            litUsername.Text = Server.HtmlEncode(user.Username);
            litEmail.Text = Server.HtmlEncode(user.Email);
            litCreatedAt.Text = Server.HtmlEncode(FormatDate(user.CreatedAt));
            litUpdatedAt.Text = Server.HtmlEncode(FormatDate(user.UpdatedAt));

            txtFullName.Text = user.FullName;
            txtPhone.Text = user.Phone;
            txtDateOfBirth.Text = FormatDateInput(user.DateOfBirth);

            ListItem genderItem = ddlGender.Items.FindByValue(
                user.Gender ?? string.Empty);
            ddlGender.ClearSelection();
            if (genderItem != null)
            {
                genderItem.Selected = true;
            }
            else
            {
                ddlGender.SelectedIndex = 0;
            }
        }

        private void ShowMessage(string message, bool isError)
        {
            pnlProfileMessage.Visible = true;
            pnlProfileMessage.CssClass = isError
                ? "profile-message is-error"
                : "profile-message is-success";
            lblProfileMessage.Text = Server.HtmlEncode(message);
        }

        private void UpdateMasterUserName(string fullName)
        {
            string displayName = string.IsNullOrWhiteSpace(fullName)
                ? "Người dùng"
                : fullName.Trim();
            string initials = GetInitials(displayName);

            Label sidebarName =
                FindControlRecursive(Master, "lblSidebarName") as Label;
            Label headerName =
                FindControlRecursive(Master, "lblHeaderName") as Label;
            Label sidebarInitials =
                FindControlRecursive(Master, "lblSidebarInitials") as Label;
            Label headerInitials =
                FindControlRecursive(Master, "lblHeaderInitials") as Label;

            if (sidebarName != null)
            {
                sidebarName.Text = Server.HtmlEncode(displayName);
            }
            if (headerName != null)
            {
                headerName.Text = Server.HtmlEncode(displayName);
            }
            if (sidebarInitials != null)
            {
                sidebarInitials.Text = Server.HtmlEncode(initials);
            }
            if (headerInitials != null)
            {
                headerInitials.Text = Server.HtmlEncode(initials);
            }
        }

        private static Control FindControlRecursive(Control root, string id)
        {
            if (root == null)
            {
                return null;
            }

            if (root.ID == id)
            {
                return root;
            }

            foreach (Control child in root.Controls)
            {
                Control match = FindControlRecursive(child, id);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private static string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "SV";
            }

            string[] words = fullName.Trim().Split(
                new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 1)
            {
                return words[0].Substring(0, 1).ToUpperInvariant();
            }

            return (
                words[0].Substring(0, 1) +
                words[words.Length - 1].Substring(0, 1)
            ).ToUpperInvariant();
        }

        private static string GetStatusText(string status)
        {
            if (string.Equals(
                status,
                "Active",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Đang hoạt động";
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                return "Chưa xác định";
            }

            return status;
        }

        private static string FormatDate(string value)
        {
            DateTime date;
            if (!DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date))
            {
                return "Chưa cập nhật";
            }

            return date.ToString(
                "dd/MM/yyyy",
                CultureInfo.GetCultureInfo("vi-VN"));
        }

        private static string FormatDateInput(string value)
        {
            DateTime date;
            if (!DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date))
            {
                return string.Empty;
            }

            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
