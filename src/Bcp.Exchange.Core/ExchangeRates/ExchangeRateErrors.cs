namespace Bcp.Exchange.Core.ExchangeRates;

using Shared;

public static class ExchangeRateErrors
{
    public static Error NotFound(Guid id) =>
        Error.Create("ExchangeRate.NotFound", "No se encontró la tasa de cambio");

    public static Error NotFoundByCurrencyPair(Guid sourceCurrencyId, Guid targetCurrencyId) =>
        Error.Create(
            "ExchangeRate.NotFoundByCurrencyPair",
            "No se encontró la tasa de cambio para el par de monedas seleccionado"
        );

    public static Error AlreadyExists() =>
        Error.Create("ExchangeRate.AlreadyExists", $"Ya existe una tasa de cambio activa");

    public static Error CurrencyNotFound(Guid currencyId) =>
        Error.Create(
            "ExchangeRate.CurrencyNotFound",
            "No se encontró la moneda seleccionada"
        );

    public static Error SameCurrency() =>
        Error.Create(
            "ExchangeRate.SameCurrency",
            "Las monedas de origen y destino no pueden ser iguales"
        );

    public static Error InvalidRate() =>
        Error.Create("ExchangeRate.InvalidRate", "La tasa de cambio debe ser mayor que cero");
}
