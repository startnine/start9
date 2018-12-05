# ![Start9](https://i.imgur.com/U31cS2J.png)
Start9 is a WIP extensible, customizable shell supplement for Windows, one which aims to be not only modular, but extends to many needs.

## Planned Modules
*Start9 is planned with with 7 modules bundled, with more in the Module Marketplace. The default modules list includes*:
- Classic Start Menu
- Windows XP Start Menu 
- Windows 7 Start Menu
- Windows 10 AU Start Menu
- Classic Taskbar
- Superbar
- macOS Dock
- Longhorn (4074) Sidebar
- Windows 10 Notifcation Center

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

---
## Start9
A beautiful and simple front-end for managing Start9 modules.

### Contributing and Conduct
This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.
For more information, see the [Contributor Covenant code of conduct](https://www.contributor-covenant.org/).

In addition, please follow the [contributing guidelines](https://github.com/StartNine/Start9.Host/blob/master/CONTRIBUTING.md) for all Start9 projects.

### Dependencies and Frameworks
- [.NET Framework 4.7.2](https://www.microsoft.com/net/download/dotnet-framework-runtime/net472)
	- Official builds of Start9 are built with .NET Framework 4.7.2. However, you may want to retarget it to be able to access newer features and load in modules compiled for later versions of .NET.
- WPF related assemblies (PresentationFramework, etc.)
	- These assemblies make Start9 incompatible with .NET Core and other non-Microsoft .NET implementations. 
- [Start9 API](https://github.com/StartNine/start9-api)
	- This is the home to various abstractions related to Windows objects like programs and open windows. It's also home to the Plex themed UI controls, and a few additional ones.
- System.AddIn
	- This represents the Microsoft Addin Framework assembly. We use this in Start9 for the Host-side adapter part of the addin pipeline.

Other, smaller dependencies can be seen in the [csproj file for the project](https://github.com/StartNine/start9/blob/master/Start9/Start9.Host.csproj#L40). 


---
Interested? Join our Discord: [![Discord](https://img.shields.io/discord/321793250602254336.svg?style=flat-square&colorB=7289DA)](https://discord.gg/6cpvxBS)
