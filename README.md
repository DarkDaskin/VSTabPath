# TabPath extension

[![Build status](https://ci.appveyor.com/api/projects/status/wto719b7q7k0lpup?svg=true)](https://ci.appveyor.com/project/DarkDaskin/vstabpath)

Download: [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=darkdaskin.tabpath)

## Purpose

Visual Studio extension which shows file paths in tab titles with same file names.

It is inspired by a built-in feature of Visual Studio Code and has similar look and feel, showing relevant path segments next to file names.

Improvement suggestions are welcome.

## Compatibility

The extension currently works with Visual Studio 2017 - 2022. Support for other versions might be added in future.

The extension is compatible with the [Custom Document Well](https://marketplace.visualstudio.com/items?itemName=VisualStudioPlatformTeam.CustomDocumentWell) extension (a component of [Productivity Power Tools 2017](https://marketplace.visualstudio.com/items?itemName=VisualStudioProductTeam.ProductivityPowerPack2017)).

## Building

The extension references few assemblies directly from Visual Studio directory, so Visual Studio 2017 has to be installed to build it. No specific work loads are required.

In case you need to build the extension having only newer version of Visual Studio installed, override the `MinimumVisualStudioVersion` project property (see *VisualStudio.props* for possible values). This will make the extension incompatible with earlier Visual Studio versions.