using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using System.Collections.Immutable;
using Prode.API.Models;
using System.Text;

namespace Prode.API.Services
{

    public interface IMailServices
    {
        Task<HttpStatusCode> SendHelloMail(string MailTo);

        Task<HttpStatusCode> SendRecoverPassword(string mail, Guid guid);

        Task<HttpStatusCode> SendMyResultsAsync(string mail, string TeamName, ImmutableArray<Matchs> matchs);

        Task<HttpStatusCode> SendAdminResultsAsync(ImmutableArray<string> MailsTo, string TeamName, ImmutableArray<Matchs> matchs);
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
            ApiKey = _configuration.GetValue<string>("APIKEY");
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
Podes ir a la seccion perfil y pedir recibir un mail cada vez que cambies tu pronostico (o cuando el administrador cambie el suyo).

(Luego del mundial, la base va a ser borrada, asi que olvidense de recibir mas mails).
"
            };
            msg.AddTo(new EmailAddress(MailTo));
            var response = await client.SendEmailAsync(msg);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> SendRecoverPassword(string MailTo, Guid guid)
        {
            var client = new SendGridClient(ApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(FromAddres, FromName),
                Subject = "Recuperar password de prode mundial!",
                PlainTextContent = $@"
Si perdiste la password, presiona el siguiente link (o copialo):

https://prodemundial.netlify.com/#/recoverpass?code={guid}"
            };
            msg.AddTo(new EmailAddress(MailTo));
            var response = await client.SendEmailAsync(msg);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> SendMyResultsAsync(string mail, string TeamName, ImmutableArray<Matchs> matchs)
        {
            var client = new SendGridClient(ApiKey);
            StringBuilder b = new StringBuilder();
            b.AppendLine("Estos son los resultados de los partidos que pronosticastes!");
            b.AppendLine();
            b.AppendLine("----------");
            b.AppendLine();
            foreach(var m in matchs)
            {
                b.AppendLine(m.Team1Name + " " + m.Team1Forecast + "-" + m.Team2Forecast + " " + m.Team2Name);
            }
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(FromAddres, FromName),
                Subject = "Tus nuevos resultados",
                PlainTextContent = b.ToString()
            };
            msg.AddTo(new EmailAddress(mail));
            var response = await client.SendEmailAsync(msg);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> SendAdminResultsAsync(ImmutableArray<string> MailsTo, string TeamName, ImmutableArray<Matchs> matchs)
        {
            var client = new SendGridClient(ApiKey);
            StringBuilder b = new StringBuilder();
            b.AppendLine("Estos son los resultados de los partidos que pronosticastes!");
            b.AppendLine();
            b.AppendLine("----------");
            b.AppendLine();
            foreach (var m in matchs)
            {
                b.AppendLine(m.Team1Name + " " + m.Team1Forecast + "-" + m.Team2Forecast + " " + m.Team2Name);
            }
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(FromAddres, FromName),
                Subject = "Tus nuevos resultados",
                PlainTextContent = b.ToString()
            };
            msg.AddTo(new EmailAddress(FromAddres));
            foreach (string s in MailsTo)
            {
                msg.AddBcc(new EmailAddress(s));
            }
            var response = await client.SendEmailAsync(msg);
            return response.StatusCode;
        }
    }
}
