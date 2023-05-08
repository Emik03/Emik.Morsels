#!/bin/sh
morsels="$(dirname "$(dirname "$(realpath "$0")")")"
above="$(dirname $morsels)"

csharprepl -r "nuget: JetBrains.Annotations, 2023.2.0-eap2" "nuget: Emik.Results, 1.3.1" "nuget: Emik.Unions, 1.1.4" "$morsels/Experimental/REPL.csx"

