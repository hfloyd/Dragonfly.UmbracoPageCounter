namespace Dragonfly.UmbracoPageCounter.Migrations
{
    using System.Linq;
    using Dragonfly.UmbracoPageCounter.Models;
    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;
    using Constants = Dragonfly.UmbracoPageCounter.Constants;

    [Migration("1.0.0", 1, Constants.ProductName)]
    public class CreatePageViewCounterTable : MigrationBase
    {
        //private readonly UmbracoDatabase _database = ApplicationContext.Current.DatabaseContext.Database;
        //private readonly DatabaseSchemaHelper _schemaHelper;

        public CreatePageViewCounterTable(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
            //_schemaHelper = new DatabaseSchemaHelper(_database, logger, sqlSyntax);
        }

        public override void Up()
        {
            var updated = false;
            var tables = SqlSyntax.GetTablesInSchema(Context.Database).ToList();
            //var constraints = SqlSyntax.GetConstraintsPerColumn(Context.Database).Distinct().ToArray();
            //var columns = SqlSyntax.GetColumnsInSchema(Context.Database).ToArray();

            if (tables.InvariantContains(PageViewCounter.SqlTableName) == false)
            {
                Create.Table<PageViewCounter>();
                updated = true;
            }

        }

        public override void Down()
        {
            Delete.Table(PageViewCounter.SqlTableName);
        }


    }
}
