// SPDX-License-Identifier: MPL-2.0
using System.Reflection;
using System.Runtime.CompilerServices;

try
{
    if (args is not [var path] || Assembly.LoadFile(path) is not { } asm)
        return;

    Iterate(asm);
}
catch (Exception ex)
{
    Error("REPL threw", ex);
}

Environment.Exit(0);

static void Error(string message, Exception ex) => Log("ERROR", message, ex);

static void Warning(string message, Exception ex) => Log("WARNING", message, ex);

static void Log(string prefix, string message, Exception ex) =>
    Console.WriteLine($"{prefix}: {message} {ex.GetType()}: {ex.Message}");

static void Iterate(Assembly asm) =>
    ToTypes(asm)
        .Where(x => x is not null)
        .ToList()
        .ForEach(x => RunClassConstructor(x));

static void RunClassConstructor(Type type)
{
    try
    {
        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
    }
    catch (TypeInitializationException ex)
    {
        var reason = Innermost(ex);

        if (reason is FileNotFoundException)
            return;

        Error($"The type \"{type.Name}\" threw a", reason);
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

        Warning($"The assembly \"{asm.GetName().Name}\" can only test {types.Count} types ({names}) due to a", reason);
        return types;
    }
}
