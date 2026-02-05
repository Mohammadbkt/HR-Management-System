using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.api.Data.Configurations
{
    public class EmployeeShiftConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.ToTable("Schedules");

            builder.HasKey(es => new { es.EmployeeId, es.ShiftId, es.ShiftDate });

            builder.HasOne(es => es.Employee)
                .WithMany(e => e.Schedules)
                .HasForeignKey(es => es.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(es => es.Shift)
                .WithMany(s => s.Schedules)
                .HasForeignKey(es => es.ShiftId)
                .OnDelete(DeleteBehavior.NoAction);

            
            builder.HasIndex(es => new { es.ShiftId, es.ShiftDate });
        }
    }
}