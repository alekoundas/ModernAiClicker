using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model.Models;

namespace DataAccess.Configurations
{
    public class ExecutionConfiguration : IEntityTypeConfiguration<Execution>
    {
        public void Configure(EntityTypeBuilder<Execution> builder)
        {
            builder.HasIndex(x => x.Id).IsUnique();

            builder.HasOne(x => x.Flow)
                .WithOne(x => x.Execution)
                .HasForeignKey<Execution>(x => x.FlowId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(x => x.FlowStep)
                .WithOne(x => x.Execution)
                .HasForeignKey<Execution>(x => x.FlowStepId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
