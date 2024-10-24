#!/bin/sh
morsels="$(dirname "$(dirname "$(realpath "$0")")")"
make -C "$morsels" Snippets/REPL.csx
command -v csharprepl || dotnet tool install -g csharprepl

exec csharprepl -r "nuget: CommunityToolkit.Common, 8.3.2" "nuget: CommunityToolkit.Diagnostics, 8.3.2" "nuget: CommunityToolkit.HighPerformance, 8.3.2" "nuget: Emik.Results, 4.0.0" "nuget: FastGenericNew, 3.3.1" "nuget: JetBrains.Annotations, 2024.3.0-eap1" "nuget: TextCopy, 6.2.1" "$morsels/Snippets/REPL.csx"
