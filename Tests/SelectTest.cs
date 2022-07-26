using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kusachius.QueryMapper.Test
{
    public class SelectTest : TestBase
    {
        private string testval = "test prop";

        public SelectTest(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void SimpleInsertAndSelect_CorrectSql()
        {
            var sql = context.From<TestEntity1>()
                .Select(a => new { a.Name, a.Date, Descr = a.Option })
                .InsertInto<TestEntity2>().ToSql();

            Assert.Equal("INSERT INTO \"Test2\" (\"Name\", \"date\", \"Descr\")\nSELECT a.\"Name\", a.\"date\", a.\"Option\" AS \"Descr\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
        }

        [Fact]
        public void SelectIntConstant_CorrectSql()
        {
            IEnumerable<object> paramValues;
            var sql = context.From<TestEntity1>()
                .Select(a => new { Name = "testname", a.Date, Descr = a.Option, Id = 10 })
                .InsertInto<TestEntity2>().ToSql(out paramValues);

            Assert.Equal("INSERT INTO \"Test2\" (\"Name\", \"date\", \"Descr\", \"Id\")\nSELECT {0} AS \"Name\", a.\"date\", a.\"Option\" AS \"Descr\", 10 AS \"Id\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
            Assert.Single(paramValues);
            Assert.Equal("testname", paramValues.First() as string);
        }

        [Fact]
        public void SelectEnumConstant_CorrectSql()
        {
            var sql = context.From<TestEntity1>()
                .Select(a => new { a.Date, Descr = a.Option, Id = TestEnum.Tenth })
                .InsertInto<TestEntity2>().ToSql();

            Assert.Equal("INSERT INTO \"Test2\" (\"date\", \"Descr\", \"Id\")\nSELECT a.\"date\", a.\"Option\" AS \"Descr\", 10 AS \"Id\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
        }

        [Fact]
        public void SelectCorrectOrder_CorrectSql()
        {
            var sql = context.From<TestEntity1>()
                .Select(a => new { a.Date, Descr = a.Option, Id = TestEnum.Tenth })
                .InsertInto<TestEntity2>().ToSql();

            Assert.Equal("INSERT INTO \"Test2\" (\"date\", \"Descr\", \"Id\")\nSELECT a.\"date\", a.\"Option\" AS \"Descr\", 10 AS \"Id\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
        }

        [Fact]
        public void SelectIntvariable_CorrectSql()
        {
            IEnumerable<object> paramValues;
            var intvar = 10;
            var sql = context.From<TestEntity1>()
                .Select(a => new { Name = "testname", a.Date, Descr = a.Option, Id = intvar })
                .InsertInto<TestEntity2>().ToSql(out paramValues);

            Assert.Equal("INSERT INTO \"Test2\" (\"Name\", \"date\", \"Descr\", \"Id\")\nSELECT {0} AS \"Name\", a.\"date\", a.\"Option\" AS \"Descr\", 10 AS \"Id\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
            Assert.Single(paramValues);
            Assert.Equal("testname", paramValues.First() as string);
        }

        [Fact]
        public void SelectStringvariable_CorrectSql()
        {
            IEnumerable<object> paramValues;
            var strvar = "test";
            var sql = context.From<TestEntity1>()
                .Select(a => new { Name = strvar, a.Date })
                .InsertInto<TestEntity2>().ToSql(out paramValues);

            Assert.Equal("INSERT INTO \"Test2\" (\"Name\", \"date\")\nSELECT {0} AS \"Name\", a.\"date\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
            Assert.Single(paramValues);
            Assert.Equal("test", paramValues.First() as string);
        }

        [Fact]
        public void SelectDateVariable_CorrectSql()
        {
            IEnumerable<object> paramValues;
            var datevar = new DateOnly(2022, 05, 06);
            var sql = context.From<TestEntity1>()
                .Select(a => new { a.Name, Date = datevar })
                .InsertInto<TestEntity2>().ToSql(out paramValues);

            Assert.Equal("INSERT INTO \"Test2\" (\"Name\", \"date\")\nSELECT a.\"Name\", {0} AS \"date\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
            Assert.Single(paramValues);
            Assert.Equal(datevar, paramValues.First() as DateOnly?);
        }

        [Fact]
        public void SelectNegate_CorrectSql()
        {
            var sql = context.From<TestEntity1>()
                .Select(a => new { a.Name, Id = -a.Id })
                .InsertInto<TestEntity2>().ToSql();

            Assert.Equal("INSERT INTO \"Test2\" (\"Name\", \"Id\")\nSELECT a.\"Name\", -(a.\"Id\") AS \"Id\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
        }

        [Fact]
        public void SelectAddStrings_CorrectSql()
        {
            IEnumerable<object> paramValues;
            var sql = context.From<TestEntity1>()
                .Select(a => new { Name = a.Name + "a" })
                .InsertInto<TestEntity2>().ToSql(out paramValues);

            Assert.Equal("INSERT INTO \"Test2\" (\"Name\")\nSELECT CONCAT(a.\"Name\", {0}) AS \"Name\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
            Assert.Equal("a", paramValues.First() as string);
        }

        [Fact]
        public void SelectAddInt_CorrectSql()
        {
            var sql = context.From<TestEntity1>()
                .Select(a => new { Id = a.Id + 5 })
                .InsertInto<TestEntity2>().ToSql();

            Assert.Equal("INSERT INTO \"Test2\" (\"Id\")\nSELECT (a.\"Id\" + 5) AS \"Id\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
        }

        [Fact]
        public void SelectGetValueOrDefaultInt_CorrectSql()
        {
            var sql = context.From<TestEntity4>()
                .Select(a => new { a.Id, Id2 = a.Id2.GetValueOrDefault() })
                .InsertInto<TestEntity4>().ToSql();

            Assert.Equal("INSERT INTO \"Test4\" (\"Id\", \"Id2\")\nSELECT a.\"Id\", COALESCE(a.\"Id2\", 0) AS \"Id2\"\nFROM \"Test4\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
        }

        [Fact]
        public void SelectCase_CorrectSql()
        {
            var sql = context.From<TestEntity4>()
                .Select(a => new { a.Id, Id2 = a.Id2 == 1 ? 10 : (int?)null })
                .InsertInto<TestEntity4>().ToSql();

            Assert.Equal("INSERT INTO \"Test4\" (\"Id\", \"Id2\")\nSELECT a.\"Id\", CASE WHEN a.\"Id2\" = 1 THEN 10 ELSE NULL END AS \"Id2\"\nFROM \"Test4\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
        }

        [Fact]
        public void SelectNot_CorrectSql()
        {
            var sql = context.From<TestEntity4>()
                .Select(a => new { a.Id, Id2 = !(a.Id2 == 1) ? 10 : (int?)null })
                .InsertInto<TestEntity4>().ToSql();

            Assert.Equal("INSERT INTO \"Test4\" (\"Id\", \"Id2\")\nSELECT a.\"Id\", CASE WHEN NOT (a.\"Id2\" = 1) THEN 10 ELSE NULL END AS \"Id2\"\nFROM \"Test4\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
        }

        [Fact]
        public void SelectToString_CorrectSql()
        {
            var sql = context.From<TestEntity4>()
                .Select(a => new { a.Id, Name = a.Id2.ToString() })
                .InsertInto<TestEntity4>().ToSql();

            Assert.Equal("INSERT INTO \"Test4\" (\"Id\", \"Name\")\nSELECT a.\"Id\", (a.\"Id2\")::text AS \"Name\"\nFROM \"Test4\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
        }

        [Fact]
        public void SelectToStringWithFormat_CorrectSql()
        {
            var sql = context.From<TestEntity4>()
                .Select(a => new { a.Id2, Name = a.Id.ToString("000000") })
                .InsertInto<TestEntity4>().ToSql();

            Assert.Equal("INSERT INTO \"Test4\" (\"Id2\", \"Name\")\nSELECT a.\"Id2\", to_char(a.\"Id\", 'fm000000') AS \"Name\"\nFROM \"Test4\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
        }

        [Fact]
        public void SelectRound_CorrectSql()
        {
            var sql = context.From<TestEntity4>()
                .Select(a => new { Id = Math.Round((decimal)a.Id / 2, 2) })
                .InsertInto<TestEntity4>().ToSql(out var sqlParams);

            Assert.Equal("INSERT INTO \"Test4\" (\"Id\")\nSELECT ROUND((a.\"Id\" / 2), 2) AS \"Id\"\nFROM \"Test4\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
        }

        [Fact]
        public void SelectIfNotNull_CorrectSql()
        {
            var sql = context.From<TestEntity4>()
                .Select(a => new { a.Id, Id2 = a.Id2 ?? 10 })
                .InsertInto<TestEntity4>().ToSql();

            Assert.Equal("INSERT INTO \"Test4\" (\"Id\", \"Id2\")\nSELECT a.\"Id\", COALESCE(a.\"Id2\", 10) AS \"Id2\"\nFROM \"Test4\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
        }

        [Fact]
        public void SelectReplace_CorrectSql()
        {
            var sql = context.From<TestEntity4>()
                .Select(a => new { Name = a.Name.Replace("aa", "bb") })
                .InsertInto<TestEntity4>().ToSql(out var sp);

            Assert.Equal("INSERT INTO \"Test4\" (\"Name\")\nSELECT REPLACE(a.\"Name\", {0}, {1}) AS \"Name\"\nFROM \"Test4\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
            Assert.Equal("aa", sp.First().ToString());
            Assert.Equal("bb", sp.Last().ToString());
        }

        [Fact]
        public void SelectAddDays_CorrectSql()
        {
            var sql = context.From<TestEntity1>()
                .Select(a => new { Date = a.Date.AddDays(2) })
                .InsertInto<TestEntity1>().ToSql();

            Assert.Equal("INSERT INTO \"Test1\" (\"date\")\nSELECT (a.\"date\" + INTERVAL '2 DAY') AS \"date\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL);\n", sql);
        }

        //[Fact]
        //public void OrderBy_CorrectSql()
        //{
        //	var sql = context.From<TestEntity1>()
        //		.Select(a => new { a.Name, a.Date, Descr = a.Option })
        //		.InsertInto<TestEntity2>()
        //		.OrderBy(a => a.Date)
        //		.ToSql();


        //	Assert.Equal("INSERT INTO \"Test2\" (\"Name\", \"date\", \"Descr\")\nSELECT a.\"Name\", a.\"date\", a.\"Option\" AS \"Descr\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL)\nORDER BY a.\"date\";\n", sql);
        //}

        [Fact]
        public void OrderByMany_CorrectSql()
        {
            var sql = context.From<TestEntity1>()
                .Select(a => new { a.Name, a.Date, Descr = a.Option })
                .InsertInto<TestEntity2>()
                .OrderBy(a => new { a.Date, a.Name })
                .ToSql();


            Assert.Equal("INSERT INTO \"Test2\" (\"Name\", \"date\", \"Descr\")\nSELECT a.\"Name\", a.\"date\", a.\"Option\" AS \"Descr\"\nFROM \"Test1\" a\nWHERE (a.\"DeletedOn\" IS NULL)\nORDER BY a.\"date\", a.\"Name\";\n", sql);
        }

        enum TestEnum
        {
            First = 1,
            Tenth = 10,
        }
    }
}
