using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Xunit;

namespace Kusachius.QueryMapper.Test
{
	[Collection("Database collection")]
	public abstract class TestBase
	{
		protected TestDbContext context;
		protected readonly DatabaseFixture fixture;

		public TestBase(DatabaseFixture fixture)
		{
			this.fixture = fixture;
			context = new TestDbContext(fixture.Options);
		}
	}
}

