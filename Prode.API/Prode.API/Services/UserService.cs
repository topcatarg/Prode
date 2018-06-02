using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        Task<bool> StoreGuidRecovery(Guid guid, string mail);

        Task<bool> ChangePasswordAfterLost(string pass, string guid);

        Task<bool> ChangePassword(int UserId, string pass);

        Task<bool> ChangeTeamName(int UserId, string TeamName);

        Task<bool> ChangeReceiveMails(int UserId, int ReceiveMails, int ReceiveAdminMails);

        Task<ImmutableArray<string>> GetMailsFromAdminForecastReceivers();

        Task<bool> JoinGroup(int UserId, string GroupName);

        Task<ImmutableArray<Groups>> GetUserGroups(int UserId);
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
insert into UserGroups (UserId, GroupId)
values (@userId, @GameGroupId);

insert into UserForecast (UserId,MatchId,Team1Goals,Team2Goals,ScorePerGame)
select @userId,id,0,0,0
from Matches;", new
                {
                    userId,
                    GameGroupId
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

        public async Task<bool> StoreGuidRecovery(Guid guid, string mail)
        {
            int userid = await GetUserIdFromMail(mail);
            using (var db = _dbService.SimpleDbConnection())
            {
                return (await db.ExecuteAsync(@"
Delete from PassRecovery
where UserId = @userid;

Insert into PassRecovery (GUID, UserId)
Values (@guid, @userid);", new
                {
                    userid,
                    guid = guid.ToString()
                }) > 0);
            }
        }

        public async Task<bool> ChangePasswordAfterLost(string pass, string guid)
        {
            string newpass = EncryptPassword(pass);
            using (var db = _dbService.SimpleDbConnection())
            {
                //get user from guid
                int userid = await db.ExecuteScalarAsync<int>(@"
Select UserId
From PassRecovery
Where GUID = @guid", new
                {
                    guid
                });
                if (userid == 0)
                {
                    return false;
                }
                return (await db.ExecuteAsync(@"
Update Users
set Password = @newpass
Where ID = @userid;

Delete From PassRecovery
Where UserId = @userid;", new
                {
                    newpass,
                    userid
                }) > 0);
            }
        }

        public async Task<bool> ChangePassword(int UserId, string pass)
        {
            string newpass = EncryptPassword(pass);
            using (var db = _dbService.SimpleDbConnection())
            {
                return (await db.ExecuteAsync(@"
Update Users
set Password = @newpass
Where ID = @userid", new
                {
                    newpass,
                    userid = UserId
                }) > 0);
            }
        }

        public async Task<bool> ChangeTeamName(int UserId, string TeamName)
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                return (await db.ExecuteAsync(@"
Update Users
set TeamName = @TeamName
Where ID = @userid", new
                {
                    TeamName,
                    userid = UserId
                }) > 0);
            }
        }

        public async Task<bool> ChangeReceiveMails(int UserId, int ReceiveMails, int ReceiveAdminMails)
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                return (await db.ExecuteAsync(@"
Update Users
set ReceiveMails = @ReceiveMails,
ReceiveAdminMails = @ReceiveAdminMails
Where ID = @userid", new
                {
                    ReceiveMails,
                    ReceiveAdminMails,
                    userid = UserId
                }) > 0);
            }
        }

        public async Task<ImmutableArray<string>> GetMailsFromAdminForecastReceivers()
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                return (await db.QueryAsync<string>(@"
Select Mail
From Users
Where ReceiveAdminMails = 1")).ToImmutableArray();
            }
        }

        public async Task<bool> JoinGroup(int UserId, string GroupName)
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                return (await db.ExecuteAsync(@"
insert into UserGroups (UserId, GroupId)
values (@UserId, (select Id from GameGroups where gamegroup = @GroupName))", new
                {
                    UserId,
                    GroupName
                }) > 0);
            }
        }
        public async Task<ImmutableArray<Groups>> GetUserGroups(int UserId)
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                return (await db.QueryAsync<Groups>(@"
Select *
From GameGroups
Where Id in 
(Select GroupId From UserGroups where UserId = @UserId)", new
                {
                    UserId
                })).ToImmutableArray();
            }
        }

        #region Private methods

        private async Task<int> GetUserIdFromMail(string mail)
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                return await db.ExecuteScalarAsync<int>(@"
Select ID 
From Users
Where Mail = @mail", new
                {
                    mail
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

        #endregion
    }
}
