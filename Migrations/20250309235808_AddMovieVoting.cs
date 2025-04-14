using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieWatching.Migrations
{
    /// <inheritdoc />
    public partial class AddMovieVoting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    AddedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AddedById = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movies_Users_AddedById",
                        column: x => x.AddedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserOrganizers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrganizers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOrganizers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MovieId = table.Column<Guid>(type: "TEXT", nullable: false),
                    When = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieSessions_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieSessionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OrganizerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieSessionPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieSessionPlans_MovieSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "MovieSessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MovieSessionPlans_UserOrganizers_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "UserOrganizers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    VoterId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MovieId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MovieSessionPlanId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieVotes_MovieSessionPlans_MovieSessionPlanId",
                        column: x => x.MovieSessionPlanId,
                        principalTable: "MovieSessionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieVotes_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieVotes_Users_VoterId",
                        column: x => x.VoterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_AddedById",
                table: "Movies",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_MovieSessionPlans_OrganizerId",
                table: "MovieSessionPlans",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieSessionPlans_SessionId",
                table: "MovieSessionPlans",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieSessions_MovieId",
                table: "MovieSessions",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieVotes_MovieId",
                table: "MovieVotes",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieVotes_MovieSessionPlanId",
                table: "MovieVotes",
                column: "MovieSessionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieVotes_VoterId",
                table: "MovieVotes",
                column: "VoterId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizers_UserId",
                table: "UserOrganizers",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieVotes");

            migrationBuilder.DropTable(
                name: "MovieSessionPlans");

            migrationBuilder.DropTable(
                name: "MovieSessions");

            migrationBuilder.DropTable(
                name: "UserOrganizers");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
