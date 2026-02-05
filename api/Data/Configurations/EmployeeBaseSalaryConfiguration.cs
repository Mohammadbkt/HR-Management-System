using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.api.entities;

namespace HRMS.api.Data.Configurations
{
    public class EmployeeBaseSalaryConfiguration : IEntityTypeConfiguration<EmployeeBaseSalary>
    {
        public void Configure(EntityTypeBuilder<EmployeeBaseSalary> builder)
        {
            builder.ToTable("EmployeeBaseSalaries");

            builder.HasKey(eb => eb.Id);

            // builder.HasOne(eb=>eb.Employee)
            // .WithOne(e=>e.EmployeeBaseSalary)

            builder.Property(eb => eb.BaseSalary)
            .HasColumnType("DECIMAL(18, 2)")
               .IsRequired();

            builder.Property(ebs => ebs.EffectiveFrom)
               .HasColumnType("Date")
               .IsRequired();

            builder.Property(ebs => ebs.EffectiveTo)
               .HasColumnType("Date")
               .IsRequired();

            builder.ToTable(eb => eb.HasCheckConstraint("CK_EmployeeBaseSalary_Effective",
                "[EffectiveTo]>=[EffectiveFrom]"));


            builder.HasOne(ebs => ebs.Employee)
                .WithOne(e => e.BaseSalary)
                .HasForeignKey<EmployeeBaseSalary>(ebs => ebs.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            
            builder.HasIndex(e=> new {e.EmployeeId, e.EffectiveFrom, e.EffectiveTo});
            
            builder.ToTable(t => t.HasCheckConstraint("CK_EmployeeBaseSalary_DateRange", 
        "EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom"));


        }
    }
}