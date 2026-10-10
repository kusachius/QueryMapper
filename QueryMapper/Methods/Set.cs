using System.Linq.Expressions;

namespace Kusachius.QueryMapper
{
    public partial class QueryMapper<TSource> where TSource : new()
	{
		public QueryMapper<TSource> Set(Expression<Func<TSource, object>> predicate)
		{

			if (predicate.Body is NewExpression expr)
			{
				for (var i = 0; i < expr.Arguments.Count; i++)
				{
					var arg = expr.Arguments[i];
					var member = expr.Members[i];

					var col = new SelectedFieldInfo
					{
						TargetField = member.Name
					};

					col.SqlRaw = LambdaToSql(arg, out var srcCol);
					col.SrcCol = srcCol;

					setFieldInfoList.Add(col);
				}
			}
			else
			{
				throw new ArgumentException("Invalid lambda expression type in set");
			}

			return this;
		}

		public QueryMapper<TSource> Set<TJoin1>(Expression<Func<TSource, TJoin1, object>> predicate)
		{

			var fromAlias = predicate.Parameters[1].Name;
			var fromTable = GetTableName(typeof(TJoin1));
			_updateFrom = $"\nFROM {fromTable} {fromAlias}";


			if (predicate.Body is NewExpression expr)
			{
				for (var i = 0; i < expr.Arguments.Count; i++)
				{
					var arg = expr.Arguments[i];
					var member = expr.Members[i];

					var col = new SelectedFieldInfo
					{
						TargetField = member.Name
					};

					col.SqlRaw = LambdaToSql(arg, out var srcCol);
					col.SrcCol = srcCol;

					setFieldInfoList.Add(col);
				}
			}
			else
			{
				throw new ArgumentException("Invalid lambda expression type in set");
			}

			return this;
		}
	}
}
