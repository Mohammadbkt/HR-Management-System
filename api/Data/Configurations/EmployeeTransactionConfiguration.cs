using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.api.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.api.Data.Configurations
{
    public class EmployeeTransactionConfiguration : IEntityTypeConfiguration<EmployeeTransaction>
    {
        public void Configure(EntityTypeBuilder<EmployeeTransaction> builder)
        {
            builder.ToTable("EmployeeTransactions");

            builder.HasKey(et => et.Id);


            builder.HasOne(et => et.Employee)
                .WithMany(e => e.Transactions)
                .HasForeignKey(et => et.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.HasOne(et => et.Request)
                .WithMany(r => r.Transactions)
                .HasForeignKey(et => et.RequestId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}