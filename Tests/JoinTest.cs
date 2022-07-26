using Xunit;

namespace Kusachius.QueryMapper.Test
{
	public class JoinTest : TestBase
	{
		public JoinTest(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public void SimpleJoin_CorrectSql()
		{
			var sql = context.From<TestEntity1>()
				.Join<TestEntity2a>((a, b) => a.Name == b.Name)
				.Select<TestEntity2a>((a, b) => new { a.Name, a.Date, b.Descr, Id = 1 })
				.InsertInto<TestEntity2>().ToSql();

			Assert.Equal(@"INSERT INTO ""Test2"" (""Name"", ""date"", ""Descr"", ""Id"")
SELECT a.""Name"", a.""date"", b.""Descr"", 1 AS ""Id""
FROM ""Test1"" a
JOIN ""Test2a"" b ON a.""Name"" = b.""Name""
WHERE (a.""DeletedOn"" IS NULL);
".Replace("\r\n", "\n"), sql);
		}

		[Fact]
		public void LefteJoinAnd_CorrectSql()
		{
			var sql = context.From<TestEntity1>()
				.LeftJoin<TestEntity2a>((a, b) => a.Id == b.Id && a.Name == "2")
				.Select<TestEntity2a>((a, b) => new { a.Name, a.Date, b.Descr, Id = 1 })
				.InsertInto<TestEntity2>().ToSql(out var sqlParams);

			Assert.Equal(@"INSERT INTO ""Test2"" (""Name"", ""date"", ""Descr"", ""Id"")
SELECT a.""Name"", a.""date"", b.""Descr"", 1 AS ""Id""
FROM ""Test1"" a
LEFT JOIN ""Test2a"" b ON a.""Id"" = b.""Id""
	AND a.""Name"" = {0}
WHERE (a.""DeletedOn"" IS NULL);
".Replace("\r\n", "\n"), sql);
			Assert.Single(sqlParams);
			Assert.Equal("2", sqlParams.First());
		}

		[Fact]
		public void SimpleJoinInterface_CorrectSql()
		{
			var sql = context.From<TestEntity3>()
				.Select<TestEntity1>((a, b) => new { a.Name, a.Date, Descr = b.Option, Id = 1 })
				.AddFilter()
				.InsertInto<TestEntity2>().ToSql(out _);

			Assert.Equal(@"INSERT INTO ""Test2"" (""Name"", ""date"", ""Descr"", ""Id"")
SELECT a.""Name"", a.""date"", b.""Option"" AS ""Descr"", 1 AS ""Id""
FROM ""Test3"" a
JOIN ""Test1"" b ON b.""DeletedOn"" IS NULL AND b.""Id"" = a.""Id""
	AND b.""Name"" = {0}
WHERE (a.""DeletedOn"" IS NULL);
".Replace("\r\n", "\n"), sql);
		}


	}

	public static class JoinTestExtensions
	{
		public static QueryMapper<T> AddFilter<T>(this QueryMapper<T> q) where T : new()
		{
			q.Join<TestEntity1>((a, b) => b.Id == (a as ITestId).Id && b.Name == "123");

			return q;
		}
	}
}
