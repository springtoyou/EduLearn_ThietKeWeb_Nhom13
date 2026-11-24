using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using EduLearn.Instructor.Data;
using EduLearn.Services;

namespace EduLearn.Instructor
{
    public partial class Notifications : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Instructor")) return;

            if (!IsPostBack)
            {
                string key = InstructorStore.CurrentKey();
                ddlTargetCourse.Items.Add(new ListItem("Tất cả các lớp đang phụ trách", "ALL"));
                foreach (CourseItem c in InstructorStore.GetCourses(key))
                {
                    ddlTargetCourse.Items.Add(new ListItem(c.Name + " (" + c.Id + ")", c.Id));
                }

                if (Request.QueryString["new"] == "1")
                {
                    txtNotiTitle.Focus();
                }

                BindNotifications();
            }
        }

        protected void btnSendNoti_Click(object sender, EventArgs e)
        {
            Page.Validate("NotiGroup");
            if (!Page.IsValid) return;

            InstructorStore.CreateNotification(
                InstructorStore.CurrentKey(),
                ddlTargetCourse.SelectedValue,
                txtNotiTitle.Text.Trim(),
                txtNotiMessage.Text.Trim()
            );

            txtNotiTitle.Text = string.Empty;
            txtNotiMessage.Text = string.Empty;
            lblAlert.Text = "Đã gửi thông báo thành công tới sinh viên.";
            lblAlert.Visible = true;
            BindNotifications();
        }

        protected void btnMarkAllRead_Click(object sender, EventArgs e)
        {
            InstructorStore.MarkAllNotificationsRead(InstructorStore.CurrentKey());
            lblAlert.Text = "Đã đánh dấu tất cả thông báo là đã đọc.";
            lblAlert.Visible = true;
            BindNotifications();
        }

        protected void rptNotifications_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string key = InstructorStore.CurrentKey();
            string id = Convert.ToString(e.CommandArgument);

            if (e.CommandName == "MarkRead")
                InstructorStore.MarkNotificationRead(key, id);
            else if (e.CommandName == "DeleteNoti")
                InstructorStore.DeleteNotification(key, id);

            BindNotifications();
        }

        private void BindNotifications()
        {
            string key = InstructorStore.CurrentKey();
            List<NotificationItem> list = InstructorStore.GetNotifications(key);
            int unread = InstructorStore.UnreadCount(key);

            litUnreadSummary.Text = unread + " chưa đọc / " + list.Count + " thông báo";
            pnlEmpty.Visible = list.Count == 0;
            rptNotifications.DataSource = list;
            rptNotifications.DataBind();
        }
    }
}