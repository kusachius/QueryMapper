using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kusachius.QueryMapper
{
	public partial class QueryMapper<TSource> where TSource : new()
	{
		private readonly DbContext _context;
		private readonly bool _isUpdate;

		private readonly List<SelectedFieldInfo> selectedFieldInfoList = new();
		private readonly List<SelectedFieldInfo> setFieldInfoList = new();
		private readonly List<SelectedFieldInfo> orderByFieldInfoList = new();
		private readonly List<SelectedFieldInfo> groupByFieldInfoList = new();
		private readonly List<TableInfo> _tableInfo = new();
		private TableInfo _destTable;

		private readonly List<string> _where = new();
		private readonly List<string> _join = new();
		private string _updateFrom = string.Empty;
		private readonly List<object> _params = new();
		private readonly SelectOptions _selectOptions = new();

		protected string srcTableAlias = "a";

		internal TableInfo SrcTable => _tableInfo.First();
		internal TableInfo DestTable => _destTable;

		public QueryMapper(DbContext context, bool isUpdate = false)
		{
			_context = context;
			_isUpdate = isUpdate;
			if (isUpdate)
            {
				srcTableAlias = "u";
            }
			GetTableInfo(typeof(TSource));
		}

		public void Execute()
		{
			var sql = ToSql(out var sqlParams);

			_context.Database.ExecuteSqlRaw(sql, sqlParams);
		}


		public QueryMapper<TSource> InsertInto<TDest>(Func<TSource, object> func)
		{
			return this;
		}

		public QueryMapper<TSource> InsertInto<TDest>()
		{
			_destTable = GetTableInfo(typeof(TDest));
			return this;
		}

		public QueryMapper<TSource> WhereRaw(string sql)
		{
			_where.Add(sql);

			return this;
		}

		public QueryMapper<TSource> Where(Expression<Func<TSource, string>> predicate)
		{
			var func = predicate.Compile();
			var str = func.Invoke(new TSource());

			var arg0 = ((MethodCallExpression)predicate.Body).Arguments[0] as MemberExpression;

			var fieldName = arg0.Member.Name;
			//_where.Add(fieldName);
			AddWhere(str, fieldName, srcTableAlias, arg0.Member.DeclaringType);

			return this;
		}

		public QueryMapper<TSource> Where(Expression<Func<TSource, bool>> predicate)
		{

			_where.Add(LambdaToSql(predicate.Body, out _));

			return this;
		}

		public QueryMapper<TSource> Where<TJoin1>(Expression<Func<TSource, TJoin1, bool>> predicate)
		{
			_where.Add(LambdaToSql(predicate.Body, out _));

			return this;
		}

		protected void AddWhere(string expr, string fieldName, string tableAlias, Type type)
		{
			var col = GetTableInfo(type).GetColumnName(fieldName);

			var str = string.Format(expr, $"{tableAlias}.{col}");
			_where.Add(str);
		}

		protected void AddWhere(string expr)
		{
			_where.Add(expr);
		}

		//private void SetDestTable(Type type)
		//{
		//	destEntity = _context.Model.FindEntityType(type);
		//	if (destEntity == null)
		//	{
		//		throw new ArgumentException($"Entity {type} is not a part in the context {_context.GetType().Name}");
		//	}
		//	destTableName = destEntity.GetTableName();
		//	destTableIdentifier = StoreObjectIdentifier.Table(destTableName, destEntity.GetSchema());
		//	_allDestTableProps = destEntity.GetProperties();

		//}

		private string SrcFieldToCol(string fieldName) => SrcTable.GetColumnName(fieldName);
	
		private string DestFieldToCol(string fieldName) => _destTable.GetColumnName(fieldName);

		private bool HasSrcDeletedOnField() => SrcTable.GetProps().Any(x => x.Name == Constants.DeletedOn);
	}
}
