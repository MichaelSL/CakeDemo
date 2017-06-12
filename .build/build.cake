#tool "nuget:?package=xunit.runner.console"

var target = Argument("target", "Default");
var slnFile = Argument("slnFile", "../CakeDemo.sln");

var xUnitReportsPath = "./xUnitReports";
var nugetPackagerOutputDir = "./nuget";

Task("Init")
    .Does(() =>
{
    CleanDirectories(new string[] {xUnitReportsPath, nugetPackagerOutputDir});
});

Task("NugetRestore")
    .IsDependentOn("Init")
    .Does(() =>
{
    NuGetRestore(slnFile);
});

Task("Build")
    .IsDependentOn("NugetRestore")
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
    .IsDependentOn("Test");

RunTarget(target);