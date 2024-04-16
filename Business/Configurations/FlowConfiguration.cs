using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model.Models;

namespace DataAccess.Configurations
{
    public class FlowConfiguration : IEntityTypeConfiguration<Flow>
    {
        public void Configure(EntityTypeBuilder<Flow> builder)
        {
            builder.HasIndex(x => x.Id).IsUnique();

        }
    }
}
