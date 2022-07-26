using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kusachius.QueryMapper.Test
{
	public class TestEntity1a
	{
		[Key]
		public int Id { get; set; }
		[Column("date")]
		public DateOnly Date { get; set; }
		public string Name { get; set; }
		public string Option { get; set; }
	}
}
