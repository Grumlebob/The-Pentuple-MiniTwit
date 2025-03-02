using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MiniTwit.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "latest",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    latest_event = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_latest", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    user_id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    username = table.Column<string>(type: "TEXT", nullable: false),
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    pw_hash = table.Column<string>(type: "TEXT", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                }
            );

            migrationBuilder.CreateTable(
                name: "follower",
                columns: table => new
                {
                    who_id = table.Column<int>(type: "integer", nullable: false),
                    whom_id = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_follower", x => new { x.who_id, x.whom_id });
                    table.ForeignKey(
                        name: "FK_follower_user_who_id",
                        column: x => x.who_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_follower_user_whom_id",
                        column: x => x.whom_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "message",
                columns: table => new
                {
                    message_id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "TEXT", nullable: false),
                    pub_date = table.Column<int>(type: "integer", nullable: true),
                    flagged = table.Column<int>(type: "integer", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message", x => x.message_id);
                    table.ForeignKey(
                        name: "FK_message_user_author_id",
                        column: x => x.author_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.InsertData(
                table: "latest",
                columns: new[] { "id", "latest_event" },
                values: new object[] { 1, 0 }
            );

            migrationBuilder.CreateIndex(
                name: "IX_follower_whom_id",
                table: "follower",
                column: "whom_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_message_author_id",
                table: "message",
                column: "author_id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "follower");

            migrationBuilder.DropTable(name: "latest");

            migrationBuilder.DropTable(name: "message");

            migrationBuilder.DropTable(name: "user");
        }
    }
}
