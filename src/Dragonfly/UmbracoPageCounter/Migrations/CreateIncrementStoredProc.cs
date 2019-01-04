namespace Dragonfly.UmbracoPageCounter.Migrations
{
    using Dragonfly.UmbracoPageCounter.Models;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;

    [Migration("1.0.0", 2, Constants.ProductName)]
    public class CreateIncrementStoredProc : MigrationBase
    {
        //private readonly UmbracoDatabase _database = ApplicationContext.Current.DatabaseContext.Database;
        //private readonly DatabaseSchemaHelper _schemaHelper;

        public CreateIncrementStoredProc(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
            //_schemaHelper = new DatabaseSchemaHelper(_database, logger, sqlSyntax);
        }

        public override void Up()
        {
            var pvcTableName = PageViewCounter.SqlTableName;
            var nodeIdCol = "NodeId";
            var counterCol = "Counter";
            var lastVisitCol = "LastVisit";

            var storedProcSql = $@"
                    SET ANSI_NULLS ON
                    GO

                    SET QUOTED_IDENTIFIER ON
                    GO

                    CREATE PROCEDURE [dbo].[{Constants.IncrementStoredProcName}]
                    (
                        @@node_id INT
                    )
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        IF EXISTS(SELECT 1 FROM [dbo].[{pvcTableName}] WHERE [{nodeIdCol}] = @@node_id)
                        BEGIN
                            UPDATE [dbo].[{pvcTableName}] 
                            SET [{counterCol}] = [{counterCol}] + 1, [{lastVisitCol}] = GETDATE()
                            WHERE [{nodeIdCol}] = @@node_id
                        END
                        ELSE
                        BEGIN
                            INSERT INTO [dbo].[{pvcTableName}]([{nodeIdCol}], [{counterCol}],[{lastVisitCol}] ) VALUES (@@node_id, 1, GETDATE())
                        END
                    END
                    GO
                ";

            Execute.Sql(storedProcSql);

        }

        public override void Down()
        {
           //TODO: SQL to Delete StoredProc UmbracoPageCounter.Constants.IncrementStoredProcName
        }


    }
}
