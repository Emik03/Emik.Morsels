#!/bin/sh
:"${1:?line number not specified, run like "$0" <line> [column], or "$0" [line],[column]}"
morsels="$(dirname "$(dirname "$(realpath "$0")")")"
line="$(echo "$1" | cut -d ',' -f 1)"
column="$(echo "$1" | cut -d ',' -f 2)"
column="${column:-1}"

if test -z "$column"; then
  column="$2"
fi

if ! test "$line" -gt 0; then
  echo "line must be a positive integer" > /dev/stderr
  exit 1
fi

if ! test "$column" -gt 0; then
  echo "column must be a positive integer" > /dev/stderr
  exit 1
fi

micro "$morsels/Snippets/REPL.csx" "+$line:$column"
