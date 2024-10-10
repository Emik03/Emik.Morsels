#!/bin/sh
morsels="$(dirname "$(dirname "$(realpath "$0")")")"
make -C "$morsels" Snippets/REPL.csx
command -v csharprepl || dotnet tool install -g csharprepl

exec csharprepl -r "nuget: CommunityToolkit.Common, 8.3.2" "nuget: CommunityToolkit.Diagnostics, 8.3.2" "nuget: CommunityToolkit.HighPerformance, 8.3.2" "nuget: Emik.Results, 4.0.0" "nuget: FastGenericNew, 3.3.1" "nuget: JetBrains.Annotations, 2024.2.0" "nuget: TextCopy, 6.2.1" "nuget: Serilog, 4.0.2" "nuget: Serilog.Formatting.Compact, 3.0.0" "nuget: Serilog.Sinks.Console, 6.0.0" "nuget: Serilog.Sinks.File, 6.0.0" "$(realpath Snippets/REPL.csx)"
