using Microsoft.Extensions.Configuration;
//using System.Data.SQLite;
using Microsoft.Data.Sqlite;
//using StackExchange.Profiling.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Prode.API.Services
{
    public interface IDbService
    {
        SqliteConnection SimpleDbConnection();
    }

    public class DbService: IDbService
    {

        public static string DbFile
        {
            get { return Environment.CurrentDirectory + "\\prode.sqlite"; }
        }

        public SqliteConnection SimpleDbConnection()
        {
            //var v = new SQLiteConnection();
            return new SqliteConnection("Data Source=" + DbFile);
        }

        public DbService(IConfiguration configuration)
        {
            if (!File.Exists(DbFile))
            {
                using (var cnn = SimpleDbConnection())
                {
                    cnn.Open();
                }
            }
        }

    }
}
