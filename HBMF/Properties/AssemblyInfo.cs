using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;

[assembly: AssemblyTitle(HBMF.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(HBMF.BuildInfo.Company)]
[assembly: AssemblyProduct(HBMF.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + HBMF.BuildInfo.Author)]
[assembly: AssemblyTrademark(HBMF.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(HBMF.BuildInfo.Version)]
[assembly: AssemblyFileVersion(HBMF.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(HBMF.HBMF), HBMF.BuildInfo.Name, HBMF.BuildInfo.Version, HBMF.BuildInfo.Author, HBMF.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame(null, null)]