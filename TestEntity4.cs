using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kusachius.QueryMapper.Test
{
	public class TestEntity4 : TestEntityBase
	{
		[Key]
		public int Id { get; set; }
		public int? Id2 { get; set; }
		public string Name { get; set; }
		public string Option { get; set; }
		public DateTime DeletedOn { get; set; }
	}
}
