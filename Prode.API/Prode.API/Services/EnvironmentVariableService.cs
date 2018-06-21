using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Prode.API.Services
{
    public interface IEnvironmentVariableService
    {
        Task<string> GetValueAsync(string key);
    }

    public class EnvironmentVariableService:IEnvironmentVariableService
    {

        private readonly IDbService _dbService;

        public EnvironmentVariableService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<string> GetValueAsync(string key)
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                var v = await db.ExecuteScalarAsync<string>(@"
Select Value
From EnvironmentVariables
Where Key = @key", new
                {
                    key
                });
                return (string.IsNullOrEmpty(v) ? "": v);
            }
        }
    }
}
