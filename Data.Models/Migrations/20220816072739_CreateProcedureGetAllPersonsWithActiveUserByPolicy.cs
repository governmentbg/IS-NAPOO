using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateProcedureGetAllPersonsWithActiveUserByPolicy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createProcSql = @"EXEC('CREATE OR ALTER PROC GetAllPersonsWithActiveUserByPolicy( @ClaimType nvarchar(255), @UserStatus nvarchar(255) ) AS SELECT DISTINCT  Person.* FROM Person INNER JOIN AspNetUsers ON Person.IdPerson = AspNetUsers.IdPerson INNER JOIN AspNetUserRoles ON AspNetUsers.Id = AspNetUserRoles.UserId INNER JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id INNER JOIN AspNetRoleClaims ON AspNetRoles.Id = AspNetRoleClaims.RoleId INNER JOIN  KeyValue ON AspNetUsers.IdUserStatus = KeyValue.IdKeyValue where AspNetRoleClaims.ClaimType = @ClaimType and KeyValue.KeyValueIntCode = @UserStatus')";
            migrationBuilder.Sql(createProcSql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropProcSql = "DROP PROC GetAllPersonsWithActiveUserByPolicy";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}
