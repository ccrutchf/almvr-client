#addin "Cake.Incubator"
#addin "Cake.Docker"
#addin nuget:https://www.myget.org/F/alm-vr/api/v2?package=Cake.GitVersioning&prerelease
#addin "Cake.Powershell"
#addin "Cake.FileHelpers"

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
var unityBuildDir = Directory("./build") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
	CleanDirectory(clientBuildDir);
    CleanDirectory(unityBuildDir);

	if (FileExists("./almvr.zip"))
		DeleteFile("./almvr.zip");
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
	.WithCriteria(!string.IsNullOrWhiteSpace(EnvironmentVariable("UNITY_EMAIL")))
	.WithCriteria(!string.IsNullOrWhiteSpace(EnvironmentVariable("UNITY_PASSWORD")))
	.IsDependentOn("Git-Versioning")
	.IsDependentOn("Build-Client")
	.Does(() =>
{
	var unityEmail = EnvironmentVariable("UNITY_EMAIL");
	var unityPassword = EnvironmentVariable("UNITY_PASSWORD");

	var unityEditorLocation = EnvironmentVariable("UNITY_EDITOR_LOCATION") ?? @"C:\Program Files\Unity\Editor\Unity.exe";
	Information($"Unity Editor Location: {unityEditorLocation}");

	var projectPath = System.IO.Path.GetFullPath("./src/headset");
	Information($"Unity Project Path: {projectPath}");

	var outPath = System.IO.Path.GetFullPath((string)unityBuildDir) + "\\AlmVR.exe";
	Information($"Unity Out Path: {outPath}");

	var logPath = System.IO.Path.GetFullPath(".\\unity.log");
	Information($"Unity Log Path: {logPath}");
	
	var settings = new ProcessSettings
	{
		Arguments = 
			"-quit " +
			"-batchmode " + 
			$"-projectpath \"{projectPath}\" " +
			$"-buildWindows64Player \"{outPath}\" " +
			"-nographics " +
			$"-log \"{logPath}\" " + 
			$"-username {unityEmail} " +
			$"-password {unityPassword}",

		RedirectStandardError = true,
		RedirectStandardOutput = true
	};

	IEnumerable<string> redirectedStandardOutput;
	IEnumerable<string> redirectedErrorOutput;
	int exitCode = StartProcess(
		unityEditorLocation, 
		settings,
		out redirectedStandardOutput,
		out redirectedErrorOutput
	);

	Information(FileReadText(logPath));

	foreach (var line in redirectedStandardOutput)
	{
		Information(line);
	}

	// Throw exception if anything was written to the standard error.
	if (redirectedErrorOutput.Any())
	{
		throw new Exception(
			string.Format(
				"Errors ocurred: {0}",
				string.Join(", ", redirectedErrorOutput)
			)
		);
	}

	if (exitCode != 0)
		throw new Exception($"Unity returned a non-zero exit code: \"{exitCode}\".");

	if (!FileExists(unityBuildDir + File("AlmVR.exe")))
		throw new Exception("Expected build output not created");
});

Task("Package")
	.IsDependentOn("Build-Unity")
	.WithCriteria(FileExists(unityBuildDir + File("AlmVR.exe")))
	.Does(() =>
{
	Zip(unityBuildDir, "./almvr.zip");
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
