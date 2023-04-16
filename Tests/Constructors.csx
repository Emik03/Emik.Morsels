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

static void Error(string message, Exception ex) => Console.WriteLine($"ERROR: {message} {ex.GetType()}: {ex.Message}");

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
        Error($"The type {type.Name} threw a", reason);
    }
}

static Exception Innermost(TypeInitializationException ex) =>
    ex.InnerException is var inner && inner is TypeInitializationException nested ? Innermost(nested) : inner;

static Type[] ToTypes(Assembly asm)
{
    try
    {
        return asm.GetTypes();
    }
    catch (ReflectionTypeLoadException ex)
    {
        return ex.Types;
    }
}
