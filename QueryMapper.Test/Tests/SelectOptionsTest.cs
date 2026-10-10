using Xunit;

namespace Kusachius.QueryMapper.Test
{
	public class SelectOptionsTest : TestBase
	{
		public SelectOptionsTest(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public void InsertAndSelectSelectRemaining_CorrectSql()
		{
			var sql = context.From<TestEntity1>()
				.Select(x => new { Id = 1 }, opt => opt.SelectRemainingFields())
				.InsertInto<TestEntity1a>()
				.ToSql();

			Assert.Equal(@"INSERT INTO ""Test1a"" (""Id"", ""date"", ""Name"", ""Option"")
SELECT 1 AS ""Id"", a.""date"", a.""Name"", a.""Option""
FROM ""Test1"" a
WHERE (a.""DeletedOn"" IS NULL);
".Replace("\r\n", "\n"), sql);
		}

		[Fact]
		public void SimpleInsertAndSelectRemainingTRhrowsIfNotExtsis_CorrectSql()
		{
			var query = context.From<TestEntity1>()
				.Select(x => new { Id = 1 }, opt => opt.SelectRemainingFields())
				.InsertInto<TestEntity2>();

			Assert.Throws<ArgumentException>(() => query.ToSql());
		}

		[Fact]
		public void SimpleInsertAndSelectRemainingAndSkipNonExinsting_CorrectSql()
		{
			var sql = context.From<TestEntity1>()
				.Select(x => new { Id = 1 }, opt => opt.SelectRemainingExistingInDestFields())
				.InsertInto<TestEntity2>()
				.ToSql();

			Assert.Equal(@"INSERT INTO ""Test2"" (""Id"", ""date"", ""Name"")
SELECT 1 AS ""Id"", a.""date"", a.""Name""
FROM ""Test1"" a
WHERE (a.""DeletedOn"" IS NULL);
".Replace("\r\n", "\n"), sql);
		}

		[Fact]
		public void SimpleInsertAndSelectSkipNonExisting_CorrectSql()
		{
			var sql = context.From<TestEntity1>()
				.Select(x => new { Id = 1 }, opt => opt.SelectRemainingFields().SkipNonExistingInDestinationFields())
				.InsertInto<TestEntity2>()
				.ToSql();

			Assert.Equal(@"INSERT INTO ""Test2"" (""Id"", ""date"", ""Name"")
SELECT 1 AS ""Id"", a.""date"", a.""Name""
FROM ""Test1"" a
WHERE (a.""DeletedOn"" IS NULL);
".Replace("\r\n", "\n"), sql);
		}
	}
}
