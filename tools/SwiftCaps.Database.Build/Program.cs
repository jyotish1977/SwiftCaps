using System.IO;
using static Bullseye.Targets;
using static SimpleExec.Command;

namespace build
{
    class Program
    {
        static void Main(string[] args)
        {
            const string PackOutput = "./artifacts";
            const string ProjectFile = "./Shared/SwiftCaps.Data";
            const string StartupProjectFile = "./Shared/SwiftCaps.EF.Support";
            const string DbContext = "SwiftCaps.Data.Context.SwiftCapsContext";
            const string CleanPackOutput = "clean-pack-output";
            const string Restore = "restore";
            const string Build = "build";
            const string Publish = "publish";
            const string Prefix = "SwiftCAPS DB";
            string sqlFileName = Path.Combine(PackOutput,"swiftcaps.sql");

            Target(Restore, () =>
            {
                Run("dotnet", $"restore --configfile Nuget.config \"{ProjectFile}\"", echoPrefix: Prefix);
            });

            Target(Build, DependsOn(Restore), () =>
            {
                Run("dotnet", $"build -c Release --nologo --no-restore \"{ProjectFile}\"", echoPrefix: Prefix);
            });


            Target(CleanPackOutput, () =>
            {
                if (Directory.Exists(PackOutput))
                {
                    Directory.Delete(PackOutput, true);
                }
            });

            Target(Publish, DependsOn(CleanPackOutput, Build), () =>
            {
                Run("dotnet", $"ef migrations script -p \"{ProjectFile}\" -s \"{StartupProjectFile}\" -i -c {DbContext} -o \"{sqlFileName}\" -v", echoPrefix: Prefix);
            });

            Target("default", DependsOn(Publish));

            RunTargetsAndExit(args, ex => ex is SimpleExec.NonZeroExitCodeException || ex.Message.EndsWith("Environment variables missing."), Prefix);
        }
    }
}
