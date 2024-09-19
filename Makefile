.DEFAULT: help
.PHONY: help repl
.SILENT: help

default: help

help:
	echo "Please use \`make <target>' where <target> is one of"
	echo "  help              to see this message"
	echo "  repl              to launch CSharpRepl with Emik.Morsels as an embedded dependency"
	echo "  Snippets/REPL.csx to concatenate all of the source files into one large file a REPL environment could read"

repl: Snippets/REPL.csx
	command -v csharprepl || dotnet tool install -g csharprepl
	csharprepl -r "nuget: CommunityToolkit.Common, 8.3.2" "nuget: CommunityToolkit.Diagnostics, 8.3.2" "nuget: CommunityToolkit.HighPerformance, 8.3.2" "nuget: Emik.Results, 4.0.0" "nuget: FastGenericNew, 3.3.1" "nuget: JetBrains.Annotations, 2024.2.0" "nuget: TextCopy, 6.2.1" "nuget: Serilog, 4.0.2-dev-02226" "nuget: Serilog.Formatting.Compact, 3.0.0" "nuget: Serilog.Sinks.Console, 6.0.0" "nuget: Serilog.Sinks.File, 6.0.0" "$(realpath Snippets/REPL.csx)"

Snippets/REPL.csx: Snippets/REPL.tt $(shell find Compile/Source/References/Emik -type f -name '*.cs')
	t4 Snippets/REPL.tt
