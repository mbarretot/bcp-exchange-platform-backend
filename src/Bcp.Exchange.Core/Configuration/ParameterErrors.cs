namespace Bcp.Exchange.Core.Configuration;

using Shared;

public static class ParameterErrors
{
    public static Error NotFound(string code) =>
        Error.Create("Parameter.NotFound", $"No se encontró el parámetro con código '{code}'");

    public static Error NotFoundById(Guid id) =>
        Error.Create("Parameter.NotFound", "No se encontró el parámetro");

    public static Error NotFoundByCode(string code) =>
        Error.Create("Parameter.NotFound", $"No se encontró el parámetro con código '{code}'");

    public static Error DuplicateCode(string code) =>
        Error.Create("Parameter.DuplicateCode", $"Ya existe un parámetro con el código '{code}'");

    public static Error InvalidCode(string code) =>
        Error.Create("Parameter.InvalidCode", $"El código de parámetro '{code}' no es válido");

    public static Error HasDependencies(string code) =>
        Error.Create(
            "Parameter.HasDependencies",
            $"No se puede eliminar el parámetro '{code}' porque tiene dependencias"
        );

    public static Error ParentNotFound(Guid parentId) =>
        Error.Create(
            "Parameter.ParentNotFound",
            "No se encontró el parámetro padre"
        );

    public static Error CannotDeleteParent(string code) =>
        Error.Create(
            "Parameter.CannotDeleteParent",
            $"No se puede eliminar el parámetro padre '{code}' porque tiene hijos"
        );

    public static Error InvalidDisplayOrder(int order) =>
        Error.Create(
            "Parameter.InvalidDisplayOrder",
            $"El orden de visualización '{order}' debe ser mayor o igual a cero"
        );
}
