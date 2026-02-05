using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.api.Data.Configurations
{
    public class SalaryComponentConfiguration : IEntityTypeConfiguration<SalaryComponent>
    {
        public void Configure(EntityTypeBuilder<SalaryComponent> builder)
        {
            builder.ToTable("SalaryComponents");

            builder.HasKey(sc => sc.Id);

            builder.Property(sc => sc.Amount)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();

            builder.HasOne(sc => sc.Employee)
                .WithMany(e => e.SalaryComponents)
                .HasForeignKey(sc => sc.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(sc => sc.SalaryType)
                .WithMany()
                .HasForeignKey(sc => sc.SalaryTypeId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.HasIndex(e => new { e.EmployeeId, e.SalaryTypeId })
                    .IsUnique();

        }
    }
}