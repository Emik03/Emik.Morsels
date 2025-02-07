#!/bin/sh
morsels="$(dirname "$(dirname "$(realpath "$0")")")"
make -C "$morsels" Snippets/REPL.csx
command -v csharprepl || dotnet tool install -g csharprepl

exec csharprepl -r "nuget: CommunityToolkit.Common, 8.4.0" "nuget: CommunityToolkit.Diagnostics, 8.4.0" "nuget: CommunityToolkit.HighPerformance, 8.4.0" "nuget: FastGenericNew, 3.3.1" "nuget: JetBrains.Annotations, 2024.3" "nuget: TextCopy, 6.2.1" "$morsels/Snippets/REPL.csx"
