using GBX.NET.Engines.Game;
using GbxToolAPI;
using System.Text;
using TmEssentials;

namespace EnvimixForTM2020;

[ToolName("Envimix for TM2020")]
[ToolDescription("Generates environment mix variants of a TM2020 map.")]
[ToolGitHub("bigbang1112-cz/envimix-for-tm2020")]
public class EnvimixForTM2020Tool : ITool, IHasOutput<IEnumerable<NodeFile<CGameCtnChallenge>>>, IConfigurable<EnvimixForTM2020Config>
{
    private readonly CGameCtnChallenge map;

    private static readonly string[] cars = new[] { "CarSport", "CarSnow", "CharacterPilot" };

    public EnvimixForTM2020Config Config { get; set; } = new();

    public EnvimixForTM2020Tool(CGameCtnChallenge map)
    {
        this.map = map;
    }

    public IEnumerable<NodeFile<CGameCtnChallenge>> Produce()
    {
        if (GameVersion.IsTM2020(map) == false)
        {
            throw new Exception("This tool is only for TM2020 maps.");
        }

        var includes = new[]
        {
            Config.IncludeCarSport, Config.IncludeCarSnow, Config.IncludeCharacterPilot
        };

        var prevPlayerModel = map.PlayerModel;
        var defaultMapUid = map.MapUid;
        var defaultMapName = map.MapName;

        for (int i = 0; i < cars.Length; i++)
        {
            var car = cars[i];
            var include = includes[i];

            if (!include)
            {
                continue;
            }

            if (!Config.GenerateDefaultCarVariant)
            {
                if (map.Collection == 26 && car == "CarSport") continue;
            }

            map.PlayerModel = (car, 10003, "");
            map.MapUid = $"{Convert.ToBase64String(Encoding.ASCII.GetBytes(Guid.NewGuid().ToString()))[..10]}{defaultMapUid.Substring(9, 10)}ENVIMIX";
            map.MapName = string.Format(Config.MapNameFormat, defaultMapName, car);

            map.CrackPassword();

            var pureFileName = $"{TextFormatter.Deformat(map.MapName)}.Map.Gbx";
            var validFileName = string.Join("_", pureFileName.Split(Path.GetInvalidFileNameChars()));

            // Each map executes the save method
            yield return new(map, $"Maps/Envimix/{validFileName}", IsManiaPlanet: true);
        }

        // Return to previous to temporarily fix the mutability issue
        map.PlayerModel = prevPlayerModel;
        map.MapName = defaultMapName;
    }
}