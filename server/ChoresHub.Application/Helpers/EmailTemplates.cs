using System;

namespace ChoresHub.Application.Helpers
{
    public static class EmailTemplates
    {
        public static (string Html, string Text) BuildResetPassword(string appName, string resetLink)
        {
            var html = $@"
<!doctype html>
<html>
  <body style=""margin:0;padding:0;font-family:Arial,sans-serif;background:#f6f7fb;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""padding:24px;"">
      <tr>
        <td align=""center"">
          <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#ffffff;border-radius:8px;padding:24px;border:1px solid #e6e9ef;"">
            <tr>
              <td style=""font-size:20px;font-weight:700;color:#111827;"">{appName}</td>
            </tr>
            <tr>
              <td style=""padding-top:16px;font-size:14px;color:#374151;"">
                You requested a password reset. Click the button below to continue.
              </td>
            </tr>
            <tr>
              <td style=""padding-top:20px;"">
                <a href=""{resetLink}"" style=""background:#2563eb;color:#ffffff;text-decoration:none;padding:10px 16px;border-radius:6px;display:inline-block;"">Reset Password</a>
              </td>
            </tr>
            <tr>
              <td style=""padding-top:16px;font-size:12px;color:#6b7280;"">
                This link expires in 15 minutes. If you didn’t request this, you can ignore this email.
              </td>
            </tr>
            <tr>
              <td style=""padding-top:12px;font-size:12px;color:#9ca3af;"">
                Or copy and paste this link: {resetLink}
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>
";
            var text =
$@"{appName}
You requested a password reset.

Open this link to reset your password:
{resetLink}

This link expires in 15 minutes. If you didn’t request this, ignore this email.";
            return (html, text);
        }
    }
}
