using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieWatching.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationshipMovieSessionPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieSessionPlans_MovieSessions_SessionId",
                table: "MovieSessionPlans");

            migrationBuilder.DropIndex(
                name: "IX_MovieSessionPlans_SessionId",
                table: "MovieSessionPlans");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "MovieSessionPlans");

            migrationBuilder.AddColumn<Guid>(
                name: "MovieSessionPlanId",
                table: "MovieSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MovieSessions_MovieSessionPlanId",
                table: "MovieSessions",
                column: "MovieSessionPlanId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieSessions_MovieSessionPlans_MovieSessionPlanId",
                table: "MovieSessions",
                column: "MovieSessionPlanId",
                principalTable: "MovieSessionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieSessions_MovieSessionPlans_MovieSessionPlanId",
                table: "MovieSessions");

            migrationBuilder.DropIndex(
                name: "IX_MovieSessions_MovieSessionPlanId",
                table: "MovieSessions");

            migrationBuilder.DropColumn(
                name: "MovieSessionPlanId",
                table: "MovieSessions");

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "MovieSessionPlans",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieSessionPlans_SessionId",
                table: "MovieSessionPlans",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieSessionPlans_MovieSessions_SessionId",
                table: "MovieSessionPlans",
                column: "SessionId",
                principalTable: "MovieSessions",
                principalColumn: "Id");
        }
    }
}
