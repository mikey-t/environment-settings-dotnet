# environment-settings-dotnet

A wrapper for accessing environment variables from within a dotnet core application.

## Goals

- Make it easy to add new environment variables to a project in a strongly typed way
- Use declarative meta data on settings using attributes
- Allow selectively using default values depending on the environment
- Abstract the interface for getting environment variables to allow easy unit testing with different values
- Abstract secrets access so that in code environment variables and secrets are all just "environment settings"

## Example Usage

Create enum and mark values with `SettingInfo`.

Example settings enum:

```c#
using MikeyT.EnvironmentSettingsNS.Attributes;
using MikeyT.EnvironmentSettingsNS.Enums;

public enum GlobalSettings
{
    [SettingInfo(ShouldLogValue = true)]
    ASPNETCORE_ENVIRONMENT,

    [SettingInfo(DefaultValue = "localhost",
        DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly,
        ShouldLogValue = true)]
    POSTGRES_HOST,

    [SettingInfo(DefaultValue = "5432",
        DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly,
        ShouldLogValue = true)]
    POSTGRES_PORT,

    [SettingInfo(DefaultValue = "postgres",
        DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
    POSTGRES_USER,

    [SettingInfo(DefaultValue = "super_secret",
        DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
    POSTGRES_PASSWORD,

    [SettingInfo(DefaultValue = "my_db",
        DefaultForEnvironment = DefaultSettingForEnvironment.AllEnvironments,
        ShouldLogValue = true)]
    DB_NAME,
    
    [SettingInfo(DefaultValue = "SomeValue")]
    SOME_KEY
}
```

Create a `.env` file in your project so other developers can have different values. Add `.env` to your `.gitignore` so that your personal settings are not checked into source control.

Example `.env` file:

```text
POSTGRES_HOST=localhost
POSTGRES_PORT=5432
POSTGRES_USER=postgres
POSTGRES_PASSWORD=super_secret
```

In your `Program.cs` load environment variables from `.env`:

```c#
using MikeyT.EnvironmentSettingsNS.Logic;

public static void Main(string[] args)
{
    DotEnv.Load();
    // ...
}

```

In your `Startup.cs` `ConfigureServices` method, instantiate and populate settings, and then optionally setup dependency injection singleton:
using MikeyT.EnvironmentSettingsNS.Interface;
using MikeyT.EnvironmentSettingsNS.Logic;
```c#
private ILogger _logger;
private IEnvironmentSettings _envSettings;

public class Startup
{
    _envSettings = new EnvironmentSettings(new EnvironmentVariableProvider());
    _envSettings.AddSettings<GlobalSettings>();
    
    // Log all environment variables that are white-listed for logging
    _logger.Information("Loaded environment settings\n{EnvironmentSettings}", _envSettings.GetAllAsSafeLogString());
    
    // Setup dependency injection
    services.AddSingleton(_envSettings);
}
   
```
In a controller, use dependency injection to get an instance of `IEnvironmentSettings` and use that to access settings:

```c#
[ApiController]
[Route("api/Weather/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherGetter _weatherGetter;
    private readonly IEnvironmentSettings _envSettings;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IWeatherGetter weatherGetter,
        IEnvironmentSettings envSettings)
    {
        _logger = logger;
        _weatherGetter = weatherGetter;
        _envSettings = envSettings;
    }

    [HttpGet("RandomForecasts")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation(
            "Your environment value for key SOME_KEY is {SomeKeyValue}",
            envSettings.GetString(GlobalSettings.SOME_KEY));
        return _weatherGetter.GetRandomWeatherForecasts();
    }
}
```

Use different environment settings when unit testing by mocking `IEnvironmentSettings`:

```c#
public enum YourSettings
{
    [SettingInfo(DefaultValue = "SomeValue")]
    SOME_KEY
}

public class YourClass
{
    private readonly IEnvironmentSettings _envSettings;
    
    public YourClass(IEnvironmentSettings envSettings)
    {
        _envSettings = envSettings;
    }
    
    // ...
}

public class YourClassTest
{
    private YourClass _yourClass;
    private Mock<IEnvironmentSettings> _envSettings = new();
        
    public YourClassTest()
    {
        _yourClass = new YourClass(_envSettings.Object);
    }
    
    [Fact]
    public void YourMethod_withSomeState_doesSomething()
    {
        // Arrange
        _envSettings.Setup(ev => ev.GetString(YourSettings.SOME_KEY)).Returns("SomeTestValue");

        // Act
        // ...

        // Assert
        // ...
    }
}
```

## TODO

- Add interface for getting secrets and add `EnvironmentSettings` constructor overload
- Add `throwIfNotSet` functionality to `SettingInfo` and wire up to `EnvironmentSettings.Load()`
- More unit tests
