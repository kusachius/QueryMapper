using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kusachius.QueryMapper.Test
{
	public class UpdateTest : TestBase
	{
		public UpdateTest(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public void SimpleSet_CorrectSql()
		{
			var date = new DateOnly(2022, 1, 5);
			var sql = context.Update<TestEntity1>()
				.Set(u => new { Name = "xxx", Id = 2 })
				.Where(u => u.Date == date)
				.ToSql(out var sqlParams);

			Assert.Equal(@"UPDATE ""Test1"" u
SET ""Name"" = {0},
	""Id"" = 2
WHERE (u.""DeletedOn"" IS NULL)
	AND (u.""date"" = {1});
".Replace("\r\n", "\n"), sql);
		}

		[Fact]
		public void UpdateWithJoin_CorrectSql()
		{
			var date = new DateOnly(2022, 1, 5);
			var sql = context.Update<TestEntity1>()
				.Set<TestEntity2>((u, x) => new { Name = x.Name, Id = 2 })
				.Where<TestEntity2>((u, x) => u.Date == date && u.Id == x.Id)
				.ToSql(out var sqlParams);

			Assert.Equal(@"UPDATE ""Test1"" u
SET ""Name"" = x.""Name"",
	""Id"" = 2
FROM ""Test2"" x
WHERE (u.""DeletedOn"" IS NULL)
	AND (u.""date"" = {0}
	AND u.""Id"" = x.""Id"");
".Replace("\r\n", "\n"), sql);
		}
	}
}
