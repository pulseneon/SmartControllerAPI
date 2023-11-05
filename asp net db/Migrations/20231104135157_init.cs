using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace asp_net_db.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConnectionsInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TotalConnections = table.Column<int>(type: "integer", nullable: false),
                    NonIdleConnections = table.Column<int>(type: "integer", nullable: false),
                    MaxConnections = table.Column<string>(type: "text", nullable: false),
                    ConnectionsUtilization = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionsInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaxLoadingProcessor = table.Column<int>(type: "integer", nullable: false),
                    DatabaseSizePercent = table.Column<int>(type: "integer", nullable: false),
                    MaxConnectionsPercent = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServerId = table.Column<int>(type: "integer", nullable: false),
                    ProcessorPercentLoading = table.Column<double>(type: "double precision", nullable: false),
                    DatabaseSize = table.Column<string>(type: "text", nullable: false),
                    ConnectionInfoId = table.Column<int>(type: "integer", nullable: false),
                    WritedAt = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerStats_ConnectionsInfo_ConnectionInfoId",
                        column: x => x.ConnectionInfoId,
                        principalTable: "ConnectionsInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayedName = table.Column<string>(type: "text", nullable: false),
                    Host = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<string>(type: "text", nullable: false),
                    DbName = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    AllocatedSpace = table.Column<int>(type: "integer", nullable: false),
                    UseSSH = table.Column<bool>(type: "boolean", nullable: false),
                    ServerOS = table.Column<string>(type: "text", nullable: true),
                    PortSSH = table.Column<int>(type: "integer", nullable: true),
                    HostnameSSH = table.Column<string>(type: "text", nullable: true),
                    UsernameSSH = table.Column<string>(type: "text", nullable: true),
                    PasswordSSH = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    SettingsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servers_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Connection",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Interval = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Pid = table.Column<int>(type: "integer", nullable: false),
                    Datname = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    Query = table.Column<string>(type: "text", nullable: false),
                    ServerStatsId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connection", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Connection_ServerStats_ServerStatsId",
                        column: x => x.ServerStatsId,
                        principalTable: "ServerStats",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Connection_ServerStatsId",
                table: "Connection",
                column: "ServerStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_SettingsId",
                table: "Servers",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerStats_ConnectionInfoId",
                table: "ServerStats",
                column: "ConnectionInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Connection");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropTable(
                name: "ServerStats");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "ConnectionsInfo");
        }
    }
}
