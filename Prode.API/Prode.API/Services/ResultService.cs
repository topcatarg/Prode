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
Select TeamName, score
From UserGroups ug inner join Users us on ug.UserId = us.ID 
Where GroupId = @GroupId
Order by score desc", new
                {
                    GroupId
                });
                return v.ToImmutableArray();
            }
        }
    }
}
