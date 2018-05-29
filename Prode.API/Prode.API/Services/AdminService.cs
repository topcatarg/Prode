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

		Task<bool> UpdateScores();
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

		public async Task<bool> UpdateScores()
		{
			using (var db = _dbService.SimpleDbConnection())
			{
				//get all valid results
				var results = await db.QueryAsync<ValidResults>(@"
select m.*, userid, uf.team1goals as usergoals1, uf.team2goals as usergoals1,
case 
	when m.team1goals = uf.team1goals and m.team2goals = uf.team2goals then 3
	when m.team1goals = m.team2goals and uf.team1goals = uf.team2goals then 1
	when m.team1goals - m.team2goals = uf.team1goals - uf.team2goals then 2
	when (m.team1goals > m.team2goals and uf.team1goals > uf.team2goals)
		or (m.team1goals < m.team2goals and uf.team1goals < uf.team2goals) then 1
	else 0 
end as score
from 
(select id, team1goals, team2goals
from Matches
where closed = 1) m inner join UserForecast uf on m.id = uf.MatchId");
				foreach (var r in results)
				{
					if (r.score > 0)
					{
						var rest = await db.ExecuteAsync(@"
Update UserForecast 
Set ScorePerGame = @score
Where UserId = @userid and MatchId = @matchid", new
						{
							r.score,
							r.userid,
							matchid = r.id
						});
						if (rest == 0)
						{
							return false;
						}

					}
				}
				var scores = await db.QueryAsync<UserScores>(@"
select userid, sum(scorepergame) as score
from UserForecast
group by userid");
				foreach(var s in scores)
				{
					var r = await db.ExecuteAsync(@"
Update Users
Set Score = @score
Where Id = @userid", new
					{
						score = s.Score,
						userid = s.Userid
					});
					if (r == 0)
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
