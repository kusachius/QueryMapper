using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kusachius.QueryMapper
{
	public partial class QueryMapper<TSource> where TSource : new()
	{
		internal QueryMapper<TSource> AnyJoin<TJoin1>(string joinType, Expression<Func<TSource, TJoin1, bool>> joinExpr, bool includeDeleted = false)
		{
			var joinAlias = joinExpr.Parameters[1].Name;
			var	joinTable = GetTableName(typeof(TJoin1));

			var deletedExpr = !includeDeleted && GetTableInfo(typeof(TJoin1)).HasDeletedOnField() ? $"{joinAlias}.\"DeletedOn\" IS NULL AND " : string.Empty;


			_join.Add($"\n{joinType}JOIN {joinTable} {joinAlias} ON {deletedExpr}{LambdaToSql(joinExpr.Body, out var _)}");

			return this;
		}

		internal QueryMapper<TSource> AnyJoin<TJoin1, TJoin2>(string joinType, Expression<Func<TSource, TJoin1, TJoin2, bool>> joinExpr)
		{
			var joinAlias = joinExpr.Parameters[2].Name;
			var joinTable = GetTableName(typeof(TJoin2));

			var deletedExpr = GetTableInfo(typeof(TJoin2)).HasDeletedOnField() ? $"{joinAlias}.\"DeletedOn\" IS NULL AND " : string.Empty;

			_join.Add($"\n{joinType}JOIN {joinTable} {joinAlias} ON {deletedExpr}{LambdaToSql(joinExpr.Body, out var _)}");

			return this;
		}

		public QueryMapper<TSource> Join<TJoin1>(Expression<Func<TSource, TJoin1, bool>> predicate) => AnyJoin<TJoin1>(String.Empty, predicate);
        public QueryMapper<TSource> JoinIncludingDeleted<TJoin1>(Expression<Func<TSource, TJoin1, bool>> predicate) => AnyJoin<TJoin1>(String.Empty, predicate, includeDeleted: true);
        public QueryMapper<TSource> LeftJoin<TJoin1>(Expression<Func<TSource, TJoin1, bool>> predicate) => AnyJoin<TJoin1>("LEFT ", predicate);
		public QueryMapper<TSource> Join<TJoin1, TJoin2>(Expression<Func<TSource, TJoin1, TJoin2, bool>> predicate) => AnyJoin<TJoin1, TJoin2>(String.Empty, predicate);
		public QueryMapper<TSource> LeftJoin<TJoin1, TJoin2>(Expression<Func<TSource, TJoin1, TJoin2, bool>> predicate) => AnyJoin<TJoin1, TJoin2>("LEFT ", predicate);
	}
}
