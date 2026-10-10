using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kusachius.QueryMapper.Test
{
	public class RawSqlTest : TestBase
	{
		public RawSqlTest(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public void SelectRawSql_CorrectSql()
		{
			var sql = context.From<TestEntity1>()
				.Select(a => new { Name = (RawSql)"(select 'xxx')", a.Date, Descr = a.Option, Id = 10 })
				.InsertInto<TestEntity2>().ToSql();

			Assert.Equal("INSERT INTO \"Test2\" (\"Name\", \"date\", \"Descr\", \"Id\")\nSELECT (select 'xxx') AS \"Name\", a.\"date\", a.\"Option\" AS \"Descr\", 10 AS \"Id\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
		}

		[Fact]
		public void SelectRawSqlAsVariable_CorrectSql()
		{
			var rawSql = "(select 'xxx')";
			var sql = context.From<TestEntity1>()
				.Select(a => new { Name = (RawSql)rawSql, a.Date, Descr = a.Option, Id = 10 })
				.InsertInto<TestEntity2>().ToSql();

			Assert.Equal("INSERT INTO \"Test2\" (\"Name\", \"date\", \"Descr\", \"Id\")\nSELECT (select 'xxx') AS \"Name\", a.\"date\", a.\"Option\" AS \"Descr\", 10 AS \"Id\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
		}

	}
}
