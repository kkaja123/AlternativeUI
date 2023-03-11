# AlternativeUI (AUI)
## An Alternative UI mod for KSP 2
This mod for KSP 2 provides a set of alternative UI features and layouts compared to the stock UI.

This may include replacing UI assets, repositioning UI elements, adding UI assets that may enhance the existing UI, bugfixes, and custom UI layouts.

The primary goal of this mod isn't to completely replace the stock UI of KSP 2, but to extend what is already there. Another goal is to provide flexibility to the player for configuring the UI to their liking. Initial customizations for this mod will be little things here and there, but the plan is to go as far as providing a lite, in-game, UI layout editor so the user can adjust UI elements to their liking and save the layout as a custom UI configuration.

## Current Features
* Add a toggle button that expands and collapses the OAB's Parts Picker Drawer.

## Installing AUI
### Prerequisites
* **BepInEx** and **SpaceWarp**
  * Option 1: Combined
    * Space Warp + BepInEx ([SpaceDock](https://spacedock.info/mod/3277))
  * Option 2: Individually
    * BepInEx for KSP 2 ([SpaceDock](https://spacedock.info/mod/3255)) ([github](https://github.com/BepInEx/BepInEx))
    * SpaceWarp ([SpaceDock](https://spacedock.info/mod/3257)) ([github](https://github.com/SpaceWarpDev/SpaceWarp))

### Instructions
Before preceding, make sure you refer to the [Releases](https://github.com/kkaja123/AlternativeUI/releases) page to find the minimum required version of this mod's dependencies.
1. Download the `alternative_ui-release-<version number>.zip` file from the [Releases](https://github.com/kkaja123/AlternativeUI/releases) page.
2. Extract the contents of the zip file.
3. Place the extracted contents into the game folder for KSP 2. *(Refer to detailed instructions below on how to find the game folder.)*

The resulting folder structure should look similar the following:
```
Kerbal Space Program 2/
├── BepInEx/
│   ├── cache/
│   ├── _config/
│   ...
│   └── plugins/
│       ├── AUI/
│       │   ├── AUI.dll
│       │   └── swinfo.json
│       └── SpaceWarp/
├── KSP2_x64_Data/
├── MonoBleedingEdge/
├── KSP2_x64.exe
└── winhttp.dll
```

#### Finding the Game Folder

Steam:
1. Go to "Kerbal Space Program 2" in your Steam library.
2. Right-click -> "Manage" -> "Browse local files"

Epic Game Store (EGS):
1. Go to "Kerbal Space Program 2" in your EGS library.
2. Right-click -> "Manage" -> "Installation" -> Click on the button with the folder 📁 icon.

## Building the Project
Before building the project, if you want to have Visual Studio automatically export the built assembly and `swinfo.json` to your KSP game folder, edit `build_scripts/local_deploy.bat` to have the destination path be in your game folder's path.

1. Use the C# project file located at `alternative_ui/alternative_ui.csproj`.
2. Update any failed references in the project. (Point them at the assemblies in the KSP 2 game folder for convenience.)
3. Build the project.
     * Visual Studio 2022
     * .NET Framework 4.7.2
4. The default output location is `alternative_ui/bin/<build config>/AUI.dll`.

## Contributing to the Project
*TODO: Create a CONTRIBUTING.md guide.*

*When contributing code to the project, please conform to the existing coding style. You will make life easier on all of the contributors of the project if your code looks like existing code.*
### What should I work on?
Probably an [issue](https://github.com/kkaja123/AlternativeUI/issues) on GitHub that has not already been assigned to a contributor. Cowboy-coded PRs (e.g. unwarranted changes) are unlikely to be approved or merged without good merit.

### How can I add a new feature?
First, create an [issue](https://github.com/kkaja123/AlternativeUI/issues/new/choose) on GitHub for a feature enhancement. This will allow for discussion *before* you invest a lot of time on writing code that may need to get refactored in the end. Once given the go-ahead ( a maintainer or collaborator says "okay" or assigns the issue to you), write/finish the new feature and submit a pull request for that feature.

Be sure to [link the pull request to the feature's issue](https://docs.github.com/en/issues/tracking-your-work-with-issues/linking-a-pull-request-to-an-issue).

Pull requests are created using a branch from a fork of this repository, unless you are a "collaborator".

### What to do when your feature is rejected
AUI is provided with the very permissive MIT License. That means that you can download the code, make your own changes to it, and distribute it as your own alternative to AlternativeUI!

But before creating a fork and going wild, consider if its possible to create your own mod that uses AUI as a dependency. AUI exposes some API options that are fairly flexible, so you may not need to edit this project's code to get your idea off the ground. If there's some aspect of the API that you find lacking or unsuitable for your mod, maybe that would make for a good feature request instead?

### I don't know about any of that "programming" stuff. I just have a cool idea to recommend!
Sweet! Create an [issue](https://github.com/kkaja123/AlternativeUI/issues/new/choose) on GitHub for a new feature request, and it may be considered.

Also, I'm sure there's a bunch of people in the modding community that wouldn't mind showing you the ropes for programming! If you're interested, consider checking out the KSP forums or creating a new discussion on this repository's [discussions](https://github.com/kkaja123/AlternativeUI/discussions) page.