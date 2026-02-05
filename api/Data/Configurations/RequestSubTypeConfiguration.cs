using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.api.Data.Configurations
{
    public class RequestSubTypeConfiguration : IEntityTypeConfiguration<RequestSubType>
    {
        public void Configure(EntityTypeBuilder<RequestSubType> builder)
        {
            builder.ToTable("RequestSubTypes");

            builder.HasKey(rst => rst.Id);

            builder.Property(rst => rst.SubTypeName)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasOne(rst => rst.RequestType)
                .WithMany(rt => rt.SubTypes)
                .HasForeignKey(rst => rst.RequestTypeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}