#!/bin/sh
morsels="$(dirname "$(dirname "$(realpath "$0")")")"
make -C "$morsels" Snippets/REPL.csx
command -v csharprepl || dotnet tool install -g csharprepl

exec csharprepl -r "nuget: CommunityToolkit.HighPerformance, 8.4.2" "nuget: JetBrains.Annotations, 2026.2.0" "nuget: TextCopy, 6.2.1" "$morsels/Snippets/REPL.csx"
