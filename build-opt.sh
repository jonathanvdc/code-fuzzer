#!/usr/bin/env bash

# Compiles and statically links code-fuzzer and mini-c-fuzzer
# A recent version of dsc is used to optimize mini-c-fuzzer's IR, 
# because Flame 0.8.8, on which ecsc is based (ATM), contains a 
# bug that breaks -O3 optimizations for code-fuzzer. 
# This has been fixed in later versions of Flame, but
# ecsc's Flame dependency has not been updated at the time of writing. 
# In the future, ecsc can be used instead.

ecsc code-fuzzer/code-fuzzer.ecsproj -platform ir -runtime clr -o code-fuzzer/bin/code-fuzzer.flo -repeat-command
ecsc mini-c-fuzzer/mini-c-fuzzer.ecsproj code-fuzzer/bin/code-fuzzer.flo -platform ir -runtime clr -o mini-c-fuzzer/bin/mini-c-fuzzer.flo -repeat-command
dsc mini-c-fuzzer/bin/mini-c-fuzzer.flo -platform clr -O3 -o mini-c-fuzzer/bin/mini-c-fuzzer.exe -repeat-command
