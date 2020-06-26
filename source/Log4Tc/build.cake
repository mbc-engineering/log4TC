// Importand: Execute Set-ExecutionPolicy RemoteSigned and Set-ExecutionPolicy RemoteSigned -Scope Process as Administrator in x86 and x64 powershell!

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var testreportfolder = Argument("testreportfolder", "testresult").TrimEnd('/');
var testresultsfile =  Argument("testresultsfile", "testresults.trx");


///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
  .Does(() =>
{
    DotNetCoreClean("Log4Tc.sln", new DotNetCoreCleanSettings()
    {
        Configuration = configuration,
    });
    
    Information($"Clean Output folders of configuration='{configuration}'");
    var directoriesToClean = GetDirectories($"./**/bin/{configuration}/**/publish");
    DeleteDirectories(directoriesToClean, new DeleteDirectorySettings() { Recursive = true, Force = true });
    var pluginDirectoriesToClean = GetDirectories($"./**/bin/{configuration}/plugins");
    DeleteDirectories(pluginDirectoriesToClean, new DeleteDirectorySettings() { Recursive = true, Force = true });

    Information($"Clean docs output folder");
    DeleteDirectories(GetDirectories($"./../../docs/_site"), new DeleteDirectorySettings() { Recursive = true, Force = true });
});

Task("Build")
  .IsDependentOn("Clean")
  .Does(() =>
{        
    var solutions  = GetFiles("./**/*.sln");
    foreach (var solution in solutions)
    {
        Information($"Build Solution: {solution}");
        MSBuild(solution, configurator =>
            configurator                
                .SetConfiguration(configuration)
                .WithRestore()  
                .SetVerbosity(Verbosity.Minimal)                
                .UseToolVersion(MSBuildToolVersion.VS2019)
                .SetMSBuildPlatform(MSBuildPlatform.x86)    // x86 Required for WIX
                .SetPlatformTarget(PlatformTarget.MSIL));   // MSIL = AnyCPU
    }
});

Task("Test")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings
     {
         Configuration = configuration,
         Logger = $"trx;LogFileName=\"{testresultsfile}\"",  // by default results in Mbc.Log4Tc.Output.NLog.Test\TestResults\testresults.trx
         // ArgumentCustomization = args=>args.Append("--collect:\"XPlat Code Coverage\""),        // code coverage output for package coverlet.collector
     };

     var projectFiles = GetFiles("./**/*.Test.csproj");

     foreach(var file in projectFiles)
     {
         DotNetCoreTest(file.FullPath, settings);
     }
});

Task("SmokeTest")
    .IsDependentOn("Test")
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings
     {
         Configuration = configuration,
         Logger = $"trx;LogFileName=\"{testresultsfile}\"",  // by default results in Mbc.Log4Tc.Output.NLog.Test\TestResults\testresults.trx
         // ArgumentCustomization = args=>args.Append("--collect:\"XPlat Code Coverage\""),        // code coverage output for package coverlet.collector
     };

     var projectFiles = GetFiles("./**/*.SmokeTest.csproj");

     foreach(var file in projectFiles)
     {
         DotNetCoreTest(file.FullPath, settings);
     }
});

Task("Publish")
    .IsDependentOn("Clean")
    .IsDependentOn("Test")
    .Does(() =>
{
    Information($"Publish service for windows runtime");
    DotNetCorePublish("Mbc.Log4Tc.Service/Mbc.Log4Tc.Service.csproj", new DotNetCorePublishSettings()
    {
        Configuration = configuration,
    });

    Information("Create docfx docs html content");
    var settings = new DotNetCoreBuildSettings
     {
         Configuration = configuration,
     };
    DotNetCoreBuild("./../../docs/", settings);
    
    Information("Create the setup for published service");
    foreach (var platform in new [] { PlatformTarget.x64, PlatformTarget.x86 })
    {
        Information($"MSI {platform.ToString()}");
        MSBuild("Log4Tc.sln", configurator =>
            configurator                
                .SetConfiguration(configuration)
                .SetPlatformTarget(platform)
                .WithRestore()
                .SetVerbosity(Verbosity.Normal)                
                .UseToolVersion(MSBuildToolVersion.VS2019)
                .SetMSBuildPlatform(MSBuildPlatform.x86));    // x86 Required for WIX
    }
});

Task("Default")
    .IsDependentOn("Test")
    .IsDependentOn("Publish");

RunTarget(target);