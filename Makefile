.DEFAULT: help
.PHONY: help
.SILENT: help

default: help

help:
	echo "Please use \`make <target>' where <target> is one of"
	echo "  help              to see this message"
	echo "  Snippets/REPL.csx to concatenate all of the source files into one large file a REPL environment could read"

Snippets/REPL.csx: Snippets/REPL.tt $(shell find Compile/Source/References/Emik -type f -name '*.cs')
	t4 Snippets/REPL.tt
