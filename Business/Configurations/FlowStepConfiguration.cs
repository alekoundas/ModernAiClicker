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
            builder.HasKey(x => x.Id);


            // Store Enum value as string instead of int.
            builder.Property(x => x.Type).HasConversion<string>();

            builder.HasOne(x => x.Flow)
                .WithMany(x => x.FlowSteps)
                .HasForeignKey(x => x.FlowId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ParentFlowStep)
                .WithMany(x => x.ChildrenFlowSteps)
                .HasForeignKey(x => x.ParentFlowStepId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ParentTemplateSearchFlowStep)
                .WithMany(x => x.ChildrenTemplateSearchFlowSteps)
                .HasForeignKey(x => x.ParentTemplateSearchFlowStepId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.SearchAreaParameterFlowStep)
                .WithMany(x => x.ChildrenSearchAreaParameterFlowSteps)
                .HasForeignKey(x => x.SearchAreaParameterFlowStepId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
