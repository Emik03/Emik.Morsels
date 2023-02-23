#!/bin/sh
dir="${1:?directory not specified, run like "$0" /path/to/out}"
if ! test -f "$dir"/FSharp/fsc.exe; then
    printf %s '#!/bin/sh
dotnet "$(dirname "$0")/fsc.dll" $1' > "$dir"/FSharp/fsc.exe
fi
chmod +x -- "$dir"/FSharp/fsc.dll "$dir"/FSharp/fsc.exe
