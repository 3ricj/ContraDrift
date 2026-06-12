# ContraDrift NINA Plugin

Center-crops NINA light frames and writes FITS files for the [ContraDrift standalone app](../../../ContraDrift/) to plate-solve, run PID guiding, and send tracking rates to the mount via ASCOM.

## Architecture

- **This plugin (inside NINA):** hooks image save, center-crops each frame, writes cropped FITS.
- **Standalone ContraDrift app:** watches the crop folder, plate-solves, PID loop, ASCOM mount control.

## Install

1. Build the plugin (requires .NET 10 SDK and NINA 3.3.x):

   ```bash
   dotnet build nina-plugin/ContraDrift/ContraDrift/ContraDrift.csproj
   ```

2. The post-build step copies `NINA.Contradrift.dll` to:

   `%LOCALAPPDATA%\NINA\Plugins\3.0.0\NINA.Contradrift\`

3. Restart NINA and enable **ContraDrift** under **Options → Plugins → Installed**.

## Plugin settings

| Setting | Description |
|---------|-------------|
| **Crop Size** | Square pixel size for the center crop (default 1600). |
| **Crop Path** | Optional output folder. When blank, crops go to `{NINA image save folder}/crop/`. |
| **Save Timeout** | Seconds to wait when writing each cropped FITS (default 30). |

## Standalone app setup

1. In NINA, note your **Options → Imaging → File Path** (or set a fixed **Crop Path** in the plugin).
2. In the ContraDrift standalone app, set **Watch Folder** to:
   - `{your NINA image folder}/crop`, or
   - the fixed **Crop Path** from the plugin.
3. Connect ASCOM telescope and click **Start**.

## Requirements

- NINA 3.3.0.1047 or newer
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (NINA.Plugin 3.3 targets net10.0-windows; `global.json` in this project pins the SDK)
- ContraDrift standalone app for plate-solving and mount correction
