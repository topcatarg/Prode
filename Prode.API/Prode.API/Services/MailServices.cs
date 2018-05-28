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
        Task<HttpStatusCode> SendHelloMail(string MailTo);
    }

    public class MailServices: IMailServices
    {
        private string ApiKey { get;}
        private readonly IConfiguration _configuration;
        private const string FromAddres = @"donotreply@prodemundial.com";
        private const string FromName = @"El prode";
        
        public MailServices(IConfiguration configuration)
        {
            _configuration = configuration;
            ApiKey = _configuration.GetValue<string>("APIKEY"); ;
        }

        public async Task<HttpStatusCode> SendHelloMail(string MailTo)
        {

            var client = new SendGridClient(ApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(FromAddres, FromName),
                Subject = "Bienvenido al prode mundial!",
                PlainTextContent = @"
Este mail es para comprobar la registración al prode.
Por favor, presione sobre el siguiente link para comprobar el mail, y asi poder recuperar su clave o recibir novedades.

(Luego del mundial, la base va a ser borrada, asi que olvidense de recibir mas mails).
"
            };
            msg.AddTo(new EmailAddress(MailTo));
            var response = await client.SendEmailAsync(msg);
            return response.StatusCode;
        }

    }
}
