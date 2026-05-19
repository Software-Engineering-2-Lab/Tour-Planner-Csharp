using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCompleteSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    from_location = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    to_location = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    transport_type = table.Column<int>(type: "integer", nullable: false),
                    distance = table.Column<double>(type: "double precision", nullable: false),
                    estimated_time = table.Column<double>(type: "double precision", nullable: false),
                    route_image_path = table.Column<string>(type: "text", nullable: true),
                    popularity = table.Column<int>(type: "integer", nullable: true),
                    child_friendliness = table.Column<double>(type: "double precision", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tours_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: false),
                    difficulty = table.Column<int>(type: "integer", nullable: false),
                    total_distance = table.Column<double>(type: "double precision", nullable: false),
                    total_time = table.Column<double>(type: "double precision", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    TourId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_logs_tours_TourId",
                        column: x => x.TourId,
                        principalTable: "tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tour_images",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_path = table.Column<string>(type: "text", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    tour_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tour_images_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_logs_TourId",
                table: "logs",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_tour_images_tour_id",
                table: "tour_images",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_tours_UserId",
                table: "tours",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "logs");

            migrationBuilder.DropTable(
                name: "tour_images");

            migrationBuilder.DropTable(
                name: "tours");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
