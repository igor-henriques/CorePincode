using System;
using CorePinCash.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace CorePinCash.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20210923033746_pinitem")]
public class pinitem : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable("PinItem", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<int>("int").Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
			OperationBuilder<AddColumnOperation> code = table.Column<string>("longtext");
			OperationBuilder<AddColumnOperation> itemId = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> itemAmount = table.Column<int>("int");
			int? maxLength = 50;
			return new
			{
				Id = id,
				Code = code,
				ItemId = itemId,
				ItemAmount = itemAmount,
				ItemName = table.Column<string>("varchar(50)", null, maxLength, rowVersion: false, null, nullable: true),
				DateGenerated = table.Column<DateTime>("datetime"),
				Obtained = table.Column<bool>("tinyint(1)"),
				ObtainedBy = table.Column<int>("int"),
				DateObtained = table.Column<DateTime>("datetime")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_PinItem", x => x.Id);
		});
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable("PinItem");
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
