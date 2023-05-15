#!/bin/sh
morsels="$(dirname "$(dirname "$(realpath "$0")")")"
above="$(dirname $morsels)"

csharprepl -r "nuget: JetBrains.Annotations, 2023.2.0-eap2" "nuget: Emik.Results, 2.0.0" "nuget: Emik.Unions, 2.0.0" "$morsels/Experimental/REPL.csx"

