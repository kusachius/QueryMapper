using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kusachius.QueryMapper
{
	public partial class QueryMapper<TSource> where TSource : new()
	{
		public string ToSql()
		{
			IEnumerable<object> paramValues;
			var sql = ToSql(out paramValues);

			if (paramValues.Count() > 0)
			{
				throw new Exception("Query has parameters, use ToSql(out IEnumerable<object> parameterValues) instead of ToSql()");
			}
			return sql;
		}

		public string ToSql(out IEnumerable<object> paramValues)
		{
			var sb = new StringBuilder();

			if (selectedFieldInfoList.Count() == 0)
			{
				_selectOptions.SelectRemainingFields(true);
			}

			// add all remaining fields
			if (_selectOptions.SelectRemainingFieldsSet)
			{
				var selectedProps = SrcTable.GetProps()
					.Where(p => !Constants.DelAuditFields.Contains(p.Name))
					.Where(x => !selectedFieldInfoList.Select(a => a.TargetField).Contains(x.Name)).ToList();
				if (_selectOptions.SkipNonExistingInDestinationFieldsSet)
				{
					selectedProps = selectedProps.Where(x => DestTable.GetProps().Select(a => a.Name).Contains(x.Name)).ToList();
				}

				foreach (var srcProp in selectedProps)
				{
					selectedFieldInfoList.Add(new SelectedFieldInfo { TargetField = srcProp.Name, SourceAlias = srcTableAlias, SourceField = srcProp.Name });
				}
			}

			// INTO
			if (!_isUpdate)
			{
				var destColList = new List<string>();

				foreach (var fld in selectedFieldInfoList)
				{
					destColList.Add(DestFieldToCol(fld.TargetField));
				}

				var destCols = string.Join(", ", destColList.Select(x => x));
				if (string.IsNullOrEmpty(destCols))
				{
					throw new Exception("No fields selected");
				}
				sb.Append($"INSERT INTO {DestTable.TableName} ({destCols})\n");


				// SELECT
				var srcCols = string.Join(", ", selectedFieldInfoList.Select(x => x.ToSql(DestFieldToCol, SrcFieldToCol)));
				sb.Append($"SELECT {srcCols}\nFROM {SrcTable.TableName} {srcTableAlias}");
			}
			else
			{
				// UPDATE
				sb.Append($"UPDATE {SrcTable.TableName} u\nSET ");
				var setCols = string.Join(",\n\t", setFieldInfoList.Select(x => x.ToSetSql(SrcFieldToCol)));
				sb.Append(setCols);

				if (!string.IsNullOrEmpty(_updateFrom))
                {
					sb.Append(_updateFrom);
				}
			}
			// JOIN
			foreach (var join in _join)
			{
				sb.Append(join);
			}

			// WHERE
			if (HasSrcDeletedOnField())
			{
				_where.Insert(0, $"{ srcTableAlias}.\"DeletedOn\" IS NULL");
			}
			if (_where.Any())
			{
				sb.Append($"\nWHERE ({_where.First()})");
				foreach (var wh in _where.Skip(1))
				{
					sb.Append($"\n\tAND ({wh})");
				}
			}
			if (groupByFieldInfoList.Any())
			{
				var groupByCols = string.Join(", ", groupByFieldInfoList.Select(x => x.ToOrderSql(SrcFieldToCol)));
				sb.Append($"\nGROUP BY {groupByCols}");
			}
			if (orderByFieldInfoList.Any())
			{
				var orderCols = string.Join(", ", orderByFieldInfoList.Select(x => x.ToOrderSql(SrcFieldToCol)));
				sb.Append($"\nORDER BY {orderCols}");
			}

			sb.Append(";\n");
			paramValues = _params;

			return sb.ToString();
		}
	}
}
