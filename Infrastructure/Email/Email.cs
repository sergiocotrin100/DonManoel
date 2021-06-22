using Core.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Infrastructure.Email
{
    public class Email : IEmail
    {
        public Email(IOptions<EmailConfiguracao> emailConfiguracao)
        {
            _emailConfiguracao = emailConfiguracao.Value;
        }

        public EmailConfiguracao _emailConfiguracao { get; }
        public Task EnviarEmailAsync(string email, string assunto, string mensagem, List<string> lstEmailsCopia = null)
        {
            try
            {
                Execute(email, assunto, mensagem, lstEmailsCopia).Wait();
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task EnviarEmailAsync(string email, string subject, string body, string username, string password, string frommail, string fromname, bool sMTPAuth, int port, string host, string anexo, List<string> lstEmailsCopia = null)
        {
            try
            {
                Execute(email, subject, body, username, password, frommail, fromname, sMTPAuth, port, host, anexo, lstEmailsCopia).Wait();
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task Execute(string email, string subject, string body, List<string> lstEmailsCopia = null)
        {
            try
            {
                SmtpClient client = new SmtpClient(_emailConfiguracao.PrimaryDomain);
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_emailConfiguracao.UsernameEmail, _emailConfiguracao.UsernamePassword);

                string toEmail = string.IsNullOrEmpty(email) ? _emailConfiguracao.ToEmail : email;
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_emailConfiguracao.UsernameEmail);
                mailMessage.To.Add(toEmail);
                if(lstEmailsCopia != null && lstEmailsCopia.Count>0)
                    foreach (var item in lstEmailsCopia)
                    {
                        mailMessage.CC.Add(item);
                    }
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = body;
                mailMessage.Subject = subject;
                await client.SendMailAsync(mailMessage);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Execute(string email, string subject, string body, string username, string password, string frommail, string fromname, bool sMTPAuth, int port, string host,string anexo, List<string> lstEmailsCopia = null)
        {
            try
            {
                SmtpClient client = new SmtpClient(host,port);
                
              //  client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(username, password);

                string toEmail = string.IsNullOrEmpty(email) ? _emailConfiguracao.ToEmail : email;
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(frommail, fromname);
                mailMessage.To.Add(toEmail);
                if (lstEmailsCopia != null && lstEmailsCopia.Count > 0)
                    foreach (var item in lstEmailsCopia)
                    {
                        mailMessage.CC.Add(item);
                    }
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = body;
                mailMessage.Subject = subject;

                if (File.Exists(anexo))
                {
                    mailMessage.Attachments.Add(new Attachment(anexo));
                }

                client.UseDefaultCredentials = sMTPAuth;
                // client.EnableSsl = true;

                client.Timeout = 10000;
                await client.SendMailAsync(mailMessage);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
