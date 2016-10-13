#!/usr/bin/env bash

# Compiles and statically links code-fuzzer and mini-c-fuzzer

ecsc code-fuzzer/code-fuzzer.ecsproj -platform ir -runtime clr -o code-fuzzer/bin/code-fuzzer.flo -repeat-command
ecsc mini-c-fuzzer/mini-c-fuzzer.ecsproj code-fuzzer/bin/code-fuzzer.flo -platform clr -o mini-c-fuzzer/bin/mini-c-fuzzer.exe -repeat-command
