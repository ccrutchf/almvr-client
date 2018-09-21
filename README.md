# AlmVR (Google Daydream Client)
[![Build status](https://ci.appveyor.com/api/projects/status/3rxlny8b2sj9319w/branch/master?svg=true)](https://ci.appveyor.com/project/ccrutchf/almvr-client/branch/master)

A virtual reality (VR) application life cycle (ALM) management utility written for the Google Daydream and targeting Trello.

## Demo
[![AlmVR Demo](http://img.youtube.com/vi/dSCv77CD3rA/0.jpg)](http://www.youtube.com/watch?v=dSCv77CD3rA)

## What?
This repository contains the client component of AlmVR. The client connects to the server and other clients to expose a shared scrum experience.

## How?

The client is broken into two different sections, the client libraries and the app that runs on Google Daydream compatible devices.
The client libraries are written in `C#` and are `.NET Standard` libraries which must be compiled before launching `Unity` for the first time. The client libraries use dependency injection using `Autofac` to provide implementations of core providers which use `SignalR` to communicate in real time to the server.
The app that runs on Google Daydream compatible devices is written using `Unity` and depends on the client libraries.

### Communication with the server
The client uses web sockets implemented by `SignalR` for real time communication with the server. This allows the server to be able to push updates to the client without the client polling the server for updates, thus increasing efficiency.

### Communication with the other clients using Photon
The clients use `Photon` to communicate with one another to synchronize object states between the headsets (ie position and rotation of other players). 

### Builds
The client (as well as the remainder of AlmVR) is built using `Cake Build` run in `AppVeyor`. The build executes the following on every commit that is pushed to GitHub:
1. A clean of all of the build files.
2. Build the client libraries using the `dotnet` CLI. (Note: the version is generated implicitly using  [Nerdbank.GitVersioning](https://github.com/AArnott/Nerdbank.GitVersioning).)
3. Build the `Unity` project using Unity's command line interface. (Note: this is disabled due to a bug discussed later)
4. Package the result of step 3. (Note: this is disabled due to the bug noted in step 3.)

## Why?
The choice to have a separate set of client libraries from the application started as an artifact of the way that Unity handles `.NET Standard` libraries and `NuGet` packages.
Since `Unity` originally did not have support for `.NET Standard 2.0` libraries, which is how `SignalR` is distributed, the choice was made to hide the `.NET Standard` libraries behind a `.NET Framework 4.6.1` library, since it implements `.NET Standard 2.0`.
By pulling the client libraries out into their own project, we also allowed Visual Studio to manage the necessary `NuGet` packages for the project. This was necessary because `SignalR` is distributed as a `NuGet` package.
We choose to implement AlmVR's client using `Unity` because of its support for many different kinds of headsets and its support for the large number of `.NET` libraries. (See struggles for more on headset support and `.NET` library issues)

## Struggles
* Cross-platform - originally we had intended to support more than just the Google Daydream.  We attempted to leverage VRTK and found difficulties implementing necessary features.  As this project is largely a proof-of-concept, we decided to drop support for the following headset to focus on features:
  * Oculus Rift
  * HTC Vive
  * Microsoft Mixed Reality
* Unity's support of modern implementations of `.NET Framework` - when we began this project, `.NET API v4.x` support in Unity was experimental, but many of the tools that were widely used depended on having access to `.NET API v4.x`, including but not limited to `SignalR`.
* Incompatible versions of `Newtonsoft.Json` - Since Unity internally uses `.NET API v3.5`, `Photon` packages a version of `Newtonsoft.Json` compatible with `.NET API v3.5`.  Since `SignalR` is dependent on `.NET API v4.x`, it pulls in its own version of `Newtonsoft.Json` which leads to compile time failures.  It becomes necessary to configure the `Photon` version to be used by the editor and the `SignalR` version to be used by the player.
* Alpha versions of software - Since the technologies being used were cutting edge, we found ourselves using beta versions of `Unity`, `SignalR`, `.NET Core`, `ASP.NET Core`.  As the project progressed, some of the APIs that we depended on were changed.
* Automating Unity builds using AppVeyor - Since AppVeyor provisions a new machine for each build, it is necessary to install a new instance of Unity on the newly provisioned mahcine.  Since this new instance does not yet have a user credential associated with it, it is necessary to supply one.  Unity has a long standing bug preventing the username and password from being supplied through command line arguments.  As such, we attempted to use UI automation (using Autoit) to supply the username and password but found that servers provided by AppVeyor are headless.  We found that if we use RDP to log into the build server before the UI automation occurs, the builds successful pass.  We have commented out the build setp in hope that Unity address the bug in the future.
  * When builds of Unity are automated, it will be necessary to command line arguments supplied to Unity in the Cake Build script so that a Android application is created instead of a Windows application.

## How to build
The following software is required to build this project:
* Unity 2018.2
* Visual Studio 2017
* .NET Core 2.1 SDK
* PowerShell 5.1
* JDK 8 (due to a bug in Unity, as of 2018.2, it is not currently possible to use a version later than 8 to build)
* Android Studio (including the platform tools)
* AutoIt

Execute the `build.ps1` file found in the root of the repository.  This ensures the client libraries have been created.  It is important that this is done before `Unity` is opened.

