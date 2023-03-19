// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides methods for basic benchmarking.</summary>
public static class Benchy
{
    static readonly Stopwatch s_stopwatch = new();

    /// <summary>Benchmarks the provided delegate, with the value.</summary>
    /// <param name="action">The delegate to benchmark.</param>
    /// <param name="actual">The amount of times to benchmark it within the actual stage.</param>
    /// <param name="warmup">The amount of times to benchmark it within the warmup stage.</param>
    /// <returns>The list of integers that contains the number of ticks for each round.</returns>
    public static IList<long> Bench(
        Action action,
        [NonNegativeValue] int actual = 100,
        [NonNegativeValue] int warmup = 20
    )
    {
        lock (s_stopwatch)
            return Inner(action, actual, warmup);
    }

    static IList<long> Inner(Action del, [NonNegativeValue] int actual = 100, [NonNegativeValue] int warmup = 20)
    {
        if (actual <= 0)
            throw new ArgumentOutOfRangeException(nameof(actual), actual, "value must be non-negative");

        if (warmup <= 0)
            throw new ArgumentOutOfRangeException(nameof(warmup), warmup, "value must be non-negative");

        long
            invocations = 1,
            sum = 0;

        for (var i = 0; i < warmup; i++)
        {
            var (mean, total) = Measure(del, invocations);

            if (total / 2 < Stopwatch.Frequency)
            {
                invocations *= 2;
                i--;
                continue;
            }

            sum += mean;
        }

        var average = sum / warmup;

        List<long> results = new();

        for (var i = 0; i < actual; i++)
        {
            var (mean, _) = Measure(del, average);
            results.Add(mean);
        }

        return results;
    }

    static Test Measure(Action del, long invocations)
    {
        const long Loop = 32;

        s_stopwatch.Reset();
        s_stopwatch.Start();

        for (long i = 0; i < invocations / Loop; i++)
        {
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
            del();
        }

        s_stopwatch.Stop();

        var total = TimeSpan.FromSeconds(s_stopwatch.ElapsedTicks / (double)Stopwatch.Frequency);
        var mean = total / (invocations / Loop * Loop);
        return new(mean, total);
    }

    static string Format(double time) =>
        time switch
        {
            >= 0.1 => $"{time:0.###} s.",
            >= 0.000_1 => $"{time * 1_000:0.###} ms.",
            >= 0.000_000_1 => $"{time * 1_000_000:0.###} us.",
            _ => $"{time * 1_000_000_000:0.###} ns.",
        };

    [StructLayout(LayoutKind.Auto)]
    record struct Test(long Mean, long Total);
}
