// SPDX-License-Identifier: MPL-2.0
#pragma warning disable GlobalUsingsAnalyzer
// While this import may seem redundant due to the reference of Emik.SourceGenerators.Tattoo,
// this is actually strictly necessary to allow System.Text.RegularExpressions.Generator to
// function properly, since it doesn't enjoy global imports that come from a source generator.
global using System.Text.RegularExpressions;
