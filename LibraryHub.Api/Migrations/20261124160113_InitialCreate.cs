using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Author = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Isbn = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    PublishedYear = table.Column<int>(type: "int", nullable: false),
                    TotalCopies = table.Column<int>(type: "int", nullable: false),
                    AvailableCopies = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    FullName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    JoinedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    BorrowedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DueOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ReturnedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LateFee = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loans_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Loans_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "AvailableCopies", "Isbn", "PublishedYear", "Title", "TotalCopies" },
                values: new object[,]
                {
                    { 1, "Robert C. Martin", 2, "9780132350884", 2008, "Clean Code", 3 },
                    { 2, "Andrew Hunt", 1, "9780135957059", 2019, "The Pragmatic Programmer", 2 },
                    { 3, "Erich Gamma", 2, "9780201633610", 1994, "Design Patterns", 2 },
                    { 4, "Martin Fowler", 2, "9780134757599", 2018, "Refactoring", 2 },
                    { 5, "Thomas H. Cormen", 1, "9780262046305", 2022, "Introduction to Algorithms", 1 },
                    { 6, "Jon Skeet", 3, "9781617294532", 2019, "C# in Depth", 3 },
                    { 7, "Eric Evans", 1, "9780321125217", 2003, "Domain-Driven Design", 1 },
                    { 8, "Steve McConnell", 2, "9780735619678", 2004, "Code Complete", 2 },
                    { 9, "Frederick P. Brooks", 1, "9780201835953", 1995, "The Mythical Man-Month", 1 },
                    { 10, "Eric Freeman", 2, "9781492078005", 2020, "Head First Design Patterns", 2 }
                });

            migrationBuilder.InsertData(
                table: "Members",
                columns: new[] { "Id", "Email", "FullName", "IsActive", "JoinedOn", "Phone" },
                values: new object[,]
                {
                    { 1, "ali.raza@example.com", "Ali Raza", true, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "03001234567" },
                    { 2, "sara.khan@example.com", "Sara Khan", true, new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "03111234567" },
                    { 3, "usman.tariq@example.com", "Usman Tariq", true, new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "03211234567" },
                    { 4, "ayesha.noor@example.com", "Ayesha Noor", true, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 5, "hamza.iqbal@example.com", "Hamza Iqbal", false, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "03331234567" }
                });

            migrationBuilder.InsertData(
                table: "Loans",
                columns: new[] { "Id", "BookId", "BorrowedOn", "DueOn", "LateFee", "MemberId", "ReturnedOn" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, 1, null },
                    { 2, 2, new DateTime(2026, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, 2, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_Isbn",
                table: "Books",
                column: "Isbn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BookId",
                table: "Loans",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_MemberId",
                table: "Loans",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_Email",
                table: "Members",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
