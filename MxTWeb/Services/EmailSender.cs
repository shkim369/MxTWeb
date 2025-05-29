using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using MxTWeb.Data;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MxTWeb.Services
{
    public class EmailSender : IEmailSender<ApplicationUser>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;


        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configuration["SendGrid:SenderEmail"], "VisualAcademy");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, "이메일 확인",
                "", $"<p><a href='{confirmationLink}'>계정 확인하기</a></p>");

            await client.SendEmailAsync(msg);
        }

        public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            await SendEmailAsync(email, "비밀번호 재설정",
                $"<h2>비밀번호 재설정</h2><p>다음 링크를 클릭하여 비밀번호를 재설정하세요:</p><p><a href='{resetLink}'>비밀번호 재설정하기</a></p>");
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            return SendEmailAsync(email, "비밀번호 재설정 코드",
                $"<h2>비밀번호 재설정 코드</h2><p>코드: <strong>{resetCode}</strong></p>");
        }

        private async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("VisualAcademy", _configuration["EmailSettings:SenderEmail"]));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_configuration["EmailSettings:SmtpServer"],
                int.Parse(_configuration["EmailSettings:SmtpPort"]), true);
            await client.AuthenticateAsync(_configuration["EmailSettings:SenderEmail"],
                _configuration["EmailSettings:SenderPassword"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        //public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        //{
        //    var subject = "VisualAcademy 계정 확인";
        //    var htmlContent = $@"
        //        <div style='font-family: Arial, sans-serif;'>
        //            <h2>계정 확인</h2>
        //            <p>안녕하세요, {user.UserName}님!</p>
        //            <p>VisualAcademy 가입을 완료하려면 아래 버튼을 클릭하세요:</p>
        //            <a href='{confirmationLink}' 
        //               style='background-color: #007bff; color: white; padding: 10px 20px; 
        //                      text-decoration: none; border-radius: 5px; display: inline-block;'>
        //                계정 확인하기
        //            </a>
        //            <p><small>이 메일은 발신 전용입니다.</small></p>
        //        </div>";

        //    var plainContent = $"계정 확인을 위해 다음 링크를 클릭하세요: {confirmationLink}";

        //    await SendEmailAsync(email, subject, htmlContent, plainContent);
        //}

        //public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        //{
        //    var subject = "VisualAcademy 비밀번호 재설정";
        //    var htmlContent = $@"
        //        <div style='font-family: Arial, sans-serif;'>
        //            <h2>비밀번호 재설정</h2>
        //            <p>안녕하세요, {user.UserName}님!</p>
        //            <p>비밀번호를 재설정하려면 아래 버튼을 클릭하세요:</p>
        //            <a href='{resetLink}' 
        //               style='background-color: #dc3545; color: white; padding: 10px 20px; 
        //                      text-decoration: none; border-radius: 5px; display: inline-block;'>
        //                비밀번호 재설정하기
        //            </a>
        //            <p><small>이 메일은 발신 전용입니다.</small></p>
        //        </div>";

        //    var plainContent = $"비밀번호 재설정을 위해 다음 링크를 클릭하세요: {resetLink}";

        //    await SendEmailAsync(email, subject, htmlContent, plainContent);
        //}

        //public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        //{
        //    var subject = "VisualAcademy 비밀번호 재설정 코드";
        //    var htmlContent = $@"
        //        <div style='font-family: Arial, sans-serif;'>
        //            <h2>비밀번호 재설정 코드</h2>
        //            <p>안녕하세요, {user.UserName}님!</p>
        //            <p>비밀번호 재설정 코드입니다:</p>
        //            <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; 
        //                        font-size: 20px; font-weight: bold; text-align: center;'>
        //                {resetCode}
        //            </div>
        //            <p><small>이 메일은 발신 전용입니다.</small></p>
        //        </div>";

        //    var plainContent = $"비밀번호 재설정 코드: {resetCode}";

        //    await SendEmailAsync(email, subject, htmlContent, plainContent);
        //}

        //private async Task SendEmailAsync(string email, string subject, string htmlContent, string plainContent)
        //{
        //    var apiKey = _configuration["SendGrid:ApiKey"];
        //    var client = new SendGridClient(apiKey);

        //    var from = new EmailAddress(_configuration["SendGrid:SenderEmail"], _configuration["SendGrid:SenderName"]);
        //    var to = new EmailAddress(email);

        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainContent, htmlContent);

        //    try
        //    {
        //        var response = await client.SendEmailAsync(msg);
        //        _logger.LogInformation($"Email sent to {email}. Status: {response.StatusCode}");

        //        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
        //        {
        //            var responseBody = await response.Body.ReadAsStringAsync();
        //            _logger.LogError($"Failed to send email. Status: {response.StatusCode}, Body: {responseBody}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Exception occurred while sending email to {email}");
        //        throw;
        //    }
        //}
    }
}
