using System;
using Microsoft.EntityFrameworkCore;

namespace Kusachius.QueryMapper.Test
{
	public class DbConfig
	{
		static string _connectString;

		public static bool IsInitialized()
		{
			return _connectString != null;
		}

		public static string ConnectString
		{
			get
			{
				if (_connectString == null)
				{
					throw new Exception("Connection string not configured. Call DbConfig.setConnectString(string) before using");
				}
				return _connectString;
			}
		}

		public static void SetDatabaseUrl(string url)
		{
			var uri = new Uri(url);
			string host = uri.GetComponents(UriComponents.Host, UriFormat.Unescaped);
			string port = uri.GetComponents(UriComponents.Port, UriFormat.Unescaped);
			string[] userInfo = uri.GetComponents(UriComponents.UserInfo, UriFormat.Unescaped).Split(':');
			string database = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);

			string connString = string.Format("Host={0};Port={4};Database={1};Username={2};Password={3}", host, database, userInfo[0], userInfo[1], port);
			SetConnectString(connString);
		}

		public static void SetConnectString(string connString, bool useSqlLite = false)
		{
			_connectString = connString;
		}

		public static void InitializeDb(DbContextOptionsBuilder optionsBuilder)
		{
			if (string.IsNullOrEmpty(_connectString))
			{
				throw new Exception("Connectionstring not provided");
			}

			optionsBuilder.UseNpgsql(_connectString,
				o => o.SetPostgresVersion(9, 6)
					.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
		}
	}
}
