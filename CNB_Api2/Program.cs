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


var data = new EchangeBuilder();

var dataCNBApi = app.MapGroup("/cnb");
dataCNBApi.MapGet("/", () => data.data_exchange?.DataExchangeList.ToArray());


// vyhledá podle denomnationcode
dataCNBApi.MapGet("/denominationcode={denominationcode}", (string denominationcode) =>
    data.data_exchange?.DataExchangeList.FirstOrDefault(a => a.DenominationCode.ToUpper() == denominationcode.ToUpper()) is { } datacnb
        ? Results.Ok(datacnb)
        : Results.NotFound());
// vyhledá podle denomnationcode defaultne
dataCNBApi.MapGet("/{denominationcode}", (string denominationcode) =>
    data.data_exchange?.DataExchangeList.FirstOrDefault(a => a.DenominationCode.ToUpper() == denominationcode.ToUpper()) is { } datacnb
        ? Results.Ok(datacnb)
        : Results.NotFound());
// vyhledá podle zeme
dataCNBApi.MapGet("/country={country}", (string country) =>
    data.data_exchange?.DataExchangeList.FirstOrDefault(a => a.Country.ToUpper() == country.ToUpper()) is { } datacnb
        ? Results.Ok(datacnb)
        : Results.NotFound());
app.Run();

[JsonSerializable(typeof(Data_ExchangeList[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
