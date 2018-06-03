#addin "Cake.Incubator"
#addin "Cake.Docker"
#addin nuget:https://www.myget.org/F/alm-vr/api/v2?package=Cake.GitVersioning&prerelease
#addin "Cake.Powershell"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

dynamic version;

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var clientBuildDir = Directory("./src/client/build") + Directory(configuration);
var unityBuildDir = Directory("./build");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
	CleanDirectory(clientBuildDir);
    CleanDirectory(unityBuildDir);
});

Task("Git-Versioning")
	.Does(() =>
{
	version = GitVersioningGetVersion();

	Information($"Version number: \"{version.AssemblyInformationalVersion}\".");

	var script = @"
if (Get-Command ""Update-AppveyorBuild"" -errorAction SilentlyContinue)
{{
    Update-AppveyorBuild -Version {0}
}}";

	StartPowershellScript(string.Format(script, version.AssemblyInformationalVersion));
});

Task("Build-Client")
    .IsDependentOn("Clean")
	.Does(() =>
{
	var dotNetCoreSettings = new DotNetCoreBuildSettings
	{
		Configuration = configuration
	};
	
	DotNetCoreBuild("./src/client/AlmVR.Client.sln", dotNetCoreSettings);
});

Task("Build-Unity")
	.IsDependentOn("Git-Versioning")
	.IsDependentOn("Build-Client")
	.Does(() =>
{
	var unityEditorLocation = EnvironmentVariable("UNITY_EDITOR_LOCATION") ?? @"C:\Program Files\Unity\Editor\Unity.exe";
	
	Information($"Unity Editor Location: {unityEditorLocation}");
	
	/*// Presuming the build.cake file is within the Unity3D project folder.
	var projectPath = System.IO.Path.GetFullPath("./src/AlmVR.Headset");
	
	// The location we want the build application to go
	var outputPath = System.IO.Path.Combine(projectPath, "Build", "x64", "alm-vr.exe");
	
	// Create our build options.
	var options = new Unity3DBuildOptions()
	{
		Platform = Unity3DBuildPlatform.StandaloneWindows64,
		OutputPath = outputPath,
		UnityEditorLocation = unityEditorLocation,
		ForceScriptInstall = true,
		BuildVersion = "1.0.0"
	};
	
	// Perform the Unity3d build.
	BuildUnity3DProject(projectPath, options);*/
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build-Unity");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
