// SPDX-License-Identifier: MPL-2.0
using System.Reflection;
using System.Runtime.CompilerServices;

if (Args is not [var path])
{
	Warning("No path specified.");
    return;
}

try
{
    new FileInfo(path)
        .Directory
        .EnumerateFiles("*.dll")
        .Select(x => x.FullName)
        .Where(x => x != path)
        .ToList()
        .ForEach(Load);

    var asm = Assembly.LoadFrom(path);

    Iterate(asm);
}
catch (Exception ex)
{
    Error("dotnet-script threw", ex);
}

static void Error(string message, Exception ex = null) => Log("ERROR", message, ex);

static void Hint(string message, Exception ex = null) => Log("HINT", message, ex);

static void Warning(string message, Exception ex = null) => Log("WARNING", message, ex);

static void Log(string prefix, string message, Exception ex) =>
    Console.WriteLine(
    	ex is null
    		? $"{prefix}: {message}"
    		: $"{prefix}: {message} {ex.GetType()}: {ex.Message}{(prefix is "ERROR" ? $"\t{ex.StackTrace.Replace('\n', '\t')}" : "")}"
   		);

static void Iterate(Assembly asm) =>
    ToTypes(asm)
        .Where(x => x is not null)
        .ToList()
        .ForEach(RunClassConstructor);

static void Load(string path)
{
    try
    {
        _ = Assembly.LoadFrom(path);
    }
    catch (FileLoadException ex)
    {
        Warning($"The assembly \"{path}\" threw a", ex);
    }
}

static void RunClassConstructor(Type type) => RunClassConstructor(type, type);

static void RunClassConstructor(Type type, Type originalType)
{
    if (type.BaseType is { } baseType)
        RunClassConstructor(baseType, originalType);

    try
    {
        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
    }
    catch (TypeInitializationException ex)
    {
        var reason = Innermost(ex);

        if (reason is FileNotFoundException)
            return;

        Error($"The type \"{originalType.FullName}\" threw a", reason);
    }
}

static Exception Innermost<T>(T ex)
    where T : Exception =>
    ex.InnerException is var inner && inner is null ? ex : inner is T nested ? Innermost(nested) : inner;

static IList<Type> ToTypes(Assembly asm)
{
    try
    {
        return asm.GetTypes();
    }
    catch (ReflectionTypeLoadException ex)
    {
        var reason = Innermost(ex);
        var types = ex.Types.Where(x => x is not null).ToList();
        var names = string.Join(", ", types.Select(x => x.Name));

        Warning($"The assembly \"{asm.GetName().Name}\" can only test {types.Count} types due to a", reason);
        Hint($"The types that were able to be tested include [{names}] due to afforementioned", reason);

        ex
            .LoaderExceptions
            .Where(x => x is not null)
            .GroupBy(x => x.Message)
            .ToList()
            .ForEach(x => Hint($"Potentially related exception (found {x.Count()}):", x.First()));

        return types;
    }
}
