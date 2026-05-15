using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingData.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "TableName",
                table: "AuditLogs",
                newName: "Details");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "AuditLogs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "AuditLogs",
                newName: "TableName");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "AuditLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordId",
                table: "AuditLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
