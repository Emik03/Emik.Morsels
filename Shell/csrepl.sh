#!/bin/sh
morsels="$(dirname "$(dirname "$(realpath "$0")")")"
above="$(dirname $morsels)"

csharprepl -r "nuget: JetBrains.Annotations, 2023.2.0-eap2" "$above/Emik.Unions/Emik.Unions.csproj" "$morsels/Experimental/REPL.csx"

