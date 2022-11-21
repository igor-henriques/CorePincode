using System;
using CorePinCash.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CorePinCash.Migrations;

[DbContext(typeof(ApplicationDbContext))]
internal class ApplicationDbContextModelSnapshot : ModelSnapshot
{
	protected override void BuildModel(ModelBuilder modelBuilder)
	{
		modelBuilder.HasAnnotation("Relational:MaxIdentifierLength", 64).HasAnnotation("ProductVersion", "5.0.5");
		modelBuilder.Entity("CorePinCash.Model.PinCash", delegate(EntityTypeBuilder b)
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("CashAmount").HasColumnType("int");
			b.Property<string>("Code").IsRequired().HasColumnType("longtext");
			b.Property<DateTime>("DateGenerated").HasColumnType("datetime");
			b.Property<DateTime>("DateObtained").HasColumnType("datetime");
			b.Property<bool>("Obtained").HasColumnType("tinyint(1)");
			b.Property<int>("ObtainedBy").HasColumnType("int");
			b.HasKey("Id");
			b.ToTable("PinCash");
		});
		modelBuilder.Entity("CorePinCash.Model.PinItem", delegate(EntityTypeBuilder b)
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<string>("Code").IsRequired().HasColumnType("longtext");
			b.Property<DateTime>("DateGenerated").HasColumnType("datetime");
			b.Property<DateTime>("DateObtained").HasColumnType("datetime");
			b.Property<int>("ItemAmount").HasColumnType("int");
			b.Property<int>("ItemId").HasColumnType("int");
			b.Property<string>("ItemName").HasMaxLength(50).HasColumnType("varchar(50)");
			b.Property<bool>("Obtained").HasColumnType("tinyint(1)");
			b.Property<int>("ObtainedBy").HasColumnType("int");
			b.HasKey("Id");
			b.ToTable("PinItem");
		});
	}
}
