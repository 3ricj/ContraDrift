# ContraDrift

## 1.1.0.0
- Modernized for NINA 3.3.x (net10.0-windows, NINA.Plugin 3.3.0.1047-nightly)
- Crop-only plugin: removed plate-solve, mount control, and template sequencer/dockable boilerplate
- Hybrid crop output: blank Crop Path uses `{NINA image folder}/crop/`; optional fixed Crop Path override
- Fixed Crop Path settings persistence bug
- Added folder browse button; renamed save timeout setting
- Errors during crop no longer abort NINA's full-frame save

## 1.0.0.2
- Initial crop-on-save implementation (experimental)

## 1.0.0.1
- Initial release
