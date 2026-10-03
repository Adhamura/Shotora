var runtimeArg = Argument("runtime", "linux-arm64");
var configuration = Argument("configuration", "Release");
var singleFile = Argument("singlefile", false);
var selfContained = Argument("selfcontained", false);
var outputArg = Argument("output", "./ready");
var channel = Argument("channel", "stable");
var useVp = Argument("usevp", Argument("useVp", false));
var versionArg = Argument("appversion", string.Empty); // "--version" is reserved by Cake itself

var ridMap = new Dictionary<string, (string os, string arch, string dest)>
{
    { "linux-arm64", (os: "Linux", arch: "arm64", dest: "x64") },
    { "linux-x64",   (os: "Linux", arch: "x64",   dest: "x64") },
    { "win-x64",     (os: "Windows", arch: "x64", dest: "x64") },
    { "win-x86",     (os: "Windows", arch: "x86", dest: "x86") },
    { "osx-x64",     (os: "Mac", arch: "x64",     dest: "x64") },
    { "osx-arm64",   (os: "Mac", arch: "arm64",   dest: "x64") }
};

if (!ridMap.ContainsKey(runtimeArg))
{
    throw new Exception($"Unknown runtime '{runtimeArg}'. Valid: {string.Join(", ", ridMap.Keys)}");
}

Task("Publish")
    .Does(() =>
{
    var ridInfo = ridMap[runtimeArg];
    var baseDir = MakeAbsolute(Directory(outputArg));
    EnsureDirectoryExists(baseDir);

    var nativeObj = MakeAbsolute(Directory("./NativeSupport/obj"));
    if (System.IO.Directory.Exists(nativeObj.FullPath))
    {
        Information($"Cleaning NativeSupport obj to avoid stale locks: {nativeObj}");
        CleanDirectory(nativeObj);
    }


    if (System.IO.Directory.Exists(baseDir.FullPath))
    {
        CleanDirectory(baseDir);
    }

    Information($"Publishing {runtimeArg} -> {baseDir} (single-file:{singleFile}, self-contained:{selfContained})...");

    var msbuildSettings = new DotNetMSBuildSettings()
        .WithProperty("EnableCompressionInSingleFile", singleFile ? "true" : "false")
        .WithProperty("InvariantGlobalization", "true")
        .WithProperty("DebugType", "None")
        .WithProperty("DebugSymbols", "false");

    if (!string.IsNullOrWhiteSpace(versionArg))
    {
        // Stamps every assembly so the app reports the release version (used by the in-app updater).
        msbuildSettings = msbuildSettings.WithProperty("Version", versionArg);
    }

    DotNetPublish("./Shotora.App/Shotora.App.csproj", new DotNetPublishSettings {
        Configuration = configuration,
        Runtime = runtimeArg,
        SelfContained = selfContained,
        PublishSingleFile = singleFile,
        OutputDirectory = baseDir,
        MSBuildSettings = msbuildSettings
    });

    var outputName = runtimeArg.StartsWith("win")
        ? "Shotora.App.exe"
        : "Shotora.App";

    if (singleFile)
    {

        var keepCandidates = System.IO.Directory.GetFiles(baseDir.FullPath, "Shotora.App*");
        if (keepCandidates.Length == 0)
        {
            keepCandidates = System.IO.Directory.GetFiles(baseDir.FullPath);
        }

        if (keepCandidates.Length == 0)
        {
            throw new Exception($"Single-file publish produced no output at {baseDir}");
        }

        var keepFile = keepCandidates.OrderByDescending(f => new System.IO.FileInfo(f).Length).First();
        Information($"Single-file publish cleanup: keeping {keepFile}");

        foreach (var file in System.IO.Directory.GetFiles(baseDir.FullPath))
        {
            if (!file.Equals(keepFile, StringComparison.OrdinalIgnoreCase))
            {
                System.IO.File.Delete(file);
            }
        }

        foreach (var dir in System.IO.Directory.GetDirectories(baseDir.FullPath))
        {
            System.IO.Directory.Delete(dir, recursive: true);
        }
    }
    else
    {

        var destArchDir = baseDir.Combine(Directory(ridInfo.dest));
        EnsureDirectoryExists(destArchDir);

        var nativeSource = MakeAbsolute(Directory($"./NativeSupport/Binaries/{ridInfo.os}/{ridInfo.arch}"));
        if (System.IO.Directory.Exists(nativeSource.FullPath))
        {
            CopyFiles(nativeSource.FullPath + "*", destArchDir);
        }

        var binariesDir = baseDir.Combine(Directory("Binaries"));
        if (System.IO.Directory.Exists(binariesDir.FullPath))
        {
            System.IO.Directory.Delete(binariesDir.FullPath, recursive: true);
        }
    }
});

Task("Pack")
    .IsDependentOn("Publish")
    .WithCriteria(useVp)
    .Does(() =>
{
    Information("Velopack pack requested (useVp=true); starting pack script...");
    var baseDir = MakeAbsolute(Directory(outputArg));
    EnsureDirectoryExists(baseDir);

    var scriptPath = runtimeArg.StartsWith("osx", StringComparison.OrdinalIgnoreCase) ||
                     runtimeArg.StartsWith("mac", StringComparison.OrdinalIgnoreCase)
        ? "./build/pack-macos.ps1"
        : "./build/pack-setup.ps1";

    var mainExe = runtimeArg.StartsWith("win", StringComparison.OrdinalIgnoreCase) ? "Shotora.App.exe" : "Shotora.App";
    var shell = IsRunningOnWindows() ? "powershell" : "pwsh";
    var packOutput = baseDir.Combine(Directory("velopack"));
    EnsureDirectoryExists(packOutput);

    var args = new ProcessArgumentBuilder()
        .Append("-NoProfile")
        .Append("-ExecutionPolicy").Append("Bypass")
        .Append("-File").AppendQuoted(MakeAbsolute(File(scriptPath)).FullPath)
        .Append("-Runtime").Append(runtimeArg)
        .Append("-Channel").Append(channel)
        .Append("-PackDir").AppendQuoted(baseDir.FullPath)
        .Append("-OutputDir").AppendQuoted(packOutput.FullPath)
        .Append("-MainExe").Append(mainExe);

    Information($"Packing via {scriptPath} using publish output {baseDir} -> {packOutput}");

    var settings = new ProcessSettings { Arguments = args };
    var exitCode = StartProcess(shell, settings);
    if (exitCode != 0)
    {
        throw new Exception($"Velopack pack failed with exit code {exitCode}");
    }
});

if (useVp)
{
    RunTarget("Pack");
}
else
{
    RunTarget("Publish");
}