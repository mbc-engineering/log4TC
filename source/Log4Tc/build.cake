#tool "nuget:?package=xunit.runner.console"
// Importand: Execute Set-ExecutionPolicy RemoteSigned and Set-ExecutionPolicy RemoteSigned -Scope Process as Administrator in x86 and x64 powershell!

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var testreportfolder = Argument("testreportfolder", "testresult").TrimEnd('/');


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

    Information($"Clean Output Folders of configuration='{configuration}'");
    var directoriesToClean = GetDirectories($"./**/bin/{configuration}");
    CleanDirectories(directoriesToClean);

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
    .IsDependentOn("Build")
    .Does(() =>
{
    var testAssemblies = GetFiles($"./**/bin/{configuration}/**/*.test.dll");
    var xunitSettings = new XUnit2Settings {
        Parallelism = ParallelismOption.Assemblies,
        UseX86 = false,
        HtmlReport = true,
        JUnitReport = true,
        NoAppDomain = true,
        OutputDirectory = $"./{testreportfolder}",
    };     
    
    // Run Tests in x64 Process
    XUnit2(testAssemblies, xunitSettings); 

    /* x86 Not required in NSP 
    // Run Tests in x86 Process
    xunitSettings.UseX86 = true;
    xunitSettings.OutputDirectory += "x86";
    XUnit2(testAssemblies, xunitSettings); 
    */
});

Task("Publish")
    .IsDependentOn("Clean")
    // .IsDependentOn("Test") // keine Tests vorhanden
    .Does(() =>
{
    Information($"Publish service for all runtimes");
    DotNetCorePublish("Mbc.Log4Tc.Service/Mbc.Log4Tc.Service.csproj", new DotNetCorePublishSettings()
    {
        Configuration = configuration,
    });
});

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Publish");

RunTarget(target);