using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.api.Data.Configurations
{
    public class RequestTypeConfiguration : IEntityTypeConfiguration<RequestType>
    {
        public void Configure(EntityTypeBuilder<RequestType> builder)
        {
            builder.ToTable("RequestTypes");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.TypeName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(rt => rt.RequiresBalance)
            .IsRequired()
            .HasDefaultValue(true);
        }
    }
}