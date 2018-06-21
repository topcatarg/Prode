using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Prode.API.Models;
using Dapper;
using System.Text;
using System.Security.Cryptography;

namespace Prode.API.Services
{
	public interface IAdminService
	{
		Task<ImmutableArray<Matchs>> GetAllMatchs();

		Task<ImmutableArray<Teams>> GetTeams();

		Task<ImmutableArray<Groups>> GetGroups();

		Task<ImmutableArray<UserInfo>> GetUsers(int GroupId);

		Task<bool> UpdateGame(MatchResult match);

		Task<bool> UpdateTeams(MatchResult match);

		Task<bool> UpdateScores();

		Task<bool> CreateGroup(string GroupName);

		Task<bool> ChangePaid(int UserId);

		Task<bool> DeleteUserFromGroup(int UserId, int GroupId);

		Task<bool> BlankPass(int UserId);

		Task<bool> UpdateDate(int MatchId, string Date);

		Task<ImmutableArray<EnvironmentVariables>> GetEnvironmentVariables();

		Task<bool> AddEnvironmentVariable(string key, string value);

		Task<bool> DeleteEnvironmentVariable(string key);
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

		public async Task<ImmutableArray<Groups>> GetGroups()
		{
			using (var db = _dbService.SimpleDbConnection())
			{
				var v = await db.QueryAsync<Groups>(@"
select * from GameGroups");
				return v.ToImmutableArray(); 
			}
		}

		public async Task<ImmutableArray<UserInfo>> GetUsers(int GroupId)
		{
			using (var db = _dbService.SimpleDbConnection())
			{
				return (await db.QueryAsync<UserInfo>(@"
Select u.Id, u.TeamName, u.Mail, u.HasPaid
from UserGroups ug left join Users u on ug.UserId = u.Id
Where ug.GroupId = @GroupId", new
				{
					GroupId
				})).ToImmutableArray();
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

		public async Task<bool> UpdateTeams(MatchResult match)
		{
			using (var db = _dbService.SimpleDbConnection())
			{
				var v = await db.ExecuteAsync(@"
Update Matches
Set Team1 = @team1,
Team2 = @team2
where id = @matchid", new
				{
					team1 = match.Team1,
					team2 = match.Team2,
					matchid = match.MatchId
				});
				return v > 0;
			}
		}

		public async Task<bool> UpdateDate(int MatchId, string Date)
		{
			Date = Date.Replace("T", " ");
			Date = Date.Replace(":00:00", ":00");
			using (var db = _dbService.SimpleDbConnection())
			{
				var v = await db.ExecuteAsync(@"
Update Matches
Set Date = @Date
where id = @MatchId", new
				{
					Date,
					MatchId
				});
				return v > 0;
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

		public async Task<bool> CreateGroup(string GroupName)
		{
			using (var db = _dbService.SimpleDbConnection())
			{
				return (await db.ExecuteAsync(@"
Insert into GameGroups (GameGroup)
Values (@GroupName)", new
				{
					GroupName
				}) > 0);
			}
		}

		public async Task<bool> ChangePaid(int UserId)
		{
			using (var db = _dbService.SimpleDbConnection())
			{
				return (await db.ExecuteAsync(@"
Update Users
Set HasPaid = abs(HasPaid - 1)
Where ID = @UserId", new
				{
					UserId
				}) > 0);
			}
		}

		public async Task<bool> DeleteUserFromGroup(int UserId, int GroupId)
		{
			using (var db = _dbService.SimpleDbConnection())
			{
				return (await db.ExecuteAsync(@"
Delete From UserGroups
Where UserId = @UserId and GroupId = @GroupId", new
				{
					UserId,
					GroupId
				}) > 0);
			}
		}

		public async Task<bool> BlankPass(int UserId)
		{
			string newpass = EncryptPassword("123456");
			using (var db = _dbService.SimpleDbConnection())
			{
				return (await db.ExecuteAsync(@"
Update Users
Set Password = @pass
Where ID = @UserId", new
				{
					UserId, 
					pass = newpass
				}) > 0);
			}
		}

		public async Task<ImmutableArray<EnvironmentVariables>> GetEnvironmentVariables()
		{
			using (var db = _dbService.SimpleDbConnection())
			{
				var v = await db.QueryAsync<EnvironmentVariables>(@"
select * from EnvironmentVariables");
				return v.ToImmutableArray();
			}
		}

		public async Task<bool> AddEnvironmentVariable(string key, string value)
		{
			using (var db = _dbService.SimpleDbConnection())
			{
				return (await db.ExecuteAsync(@"
Insert into EnvironmentVariables (key,value)
Values (@key,@value)", new
				{
					key,
					value
				}) > 0);
			}
		}

		public async Task<bool> DeleteEnvironmentVariable(string key)
		{
			using (var db = _dbService.SimpleDbConnection())
			{
				return (await db.ExecuteAsync(@"
Delete From EnvironmentVariables
Where key = @key", new
				{
					key
				}) > 0);
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
