#!/bin/bash
go() {
    dirs=$(find "${DOTNET_ROOT:?DOTNET_ROOT must be defined}/$2" -maxdepth 1 -type d -name "[0-9]*" -exec basename {} \; | sort)
	  upper=$(echo "$dirs" | wc -l)

    for ((i = 1; i <= "$upper"; i++)); do
        prev=$(echo "$dirs" | sed "${i}q;d")
        next=$(echo "$dirs" | sed "$((i+1))q;d")

        if [ "$(echo "$prev" | cut -d "." -f 1)" = "$(echo "$next" | cut -d "." -f 1)" ]; then
            path="${DOTNET_ROOT:?DOTNET_ROOT must be defined}/$2/$prev"

            if [ "$1" = "--trash" ] || [ "$1" = "-t" ]; then
                echo "Moving $path to the recycling bin..."
                gio trash "$path"
            else
                echo "$path"
            fi
        fi
    done
}

if [ "$1" = "--help" ] || [ "$1" = "-h" ] || [ "$1" = "-ht" ] || [ "$1" = "-th" ]; then
    echo "Purpose: Detects older patches of .NET Core.

Usage:
    $0                Lists all unneeded frameworks.
    $0 -t/--trash     Moves all unneeded frameworks in the recycling bin.
    $0 -h/--help      Print this message.

Copyright © 2024 Emik"
    exit 0
elif [ "$1" != "" ] && [ "$1" != "-t" ] && [ "$1" != "--trash" ]; then
    echo "Unknown argument. Run \"$0 -h\" or \"$0 --help\" for help."
    exit 1
fi

go "$1" "host/fxr"
go "$1" "metadata/workloads"
go "$1" "shared/Microsoft.AspNetCore.App"
go "$1" "shared/Microsoft.NETCore.App"
go "$1" "packs/Microsoft.AspNetCore.App.Ref"
go "$1" "packs/Microsoft.NET.Runtime.MonoAOTCompiler.Task"
go "$1" "packs/Microsoft.NET.Runtime.MonoTargets.Sdk"
go "$1" "packs/Microsoft.NETCore.App.Host.linux-x64"
go "$1" "packs/Microsoft.NETCore.App.Ref"
go "$1" "packs/Microsoft.NETCore.App.Runtime.AOT.linux-x64.Cross.android-arm"
go "$1" "packs/Microsoft.NETCore.App.Runtime.AOT.linux-x64.Cross.android-arm64"
go "$1" "packs/Microsoft.NETCore.App.Runtime.AOT.linux-x64.Cross.android-x64"
go "$1" "packs/Microsoft.NETCore.App.Runtime.AOT.linux-x64.Cross.android-x86"
go "$1" "packs/Microsoft.NETCore.App.Runtime.Mono.android-arm"
go "$1" "packs/Microsoft.NETCore.App.Runtime.Mono.android-arm64"
go "$1" "packs/Microsoft.NETCore.App.Runtime.Mono.android-x64"
go "$1" "packs/Microsoft.NETCore.App.Runtime.Mono.android-x86"
go "$1" "sdk"
go "$1" "sdk-advertising"
go "$1" "sdk-manifests"
go "$1" "templates"
