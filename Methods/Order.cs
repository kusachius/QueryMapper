using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kusachius.QueryMapper
{
	public partial class QueryMapper<TSource> where TSource : new()
	{
		public QueryMapper<TSource> OrderBy(Expression<Func<TSource, object>> predicate)
		{

			if (predicate.Body is NewExpression expr)
			{
				for (var i = 0; i < expr.Arguments.Count; i++)
				{
					var arg = expr.Arguments[i];
					var member = expr.Members[i];

					var col = new SelectedFieldInfo
					{
						SourceField = member.Name,
						SourceAlias = ((arg as MemberExpression).Expression as ParameterExpression).Name,
					};

					orderByFieldInfoList.Add(col);
				}
			}
			else
			{
				throw new ArgumentException("Invalid lambda expression type in order by");
			}

			return this;
		}
	}
}
