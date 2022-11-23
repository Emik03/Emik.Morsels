# Emik.Morsels

---

- [About](#about)
- [Tree Shaking](#tree-shaking)
- [Usage](#usage)
- [Contribute](#contribute)
- [License](#license)

---

## About

Inspired by [Code](https://github.com/shaynevanasperen/Code/); This is a project that contains utility code that don't belong together in a monolithic library. However, as oppose to [Code](https://github.com/shaynevanasperen/Code/), this repository has no source generators or roslyn analyzers to add these files. Instead, these are configurations which can be applied and synced to multiple projects simultaneously. This means that there is no need to add references, or figure out whether some type is needed or not, as all of them are added automatically.

## Tree Shaking

Of course, if you add every type, it often means that a lot of unnecessary code is shipped along with your assembly, which is especially not ideal if you only require a single function from the set of all types. To counteract this, an additional dependency is placed on [Absence.Fody](https://github.com/Emik03/Absence.Fody/) which automatically looks for unused types that aren't explicitly marked as either [CompilerGeneratedAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.compilergeneratedattribute?view=net-7.0) or [ImplicitlyUsedAttribute](https://www.jetbrains.com/help/resharper/Reference__Code_Annotation_Attributes.html#UsedImplicitlyAttribute). As a result, any unused types outside of [Emik.Morsels](https://github.com/Emik03/Emik.Morsels) within the project will also be discarded during compile-time.

## Usage

1. Download/clone the repository: `git clone https://github.com/Emik03/Emik.Morsels.git`
2. Modify the first [PropertyGroup](https://learn.microsoft.com/en-us/visualstudio/msbuild/propertygroup-element-msbuild?view=vs-2022) of [Directory.Build.props](https://github.com/Emik03/Emik.Morsels/blob/main/Directory.Build.props), which contains absolute paths that are system-dependent.
3. Add symbolic links to [Directory.Build.props](https://github.com/Emik03/Emik.Morsels/blob/main/Directory.Build.props) and [Directory.Build.targets](https://github.com/Emik03/Emik.Morsels/blob/main/Directory.Build.targets) for each project that references [Emik.Morsels](https://github.com/Emik03/Emik.Morsels).

* Windows:

```bat
mklink Directory.Build.props <PROPS_PATH>
mklink Directory.Build.targets <TARGETS_PATH>
```

* Mac/Linux:

```shell
ln -s Directory.Build.props <PROPS_PATH>
ln -s Directory.Build.targets <TARGETS_PATH>
```

## Contribute

Despite the types being marked internal, all scripts here act as an evolving API. As a result, feedback is much more likely to be applied as breaking changes can be more easily made without breaking preexisting code.

Issues and pull requests are welcome to help this repository be the best it can be.

## License

This repository — which includes every source file separately — fall under the [MPL-2 license](https://www.mozilla.org/en-US/MPL/2.0/).

You may copy individual or multiple source files, granted that you keep the copyright license, and disclose the source back to this repository.
