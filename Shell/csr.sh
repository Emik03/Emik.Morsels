#!/bin/sh
morsels="$(dirname "$(dirname "$(realpath "$0")")")"
make -C "$morsels" Snippets/REPL.csx
command -v csharprepl || dotnet tool install -g csharprepl

exec csharprepl -r "nuget: CommunityToolkit.HighPerformance, 8.4.2" "nuget: JetBrains.Annotations, 2025.2.4" "nuget: TextCopy, 6.2.1" "$morsels/Snippets/REPL.csx"
