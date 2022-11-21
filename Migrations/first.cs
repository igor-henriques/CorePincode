using System;
using CorePinCash.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace CorePinCash.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20210411194626_first")]
public class first : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable("PinCash", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
			Code = table.Column<string>("longtext"),
			CashAmount = table.Column<int>("int"),
			DateGenerated = table.Column<DateTime>("datetime"),
			Obtained = table.Column<bool>("tinyint(1)"),
			ObtainedBy = table.Column<int>("int"),
			DateObtained = table.Column<DateTime>("datetime")
		}, null, table =>
		{
			table.PrimaryKey("PK_PinCash", x => x.Id);
		});
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable("PinCash");
	}

	protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
	}
}
