using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.api.Data.Configurations
{
    public class RequestConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Request> builder)
        {
            builder.ToTable("Requests");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Status)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(r => r.Notes)
                .HasMaxLength(100);

            builder.ToTable(r => r.HasCheckConstraint("CK_Request_ToDate", "[ToDate]>=[FromDate]"));

            builder.Property(r => r.Status).HasConversion(
                v => v.ToString(),
                v => (RequestStatus)Enum.Parse(typeof(RequestStatus), v)
            )
            .HasMaxLength(20);

            builder.HasOne(r => r.Employee)
                .WithMany(e => e.Requests)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.RequestType)
                .WithMany(rt => rt.Requests)
                .HasForeignKey(r => r.RequestTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.RequestSubType)
                .WithMany(rst => rst.Requests)
                .HasForeignKey(r => r.RequestSubTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Indexes
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => new { r.EmployeeId, r.FromDate, r.ToDate });
        }
    }
}