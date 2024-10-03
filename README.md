# Emik.Morsels

[![License](https://img.shields.io/github/license/Emik03/Emik.Morsels.svg?color=6272a4&style=for-the-badge)](https://github.com/Emik03/Emik.Morsels/blob/main/LICENSE)

---

- [About](#about)
- [Usage](#usage)
- [Family](#family)
- [Tree Shaking](#tree-shaking)
- [Supported Frameworks](#supported-frameworks)
- [Modified Standard Library](#modified-standard-library)
- [Contribute](#contribute)
- [License](#license)

---

## About

Inspired by [Code](https://github.com/shaynevanasperen/Code/); This is a project that contains utility code that don't belong together in a monolithic library. However, as opposed to [Code](https://github.com/shaynevanasperen/Code/), this repository has no source generators or roslyn analyzers to add these files. Instead, these are configurations which can be applied and synced to multiple projects simultaneously. This means that there is no need to add references, or figure out whether some type is needed or not, as all of them are added automatically.

## Usage

1. Download the [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0).
2. Download/clone the repository: `git clone https://github.com/Emik03/Emik.Morsels.git`
3. Copy-paste [Directory.Build.local.props.template](https://raw.githubusercontent.com/Emik03/Emik.Morsels/main/Content/Properties/Directory.Build.local.props.template) and name the new duplicate `Directory.Build.local.props`.
4. Modify the [PropertyGroup](https://learn.microsoft.com/en-us/visualstudio/msbuild/propertygroup-element-msbuild?view=vs-2022) of `Directory.Build.local.props`, which contains absolute paths that are system-dependent.
5. Execute [this command on Windows](https://github.com/Emik03/Emik.Morsels/blob/main/Shell/symlink.bat) or [this command on Mac/Linux](https://github.com/Emik03/Emik.Morsels/blob/main/Shell/symlink.sh) with each [Emik.Morsels](https://github.com/Emik03/Emik.Morsels) project as the working directory to symlink everything together.
    - You can alternatively have a folder for `Emik.Morsels` projects, and all projects simply reside in nested folders. This way, you only need to set up the symlinks a single time on the root folder since [search scope applies to parent folders](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-your-build?view=vs-2022#search-scope) as well.

## Family

The following block contains officially maintained projects that implement [Emik.Morsels](https://github.com/Emik03/Emik.Morsels):

[![Absence.Fody](https://img.shields.io/github/last-commit/Emik03/Absence.Fody?style=for-the-badge&logo=GitHub&label=Absence.Fody&color=ffb86c)](https://github.com/Emik03/Absence.Fody)
[![Divorce.Fody](https://img.shields.io/github/last-commit/Emik03/Divorce.Fody?style=for-the-badge&logo=GitHub&label=Divorce.Fody&color=f1fa8c)](https://github.com/Emik03/Divorce.Fody)
[![Emik.Analyzers.Matches](https://img.shields.io/github/last-commit/Emik03/Emik.Analyzers.Matches?style=for-the-badge&logo=GitHub&label=Emik.Analyzers.Matches&color=ffb86c)](https://github.com/Emik03/Emik.Analyzers.Matches)
[![Emik.Kebnekaise.Gramophones](https://img.shields.io/github/last-commit/Emik03/Emik.Kebnekaise.Gramophones?style=for-the-badge&logo=GitHub&label=Emik.Kebnekaise.Gramophones&color=f1fa8c)](https://github.com/Emik03/Emik.Kebnekaise.Gramophones)
[![Emik.Kebnekaise.Pistons](https://img.shields.io/github/last-commit/Emik03/Emik.Kebnekaise.Pistons?style=for-the-badge&logo=GitHub&label=Emik.Kebnekaise.Pistons&color=ffb86c)](https://github.com/Emik03/Emik.Kebnekaise.Pistons)
[![Emik.Morsels.ProjectTemplates](https://img.shields.io/github/last-commit/Emik03/Emik.Morsels.ProjectTemplates?style=for-the-badge&logo=GitHub&label=Emik.Morsels.ProjectTemplates&color=f1fa8c)](https://github.com/Emik03/Emik.Morsels.ProjectTemplates)
[![Emik.Numerics.Fractions](https://img.shields.io/github/last-commit/Emik03/Emik.Numerics.Fractions?style=for-the-badge&logo=GitHub&label=Emik.Numerics.Fractions&color=ffb86c)](https://github.com/Emik03/Emik.Numerics.Fractions)
[![Emik.Results](https://img.shields.io/github/last-commit/Emik03/Emik.Results?style=for-the-badge&logo=GitHub&label=Emik.Results&color=f1fa8c)](https://github.com/Emik03/Emik.Results)
[![Emik.Rhainterop](https://img.shields.io/github/last-commit/Emik03/Emik.Rhainterop?style=for-the-badge&logo=GitHub&label=Emik.Rhainterop&color=ffb86c)](https://github.com/Emik03/Emik.Rhainterop)
[![Emik.Rubbish](https://img.shields.io/github/last-commit/Emik03/Emik.Rubbish?style=for-the-badge&logo=GitHub&label=Emik.Rubbish&color=ffb86c)](https://github.com/Emik03/Emik.Rubbish)
[![Emik.SourceGenerators.Choices](https://img.shields.io/github/last-commit/Emik03/Emik.SourceGenerators.Choices?style=for-the-badge&logo=GitHub&label=Emik.SourceGenerators.Choices&color=f1fa8c)](https://github.com/Emik03/Emik.SourceGenerators.Choices)
[![Emik.SourceGenerators.Implicit](https://img.shields.io/github/last-commit/Emik03/Emik.SourceGenerators.Implicit?style=for-the-badge&logo=GitHub&label=Emik.SourceGenerators.Implicit&color=f1fa8c)](https://github.com/Emik03/Emik.SourceGenerators.Implicit)
[![Emik.SourceGenerators.Tattoo](https://img.shields.io/github/last-commit/Emik03/Emik.SourceGenerators.Tattoo?style=for-the-badge&logo=GitHub&label=Emik.SourceGenerators.Tattoo&color=ffb86c)](https://github.com/Emik03/Emik.SourceGenerators.Tattoo)
[![Emik.SourceGenerators.TheSquareHole](https://img.shields.io/github/last-commit/Emik03/Emik.SourceGenerators.TheSquareHole?style=for-the-badge&logo=GitHub&label=Emik.SourceGenerators.TheSquareHole&color=f1fa8c)](https://github.com/Emik03/Emik.SourceGenerators.TheSquareHole)
[![Emik.Unions](https://img.shields.io/github/last-commit/Emik03/Emik.Unions?style=for-the-badge&logo=GitHub&label=Emik.Unions&color=ffb86c)](https://github.com/Emik03/Emik.Unions)
[![wawa](https://img.shields.io/github/last-commit/Emik03/wawa?style=for-the-badge&logo=GitHub&label=wawa&color=f1fa8c)](https://github.com/Emik03/wawa)

## Tree Shaking

Of course, if you add every type, it often means that a lot of unnecessary code is shipped along with your assembly, which is especially not ideal if you only require a single function from the set of all types. To counteract this, an additional dependency is placed on [Absence.Fody](https://github.com/Emik03/Absence.Fody/) which automatically looks for unused types that aren't explicitly marked as either [CompilerGeneratedAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.compilergeneratedattribute?view=net-7.0) or [ImplicitlyUsedAttribute](https://www.jetbrains.com/help/resharper/Reference__Code_Annotation_Attributes.html#UsedImplicitlyAttribute). As a result, any unused types outside of [Emik.Morsels](https://github.com/Emik03/Emik.Morsels) within the project will also be discarded during compile-time.

## Supported Frameworks

Emik.Morsels currently supports the following frameworks:

- .NET Framework: 2.0 - 4.8.1
- .NET Standard: 1.0 - 2.1
- .NET Core: 1.0 - 3.1
- .NET: 5.0 - 8.0

Different scripts and configurations within this repository will act accordingly to the current framework, such as polyfills, or by taking advantage of .NET 6+'s [`static abstract`](https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/static-abstract-interface-methods) features.

## Modified Standard Library

Emik.Morsels also contains an IL-modified version of the .NET Framework 3.5 reference assembly. This is a reference assembly that has a weaved `System.Type.op_Equality`, allowing you to use record types without [Emik.Net20Records](https://github.com/Emik03/Emik.Net20Records).

This method is attributed with [`InlineAttribute`](https://github.com/oleg-st/InlineMethod.Fody) to ensure removal to the modified reference, changing it to a simple `ceq` (`(object)lh == rh`) instruction.

The code that was used to generate this assembly can be found [here](https://gist.github.com/Emik03/d88efe49a874b7d5f45e4bfb96fa541f).

There are also symlinks for `System.dll` and `System.Numerics.dll` that both point to said standard library, meant as a silly workaround for expecting those two files when compiling in F#, since `System.Numerics` doesn't exist in .NET 3.5.

## Contribute

Despite the types being marked internal, all scripts here act as an evolving API. As a result, feedback is much more likely to be applied as breaking changes can be more easily made without breaking preexisting code.

Issues and pull requests are welcome to help this repository be the best it can be.

## License

This repository — which includes every source file separately — fall under the [MPL-2 license](https://www.mozilla.org/en-US/MPL/2.0/).

You may copy individual or multiple source files, granted that you keep the copyright license, and disclose the source back to this repository.
