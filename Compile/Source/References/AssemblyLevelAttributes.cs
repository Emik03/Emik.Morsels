// SPDX-License-Identifier: MPL-2.0
[assembly: CLSCompliant(true)]
#if NET452
[assembly: RemoveReference("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
#elif ROSLYN
[assembly: NullGuard(ValidationFlags.None)]
#elif WAWA
[assembly: NullGuard(ValidationFlags.AllPublicArguments)]
#endif
