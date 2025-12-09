using System.Net.Http.Json;
using DotNetEnv;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

var app = builder.Build();

// app.UseHttpsRedirection();



string? apiKey = Environment.GetEnvironmentVariable("API_KEY");

if (string.IsNullOrEmpty(apiKey))
{
    throw new InvalidOperationException("Api key not set");
}

string city = "Helsinki";

app.MapGet("/weather", async (IHttpClientFactory http) =>
{
    var client = http.CreateClient();
    var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

    var weather = await client.GetFromJsonAsync<WeatherResponse>(url);

    if (weather == null)
    {
        return Results.NotFound("City not found");
    }
    else
    {
        var result = new
        {
            weather.Name,
            Temperature = $"{weather.Main.Temp}C",
            Humidity = $"{weather.Main.Humidity}%",
            Description = weather.Weather[0].Description
        };

        // return weather is not null ? Results.Ok(weather) : Results.NotFound();
        return Results.Ok(result);
    }
});

app.Run();


public class WeatherResponse
{
    public MainInfo Main { get; set; }
    public string Name { get; set; }
    public WeatherInfo[] Weather { get; set; }
}

public class MainInfo
{
    public float Temp { get; set; }
    public float Humidity { get; set; }
}

public class WeatherInfo
{
    public string Description { get; set; }
}
