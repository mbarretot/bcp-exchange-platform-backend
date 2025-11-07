using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bcp.Exchange.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    Description = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    LongDescription = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    NumericValue = table.Column<decimal>(
                        type: "decimal(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: true
                    ),
                    TextValue = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    ModifiedBy = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parameters_Parameters_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Parameters_Parameters_ParentId1",
                        column: x => x.ParentId1,
                        principalTable: "Parameters",
                        principalColumn: "Id"
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rate = table.Column<decimal>(
                        type: "decimal(18,6)",
                        precision: 18,
                        scale: 6,
                        nullable: false
                    ),
                    CurrencySourceId = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: false
                    ),
                    CurrencyTargetId = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: false
                    ),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    ModifiedBy = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Parameters_CurrencySourceId",
                        column: x => x.CurrencySourceId,
                        principalTable: "Parameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Parameters_CurrencyTargetId",
                        column: x => x.CurrencyTargetId,
                        principalTable: "Parameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_CurrencySourceId_CurrencyTargetId_IsActive",
                table: "ExchangeRates",
                columns: new[] { "CurrencySourceId", "CurrencyTargetId", "IsActive" },
                filter: "[IsActive] = 1"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_CurrencyTargetId",
                table: "ExchangeRates",
                column: "CurrencyTargetId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_Code",
                table: "Parameters",
                column: "Code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_ParentId",
                table: "Parameters",
                column: "ParentId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_ParentId1",
                table: "Parameters",
                column: "ParentId1"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ExchangeRates");

            migrationBuilder.DropTable(name: "Parameters");
        }
    }
}
