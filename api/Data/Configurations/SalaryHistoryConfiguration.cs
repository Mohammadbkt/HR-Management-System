using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.api.Data.Configurations
{
    public class SalaryHistoryConfiguration : IEntityTypeConfiguration<SalaryHistory>
    {
        public void Configure(EntityTypeBuilder<SalaryHistory> builder)
        {
            builder.ToTable("SalaryHistory");

            builder.HasKey(sh => sh.Id);

            builder.Property(sh => sh.BaseSalary)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();

            builder.Property(sh => sh.TotalAllowances)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();

            builder.Property(sh => sh.TotalDeductions)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();

            // Computed columns
            builder.Property(sh => sh.GrossSalary)
                .HasComputedColumnSql("[BaseSalary]+[TotalAllowances]", stored: true)
                .HasColumnType("decimal(18, 2)");

            builder.Property(sh => sh.NetSalary)
                .HasComputedColumnSql("([BaseSalary]+[TotalAllowances])-[TotalDeductions]", stored: true)
                .HasColumnType("decimal(18, 2)");

            builder.HasIndex(sh => new { sh.EmployeeId, sh.Month, sh.Year })
                .IsUnique();

            builder.ToTable(sh => sh.HasCheckConstraint("CK_SalaryHistory_Month",
                "[Month]>=(1) AND [Month]<=(12)"));

            builder.ToTable(sh => sh.HasCheckConstraint("CK_SalaryHistory_Year",
                "[Year]>(2000)"));


            builder.HasOne(sh => sh.Employee)
                .WithMany(e => e.SalaryHistories)
                .HasForeignKey(sh => sh.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}