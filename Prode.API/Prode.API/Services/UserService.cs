using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Prode.API.Models;

namespace Prode.API.Services
{

    public interface IUserService
    {
        Task<UserInfo> LoginUserAsync(string user, string password);

        Task<bool> CreateUserAsync(string user, string password, string mail, string TeamName, int GameGroupId);

        Task<bool> UserExists(string user);

        Task<bool> MailExists(string mail);

        Task<int> GroupExistAsync(string group);
    }

    public class UserService: IUserService
    {

        private readonly IDbService _dbService;

        public UserService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<UserInfo> LoginUserAsync(string user, string password)
        {
            string newpass = EncryptPassword(password);
            UserInfo v;
            using (var db = _dbService.SimpleDbConnection())
            {
                v = await db.QueryFirstOrDefaultAsync<UserInfo>(@"
Select * 
From Users
Where Name=@name 
And   Password=@password", new
                {
                    name = user,
                    password = newpass
                });
            }
            return v;
        }

        public async Task<bool> CreateUserAsync(string user, string password, string mail, string TeamName, int GameGroupId)
        {
            //Encripto el pass
            string newpass = EncryptPassword(password);
            int v;
            using (var db = _dbService.SimpleDbConnection())
            {
                v = await db.ExecuteAsync(@"
Insert into Users
(Name, Password, TeamName, Mail, GameGroupId)
Values(@name,@password,@TeamName, @mail, @GameGroupId)", new
                {
                    name = user,
                    password = newpass,
                    mail,
                    TeamName,
                    GameGroupId
                });
                if (v == 0)
                {
                    return false;
                }
                //Get the user
                var userId = await db.ExecuteScalarAsync<string>(@"
Select ID
From Users
Where Name = @name", new
                {
                    name = user

                });
                v = await db.ExecuteAsync(@"
insert into UserForecast (UserId,MatchId,Team1Goals,Team2Goals,ScorePerGame)
select @userId,id,0,0,0
from Matches", new
                {
                    userId
                });
            }
            return (v > 0);
        }

        public async Task<bool> UserExists(string user)
        {
            int v;
            using (var db = _dbService.SimpleDbConnection())
            {
                v = await db.ExecuteScalarAsync<int>(@"
Select count(*)
From Users
Where Name=@name", new
                {
                    name = user
                });
            }
            return (v > 0);
        }

        public async Task<bool> MailExists(string mail)
        {
            int v;
            using (var db = _dbService.SimpleDbConnection())
            {
                v = await db.ExecuteScalarAsync<int>(@"
Select count(*)
From Users
Where Mail=@mail", new
                {
                    mail
                });
            }
            return (v > 0);
        }

        public async Task<int> GroupExistAsync(string group)
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                return await db.ExecuteScalarAsync<int>(@"
Select count(*)
From GameGroups
Where GameGroup = @gamegroup", new
                {
                    gamegroup = group.Trim().ToLower()
                });
            }
        }

        private string EncryptPassword(string pass)
        {
            byte[] data = Encoding.UTF8.GetBytes(pass);
            SHA512 shaM = new SHA512Managed();
            byte[] resul = shaM.ComputeHash(data);
            return Encoding.UTF8.GetString(resul);
        }
    }
}
