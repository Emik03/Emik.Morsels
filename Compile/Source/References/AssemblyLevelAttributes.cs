// SPDX-License-Identifier: MPL-2.0
[assembly: CLSCompliant(true)]
#if ROSLYN
[assembly: NullGuard(ValidationFlags.None)]
#elif WAWA
[assembly: NullGuard(ValidationFlags.AllPublicArguments)]
#endif
