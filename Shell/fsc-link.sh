#!/bin/sh
dir="${1:?directory not specified, run like "$0" /path/to/out}"
chmod +x $dir/FSharp/fsc.dll && ln -s $dir/FSharp/fsc.dll $dir/FSharp/fsc.exe && chmod +x $dir/FSharp/fsc.exe
