using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kusachius.QueryMapper
{
	public partial class QueryMapper<TSource> where TSource : new()
	{
		private TableInfo GetTableInfo(Type type)
		{
			var tableInfo = _tableInfo.FirstOrDefault(x => x.Type == type);
			if (tableInfo == null)
			{
				var entity = _context.Model.FindEntityType(type);
				if (entity == null)
				{
					throw new ArgumentException($"Entity {type} is not a part in the context {_context.GetType().Name}");
				}
				var identifier = StoreObjectIdentifier.Table(entity.GetTableName(), entity.GetSchema());
				var props = entity.GetProperties();
				tableInfo = new TableInfo(type, entity, props, identifier);
				_tableInfo.Add(tableInfo);
			}

			return tableInfo;
		}

		private string GetTableName(Type type)
		{
			var tableInfo = GetTableInfo(type);

			return tableInfo.TableName;
		}
	}
}
