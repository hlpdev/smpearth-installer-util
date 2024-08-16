using System.IO.Compression;
using System.Runtime.InteropServices;
using NLog;
using NLog.Config;
using NLog.Targets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nigle;

public static partial class Program {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    [LibraryImport("msvcrt.dll", EntryPoint = "system", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int System(string format);

    public static async Task Main(string[] args) {
        try {

            { // Setup logging
                LoggingConfiguration config = new LoggingConfiguration();
            
                FileTarget logFile = new FileTarget("logfile") { FileName = "nigle-latest.log" };
                ColoredConsoleTarget logConsole = new ColoredConsoleTarget("logconsole");
            
                config.AddRule(LogLevel.Info, LogLevel.Fatal, logConsole);
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, logFile);
            
                LogManager.Configuration = config;
                
                Logger.Debug("Start of Application");
                Logger.Debug("Initialized logger");
            }

            { // Install forge
                Logger.Info("Beginning forge installation...");
                
                FileInfo file = new FileInfo("forge_installer.jar");

                if (!file.Exists) {
                    throw new FileNotFoundException("The forge installer could not be found!");
                }

                System($"java -jar \"{file.FullName}\"");
                
                Logger.Info("Finished forge installation");
            }

            { // Create forge profile
                Logger.Info("Creating launcher profile...");
                
                FileInfo file = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.minecraft\launcher_profiles.json");

                if (!file.Exists) {
                    throw new FileNotFoundException(
                        "The launcher_profiles.json file was not found in your .minecraft folder!");
                }

                JObject launcherProfiles = JObject.Parse(await File.ReadAllTextAsync(file.FullName))!;

                JObject newProfile = new JObject {
                    ["gameDir"] = "C:\\HNT8\\SMPEarth",
                    ["icon"] =
                        "data:image/png;base64,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",
                    ["javaArgs"] =
                        "-Xmx8G -XX:+UnlockExperimentalVMOptions -XX:+UseG1GC -XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 -XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=32M",
                    ["lastUsed"] = "2024-08-15T22:33:59.635Z",
                    ["lastVersionId"] = "1.19.2-forge-43.4.0",
                    ["name"] = "Nigle\u0027s SMP Earth",
                    ["type"] = "custom"
                };

                launcherProfiles["profiles"]!["nigle"] = newProfile;

                await File.WriteAllTextAsync(file.FullName, launcherProfiles.ToString());
                
                Logger.Info("Created launcher profile");
            }

            { // Copy custom .minecraft over to HNT8 dir
                Logger.Info("Writing modpack contents to disk");

                if (Directory.Exists("C:\\HNT8\\SMPEarth")) {
                    Directory.Delete("C:\\HNT8\\SMPEarth");
                }
                
                Directory.CreateDirectory("C:\\HNT8\\SMPEarth");
                File.Copy("SMPEarth.zip", "C:\\HNT8\\SMPEarth\\contents.zip");
                ZipFile.ExtractToDirectory("C:\\HNT8\\SMPEarth\\contents.zip", "C:\\HNT8\\SMPEarth");
                
                Logger.Info("Successfully wrote modpack contents to disk");
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Modpack installation was successful!");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

        } catch (Exception e) {
            Logger.Error(e, "An error occured during installation");
            throw;
        }
    }
}