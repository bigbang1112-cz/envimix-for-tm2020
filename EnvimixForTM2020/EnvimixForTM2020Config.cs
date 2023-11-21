using GbxToolAPI;

namespace EnvimixForTM2020;

public class EnvimixForTM2020Config : Config
{
    public string MapNameFormat { get; set; } = "$<{0}$>.{1}";
    public bool IncludeCarSport { get; set; } = true;
    public bool IncludeCarSnow { get; set; } = true;
    public bool IncludeCharacterPilot { get; set; } = false;
    public ValidationMode ValidationMode { get; set; }
    public bool GenerateDefaultCarVariant { get; set; }
}