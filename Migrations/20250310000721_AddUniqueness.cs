using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieWatching.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MovieVotes_MovieSessionPlanId",
                table: "MovieVotes");

            migrationBuilder.CreateIndex(
                name: "IX_MovieVotes_MovieSessionPlanId_MovieId_VoterId",
                table: "MovieVotes",
                columns: new[] { "MovieSessionPlanId", "MovieId", "VoterId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MovieVotes_MovieSessionPlanId_MovieId_VoterId",
                table: "MovieVotes");

            migrationBuilder.CreateIndex(
                name: "IX_MovieVotes_MovieSessionPlanId",
                table: "MovieVotes",
                column: "MovieSessionPlanId");
        }
    }
}
