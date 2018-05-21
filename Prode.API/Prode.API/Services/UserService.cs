using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.SQLite;
using Prode.API.Models;

namespace Prode.API.Services
{

    public interface IUserService
    {
        Task<UserInfo> LoginUserAsync(string user, string password);

        Task<bool> CreateUserAsync(string user, string password, string mail);
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
                    password
                });
            }
            return v;
        }

        public async Task<bool> CreateUserAsync(string user, string password, string mail)
        {
            int v;
            using (var db = _dbService.SimpleDbConnection())
            {
                v = await db.ExecuteAsync(@"
Insert into Users
(Name, Password, Mail)
Values(@name,@password,@mail)", new
                {
                    name = user,
                    password,
                    mail
                });
            }
            return (v > 0);
        }
    }
}
