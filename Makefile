.PHONY: help repl

default: help

help:
	@echo "Please use \`make <target>' where <target> is one of"
	@echo "  help              to see this message"
	@echo "  repl              to launch CSharpRepl with Emik.Morsels as an embedded dependency"
	@echo "  Snippets/REPL.csx to concatenate all of the source files into one large file a REPL environment could read"

repl: Snippets/REPL.csx
	command -v csharprepl || dotnet tool install -g csharprepl
	csharprepl -r "nuget: CommunityToolkit.Common, 8.2.1" "nuget: CommunityToolkit.Diagnostics, 8.2.1" "nuget: CommunityToolkit.HighPerformance, 8.2.1" "nuget: TextCopy, 6.2.1" "nuget: JetBrains.Annotations, 2023.3.0-eap3" "nuget: Emik.Results, 2.1.1" "$(realpath Snippets/REPL.csx)"

Snippets/REPL.csx: $(shell find Compile/Source/References/Emik -type f -name '*.cs')
	t4 Snippets/REPL.tt

