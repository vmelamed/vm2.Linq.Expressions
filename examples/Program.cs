#!/usr/bin/env dotnet

// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

#:property TargetFramework=net10.0
#:project ../src/Linq.Expressions/Linq.Expressions.csproj

using static System.Console;
using static System.Text.Encoding;

using vm2.Linq.Expressions;

using static vm2.Linq.Expressions.Linq.ExpressionsApi;

Console.WriteLine("Linq.Expressions example");

Console.WriteLine(Echo("hello", "fallback"));
Console.WriteLine(Echo(null, "fallback"));
