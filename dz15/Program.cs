using System.Globalization;
using System.Text.Json;

namespace dz15
{
    public class ConfigManager
    {
        private static readonly Lazy<ConfigManager> _singleton = new(() => new ConfigManager());
        private Dictionary<string, string> _configData;
        private const string ConfigFile = "settings.json";

        private ConfigManager()
        {
            _configData = File.Exists(ConfigFile)
                ? JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(ConfigFile))
                : new Dictionary<string, string>();
        }

        public static ConfigManager Instance => _singleton.Value;

        public void UpdateConfig(string parameter, string setting)
        {
            _configData[parameter] = setting;
            PersistConfig();
        }

        public string RetrieveConfig(string parameter)
        {
            return _configData.ContainsKey(parameter) ? _configData[parameter] : "Not available";
        }

        private void PersistConfig()
        {
            string jsonOutput = JsonSerializer.Serialize(_configData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFile, jsonOutput);
        }
    }
    public interface IFigure
    {
        void Render();
    }

    public class Oval : IFigure
    {
        public void Render()
        {
            Console.WriteLine("Rendering an oval shape");
        }
    }

    public class Box : IFigure
    {
        public void Render()
        {
            Console.WriteLine("Rendering a rectangular shape");
        }
    }

    public abstract class FigureCreator
    {
        public abstract IFigure GenerateShape();
    }

    public class OvalCreator : FigureCreator
    {
        public override IFigure GenerateShape()
        {
            return new Oval();
        }
    }

    public class BoxCreator : FigureCreator
    {
        public override IFigure GenerateShape()
        {
            return new Box();
        }
    }

    public interface IGeoService
    {
        (double Lat, double Lng) FetchCoordinates();
    }

    public class BasicGeo
    {
        public string ProvideRawLocation()
        {
            return "37.7749, -122.4194";
        }
    }

    public class GeoAdapter : IGeoService
    {
        private readonly BasicGeo _locationProvider;

        public GeoAdapter(BasicGeo locationProvider)
        {
            _locationProvider = locationProvider;
        }

        public (double Lat, double Lng) FetchCoordinates()
        {
            string[] data = _locationProvider.ProvideRawLocation().Split(',');
            double latitude = double.Parse(data[0].Trim(), CultureInfo.InvariantCulture);
            double longitude = double.Parse(data[1].Trim(), CultureInfo.InvariantCulture);
            return (latitude, longitude);
        }
    }

    class Launcher
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string[] tasks = { "Singleton Config Test", "Shape Factory Demo", "Geo Adapter Check" };
            Action[] actions =
            {
                () => {
                    var cfg = ConfigManager.Instance;
                    cfg.UpdateConfig("Theme", "DarkMode");
                    cfg.UpdateConfig("Locale", "en-US");
                    Console.WriteLine($"[Config] Theme => {cfg.RetrieveConfig("Theme")}");
                    Console.WriteLine($"[Config] Locale => {cfg.RetrieveConfig("Locale")}\n");
                },
                () => {
                    foreach (FigureCreator fc in new FigureCreator[] { new OvalCreator(), new BoxCreator() })
                    {
                        fc.GenerateShape().Render();
                    }
                    Console.WriteLine();
                },
                () => {
                    var adapter = new GeoAdapter(new BasicGeo());
                    var coords = adapter.FetchCoordinates();
                    Console.WriteLine($"Geo Location -> Lat: {coords.Lat}, Lng: {coords.Lng}");
                }
            };

            for (int i = 0; i < tasks.Length; i++)
            {
                Console.WriteLine($"=== {tasks[i]} ===");
                actions[i]();
            }
        }
    }
}
