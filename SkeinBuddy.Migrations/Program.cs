using SkeinBuddy.Migrations.Migrations;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Logging;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace SkeinBuddy.Migrations;
class Program
{
    public static string DevEnvironmentName = "Dev";
    public static string QAEnvironmentName = "QA";
    public static string ProdEnvironmentName = "Prod";

    public static List<string> environmentNames = new List<string> { DevEnvironmentName, /*QAEnvironmentName, ProdEnvironmentName*/ };

    public static string MigrateUpOption = "Migrate Up";
    public static string RollbackOption = "Rollback";
    public static string ListMigrationsOption = "List Migrations";
    public static string ChangeEnvironmentOption = "Change Environment";
    public static string QuitOption = "Quit";

    public static List<string> migrationOptions = new List<string> { MigrateUpOption, RollbackOption, ListMigrationsOption, ChangeEnvironmentOption, QuitOption };

    public static Dictionary<string, Environment> environments = new Dictionary<string, Environment>
    {
        { DevEnvironmentName, new Environment { HostName = "100.64.129.86", DbName = "skein_buddy_dev", Port = 5344 } },
    };

    static void Main(string[] args)
    {
        bool quit = false;

        Environment? environment = null;
        string? username = null;
        string? password = null;

        while (!quit)
        {
            if (environment == null)
            {
                string environmentName = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an environment")
                        .PageSize(10)
                        .MoreChoicesText("Scroll down to see more environments")
                        .AddChoices(environmentNames)
                );

                environment = environments[environmentName];
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                username = AnsiConsole.Prompt(new TextPrompt<string>($"Enter the username for {environment.HostName}/{environment.DbName}: "));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                password = AnsiConsole.Prompt(new TextPrompt<string>($"Enter the password for {environment.HostName}/{environment.DbName}: ").Secret());
                AnsiConsole.WriteLine();
            }

            string selectedMigrationOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an option")
                    .PageSize(10)
                    .MoreChoicesText("Scroll down to see more options")
                    .AddChoices(migrationOptions)
            );

            using (var serviceProvider = CreateServices(environment, username, password))
            using (var scope = serviceProvider.CreateScope())
            {
                // Put the database update into a scope to ensure
                // that all resources will be disposed.


                if (selectedMigrationOption == MigrateUpOption)
                {
                    MigrateUp(scope.ServiceProvider);
                }
                else if (selectedMigrationOption == RollbackOption)
                {
                    Rollback(scope.ServiceProvider);
                }
                else if (selectedMigrationOption == ListMigrationsOption)
                {
                    ListMigrations(scope.ServiceProvider);
                }
                else if (selectedMigrationOption == ChangeEnvironmentOption)
                {
                    environment = null;
                    username = null;
                    password = null;
                    AnsiConsole.Clear();
                }
                else if (selectedMigrationOption == QuitOption)
                {
                    quit = true;
                }
            }
        }
    }

    /// <summary>
    /// Configure the dependency injection services
    /// </summary>
    private static ServiceProvider CreateServices(Environment environment, string username, string password)
    {
        return new ServiceCollection()
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                // Add Postgres support to FluentMigrator
                .AddPostgres()
                // Set the connection string
                .WithGlobalConnectionString($"host={environment.HostName};port={environment.Port};username={username};password={password};database={environment.DbName}")
                // Define the assembly containing the migrations
                .ScanIn(typeof(AddInitialTables).Assembly).For.Migrations())
            // Enable logging to console in the FluentMigrator way
            .AddLogging(lb => {
                lb.AddFluentMigratorConsole();
            })
            .AddSingleton<ILoggerProvider, LogFileFluentMigratorLoggerProvider>()
            .Configure<LogFileFluentMigratorLoggerOptions>(options =>
            {
                options.OutputFileName = "migration.log";
                options.OutputGoBetweenStatements = true;
                options.ShowSql = true;
            })
            // Build the service provider
            .BuildServiceProvider(false);
    }

    /// <summary>
    /// Execute all migrations up to the latest
    /// </summary>
    private static void MigrateUp(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        runner.MigrateUp();
        AnsiConsole.WriteLine();

        ListMigrations(serviceProvider);
    }

    /// <summary>
    /// Roll back by one migration
    /// </summary>
    private static void Rollback(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Rollback one step
        runner.Rollback(1);
        AnsiConsole.WriteLine();

        ListMigrations(serviceProvider);
    }

    /// <summary>
    /// List all the migrations in order and mark the current one in green
    /// </summary>
    private static void ListMigrations(IServiceProvider serviceProvider)
    {
        IMigrationRunner runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        IVersionLoader versionLoader = serviceProvider.GetRequiredService<IVersionLoader>();
        long latest = versionLoader.VersionInfo.Latest();

        // List the migrations
        var migrations = runner.MigrationLoader.LoadMigrations();

        AnsiConsole.WriteLine();

        foreach (var migration in migrations)
        {
            bool isLatest = migration.Value.Version == latest;
            string fontColor = isLatest ? "green" : "white";

            AnsiConsole.MarkupLine($"[bold {fontColor}]{migration.Value.GetName()} {(isLatest ? "(current)" : "")}[/]");
        }
        AnsiConsole.WriteLine();
    }
}