using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kusachius.QueryMapper.Test
{
	public class TestEntityBase
	{
		[Key]
		public int Id { get; set; }
	}
	public class TestEntity1 : TestEntityBase
	{
		[Column("date")]
		public DateOnly Date { get; set; }
		public string Name { get; set; }
		public string Option { get; set; }
		public DateTime DeletedOn { get; set; }
	}
}
