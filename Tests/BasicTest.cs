using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kusachius.QueryMapper.Test
{
	public class BasicTest : TestBase
	{
		public BasicTest(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public void SimpleInsertAndSelect_CorrectSql()
		{
			var sql = context.From<TestEntity1>().InsertInto<TestEntity1a>().ToSql();

			Assert.Equal("INSERT INTO \"Test1a\" (\"Id\", \"date\", \"Name\", \"Option\")\nSELECT a.\"Id\", a.\"date\", a.\"Name\", a.\"Option\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
		}

		[Fact]
		public void SimpleInsertAndSelectNoDeletedOn_CorrectSql()
		{
			var sql = context.From<TestEntity1a>().InsertInto<TestEntity1>().ToSql();

			Assert.Equal("INSERT INTO \"Test1\" (\"Id\", \"date\", \"Name\", \"Option\")\nSELECT a.\"Id\", a.\"date\", a.\"Name\", a.\"Option\"\nFROM \"Test1a\" a;\n", sql);
		}
	}
}
