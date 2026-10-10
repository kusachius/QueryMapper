using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kusachius.QueryMapper
{
	internal class TableInfo
	{
		Type type;
		IEntityType entityType;
		IEnumerable<IProperty> props;
		StoreObjectIdentifier storeObjectIdentifier;

		public TableInfo(Type type, IEntityType entityType, IEnumerable<IProperty> props, StoreObjectIdentifier storeObjectIdentifier)
		{
			this.type = type;
			this.entityType = entityType;
			this.props = props;
			this.storeObjectIdentifier = storeObjectIdentifier;
		}

		public string TableName => $"\"{entityType.GetTableName()!}\"";
		public Type Type => type;
		public IEnumerable<IProperty> GetProps() => props;
		public string GetColumnName(string field)
		{
			var prop = props.FirstOrDefault(p => p.Name == field);
			if (prop == null)
			{
				throw new ArgumentException($"Field {field} does not exist on entity {type}");
			}
			return $"\"{prop.GetColumnName(storeObjectIdentifier)!}\"";
		}

		public bool HasDeletedOnField() => GetProps().Any(x => x.Name == Constants.DeletedOn);
	}
}
