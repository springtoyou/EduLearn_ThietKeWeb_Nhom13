
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using EduLearn.DataAccess;

namespace EduLearn
{
    public partial class XmlTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Chỉ cho phép kiểm tra từ máy đang chạy website.
            if (!Request.IsLocal)
            {
                Response.StatusCode = 403;
                litResult.Text = "<p>Không được phép truy cập.</p>";
                return;
            }

            List<string> errors = XmlDataValidator.ValidateAll();

            StringBuilder html = new StringBuilder();

            if (errors.Count == 0)
            {
                html.Append(
                    "<div class='success'>" +
                    "<h3>Kiểm tra thành công!</h3>" +
                    "<p>Không phát hiện lỗi liên kết dữ liệu XML.</p>" +
                    "</div>"
                );
            }
            else
            {
                html.Append(
                    "<h3>Phát hiện " + errors.Count +
                    " vấn đề cần kiểm tra:</h3>"
                );

                foreach (string error in errors)
                {
                    html.Append(
                        "<div class='error'>" +
                        HttpUtility.HtmlEncode(error) +
                        "</div>"
                    );
                }
            }

            litResult.Text = html.ToString();
        }
    }
}
