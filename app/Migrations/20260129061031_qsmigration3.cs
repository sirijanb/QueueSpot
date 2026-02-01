using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class qsmigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HospitalServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HospitalServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HospitalServiceAssignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    HospitalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HospitalServiceAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HospitalServiceAssignment_HospitalServices_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "HospitalServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HospitalServiceAssignment_Hospitals_HospitalId",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HospitalServiceAssignment_HospitalId",
                table: "HospitalServiceAssignment",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_HospitalServiceAssignment_ServiceId",
                table: "HospitalServiceAssignment",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HospitalServiceAssignment");

            migrationBuilder.DropTable(
                name: "HospitalServices");
        }
    }
}
