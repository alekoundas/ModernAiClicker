﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model.Models;

namespace DataAccess.Configurations
{
    public class ExecutionConfiguration : IEntityTypeConfiguration<Execution>
    {
        public void Configure(EntityTypeBuilder<Execution> builder)
        {
            builder.HasIndex(x => x.Id).IsUnique();
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Flow)
                .WithMany(x => x.Executions)
                .HasForeignKey(x => x.FlowId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.FlowStep)
                .WithMany(x => x.Executions)
                .HasForeignKey(x => x.FlowStepId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);



            builder
                .HasOne(x => x.ParentExecution)
                .WithOne(x => x.ChildExecution)
                .HasForeignKey<Execution>(x => x.ParentExecutionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.ChildExecution)
                .WithOne(x => x.ParentExecution)
                .HasForeignKey<Execution>(x => x.ChildExecutionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);




            builder
             .HasOne(x => x.ParentLoopExecution)
             .WithOne(x => x.ChildLoopExecution)
             .HasForeignKey<Execution>(x => x.ParentLoopExecutionId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);

            builder
             .HasOne(x => x.ChildLoopExecution)
             .WithOne(x => x.ParentLoopExecution)
             .HasForeignKey<Execution>(x => x.ChildLoopExecutionId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.CurrentMultipleTemplateSearchFlowStep)
              .WithMany(x => x.CurrentMultipleTemplateSearchFlowStepExecutions)
              .HasForeignKey(x => x.CurrentMultipleTemplateSearchFlowStepId)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
