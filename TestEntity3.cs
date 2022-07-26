using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kusachius.QueryMapper.Test
{
	public interface ITestId
	{
		int Id { get; set; }
	}
	public class TestEntity3 : TestEntityBase, ITestId
	{
		[Key]
		public int Id { get; set; }
		[Column("date")]
		public DateOnly Date { get; set; }
		public string Name { get; set; }
		public string Option { get; set; }
		public DateTime DeletedOn { get; set; }
	}
}
