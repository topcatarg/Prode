using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Prode.API.Models;
using Dapper;

namespace Prode.API.Services
{
    public interface IResultService
    {
        Task<ImmutableArray<Results>> GetResults(int GroupId);
    }

    public class ResultService: IResultService
    {

        private readonly IDbService _dbService;

        public ResultService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<ImmutableArray<Results>> GetResults(int GroupId)
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                var v = await db.QueryAsync<Results>(@"
Select 
teamName,
score
From Users
Order by Score desc");
                return v.ToImmutableArray();
            }
        }
    }
}
