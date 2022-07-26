namespace Kusachius.QueryMapper
{
	public class RawSql
	{
		private readonly string _sql;

		public RawSql(string sql)
		{
			_sql = sql;
		}

		public override string ToString() => _sql;

		public static implicit operator RawSql(string str)
		{
			return new RawSql(str);
		}
	}
}
