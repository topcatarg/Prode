using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Prode.API.Services
{
	public interface IMigrateService
	{
		Task<bool> Migrate1Async();
		Task<bool> Migrate2Async();

	}

	public class MigrateService: IMigrateService
	{
		private readonly IDbService _dbService;

		public MigrateService(IDbService dbService)
		{
			_dbService = dbService;
		}

		public async Task<bool> Migrate1Async()
		{
			using (var db = _dbService.SimpleDbConnection())
			{
				return (await db.ExecuteAsync(@"
CREATE TABLE ""UserGroups"" 
( `Id` INTEGER NOT NULL,
`UserId` INTEGER NOT NULL,
`GroupId` INTEGER NOT NULL,
CONSTRAINT `Unicos` UNIQUE(`UserId`,`GroupId`),
PRIMARY KEY(`Id`) 
);

Insert into UserGroups (UserId, GroupID)
Select ID, GameGroupId
From Users;") > 0);
			}
		}

		public async Task<bool> Migrate2Async()
		{
			using (var db = _dbService.SimpleDbConnection())
			{
				return (await db.ExecuteAsync(@"
CREATE TABLE `EnvironmentVariables` (
	`Key`	TEXT NOT NULL UNIQUE,
	`Value`	TEXT NOT NULL,
	PRIMARY KEY(`Key`)
);") > 0);
			}
		}

		

	}
}
