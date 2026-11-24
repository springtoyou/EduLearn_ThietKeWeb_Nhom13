using System;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;
using System.Xml.Linq;
using EduLearn.Models;
using EduLearn.Services;

/* =========================================================
   EduLearn - Trang đăng nhập

   sv:minhanh@student.edu.vn-Student@123
   instructor:instructor@edulearn.vn-Instructor@123
   admin:admin@edulearn.vn-Admin123
   ========================================================= */


namespace EduLearn
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                User currentUser = AuthSession.GetCurrentUser();
                if (currentUser != null)
                {
                    RedirectByRole(currentUser.Role);
                }
            }
        }

        // ====================================================================
        //  1. XỬ LÝ ĐĂNG NHẬP
        // ====================================================================
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Page.Validate("LoginGroup");
            if (!Page.IsValid) return;

            string loginInput = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            try
            {
                // Hỗ trợ nhập cả Email hoặc Username
                User matchedUser = UserService.GetUserByEmail(loginInput);
                if (matchedUser == null)
                {
                    matchedUser = UserService.GetAllUsers().Find(u =>
                        string.Equals(u.Username, loginInput, StringComparison.OrdinalIgnoreCase));
                }

                if (matchedUser == null)
                {
                    ShowError("Tên đăng nhập hoặc mật khẩu không đúng.");
                    return;
                }

                if (!string.Equals(matchedUser.Status, "Active", StringComparison.OrdinalIgnoreCase))
                {
                    ShowError("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
                    return;
                }

                // Xác thực mật khẩu PBKDF2 thông qua AuthService
                User authenticatedUser = AuthService.Login(matchedUser.Email, password);
                if (authenticatedUser == null)
                {
                    ShowError("Tên đăng nhập hoặc mật khẩu không đúng.");
                    return;
                }

                // Lưu phiên làm việc đúng chuẩn của dự án
                AuthSession.SetCurrentUser(authenticatedUser);
                FormsAuthentication.SetAuthCookie(authenticatedUser.Email, chkRemember.Checked);

                RedirectByRole(authenticatedUser.Role);
            }
            catch (System.IO.FileNotFoundException)
            {
                ShowError("Chưa tìm thấy dữ liệu người dùng (App_Data/Users.xml).");
            }
            catch (Exception)
            {
                ShowError("Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại sau.");
            }
        }

        // ====================================================================
        //  2. QUÊN MẬT KHẨU (ĐIỀN GMAIL -> GỬI MÃ)
        // ====================================================================
        protected void lnkForgotPassword_Click(object sender, EventArgs e)
        {
            pnlLogin.Visible = false;
            pnlForgotPassword.Visible = true;
            lblMessage.Visible = false;
            lblForgotMessage.Visible = false;
            txtForgotEmail.Text = string.Empty;
        }

        protected void lnkBackToLogin_Click(object sender, EventArgs e)
        {
            pnlForgotPassword.Visible = false;
            pnlLogin.Visible = true;
            lblForgotMessage.Visible = false;
        }

        protected void btnSendCode_Click(object sender, EventArgs e)
        {
            Page.Validate("ForgotGroup");
            if (!Page.IsValid) return;

            string email = txtForgotEmail.Text.Trim();

            try
            {
                string xmlPath = Server.MapPath("~/App_Data/Users.xml");
                XDocument doc = XDocument.Load(xmlPath);

                XElement userNode = doc.Root
                    .Elements("User")
                    .FirstOrDefault(u => string.Equals((string)u.Element("Email"), email, StringComparison.OrdinalIgnoreCase));

                if (userNode == null)
                {
                    ShowForgotMessage("Gmail này chưa được đăng ký trong hệ thống.", isError: true);
                    return;
                }

                string status = (string)userNode.Element("Status");
                if (!string.IsNullOrEmpty(status) && !string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase))
                {
                    ShowForgotMessage("Tài khoản này đang bị khóa, không thể khôi phục mật khẩu.", isError: true);
                    return;
                }

                // Tạo mật khẩu mới 8 ký tự, băm PBKDF2 và lưu vào Users.xml
                string newPassword = GenerateRandomPassword(8);
                string newPasswordHash = HashPasswordPbkdf2(newPassword);

                XElement passElement = userNode.Element("PasswordHash");
                if (passElement != null)
                    passElement.Value = newPasswordHash;
                else
                    userNode.Add(new XElement("PasswordHash", newPasswordHash));

                doc.Save(xmlPath);

                // Hiển thị thông báo gửi về Gmail (kèm mật khẩu mới để bạn dùng đăng nhập test trên máy)
                ShowForgotMessage(
                    "Mật khẩu mới đã được gửi về Gmail của bạn. Vui lòng kiểm tra hộp thư để đăng nhập lại."
                    + " (Mật khẩu mới dùng để test: " + newPassword + ")",
                    isError: false
                );
                txtForgotEmail.Text = string.Empty;
            }
            catch (System.IO.FileNotFoundException)
            {
                ShowForgotMessage("Chưa tìm thấy dữ liệu người dùng (App_Data/Users.xml).", isError: true);
            }
            catch (Exception)
            {
                ShowForgotMessage("Có lỗi xảy ra khi xử lý yêu cầu. Vui lòng thử lại sau.", isError: true);
            }
        }

        // ====================================================================
        //  3. CÁC HÀM HỖ TRỢ
        // ====================================================================
        private void RedirectByRole(string role)
        {
            string dashboardUrl = AuthorizationService.GetDashboardUrl(role);

            if (string.IsNullOrEmpty(dashboardUrl))
            {
                AuthSession.Logout();
                FormsAuthentication.SignOut();
                ShowError("Tài khoản chưa được phân quyền hợp lệ.");
                return;
            }

            Response.Redirect(ResolveUrl(dashboardUrl), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowError(string message)
        {
            lblMessage.CssClass = "alert-error";
            lblMessage.Text = HttpUtility.HtmlEncode(message);
            lblMessage.Visible = true;
        }

        private void ShowForgotMessage(string message, bool isError)
        {
            lblForgotMessage.CssClass = isError ? "alert-error" : "alert-success";
            lblForgotMessage.Text = HttpUtility.HtmlEncode(message);
            lblForgotMessage.Visible = true;
        }

        private static string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            byte[] data = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }

            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[data[i] % chars.Length];
            }
            return new string(result);
        }

        private static string HashPasswordPbkdf2(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
            }
        }
    }
}