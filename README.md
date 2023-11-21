# Envimix For TM2020 [(online)](https://gbx.bigbang1112.cz/tool/envimix-for-tm2020)

[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/BigBang1112-cz/envimix-for-tm2020?include_prereleases&style=for-the-badge)](https://github.com/BigBang1112-cz/envimix-for-tm2020/releases)
[![GitHub all releases](https://img.shields.io/github/downloads/BigBang1112-cz/envimix-for-tm2020/total?style=for-the-badge)](https://github.com/BigBang1112-cz/envimix-for-tm2020/releases)

Hosted on [Gbx Web Tools](https://github.com/bigbang1112-cz/gbx), lives on [Gbx Tool API](https://github.com/bigbang1112-cz/gbx-tool-api), internally powered by [GBX.NET](https://github.com/BigBang1112/gbx-net).

With this tool, you can generate the same maps but with different TM2020 cars.

This tool supports multiple outputs on a single tool instance.

## Settings

Configuration can be managed on the website in the Config component or inside the `Config` folder.

You can now have multiple configs and change between them.

```yml
MapNameFormat: '$<{0}$>.{1}' # {0} is original map name, {1} is the car's modern name
IncludeCarSport: true
IncludeCarSnow: true
IncludeCharacterPilot: true
GenerateDefaultCarVariant: false # If to also generate the environment's default car variant
```

## CLI build

For 100% offline control, you can use the CLI version. Drag and drop your desired maps onto the EnvimixForTM2020CLI(.exe).

**The installation paths still suggest ManiaPlanet paths, which is an oversight.** This will be possible to control in the future and fixed for this scenario.

### ConsoleOptions.yml

- **NoPause** - If true, All "Press key to continue..." will be skipped.
- **SingleOutput** - If false, dragging multiple files will produce multiple results. If true, multiple files will produce only one result.
- **CustomConfig** - Name of the config inside the `Config` folder without the extension.
- **OutputDir** - Forced output directory of produced results.

### Update notifications

The tool notifies you about new versions after launching it. You can press U to directly open the web page where you can download the new version. Auto-updater might come in the future.

### Specific command line arguments

- `-nopause`
- `-config [ConfigName]`
- `-o [OutputDir]` or `-output [OutputDir]`
- `-c:[AnySettingName] [value]` - Force setting through the command line, **currently works only for string values.**

<h2 align="center">#20yearsoftrackmania</h2>
