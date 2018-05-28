using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Prode.API.Models;
using Dapper;

namespace Prode.API.Services
{
    public interface IAdminService
    {
        Task<ImmutableArray<Matchs>> GetAllMatchs();

        Task<ImmutableArray<Teams>> GetTeams();

        Task<bool> UpdateGame(MatchResult match);
    }

    public class AdminService : IAdminService
    {

        private readonly IDbService _dbService;

        public AdminService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<ImmutableArray<Matchs>> GetAllMatchs()
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                var v = await db.QueryAsync<Matchs>(@"
Select m.*,
(select Team from Teams where id = Team1) as Team1Name,
(select Team from Teams where id = Team2) as Team2Name
From Matches m ");
                return v.ToImmutableArray(); ;
            }
        }

        public async Task<ImmutableArray<Teams>> GetTeams()
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                var v = await db.QueryAsync<Teams>(@"
select * from Teams");
                return v.ToImmutableArray(); ;
            }
        }

        public async Task<bool> UpdateGame(MatchResult match)
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                var v = await db.ExecuteAsync(@"
Update Matches
Set Team1 = @team1,
Team2 = @team2,
Team1Goals = @goals1,
Team2Goals = @goals2,
closed = 1
where id = @matchid", new
                {
                    team1 = match.Team1,
                    team2 = match.Team2,
                    goals1 = match.Team1Goals,
                    goals2 = match.Team2Goals,
                    matchid = match.MatchId
                });
                return v > 0 ;
            }
        }

    }
}
