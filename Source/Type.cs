// <copyright file="Type.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
#if NET35
#pragma warning disable GlobalUsingsAnalyzer
extern alias ms;
extern alias emik; // ReSharper disable once RedundantUsingDirective.Global
global using EType = emik::System.Type;
global using Type = ms::System.Type;
#endif
