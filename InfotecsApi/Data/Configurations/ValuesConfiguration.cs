using InfotecsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfotecsApi.Data.Configurations;

public class ValuesConfiguration : IEntityTypeConfiguration<ValueModel>
{
    public void Configure(EntityTypeBuilder<ValueModel> builder)
    {
        builder.ToTable("values");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(x => x.ExecutionTime)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired();
        
        builder.HasIndex(x => new { x.FileName, x.Date });
    }
}