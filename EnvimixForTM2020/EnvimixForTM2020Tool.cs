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

    private static readonly string[] cars = new[] { "CarSport", "CarSnow", "CarRally", "CarDesert", "CharacterPilot" };
    private static readonly string[] envs = new[] { "Stadium", "Snow", "Rally", "Desert" };

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
            Config.IncludeCarSport, Config.IncludeCarSnow, Config.IncludeCarRally, Config.IncludeCarDesert, Config.IncludeCharacterPilot
        };

        var prevPlayerModel = map.PlayerModel;
        var prevAuthorTime = map.TMObjective_AuthorTime;
        var prevGoldTime = map.TMObjective_GoldTime;
        var prevSilverTime = map.TMObjective_SilverTime;
        var prevBronzeTime = map.TMObjective_BronzeTime;

        var defaultCar = map.PlayerModel?.Id;
        if (string.IsNullOrEmpty(defaultCar))
        {
            defaultCar = "CarSport";
        }

        var defaultMapUid = map.MapUid;
        var defaultMapName = map.MapName;

        var prevGateBlocks = map.GetBlocks()
            .Select(x => x.Name)
            .Where(IsTransformationGate)
            .ToList();

        var prevGateItems = map.GetAnchoredObjects()
            .Select(x => x.ItemModel.Id)
            .Where(IsTransformationGate)
            .ToList();

        for (int i = 0; i < cars.Length; i++)
        {
            var car = cars[i];
            var env = envs.Length > i ? envs[i] : null;
            var include = includes[i];

            if (!include)
            {
                continue;
            }

            if (!Config.GenerateDefaultCarVariant && car == defaultCar)
            {
                continue;
            }

            map.PlayerModel = (car, 10003, "");
            map.MapUid = $"{Convert.ToBase64String(Encoding.ASCII.GetBytes(Guid.NewGuid().ToString()))[..10]}{defaultMapUid.Substring(9, 10)}ENVIMIX";
            map.MapName = string.Format(Config.MapNameFormat, defaultMapName, car);

            if (env is null)
            {
                RestoreGates(prevGateBlocks, prevGateItems);
            }
            else
            {
                ChangeGates(env);
            }

            switch (Config.ValidationMode)
            {
                case ValidationMode.None:
                    break;
                case ValidationMode.Fake:
                    map.TMObjective_AuthorTime = TimeInt32.MaxValue;
                    map.TMObjective_GoldTime = TimeInt32.MaxValue;
                    map.TMObjective_SilverTime = TimeInt32.MaxValue;
                    map.TMObjective_BronzeTime = TimeInt32.MaxValue;
                    break;
                case ValidationMode.Real:
                    map.TMObjective_AuthorTime = new(-1);
                    map.TMObjective_GoldTime = new(-1);
                    map.TMObjective_SilverTime = new(-1);
                    map.TMObjective_BronzeTime = new(-1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            map.CrackPassword();

            var pureFileName = $"{TextFormatter.Deformat(map.MapName)}.Map.Gbx";
            var validFileName = string.Join("_", pureFileName.Split(Path.GetInvalidFileNameChars()));

            // Each map executes the save method
            yield return new(map, $"Maps/Envimix/{validFileName}", IsManiaPlanet: true);
        }

        // Return to previous to temporarily fix the mutability issue
        map.PlayerModel = prevPlayerModel;
        map.MapName = defaultMapName;

        map.TMObjective_AuthorTime = prevAuthorTime;
        map.TMObjective_GoldTime = prevGoldTime;
        map.TMObjective_SilverTime = prevSilverTime;
        map.TMObjective_BronzeTime = prevBronzeTime;

        RestoreGates(prevGateBlocks, prevGateItems);
    }

    private static bool IsTransformationGate(string name)
    {
        return name.Contains("Gameplay") && envs.Any(env => name.Contains($"Gameplay{env}"));
    }

    private void ChangeGates(string envimixEnvironment)
    {
        foreach (var block in map.GetBlocks().Where(block => !block.Name.Contains("Gameplay")))
        {
            for (int i = 0; i < envs.Length; i++)
            {
                var env = envs[i];

                if (!block.Name.Contains($"Gameplay{env}"))
                {
                    continue;
                }

                block.Name = block.Name.Replace(env, envimixEnvironment);
            }
        }

        foreach (var item in map.GetAnchoredObjects().Where(item => !item.ItemModel.Id.Contains("Gameplay")))
        {
            for (int i = 0; i < envs.Length; i++)
            {
                var env = envs[i];

                if (!item.ItemModel.Id.Contains($"Gameplay{env}"))
                {
                    continue;
                }

                item.ItemModel = item.ItemModel with { Id = item.ItemModel.Id.Replace(env, envimixEnvironment) };
            }
        }
    }

    private void RestoreGates(IList<string> prevGateBlocks, IList<string> prevGateItems)
    {
        var index = 0;

        foreach (var block in map.GetBlocks().Where(block => IsTransformationGate(block.Name)))
        {
            block.Name = prevGateBlocks[index++];
        }

        index = 0;

        foreach (var item in map.GetAnchoredObjects().Where(item => IsTransformationGate(item.ItemModel.Id)))
        {
            item.ItemModel = item.ItemModel with { Id = prevGateItems[index++] };
        }
    }
}