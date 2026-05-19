using System.Text.Json;

namespace GAAPerform;

public class AppSettings
{
    public FirebaseSettings Firebase { get; set; } = new();

    public static AppSettings Load()
    {
        using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
    }
}

public class FirebaseSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string AuthDomain { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public string DatabaseUrl { get; set; } = string.Empty;
}