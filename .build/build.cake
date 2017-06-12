var target = Argument("target", "Default");

Task("Init")
    .Does(() =>
{
    
});

Task("Build")
    .IsDependentOn("Init")
    .Does(() =>
{
    
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    
});

Task("Nuget")
    .IsDependentOn("Test")
    .Does(() =>
{

});

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);