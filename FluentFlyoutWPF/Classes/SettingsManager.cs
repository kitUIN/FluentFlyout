using System.IO;
using System.Xml.Serialization;

namespace FluentFlyout.Classes.Settings;

/// <summary>
/// Manages the application settings and saves them to a file in \AppData\FluentFlyout.
/// </summary>
public class SettingsManager
{
    private static string SettingsFilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "FluentFlyout",
        "settings.xml"
    );
    string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FluentFlyout", "log.txt");
    private static UserSettings? _current;

    /// <summary>
    /// The current user settings stored in the app.
    /// </summary>
    /// <returns>The current user settings.</returns>
    public static UserSettings Current
    {
        get
        {
            _current ??= new UserSettings();
            return _current;
        }
        set => _current = value;
    }

    /// <summary>
    /// Checks whether the app has updated, and restores the settings from the previous version if necessary. Only updates in release mode.
    /// </summary>
    //public static void CheckAndUpdateSettings()
    //{
    //    try // gets the version of the app, works only in release mode
    //    {
    //        var version = Package.Current.Id.Version;
    //        string versionString = $"{version.Major}.{version.Minor}.{version.Build}";
    //        // if local settings file's version number is different from the current version, update the settings
    //        if (!File.Exists(SettingsFilePath)) return;
    //        if (Settings.Default.LastKnownVersion != versionString)
    //        {
    //            RestoreSettings();
    //        }
    //    }
    //    catch
    //    {
    //        // don't update settings if version is not available
    //        return;
    //    }
    //}

    /// <summary>
    /// Restores the settings `SettingsManager.Current` from the settings file.
    /// </summary>
    /// <returns>The restored settings.</returns>
    public static UserSettings RestoreSettings()
    {
        //File.AppendAllText(logFilePath, $"[{DateTime.Now}] {SettingsFilePath}\n");
        var xmlSerializer = new XmlSerializer(typeof(UserSettings));
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                using var reader = new StreamReader(SettingsFilePath);
                _current = (UserSettings)xmlSerializer.Deserialize(reader)!;
                //File.AppendAllText(logFilePath, $"[{DateTime.Now}] Settings restored\n");
                //EventLog.WriteEntry("FluentFlyout", "Settings restored", EventLogEntryType.Information);
                return _current;
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            //File.AppendAllText(logFilePath, $"[{DateTime.Now}] No permission to write in {SettingsFilePath}: {ex.Message}\n");
        }
        catch (Exception ex)
        {
            //File.AppendAllText(logFilePath, $"[{DateTime.Now}] Error saving settings: {ex.Message}\n");
        }

        // if the settings file not found or cannot be read
        //File.AppendAllText(logFilePath, $"[{DateTime.Now}] Settings file not found or cannot be read\n");
        _current = new UserSettings();
        return _current;
    }

    /// <summary>
    /// Saves the app settings to the settings file.
    /// </summary>
    public static void SaveSettings()
    {
        var xmlSerializer = new XmlSerializer(typeof(UserSettings));
        try
        {
            var directory = Path.GetDirectoryName(SettingsFilePath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var writer = new StreamWriter(SettingsFilePath);
            xmlSerializer.Serialize(writer, _current);
        }
        catch (UnauthorizedAccessException ex)
        {
            // if the app doesn't have permission to write to the settings file
        }
        catch (Exception ex)
        {
            // if the settings file cannot be saved
        }
    }
}
