#!/bin/sh
morsels="$(dirname "$(dirname "$(realpath "$0")")")"
make -C "$morsels" repl
