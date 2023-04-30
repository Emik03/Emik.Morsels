#!/bin/sh
csharprepl -r "nuget:JetBrains.Annotations, 2022.3.1" "nuget:Emik.Results, 1.0.5" "nuget:Emik.Unions, 1.0.4" "$(dirname "$(dirname "$(realpath "$0")")")/Experimental/REPL.csx"

