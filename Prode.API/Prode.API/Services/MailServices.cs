using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Prode.API.Services
{

    public interface IMailServices
    {
        void SendMail(string MailTo);
    }

    public class MailServices: IMailServices
    {
        private string ApiKey { get;}

        public MailServices(string key)
        {
            ApiKey = "";
        }

        public async void SendMail(string MailTo)
        {
            var client = new SendGridClient(ApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("donotreply@prodemundial.com", "El prode"),
                Subject = "Hello World from the SendGrid CSharp SDK!",
                PlainTextContent = "Hello, Email!",
                HtmlContent = "<strong>Hello, Email!</strong>"
            };
            msg.AddTo(new EmailAddress(MailTo));
            var response = await client.SendEmailAsync(msg);

        }
    }
}
