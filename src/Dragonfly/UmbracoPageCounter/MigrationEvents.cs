namespace Dragonfly.UmbracoPageCounter
{
    using System;
    using System.Linq;
    using Semver;
    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Web;

    public class MigrationEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            HandlePageCounterMigration(GetCurrentVersionFromMigrationService());
        }

        /// <summary>
        /// Standard Migrations Code, with the addition of an optional param
        /// to allow for calling the installation code directly for error handling/debugging purposes.
        /// </summary>
        /// <param name="CurrentVersion">Version to use to override current version as recorded in Migrations Table.</param>
        internal static void HandlePageCounterMigration(SemVersion CurrentVersion)
        {
            //Update this to trigger new migrations to run
            var targetVersion = new SemVersion(1, 0, 0);

            LogHelper.Debug<MigrationEvents>($"HandlePageCounterMigration running for {Constants.ProductName} version {targetVersion}...");

            //Initialize variable (should always be 0,0,0)
            // var currentVersion = new SemVersion(0, 0, 0);
            var currentVersion = CurrentVersion;

            LogHelper.Debug<MigrationEvents>($"HandlePageCounterMigration: {Constants.ProductName} current version is {currentVersion}...");

            if (targetVersion == currentVersion)
            {
                LogHelper.Debug<MigrationEvents>($"HandlePageCounterMigration: {Constants.ProductName} is up-to-date. No need to update.");
                return;
            }

            LogHelper.Info<MigrationEvents>($"HandlePageCounterMigration: {Constants.ProductName} current version is {currentVersion} and target version is {targetVersion}. Updating...");

            var migrationsRunner = new MigrationRunner(
                ApplicationContext.Current.Services.MigrationEntryService,
                ApplicationContext.Current.ProfilingLogger.Logger,
                currentVersion,
                targetVersion,
                Constants.ProductName);

            try
            {
                migrationsRunner.Execute(UmbracoContext.Current.Application.DatabaseContext.Database);
                LogHelper.Info<MigrationEvents>($"HandlePageCounterMigration: {Constants.ProductName} migrations run. ");
            }
            catch (Exception e)
            {
                LogHelper.Error<MigrationEvents>($"Error running {Constants.ProductName} migration", e);
            }
        }

        internal static SemVersion GetCurrentVersionFromMigrationService()
        {
            //Initialize variable (should always be 0,0,0)
            var currentVersion = new SemVersion(0, 0, 0);

            // get all migrations already executed
            var migrations = ApplicationContext.Current.Services.MigrationEntryService.GetAll(Constants.ProductName);

            // get the latest migration executed
            var latestMigration = migrations.OrderByDescending(x => x.Version).FirstOrDefault();

            if (latestMigration != null)
                currentVersion = latestMigration.Version;

            return currentVersion;
        }
    }
}

