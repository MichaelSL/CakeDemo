#tool "nuget:?package=xunit.runner.console"
#addin Cake.XdtTransform

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var slnFile = Argument("slnFile", "../CakeDemo.sln");
var customInfo = Argument<string>("customInfo", null);

var xUnitReportsPath = "./xUnitReports";
var nugetPackagerOutputDir = "./nuget";

Task("Init")
    .Does(() =>
{
    if (!String.IsNullOrEmpty(customInfo)){
        Information(customInfo);
    }
    CleanDirectories(new string[] {xUnitReportsPath, nugetPackagerOutputDir});
});

Task("NugetRestore")
    .IsDependentOn("Init")
    .Does(() =>
{
    NuGetRestore(slnFile);
});

Task("TransformXml")
    .Does(()=>
    {
        var sourceFile      = File("../CakeDemo.ConsoleHost/App.Default.config");
        var transformFile   = File("../CakeDemo.ConsoleHost/App." + configuration + ".config");
        var targetFile      = File("../CakeDemo.ConsoleHost/App.config");
        XdtTransformConfig(sourceFile, transformFile, targetFile);
    });

Task("Build")
    .IsDependentOn("NugetRestore")
    .IsDependentOn("TransformXml")
    .Does(() =>
{
    MSBuild(slnFile);
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    XUnit2("../**/bin/**/*.Tests.dll", new XUnit2Settings {
        HtmlReport = true,
        OutputDirectory = xUnitReportsPath
    });
});

Task("Nuget")
    .IsDependentOn("Test")
    .Does(() =>
{
    var nuGetPackSettings   = new NuGetPackSettings {
                                     Id                      = "CakeDemo.SharedContracts",
                                     Version                 = "0.0.0.1",
                                     Title                   = "Cake demo shared contracts",
                                     Authors                 = new[] {"Michael Samorokov"},
                                     Owners                  = new[] {"Michael Samorokov"},
                                     Description             = "Cake demo shared contracts",
                                     Summary                 = "The common types are packed in this package",
                                     ProjectUrl              = new Uri("https://github.com/SomeUser/TestNuget/"),
                                     IconUrl                 = new Uri("http://cdn.rawgit.com/SomeUser/TestNuget/master/icons/testnuget.png"),
                                     LicenseUrl              = new Uri("https://github.com/SomeUser/TestNuget/blob/master/LICENSE.md"),
                                     Copyright               = String.Format("Michael Samorokov © {0}", DateTime.Now.Year),
                                     ReleaseNotes            = new [] {"Bug fixes", "Issue fixes", "Typos"},
                                     Tags                    = new [] {"Cake", "Script", "Build"},
                                     RequireLicenseAcceptance= false,
                                     Symbols                 = false,
                                     NoPackageAnalysis       = true,
                                     OutputDirectory         = nugetPackagerOutputDir
                                 };

     NuGetPack("../CakeDemo.SharedContracts/CakeDemo.SharedContracts.csproj", nuGetPackSettings);
});

Task("Default")
    .IsDependentOn("Test")
    .Does(()=>
    {
        
    })
    .OnError(exception =>
    {
        Error(exception.Message);
        Information(@"Usage sample: .\build.ps1 -target ""Default"" -configuration ""Release"" -scriptArgs ""-customInfo='My text'""");
    });

RunTarget(target);