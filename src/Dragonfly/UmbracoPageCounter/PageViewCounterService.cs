namespace Dragonfly.UmbracoPageCounter
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Web.Configuration;
    using Dragonfly.UmbracoPageCounter.Models;
    using Semver;
    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;

    public static class PageViewCounterService
    {
        //private UmbracoDatabase db = ApplicationContext.Current.DatabaseContext.Database;

        /// <summary>
        /// Updates the Database View count and LastVisit date for a provided Node Id
        /// </summary>
        /// <param name="nodeId"></param>
        public static void IncrementPageCount(int nodeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["umbracoDbDSN"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(UmbracoPageCounter.Constants.IncrementStoredProcName, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("node_id", System.Data.SqlDbType.Int).Value = nodeId;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {
                if (sqlEx.Message.Contains("Could not find stored procedure 'Dragonfly_SP_IncrementPageViewCounter'"))
                {
                    LogHelper.Warn<PageViewCounter>($"IncrementPageCount() Error encountered. - Attempting to install StoredProc.");
                    //try re-installing sp via Migrations
                    var currentVersion = new SemVersion(0, 0, 0);
                    MigrationEvents.HandlePageCounterMigration(currentVersion);
                }
                else
                {
                    LogHelper.Error<PageViewCounter>($"IncrementPageCount() Error encountered.", sqlEx);
                    throw;
                }
            }
            catch (Exception e)
            {
                LogHelper.Error<PageViewCounter>($"IncrementPageCount() Error encountered.", e);
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific PageViewCounter for a provided NodeId from the database
        /// NOTE: Continually hitting the database to fetch this data can affect performance.
        /// Consider using GetAllPageCounters() and caching data
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static PageViewCounter GetPageCounter(int Id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;

            var counter = db.SingleOrDefault<PageViewCounter>(Id);

            return counter;
        }

        /// <summary>
        /// Returns all PageView data ordered with most visited first
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PageViewCounter> GetAllPageCounters()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            var db = dbContext.Database;
            var sqlSyntax = dbContext.SqlSyntax;

            var sql = new Sql()
                .Select("*")
                .From<PageViewCounter>(sqlSyntax)
                .OrderByDescending("[Counter]");

            var counter = db.Query<PageViewCounter>(sql);

            return counter;
        }


    }
}
