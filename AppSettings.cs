using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;

public class AppSettings
{
    ILogger logger;

    public AppSettings()
    {
    }

    public AppSettings(ILogger logger)
    {
        this.logger = logger;
        //InitializeComponent();
        string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings-zmbrowse.yml");

        if (File.Exists(settingsFilePath))
        {
            try
            {
                var yaml = new YamlDotNet.Serialization.Deserializer();
                using (var reader = new StreamReader(settingsFilePath))
                {
                    // Deserialize the YAML file into a dictionary to handle the root key
                    var root = yaml.Deserialize<Dictionary<string, AppSettings>>(reader);

                    // Check if the "zmbrowse" key exists and merge its values with defaults
                    if (root != null && root.ContainsKey("zmbrowse"))
                    {
                        var loadedSettings = root["zmbrowse"];
                        DefaultFolderPath = loadedSettings.DefaultFolderPath ?? this.DefaultFolderPath;
                        DefaultCameraName = loadedSettings.DefaultCameraName ?? this.DefaultCameraName;
                        ThumbnailTextColor = loadedSettings.ThumbnailTextColor ?? this.ThumbnailTextColor;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error reading settings file: {ex.Message}");
            }
        }
        else
        {
            logger.LogWarning("Settings file not found. Using default values.");
        }

        //return Constructor;
    }


    public string DefaultFolderPath { get; set; } = "\\";
    public string DefaultCameraName { get; set; } = "1";
    public string ThumbnailTextColor { get; set; } = "White";

}
