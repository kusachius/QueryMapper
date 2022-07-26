namespace Kusachius.QueryMapper
{
    public partial class QueryMapper<TSource> where TSource : new()
	{
		public static int Sum(int x)
		{
			return x;
		}

		public static T Min<T>(T x)
		{
			return x;
		}

		public static T Max<T>(T x)
		{
			return x;
		}

		public static int Count<T>(T x)
		{
			return 0;
		}

		public static int Count()
		{
			return 0;
		}
	}
}
