using Xunit;
using static Kusachius.QueryMapper.QueryMapper<object>;

namespace Kusachius.QueryMapper.Test
{
    public class GroupByTest : TestBase
    {
        public GroupByTest(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void SimpleGroupBy_CorrectSql()
        {
            var sql = context.From<TestEntity1>()
                .Select(a => new { a.Id })
                .GroupBy(a => new { a.Id })
                .InsertInto<TestEntity1a>()
                .ToSql();

            Assert.Equal(@"INSERT INTO ""Test1a"" (""Id"")
SELECT a.""Id""
FROM ""Test1"" a
WHERE (a.""DeletedOn"" IS NULL)
GROUP BY a.""Id"";
", sql);
        }

        [Fact]
        public void GroupByWithAggregationFunctions_CorrectSql()
        {
            var sql = context.From<TestEntity1>()
                .Select(a => new { a.Date, Id = Sum(a.Id), Name = Min(a.Name), Option = Max(a.Option) })
                .GroupBy(a => new { a.Date })
                .InsertInto<TestEntity1a>()
                .ToSql();

            Assert.Equal(@"INSERT INTO ""Test1a"" (""date"", ""Id"", ""Name"", ""Option"")
SELECT a.""date"", SUM(a.""Id"") AS ""Id"", MIN(a.""Name"") AS ""Name"", MAX(a.""Option"") AS ""Option""
FROM ""Test1"" a
WHERE (a.""DeletedOn"" IS NULL)
GROUP BY a.""date"";
", sql);
        }
    }
}
