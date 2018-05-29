#addin "Cake.Incubator"
#addin "Cake.Docker"
#addin nuget:?package=Cake.Unity3D

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/AlmVR.Server/AlmVR.Server/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Build-Server")
    .IsDependentOn("Clean")
	.Does(() =>
{
	var dotNetCoreSettings = new DotNetCoreBuildSettings
	{
		Configuration = configuration
	};
	
	DotNetCoreBuild("./src/AlmVR.Server/AlmVR.Server.sln", dotNetCoreSettings);
});

Task("Publish-Server")
	.IsDependentOn("Build-Server")
	.Does(() =>
{
     var settings = new DotNetCorePublishSettings
     {
         Configuration = "Release",
         OutputDirectory = "./src/AlmVR.Server/artifacts/"
     };

     DotNetCorePublish("./src/AlmVR.Server/AlmVR.Server.sln", settings);
});

Task("Copy-Plugins")
	.IsDependentOn("Publish-Server")
	.Does(() =>
{
	CreateDirectory("./src/AlmVR.Server/artifacts/Plugins/");
	CopyFiles("./src/AlmVR.Server/build/Plugins/*", "./src/AlmVR.Server/artifacts/Plugins/");
});

Task("Docker-Build-Server")
	.IsDependentOn("Copy-Plugins")
	.Does(() =>
{
	var dockerImageBuildSettings = new DockerImageBuildSettings
	{
		Tag = new string[] { "almvr" }
	};

	DockerBuild(dockerImageBuildSettings, "./src/AlmVR.Server");
});

Task("Build-Client")
    .IsDependentOn("Clean")
	.Does(() =>
{
	var dotNetCoreSettings = new DotNetCoreBuildSettings
	{
		Configuration = configuration
	};
	
	DotNetCoreBuild("./src/AlmVR.Server/AlmVR.Server.sln", dotNetCoreSettings);
});

Task("Build-Unity")
	.IsDependentOn("Build-Client")
	.Does(() =>
{
	var unityEditorLocation = EnvironmentVariable("UNITY_EDITOR_LOCATION") ?? @"C:\Program Files\Unity\Editor\Unity.exe";
	
	Information($"Unity Editor Location: {unityEditorLocation}");
	
	// Presuming the build.cake file is within the Unity3D project folder.
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
	BuildUnity3DProject(projectPath, options);
});

Task("Build")
	.IsDependentOn("Docker-Build-Server")
	.IsDependentOn("Build-Client");
	//.IsDependentOn("Build-Unity");

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
