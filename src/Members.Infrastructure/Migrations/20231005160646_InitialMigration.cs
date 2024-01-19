using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Members.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscordUserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    DiscordGuildDisplayName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DiscordAvatarUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DiscordUserName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SpectrumCommunityMoniker = table.Column<string>(type: "text", nullable: true),
                    GameHandle = table.Column<string>(type: "text", nullable: true),
                    IsGameHandleVerified = table.Column<bool>(type: "boolean", nullable: false),
                    GameHandleVerificationCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    VerificationCodeExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscordRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<byte>(type: "smallint", nullable: false),
                    RoleType = table.Column<byte>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Permissions = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sentences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    JudgeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SentenceType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sentences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sentences_Members_JudgeId",
                        column: x => x.JudgeId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerifyCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerifyCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerifyCodes_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberRole",
                columns: table => new
                {
                    MembersId = table.Column<Guid>(type: "uuid", nullable: false),
                    RolesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberRole", x => new { x.MembersId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_MemberRole_Members_MembersId",
                        column: x => x.MembersId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberRole_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncidentReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    AccuserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccusedId = table.Column<Guid>(type: "uuid", nullable: false),
                    SentenceId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentReports_Members_AccusedId",
                        column: x => x.AccusedId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentReports_Members_AccuserId",
                        column: x => x.AccuserId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentReports_Sentences_SentenceId",
                        column: x => x.SentenceId,
                        principalTable: "Sentences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_AccusedId",
                table: "IncidentReports",
                column: "AccusedId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_AccuserId",
                table: "IncidentReports",
                column: "AccuserId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_SentenceId",
                table: "IncidentReports",
                column: "SentenceId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberRole_RolesId",
                table: "MemberRole",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_DiscordUserId",
                table: "Members",
                column: "DiscordUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_DiscordRoleId",
                table: "Roles",
                column: "DiscordRoleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sentences_JudgeId",
                table: "Sentences",
                column: "JudgeId");

            migrationBuilder.CreateIndex(
                name: "IX_VerifyCodes_MemberId",
                table: "VerifyCodes",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentReports");

            migrationBuilder.DropTable(
                name: "MemberRole");

            migrationBuilder.DropTable(
                name: "VerifyCodes");

            migrationBuilder.DropTable(
                name: "Sentences");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
