using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kusachius.QueryMapper
{
	public partial class QueryMapper<TSource> where TSource : new()
	{
		public QueryMapper<TSource> Select(Expression<Func<TSource, object>> predicate, Func<SelectOptions, SelectOptions> options = null)
		{
			if (options != null)
			{
				options(_selectOptions);
			}

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

					selectedFieldInfoList.Add(col);
				}
			}
			else
			{
				throw new ArgumentException("Invalid lambda expression type in select");
			}

			return this;
		}

		public QueryMapper<TSource> Select<TJoin1>(Expression<Func<TSource, TJoin1, object>> predicate, Func<SelectOptions, SelectOptions> options = null)
		{
			if (options != null)
			{
				options(_selectOptions);
			}

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

					selectedFieldInfoList.Add(col);
				}
			}
			else
			{
				throw new ArgumentException("Invalid lambda expression type in select");
			}

			return this;
		}

		public QueryMapper<TSource> Select<TJoin1, TJoin2>(Expression<Func<TSource, TJoin1, TJoin2, object>> predicate, Func<SelectOptions, SelectOptions> options = null)
		{
			if (options != null)
			{
				options(_selectOptions);
			}

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

					selectedFieldInfoList.Add(col);
				}
			}
			else
			{
				throw new ArgumentException("Invalid lambda expression type in select");
			}

			return this;
		}

		public QueryMapper<TSource> Select<TJoin1, TJoin2, TJoin3>(Expression<Func<TSource, TJoin1, TJoin2, TJoin3, object>> predicate, Func<SelectOptions, SelectOptions> options = null)
		{
			if (options != null)
			{
				options(_selectOptions);
			}

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

					selectedFieldInfoList.Add(col);
				}
			}
			else
			{
				throw new ArgumentException("Invalid lambda expression type in select");
			}

			return this;
		}

		public QueryMapper<TSource> Select<TJoin1, TJoin2, TJoin3, TJoin4>(Expression<Func<TSource, TJoin1, TJoin2, TJoin3, TJoin4, object>> predicate, Func<SelectOptions, SelectOptions> options = null)
		{
			if (options != null)
			{
				options(_selectOptions);
			}

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

					selectedFieldInfoList.Add(col);
				}
			}
			else
			{
				throw new ArgumentException("Invalid lambda expression type in select");
			}

			return this;
		}

	}
}
