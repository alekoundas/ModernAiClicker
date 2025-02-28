using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model.Models;

namespace Business.Configurations
{
    public class FlowConfiguration : IEntityTypeConfiguration<Flow>
    {
        public void Configure(EntityTypeBuilder<Flow> builder)
        {
            builder.HasIndex(x => x.Id).IsUnique();
            builder.HasKey(x => x.Id);


            // Store Enum value as string instead of int.
            builder.Property(x => x.Type).HasConversion<string>();

            // 1-1
            builder.HasOne(x => x.FlowStep)           
               .WithOne(x => x.Flow)           
               .HasForeignKey<FlowStep>(x => x.FlowId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade); // Deleting Flow deletes FlowStep.

            // 1-1
            builder.HasOne(x => x.FlowParameter)
               .WithOne(x => x.Flow)
               .HasForeignKey<FlowParameter>(x => x.FlowId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade); // Deleting Flow deletes FlowStep.


            builder.HasOne(x => x.ParentSubFlowStep)
              .WithMany(x => x.SubFlows)
              .HasForeignKey(x => x.ParentSubFlowStepId)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.SetNull); // Deleting Flow deletes SubFlows.
        }
    }
}
