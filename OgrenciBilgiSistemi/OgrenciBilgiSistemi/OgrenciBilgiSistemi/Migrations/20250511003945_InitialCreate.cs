using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tDers",
                columns: table => new
                {
                    dersID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dersKodu = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    dersAd = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    kredi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tDers", x => x.dersID);
                });

            migrationBuilder.CreateTable(
                name: "tFakulte",
                columns: table => new
                {
                    fakulteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fakulteAd = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tFakulte", x => x.fakulteID);
                });

            migrationBuilder.CreateTable(
                name: "tBolum",
                columns: table => new
                {
                    bolumID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bolumAd = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    fakulteID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tBolum", x => x.bolumID);
                    table.ForeignKey(
                        name: "FK_tBolum_tFakulte_fakulteID",
                        column: x => x.fakulteID,
                        principalTable: "tFakulte",
                        principalColumn: "fakulteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tOgrenci",
                columns: table => new
                {
                    ogrenciID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    soyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    bolumID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tOgrenci", x => x.ogrenciID);
                    table.ForeignKey(
                        name: "FK_tOgrenci_tBolum_bolumID",
                        column: x => x.bolumID,
                        principalTable: "tBolum",
                        principalColumn: "bolumID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tOgrenciDers",
                columns: table => new
                {
                    ogrenciID = table.Column<int>(type: "int", nullable: false),
                    dersID = table.Column<int>(type: "int", nullable: false),
                    yil = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    yariyil = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    vize = table.Column<int>(type: "int", nullable: true),
                    final = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tOgrenciDers", x => new { x.ogrenciID, x.dersID });
                    table.ForeignKey(
                        name: "FK_tOgrenciDers_tDers_dersID",
                        column: x => x.dersID,
                        principalTable: "tDers",
                        principalColumn: "dersID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tOgrenciDers_tOgrenci_ogrenciID",
                        column: x => x.ogrenciID,
                        principalTable: "tOgrenci",
                        principalColumn: "ogrenciID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "tDers",
                columns: new[] { "dersID", "dersAd", "dersKodu", "kredi" },
                values: new object[,]
                {
                    { 1, "Programlama Temelleri", "BIL101", 5 },
                    { 2, "Veri Tabanı Yönetimi", "BIL102", 4 },
                    { 3, "Matematik I", "MAT101", 6 }
                });

            migrationBuilder.InsertData(
                table: "tFakulte",
                columns: new[] { "fakulteID", "fakulteAd" },
                values: new object[,]
                {
                    { 1, "Mühendislik Fakültesi" },
                    { 2, "Fen Edebiyat Fakültesi" }
                });

            migrationBuilder.InsertData(
                table: "tBolum",
                columns: new[] { "bolumID", "bolumAd", "fakulteID" },
                values: new object[,]
                {
                    { 1, "Bilgisayar Mühendisliği", 1 },
                    { 2, "Elektrik-Elektronik Mühendisliği", 1 },
                    { 3, "Matematik", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tBolum_fakulteID",
                table: "tBolum",
                column: "fakulteID");

            migrationBuilder.CreateIndex(
                name: "IX_tOgrenci_bolumID",
                table: "tOgrenci",
                column: "bolumID");

            migrationBuilder.CreateIndex(
                name: "IX_tOgrenciDers_dersID",
                table: "tOgrenciDers",
                column: "dersID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tOgrenciDers");

            migrationBuilder.DropTable(
                name: "tDers");

            migrationBuilder.DropTable(
                name: "tOgrenci");

            migrationBuilder.DropTable(
                name: "tBolum");

            migrationBuilder.DropTable(
                name: "tFakulte");
        }
    }
}
