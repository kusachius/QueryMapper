using Microsoft.EntityFrameworkCore;

namespace Kusachius.QueryMapper.Test
{
	public class TestDbContext : DbContext
	{

		public TestDbContext(DbContextOptions<TestDbContext> options)
			: base(options)
		{
		}
		public virtual DbSet<TestEntity1> Test1 { get; set; }
		public virtual DbSet<TestEntity1a> Test1a { get; set; }
		public virtual DbSet<TestEntity2> Test2 { get; set; }
		public virtual DbSet<TestEntity2a> Test2a { get; set; }
		public virtual DbSet<TestEntity3> Test3 { get; set; }
		public virtual DbSet<TestEntity4> Test4 { get; set; }
	}
}
