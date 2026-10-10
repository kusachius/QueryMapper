using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Kusachius.QueryMapper
{
	public static class QueryMapperExprExtensions
	{
		public static string Between(this DateOnly _, DateOnly from, DateOnly to)
		{
			return $"{{0}} BETWEEN '{from:yyyy-MM-dd}' AND '{to:yyyy-MM-dd}'"; // TODO;
		}

		//public static bool Between(this DateOnly _, DateOnly from, DateOnly to)
		//{
		//	return true;
		//}


		public static QueryMapper<T> From<T>(this DbContext context) where T : new()
		{
			return new QueryMapper<T>(context);
		}

		public static QueryMapper<T> Update<T>(this DbContext context) where T : new()
		{
			return new QueryMapper<T>(context, true);
		}

	}
}
