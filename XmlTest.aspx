
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="XmlTest.aspx.cs"
    Inherits="EduLearn.XmlTest" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <title>Kiểm tra dữ liệu XML - EduLearn</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background: #f1f5f9;
            padding: 30px;
        }

        .container {
            max-width: 800px;
            margin: auto;
            background: white;
            padding: 25px;
            border-radius: 10px;
        }

        h2 { color: #1e293b; }

        .success {
            color: #15803d;
            background: #dcfce7;
            padding: 15px;
            border-radius: 6px;
        }

        .error {
            color: #b91c1c;
            background: #fee2e2;
            padding: 15px;
            border-radius: 6px;
            margin-bottom: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Kiểm tra cơ sở dữ liệu XML EduLearn</h2>
            <asp:Literal ID="litResult" runat="server" />
        </div>
    </form>
</body>
</html>
