namespace Kusachius.QueryMapper
{
	public class SelectOptions
	{
		private bool _selectRemainingFields = false;
		private bool _skipNonExistingInDestinationFields = false;
		private string _userName;
		private bool _setAuditFields = false;

		public SelectOptions SelectRemainingFields(bool selectReMainingfFields = true)
		{
			_selectRemainingFields = selectReMainingfFields;
			return this;
		}

		public SelectOptions SelectRemainingExistingInDestFields()
		{
			_selectRemainingFields = true;
			_skipNonExistingInDestinationFields = true;
			return this;
		}


		public SelectOptions SkipNonExistingInDestinationFields(bool skipNonExistingInDestinationFields = true)
		{
			_skipNonExistingInDestinationFields = skipNonExistingInDestinationFields;
			return this;
		}

		public SelectOptions SetAuditFields(string userName, bool setAuditFields = true)
		{
			_setAuditFields = setAuditFields;
			_userName = userName;
			return this;
		}

		public bool SelectRemainingFieldsSet => _selectRemainingFields;
		public bool SkipNonExistingInDestinationFieldsSet => _skipNonExistingInDestinationFields;
		public string? UserName => _setAuditFields ? _userName : null;
	}
}
