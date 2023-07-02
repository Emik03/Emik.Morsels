#!/bin/sh
morsels="$(dirname "$(dirname "$(realpath "$0")")")"
above="$(dirname $morsels)"

csharprepl -r "nuget: TextCopy, 6.2.1" "nuget: JetBrains.Annotations, 2023.2.0-eap4" "nuget: Emik.Results, 2.1.1" "nuget: Emik.Unions, 2.1.1" "$morsels/Snippets/REPL.csx"

