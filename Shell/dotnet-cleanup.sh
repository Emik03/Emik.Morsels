#!/bin/sh
print_help() {
    echo "Purpose: Detects older patches of .NET Core.

Usage:
    $0                Lists all unneeded frameworks.
    $0 -t/--trash     Moves all unneeded frameworks in the recycling bin.
    $0 -h/--help      Print this message.

Copyright © 2024-2025 Emik"
}

: "${DOTNET_ROOT:?DOTNET_ROOT must be defined}"
trash=0
help=0

while getopts ':ht-:' opt; do
    case "$opt" in
        -)
            case "$OPTARG" in
                help)
                    help=1
                    ;;
                trash)
                    trash=1
                    ;;
                *)
                    echo "Invalid option --$OPTARG" >&2
                    print_help
                    exit 2
                    ;;
            esac
        ;;
        h)
            help=1
            ;;
        t)
            trash=1
            ;;
        *)
            echo "Invalid option -$OPTARG" >&2
            print_help
            exit 2
            ;;
    esac
done

if [ "$help" = 1 ]; then
    print_help
    exit
fi

go() {
    dirs="$(find "$DOTNET_ROOT/$1" -maxdepth 1 -type d -name "[0-9]*" -exec basename {} \; | sort)"
    upper="$(echo "$dirs" | wc -l)"

    i=1
    while [ "$i" -le "$upper" ]; do
        prev="$(echo "$dirs" | sed "${i}q;d")"
        next="$(echo "$dirs" | sed "$(("$i" + 1))q;d")"

        if [ "$(echo "$prev" | cut -d "." -f 1)" = "$(echo "$next" | cut -d "." -f 1)" ]; then
            # Gives priority to stable releases over previews and release candidates.
            path="$DOTNET_ROOT/$1/$([ "${prev##*-*}" ] && ! [ "${next##*-*}" ] && echo "$next" || echo "$prev")"

            if [ "$trash" = 1 ]; then
                echo "Moving $path to the recycling bin..."
                gio trash "$path"
            else
                echo "$path"
            fi
        fi

        i="$(("$i" + 1))"
    done
}

go host/fxr
go metadata/workloads
go shared/Microsoft.AspNetCore.App
go shared/Microsoft.NETCore.App
go packs/Microsoft.AspNetCore.App.Ref
go packs/Microsoft.NET.Runtime.MonoAOTCompiler.Task
go packs/Microsoft.NET.Runtime.MonoTargets.Sdk
go packs/Microsoft.NETCore.App.Host.linux-x64
go packs/Microsoft.NETCore.App.Ref
go packs/Microsoft.NETCore.App.Runtime.AOT.linux-x64.Cross.android-arm
go packs/Microsoft.NETCore.App.Runtime.AOT.linux-x64.Cross.android-arm64
go packs/Microsoft.NETCore.App.Runtime.AOT.linux-x64.Cross.android-x64
go packs/Microsoft.NETCore.App.Runtime.AOT.linux-x64.Cross.android-x86
go packs/Microsoft.NETCore.App.Runtime.Mono.android-arm
go packs/Microsoft.NETCore.App.Runtime.Mono.android-arm64
go packs/Microsoft.NETCore.App.Runtime.Mono.android-x64
go packs/Microsoft.NETCore.App.Runtime.Mono.android-x86
go sdk
go sdk-advertising
go sdk-manifests
go templates
