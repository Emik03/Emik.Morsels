#!/bin/sh
dir="${1:?directory not specified, run like "$0" /path/to/out}"
mkdir "$dir" && cp -r ../FSharpProject.skel/. "$dir" && mv "$dir"/FSharpProject.fsproj "$dir"/"$(basename -- "$dir")".fsproj
