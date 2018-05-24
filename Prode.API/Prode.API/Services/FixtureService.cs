using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Threading.Tasks;
using Prode.API.Models;
using System.Collections.Immutable;

namespace Prode.API.Services
{
    public interface IFixtureService
    {
        Task<ImmutableArray<Matchs>> GetAllMatchs();
    }

    public class FixtureService: IFixtureService
    {
        private readonly IDbService _dbService;

        public FixtureService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<ImmutableArray<Matchs>> GetAllMatchs()
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                var v = await db.QueryAsync<Matchs>(@"
select *,
(select Team from Teams t where t.id = m.team1) as Team1Name,
(select Team from Teams t where t.id = m.team2) as Team2Name,
(select Code from Teams t where t.id = m.team1) as Team1Flag,
(select Code from Teams t where t.id = m.team2) as Team2Flag
from Matches m");
                return v.ToImmutableArray();
            }
        }

    }
}
