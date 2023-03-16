#!/bin/sh
dir="${1:?directory not specified, run like "$0" /path/to/out}"
mkdir "$dir" && cp -r ../CSharpProject.skel/. "$dir" && mv "$dir"/CSharpProject.csproj "$dir"/"$(basename -- "$dir")".csproj && mv "$dir"/CSharpProject.Generated.Tests/ "$dir"/"$(basename -- "$dir")".Generated.Tests/ && mv "$dir"/"$(basename -- "$dir")".Generated.Tests/CSharpProject.Generated.Tests.csproj "$dir"/"$(basename -- "$dir")".Generated.Tests/"$(basename -- "$dir")".Generated.Tests.csproj

