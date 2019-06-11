using System;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Utilities.Caching.Database.Migrations
{
    public partial class InitialCreate : Migration
    {
        public static bool DoesFieldExist(string connectionString, string tableName, string fieldName)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
            }
            catch (Exception)
            {
                return false;
            }
            string SQL = "SELECT top 1 [" + fieldName + "] FROM [" + tableName + "]";
            SqlCommand cmd = new SqlCommand(SQL, conn);
            try
            {
                var i = cmd.ExecuteScalar();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        private bool DoesInitialExist()
        {

            //var r = this.Resources;
            return DoesFieldExist(Core.ConnString, "CachedEntries", "Name");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
            name: "dbo");

            if (!DoesInitialExist())
            {

                migrationBuilder.CreateTable(
                    name: "CachedEntries",
                    schema: "dbo",
                    columns: table => new
                    {
                        Id = table.Column<int>(nullable: false)
                            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                        Name = table.Column<string>(nullable: true),
                        Object = table.Column<string>(nullable: true),
                        Created = table.Column<DateTime>(nullable: false),
                        Changed = table.Column<DateTime>(nullable: false),
                        TimeOut = table.Column<DateTime>(nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_CachedEntries", x => x.Id);
                    });
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CachedEntries",
                schema: "dbo");
        }
    }
}
