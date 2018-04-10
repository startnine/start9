# Start9 ![Start9](https://startnine.github.io/assets/img/icon32.png)
Start9 is a WIP extensible, customizable shell supplement for Windows, one which aims to be not only modular, but extends to many needs.

## Planned Modules
*Start9 is planned with with 7 modules pre-installed, each module functioning like a familiar user interface.*:
- Classic Start Menu
- Windows XP Start Menu 
- Windows 7 Start Menu
- Windows 10 AU Start Menu
- Classic Taskbar
- Superbar
- macOS Dock

<!-- ## Compatability
*As Start9 is still in the early stages of development, compatibility isn't a big concern right now*
*Ranking: Excellent, Good, Satisfactory, Needs Improvement, Broken*
| OS            | Compatability | Notes |
| ------------- |---------------|---|
| Windows 10    | Excellent     ||
| Windows 8.1   | Excellent     ||
| Windows 7     | Untested      ||
| Windows Vista | Untested      ||
| Windows XP    | Untested      ||
| ReactOS       | Broken        |  Crashes silently on startup|
-->

# Start9.Host
A beautiful and simple front-end for managing Start9 modules.

## Contributing and Conduct
This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.
For more information, see the [Contributor Covenant code of conduct](https://www.contributor-covenant.org/).

In addition, Please follow the [contributing guidelines](https://github.com/StartNine/Start9.Host/blob/master/CONTRIBUTING.md) for all Start9 projects.

## Dependencies and Frameworks
- [.NET Framework 4.0](https://www.microsoft.com/en-ca/download/details.aspx?id=17718)
	- Official builds of Start9 are built with .NET Framework 4.0. However, you may want to retarget it to be able to access newer features and load in modules compiled for later versions of .NET.
- WPF related assemblies (PresentationFramework, etc.)
	- These assemblies make Start9 incompatible with .NET Core and other non-Microsoft .NET implementations. 
- [Start9.Api](https://github.com/StartNine/Start9.Api)
	- This is the home to various abstractions related to Windows objects like programs and open windows. It's also home to the Plex themed UI controls, and a few additional ones.
- System.AddIn
	- This represents the Microsoft Addin Framework assembly. We use this in Start9 for the Host-side adapter part of the addin pipeline.

Other, smaller dependencies can be seen in the [csproj file for the project](https://github.com/StartNine/Start9.Host/blob/master/Start9/Start9.csproj#L36). 


---
Interested? Join our Discord: [![Discord](https://img.shields.io/discord/321793250602254336.svg?style=flat-square&colorB=7289DA)](https://discord.gg/6cpvxBS)
