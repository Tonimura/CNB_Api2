using CNB_Api.Models;
using CNB_Api2.Models;
using System.Diagnostics.Metrics;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

//var sampleTodos = new Todo[] {
//    new(1, "Walk the dog"),
//    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
//    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
//    new(4, "Clean the bathroom"),
//    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
//};

var data = new EchangeBuilder();

//var todosApi = app.MapGroup("/todos");
//todosApi.MapGet("/", () => sampleTodos);
//todosApi.MapGet("/{id}", (int id) =>
//    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
//        ? Results.Ok(todo)
//        : Results.NotFound());

var dataCNBApi = app.MapGroup("/cnb");
dataCNBApi.MapGet("/", () => data.data_exchange.DataExchangeList.ToArray());


// vyhledá podle denomnationcode
dataCNBApi.MapGet("/denominationcode={denominationcode}", (string denominationcode) =>
    data.data_exchange.DataExchangeList.FirstOrDefault(a => a.DenominationCode.ToUpper() == denominationcode.ToUpper()) is { } datacnb
        ? Results.Ok(datacnb)
        : Results.NotFound());
// vyhledá podle denomnationcode defaultne
dataCNBApi.MapGet("/{denominationcode}", (string denominationcode) =>
    data.data_exchange.DataExchangeList.FirstOrDefault(a => a.DenominationCode.ToUpper() == denominationcode.ToUpper()) is { } datacnb
        ? Results.Ok(datacnb)
        : Results.NotFound());
// vyhledá podle zeme
dataCNBApi.MapGet("/country={country}", (string country) =>
    data.data_exchange.DataExchangeList.FirstOrDefault(a => a.Country.ToUpper() == country.ToUpper()) is { } datacnb
        ? Results.Ok(datacnb)
        : Results.NotFound());
app.Run();

//public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);
//public record ExchangeList(string Country, string DenominationName, int Count, string DenominationCode , float Coin);
[JsonSerializable(typeof(Data_ExchangeList[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
