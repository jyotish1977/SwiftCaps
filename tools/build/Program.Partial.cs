using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using static Bullseye.Targets;
using static SimpleExec.Command;

namespace build
{
    partial class Program
    {
        private const string packOutput = "./artifacts";
        private const string packOutputArchive = "./archives";
        private const string envVarMissing = " environment variable is missing. Aborting.";

        private static class Targets
        {
            public const string CleanBuildOutput = "clean-build-output";
            public const string CleanPackOutput = "clean-pack-output";
            public const string Restore = "restore";
            public const string Build = "build";
            public const string Test = "test";
            public const string Publish = "publish";
            public const string Pack = "pack";
            public const string CopyPackOutput = "copy-pack-output";

            public const string Zip = "zip";
        }

        static void Main(string[] args)
        {

            var projects = ProjectFiles.ToList();
            var testProjects = TestProjFiles.ToList();

            Target(Targets.CleanBuildOutput, () =>
            {
                //Run("dotnet", "clean -c Release -v m --nologo", echoPrefix: Prefix);
            });

            Target(Targets.Restore, DependsOn(Targets.CleanBuildOutput), () =>
            {
                projects.ForEach(project => Run("dotnet", $"restore --configfile Nuget.config \"{project}\"", echoPrefix: Prefix));
                testProjects.ForEach(project => Run("dotnet", $"restore --configfile Nuget.config \"{project}\"", echoPrefix: Prefix));
            });

            Target(Targets.Build, DependsOn(Targets.Restore), () =>
            {
                projects.ForEach(project => Run("dotnet", $"build -c Release --nologo --no-restore \"{project}\"", echoPrefix: Prefix));
                testProjects.ForEach(project => Run("dotnet", $"build -c Release --nologo --no-restore \"{project}\"", echoPrefix: Prefix));
            });

            Target(Targets.Test, DependsOn(Targets.Build), () =>
            {
                var buildSourcesDirectory = Environment.GetEnvironmentVariable("SYSTEM_DEFAULTWORKINGDIRECTORY");
                var coverageSummaryFolder = (string.IsNullOrEmpty(buildSourcesDirectory) ? "." : buildSourcesDirectory) + "/Coverage/";
                Console.WriteLine($"coverageSummaryFolder:{coverageSummaryFolder}");
                testProjects.ForEach(project => Run("dotnet", $"test \"{project}\" -c Release --logger trx --results-directory ./TestResults/ /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput={coverageSummaryFolder} --no-build", echoPrefix: Prefix));
            });

            Target(Targets.CleanPackOutput, () =>
            {
                if (Directory.Exists(packOutput))
                {
                    Directory.Delete(packOutput, true);
                }
                if(Directory.Exists(packOutputArchive))
                {
                    Directory.Delete(packOutputArchive, true);
                }
            });

            Target(Targets.Publish, DependsOn(Targets.Test, Targets.CleanPackOutput), () =>
            {
                projects.ForEach(project =>Run("dotnet", $"publish \"{project}\" -c Release --no-build  -o \"{Directory.CreateDirectory(packOutput).FullName}\"", echoPrefix: Prefix));
            });

            Target(Targets.Zip, DependsOn(Targets.Publish), () =>
            {
                var archiveFolder = Directory.CreateDirectory(packOutputArchive).FullName;

                var artifactsFolder = new DirectoryInfo(packOutput).FullName;
                Console.WriteLine($"artifactsFolder:{artifactsFolder}");
                
                var destination = Path.Combine(archiveFolder,$"{Prefix}.zip");
                Console.WriteLine($"destination:{destination}");
                Console.WriteLine("Zipping artifacts...");
                ZipFile.CreateFromDirectory(artifactsFolder,destination);
                Console.WriteLine("Finished zipping artifacts...");
            });

            Target("default", DependsOn(Targets.Zip));

            RunTargetsAndExit(args, ex => ex is SimpleExec.NonZeroExitCodeException || ex.Message.EndsWith(envVarMissing), Prefix);
        }
    }
}
