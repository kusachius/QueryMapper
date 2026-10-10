using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kusachius.QueryMapper
{
	internal class SelectedFieldInfo
	{
		public string TargetCol { get; set; }
		public string SrcCol { get; set; }
		public string TargetField { get; set; }
		public string SourceAlias { get; set; }
		public string SourceField { get; set; }
		public string SqlRaw { get; set; }

		public string ToSql(Func<string, string> getTargetColName, Func<string, string> getSrcColName)
		{
			if(TargetCol == null)
			{
				TargetCol = getTargetColName(TargetField);
			}
			if (TargetCol != null && TargetCol == SrcCol && SqlRaw != null) return $"{SqlRaw}";
			if (TargetCol != null && TargetCol != SrcCol && SqlRaw != null) return $"{SqlRaw} AS {TargetCol}";


			if (SqlRaw != null) return $"{SqlRaw} AS {TargetCol}";

			var srcColName = getSrcColName(SourceField);

			if (TargetCol == srcColName)
			{
				return $"{SourceAlias}.{srcColName}";
			}
			else
			{
				return $"{SourceAlias}.{srcColName} AS {TargetCol}";
			}
		}

		public string ToSetSql(Func<string, string> getSrcColName)
		{
			if (TargetCol == null)
			{
				TargetCol = getSrcColName(TargetField);
			}
			if (TargetCol != null && TargetCol != SrcCol && SqlRaw != null) return $"{TargetCol} = {SqlRaw}";


			if (SqlRaw != null) return $"{TargetCol} = {SqlRaw}";

			var srcColName = getSrcColName(SourceField);

			return $"{TargetCol} = {SourceAlias}.{srcColName}";
		}

		public string ToOrderSql(Func<string, string> getSrcColName)
		{
			var srcColName = SrcCol ?? getSrcColName(SourceField);

			return $"{SourceAlias}.{srcColName}";
		}
	}
}
