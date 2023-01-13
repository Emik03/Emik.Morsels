# Emik.Morsels

[![License](https://img.shields.io/github/license/Emik03/Emik.Morsels.svg?style=flat)](https://github.com/Emik03/Emik.Morsels/blob/main/LICENSE.md)

---

- [About](#about)
- [Usage](#usage)
- [Tree Shaking](#tree-shaking)
- [Supported Frameworks](#supported-frameworks)
- [Modified Standard Library](#modified-standard-library)
- [Contribute](#contribute)
- [License](#license)

---

## About

Inspired by [Code](https://github.com/shaynevanasperen/Code/); This is a project that contains utility code that don't belong together in a monolithic library. However, as oppose to [Code](https://github.com/shaynevanasperen/Code/), this repository has no source generators or roslyn analyzers to add these files. Instead, these are configurations which can be applied and synced to multiple projects simultaneously. This means that there is no need to add references, or figure out whether some type is needed or not, as all of them are added automatically.

## Usage

1. Download the [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0).
2. Download/clone the repository: `git clone https://github.com/Emik03/Emik.Morsels.git`
3. Copy-paste [Directory.Build.local.props.template](https://github.com/Emik03/Emik.Morsels/blob/main/Directory.Build.local.props.template) and name the new duplicate `Directory.Build.local.props`.
4. Modify the [PropertyGroup](https://learn.microsoft.com/en-us/visualstudio/msbuild/propertygroup-element-msbuild?view=vs-2022) of `Directory.Build.local.props`, which contains absolute paths that are system-dependent.
5. Add symbolic links to [global.json](https://github.com/Emik03/Emik.Morsels/blob/main/global.json), [stylecop.json](https://github.com/Emik03/Emik.Morsels/blob/main/stylecop.json), [Directory.Build.props](https://github.com/Emik03/Emik.Morsels/blob/main/Directory.Build.props), [Directory.Build.targets](https://github.com/Emik03/Emik.Morsels/blob/main/Directory.Build.targets), and `Directory.Build.local.props` for each project that references [Emik.Morsels](https://github.com/Emik03/Emik.Morsels).
    - You can alternatively have a folder for `Emik.Morsels` projects, where the root contains these four files, and all projects simply reside in nested folders. This way, you only need to set up the symlinks a single time.

* Windows:

```bat
mklink global.json <GLOBAL_PATH>
mklink stylecop.json <STYLECOP_PATH>
mklink Directory.Build.props <PROPS_PATH>
mklink Directory.Build.targets <TARGETS_PATH>
mklink Directory.Build.local.props <LOCAL_PROPS_PATH>
```

* Mac/Linux:

```shell
ln -s global.json <GLOBAL_PATH>
ln -s stylecop.json <STYLECOP_PATH>
ln -s Directory.Build.props <PROPS_PATH>
ln -s Directory.Build.targets <TARGETS_PATH>
ln -s Directory.Build.local.props <LOCAL_PROPS_PATH>
```

## Tree Shaking

Of course, if you add every type, it often means that a lot of unnecessary code is shipped along with your assembly, which is especially not ideal if you only require a single function from the set of all types. To counteract this, an additional dependency is placed on [Absence.Fody](https://github.com/Emik03/Absence.Fody/) which automatically looks for unused types that aren't explicitly marked as either [CompilerGeneratedAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.compilergeneratedattribute?view=net-7.0) or [ImplicitlyUsedAttribute](https://www.jetbrains.com/help/resharper/Reference__Code_Annotation_Attributes.html#UsedImplicitlyAttribute). As a result, any unused types outside of [Emik.Morsels](https://github.com/Emik03/Emik.Morsels) within the project will also be discarded during compile-time.

## Supported Frameworks

Emik.Morsels has framework-dependent dependencies to accomodate the following purposes:

| Framework                    | Purpose                                                                                                                               |
|------------------------------|---------------------------------------------------------------------------------------------------------------------------------------|
| .NET Framework 3.5           | [Keep Talking and Nobody Explodes](https://keeptalkinggame.com/) / [Unity 2017.4.22f1](https://unity3d.com/unity/whats-new/2017.4.22) |
| .NET Framework 4.5.2         | [Celeste](https://www.celestegame.com/) / [Everest](https://everestapi.github.io/)                                                    |
| .NET Standard 2.0            | [Fody](https://github.com/Fody/Home) / General                                                                                        |
| .NET Standard 2.1 / NET Core | General                                                                                                                               |

Different scripts and configurations within this repository will act accordingly to the current framework, such as polyfills, or by taking advantage of .NET 6+'s [`static abstract`](https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/static-abstract-interface-methods) features. If you do not own the products, you can simply target another framework. (e.g. .NET Framework 4.5.1)

Anything span-related will not be supported prior to .NET Standard 2.1, and .NET Framework 3.0 and older are considered too old to be necessarily supported in this repository.

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
