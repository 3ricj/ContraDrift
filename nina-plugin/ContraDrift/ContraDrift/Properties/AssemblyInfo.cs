using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("552d3a78-7883-4c52-be84-ba2004031c60")]

[assembly: AssemblyVersion("1.1.0.0")]
[assembly: AssemblyFileVersion("1.1.0.0")]

[assembly: AssemblyTitle("ContraDrift")]
[assembly: AssemblyDescription("Center-crops NINA light frames for the ContraDrift standalone guiding app")]

[assembly: AssemblyCompany("3ric Johanson")]
[assembly: AssemblyProduct("ContraDrift")]
[assembly: AssemblyCopyright("Copyright © 2024 3ric Johanson")]

[assembly: AssemblyMetadata("MinimumApplicationVersion", "3.3.0.1047")]

[assembly: AssemblyMetadata("License", "MPL-2.0")]
[assembly: AssemblyMetadata("LicenseURL", "https://www.mozilla.org/en-US/MPL/2.0/")]
[assembly: AssemblyMetadata("Repository", "https://github.com/3ricj/ContraDrift")]

[assembly: AssemblyMetadata("Homepage", "https://github.com/3ricj/ContraDrift")]
[assembly: AssemblyMetadata("Tags", "guiding,crop,contradrift")]
[assembly: AssemblyMetadata("ChangelogURL", "https://github.com/3ricj/ContraDrift/blob/main/nina-plugin/ContraDrift/ContraDrift/CHANGELOG.md")]

[assembly: AssemblyMetadata("FeaturedImageURL", "")]
[assembly: AssemblyMetadata("ScreenshotURL", "")]
[assembly: AssemblyMetadata("AltScreenshotURL", "")]
[assembly: AssemblyMetadata("LongDescription", @"
ContraDrift is a mount rate correction system that guides without a guidescope by plate-solving images from the primary OTA.

This NINA plugin handles only the imaging side: when NINA saves a light frame, it center-crops the image to a configurable square size and writes a FITS file to a crop folder. The separate ContraDrift standalone application watches that folder, plate-solves each cropped frame, runs a PID loop, and sends updated tracking rates to the mount via ASCOM.

Configure Crop Path in the plugin options, or leave it blank to write crops to {NINA image save folder}/crop/. Point the standalone app's Watch Folder at the same location.
")]

[assembly: ComVisible(false)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
