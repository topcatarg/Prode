using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;

namespace Prode.API.Services
{

    public interface IMailServices
    {
        Task<HttpStatusCode> SendMail(string MailTo);
    }

    public class MailServices: IMailServices
    {
        private string ApiKey { get;}
        private readonly IConfiguration _configuration;

        public MailServices(IConfiguration configuration)
        {
            _configuration = configuration;
            ApiKey = _configuration.GetValue<string>("APIKEY"); ;
        }

        public async Task<HttpStatusCode> SendMail(string MailTo)
        {

            var client = new SendGridClient(ApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("donotreply@prodemundial.com", "El prode"),
                Subject = "Hello World from the SendGrid CSharp SDK!",
                PlainTextContent = "Hello, Email!",
                HtmlContent = "<strong>Hello, Email!</strong> key: "
            };
            msg.AddTo(new EmailAddress(MailTo));
            var response = await client.SendEmailAsync(msg);
            return response.StatusCode;
        }
    }
}
