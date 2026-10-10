namespace Kusachius.QueryMapper
{
	internal static class Constants
	{
		public const string DeletedOn = "DeletedOn";
		public const string DeletedBy = "DeletedBy";
		public const string CreatedOn = "CreatedOn";
		public const string CreatedBy = "CreatedBy";
		public const string UpdatedOn = "UpdatedOn";
		public const string UpdatedBy = "UpdatedBy";

		public static string[] AuditFields = { DeletedOn, DeletedBy, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy };
		public static string[] DelAuditFields = { DeletedOn, DeletedBy };
	}
}
