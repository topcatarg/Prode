using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prode.API.Models;
using Dapper;
using System.Collections.Immutable;

namespace Prode.API.Services
{
    public interface IForecastService
    {
        Task<ImmutableArray<Matchs>> GetUserMatchs(int userId);

        Task<bool> FillMatch(Match MatchData);

        Task<bool> FillAllGames(Match[] MatchsData);

        Task<ImmutableArray<Matchs>> GetClosedUserMatchs(int userId);

    }

    public class ForecastService: IForecastService
    {
        private readonly IDbService _dbService;

        public ForecastService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<ImmutableArray<Matchs>> GetUserMatchs(int userId)
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                var v = await
                    db.QueryAsync<Matchs>(@"
select *,
strftime(""%d/%m %H:%M"",date) as StandardDate,
(select Team from Teams t where t.id = m.team1) as Team1Name,
(select Team from Teams t where t.id = m.team2) as Team2Name,
(select Code from Teams t where t.id = m.team1) as Team1Flag,
(select Code from Teams t where t.id = m.team2) as Team2Flag,
u.team1Goals as Team1Forecast,
u.team2Goals as Team2Forecast,
u.ScorePerGame as Points
from Matches m inner join UserForecast u on m.id = u.MatchId
where u.UserId = @userid", new
                    {
                        userid = userId
                    });
                DateTime t = GetTime();
                foreach (var l in v )
                {
                    l.CanUpdate = (l.Date.CompareTo(t) == 1);
                }
                return v.ToImmutableArray();
            }
        }

        public async Task<bool> FillMatch(Match MatchData)
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                return (await db.ExecuteAsync(@"
Update UserForecast
Set Team1Goals = @goals1, 
Team2Goals = @goals2
Where UserId = @userid and Id = @matchid", new
                {
                    userid = MatchData.UserId,
                    matchid = MatchData.MatchId,
                    goals1 = MatchData.Team1Forecast,
                    goals2 = MatchData.Team2Forecast
                }) > 0);
            }
        }


        public async Task<bool> FillAllGames(Match[] MatchsData)
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                //Get Current Time.
                DateTime ThisTime = GetTime();
                var matchs = await db.QueryAsync<Matchs>(@"
Select Date ,u.id
From Matches m join UserForecast u on m.id = matchid
where UserId = @userid", new
                {
                    userid = MatchsData[0].UserId
                });
                var validMatchs = matchs.Where(p => p.Date > ThisTime).ToList();
                foreach(var m in MatchsData)
                {
                    if (validMatchs.FirstOrDefault(p => p.Id == m.MatchId) != null)
                    {
                        var r = await db.ExecuteAsync(@"
Update UserForecast
Set Team1Goals = @goals1, 
Team2Goals = @goals2
Where UserId = @userid and Id = @matchid", new
                        {
                            userid = m.UserId,
                            matchid = m.MatchId,
                            goals1 = m.Team1Forecast,
                            goals2 = m.Team2Forecast
                        });
                        if (r == 0)
                        {
                            return false;
                        }
                    }
                }
                //Send mail with the data

                return true;
            }
        }

        public async Task<ImmutableArray<Matchs>> GetClosedUserMatchs(int userId)
        {
            using (var db = _dbService.SimpleDbConnection())
            {
                var v = await
                    db.QueryAsync<Matchs>(@"
select *,
strftime(""%d/%m %H:%M"",date) as StandardDate,
(select Team from Teams t where t.id = m.team1) as Team1Name,
(select Team from Teams t where t.id = m.team2) as Team2Name,
(select Code from Teams t where t.id = m.team1) as Team1Flag,
(select Code from Teams t where t.id = m.team2) as Team2Flag,
u.team1Goals as Team1Forecast,
u.team2Goals as Team2Forecast,
u.ScorePerGame as Points
from Matches m inner join UserForecast u on m.id = u.MatchId
where u.UserId = @userid
and m.closed = 1", new
                    {
                        userid = userId
                    });
                DateTime t = GetTime();
                foreach (var l in v)
                {
                    l.CanUpdate = (l.Date.CompareTo(t) == 1);
                }
                return v.ToImmutableArray();
            }
        }

        #region Private functions

        private DateTime GetTime()
        {
            return DateTime.UtcNow.AddMinutes(-175);
        }

        #endregion
    }
}
