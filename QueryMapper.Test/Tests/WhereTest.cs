using Xunit;

namespace Kusachius.QueryMapper.Test
{
	public class WhereTest : TestBase
	{
		public WhereTest(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public void WhereRaw_CorrectSql()
		{
			var sql = context.From<TestEntity1>()
				.InsertInto<TestEntity1a>()
				.WhereRaw("a.\"date\" = null")
				.ToSql();

			Assert.Equal("INSERT INTO \"Test1a\" (\"Id\", \"date\", \"Name\", \"Option\")\nSELECT a.\"Id\", a.\"date\", a.\"Name\", a.\"Option\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL)\n" +
				"\tAND (a.\"date\" = null);\n", sql);
		}

		[Fact]
		public void WhereBetween_CorrectSql()
		{
			var sql = context.From<TestEntity1>()
				.InsertInto<TestEntity1a>()
				.Where(a => a.Date.Between(new DateOnly(2022, 1, 1), new DateOnly(2022, 12, 31)))
				.ToSql();

			Assert.Equal("INSERT INTO \"Test1a\" (\"Id\", \"date\", \"Name\", \"Option\")\nSELECT a.\"Id\", a.\"date\", a.\"Name\", a.\"Option\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL)\n" +
				"\tAND (a.\"date\" BETWEEN '2022-01-01' AND '2022-12-31');\n", sql);
		}

		[Fact]
		public void WhereIsNull_CorrectSql()
		{
			var sql = context.From<TestEntity1>()
				.InsertInto<TestEntity1a>()
				.Where(a => a.Name == null)
				.ToSql();

			Assert.Equal("INSERT INTO \"Test1a\" (\"Id\", \"date\", \"Name\", \"Option\")\nSELECT a.\"Id\", a.\"date\", a.\"Name\", a.\"Option\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL)\n" +
				"\tAND (a.\"Name\" IS NULL);\n", sql);
		}

		[Fact]
		public void WhereIsNotNull_CorrectSql()
		{
			var sql = context.From<TestEntity1>()
				.InsertInto<TestEntity1a>()
				.Where(a => a.Name != null)
				.ToSql();

			Assert.Equal("INSERT INTO \"Test1a\" (\"Id\", \"date\", \"Name\", \"Option\")\nSELECT a.\"Id\", a.\"date\", a.\"Name\", a.\"Option\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL)\n" +
				"\tAND (a.\"Name\" IS NOT NULL);\n", sql);
		}

		[Fact]
		public void WhereWithInterface_CorrectSql()
		{
			var sql = context.From<TestEntity3>()
				.Select(a => new { a.Name, a.Date, Descr = a.Option, Id = 1 })
				.Where(a => a.Name == "123")
				.InsertInto<TestEntity2>().ToSql(out var sqlParam);

			Assert.Equal(@"INSERT INTO ""Test2"" (""Name"", ""date"", ""Descr"", ""Id"")
SELECT a.""Name"", a.""date"", a.""Option"" AS ""Descr"", 1 AS ""Id""
FROM ""Test3"" a
WHERE (a.""DeletedOn"" IS NULL)
	AND (a.""Name"" = {0});
".Replace("\r\n", "\n"), sql);
		}

		[Fact]
		public void WhereOr_CorrectSql()
		{
			var sql = context.From<TestEntity3>()
				.Select(a => new { a.Name, a.Date, Descr = a.Option, Id = 1 })
				.Where(a => a.Name == "123" || a.Name == "234")
				.InsertInto<TestEntity2>().ToSql(out var sqlParam);

			Assert.Equal(@"INSERT INTO ""Test2"" (""Name"", ""date"", ""Descr"", ""Id"")
SELECT a.""Name"", a.""date"", a.""Option"" AS ""Descr"", 1 AS ""Id""
FROM ""Test3"" a
WHERE (a.""DeletedOn"" IS NULL)
	AND ((a.""Name"" = {0} OR a.""Name"" = {1}));
".Replace("\r\n", "\n"), sql);

			Assert.Equal(2, sqlParam.Count());
			Assert.Equal("123", sqlParam.First().ToString());
			Assert.Equal("234", sqlParam.Last().ToString());
		}

		[Fact]
		public void WhereOrParenthesis_CorrectSql()
		{
			var sql = context.From<TestEntity3>()
				.Select(a => new { a.Name, a.Date, Descr = a.Option, Id = 1 })
				.Where(a => a.Id > 1 && (a.Name == "123" || a.Name == "234"))
				.InsertInto<TestEntity2>().ToSql(out var sqlParam);

			Assert.Equal(@"INSERT INTO ""Test2"" (""Name"", ""date"", ""Descr"", ""Id"")
SELECT a.""Name"", a.""date"", a.""Option"" AS ""Descr"", 1 AS ""Id""
FROM ""Test3"" a
WHERE (a.""DeletedOn"" IS NULL)
	AND (a.""Id"" > 1
	AND (a.""Name"" = {0} OR a.""Name"" = {1}));
".Replace("\r\n", "\n"), sql);

			Assert.Equal(2, sqlParam.Count());
			Assert.Equal("123", sqlParam.First().ToString());
			Assert.Equal("234", sqlParam.Last().ToString());
		}
	}
}
