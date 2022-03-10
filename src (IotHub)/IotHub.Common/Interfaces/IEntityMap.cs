using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IotHub.Common.Interfaces
{
	public interface IEntityMap<TEntity> where TEntity : class
	{
		void Configure(EntityTypeBuilder<TEntity> entityBuilder);
	}
}