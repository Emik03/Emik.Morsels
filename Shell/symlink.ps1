$files = Get-ChildItem "$PSScriptRoot\..\Content\Properties\"

foreach ($f in $files) {
    New-Item -Path "$pwd" -ItemType SymbolicLink -Value "$f"
}
