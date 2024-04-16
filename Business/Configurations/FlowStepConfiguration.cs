using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model.Models;

namespace DataAccess.Configurations
{
    public class FlowStepConfiguration : IEntityTypeConfiguration<FlowStep>
    {
        public void Configure(EntityTypeBuilder<FlowStep> builder)
        {
            builder.HasIndex(x => x.Id).IsUnique();

            builder.HasOne(x => x.Flow)
                .WithMany(x => x.FlowSteps)
                .HasForeignKey(x => x.FlowId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
