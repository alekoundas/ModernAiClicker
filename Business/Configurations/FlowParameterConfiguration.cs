using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model.Models;

namespace Business.Configurations
{
    public class FlowParameterConfiguration : IEntityTypeConfiguration<FlowParameter>
    {

        public void Configure(EntityTypeBuilder<FlowParameter> builder)
        {
            builder.HasIndex(x => x.Id).IsUnique();
            builder.HasKey(x => x.Id);

            // Store Enum value as string instead of int.
            builder.Property(x => x.Type).HasConversion<string>();
            builder.Property(x => x.TemplateSearchAreaType).HasConversion<string>();


            builder.HasOne(x => x.ParentFlowParameter)
              .WithMany(x => x.ChildrenFlowParameters)
              .HasForeignKey(x => x.ParentFlowParameterId)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
