using InfotecsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfotecsApi.Data.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<ResultModel>
{
    public void Configure(EntityTypeBuilder<ResultModel> builder)
    {
        builder.ToTable("results");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(x => x.FileName).IsUnique();

        builder.HasIndex(x => x.MinDate);
        builder.HasIndex(x => x.AvgValue);
        builder.HasIndex(x => x.AvgExecutionTime);
    }
}