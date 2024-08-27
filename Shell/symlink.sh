#!/bin/sh
find Emik.Morsels/Content/Properties/ -type f -print0 | xargs -0 -l1 ln -s
