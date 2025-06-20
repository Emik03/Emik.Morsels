// SPDX-License-Identifier: MPL-2.0

// ReSharper disable ArrangeStaticMemberQualifier CheckNamespace ClassNeverInstantiated.Global EmptyNamespace RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer
#if !KTANE && !WAWA
global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;
#endif

namespace System.Diagnostics.CodeAnalysis
{
#if NETFRAMEWORK || NETSTANDARD && !NETSTANDARD2_1_OR_GREATER
#if !WAWA
    /// <summary>Specifies that null is allowed as an input even if the corresponding type disallows it.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    sealed partial class AllowNullAttribute : Attribute;
#endif
    /// <summary>Specifies that null is disallowed as an input even if the corresponding type allows it.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    sealed partial class DisallowNullAttribute : Attribute;

    /// <summary>Applied to a method that will never return under any circumstance.</summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed partial class DoesNotReturnAttribute : Attribute;

    /// <summary>
    /// Specifies that the method will not return if the associated Boolean parameter is passed the specified value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    sealed partial class DoesNotReturnIfAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoesNotReturnIfAttribute"/> class
        /// with the specified parameter value.
        /// </summary>
        /// <param name="parameterValue">
        /// The condition parameter value. Code after the method will be considered unreachable
        /// by diagnostics if the argument to the associated parameter matches this value.
        /// </param>
        public DoesNotReturnIfAttribute(bool parameterValue) => ParameterValue = parameterValue;

        /// <summary>
        /// Gets a value indicating whether the condition parameter value
        /// is <see langword="true"/> or <see langword="false"/>.
        /// </summary>
        public bool ParameterValue { get; }
    }

    /// <summary>Specifies that an output may be null even if the corresponding type disallows it.</summary>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue
    )]
    sealed partial class MaybeNullAttribute : Attribute;

    /// <summary>
    /// Specifies that an output will not be null even if the corresponding type allows it.
    /// Specifies that an input argument was not null when the call returns.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue
    )]
    sealed partial class NotNullAttribute : Attribute;

    /// <summary>
    /// Specifies that when a method returns <see cref="ReturnValue"/>,
    /// the parameter may be null even if the corresponding type disallows it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    sealed partial class MaybeNullWhenAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaybeNullWhenAttribute"/> class
        /// with the specified return value condition.
        /// </summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter may be null.
        /// </param>
        public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

        /// <summary>
        /// Gets a value indicating whether the return value condition
        /// is <see langword="true"/> or <see langword="false"/>.
        /// </summary>
        public bool ReturnValue { get; }
    }

    /// <summary>
    /// Specifies that when a method returns <see cref="ReturnValue"/>,
    /// the parameter will not be null even if the corresponding type allows it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    sealed partial class NotNullWhenAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullWhenAttribute"/>
        /// class with the specified return value condition.
        /// </summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter will not be null.
        /// </param>
        public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

        /// <summary>
        /// Gets a value indicating whether the return value condition is <see langword="true"/> or <see langword="false"/>.
        /// </summary>
        public bool ReturnValue { get; }
    }

    /// <summary>Specifies that the output will be non-null if the named parameter is non-null.</summary>
    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue,
        AllowMultiple = true
    )]
    sealed partial class NotNullIfNotNullAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullIfNotNullAttribute"/> class
        /// with the associated parameter name.
        /// </summary>
        /// <param name="parameterName">
        /// The associated parameter name.
        /// The output will be non-null if the argument to the parameter specified is non-null.
        /// </param>
        public NotNullIfNotNullAttribute(string parameterName) => ParameterName = parameterName;

        /// <summary>Gets the associated parameter name.</summary>
        public string ParameterName { get; }
    }
#endif
#if NETFRAMEWORK || NETSTANDARD
    /// <summary>
    /// Specifies that the method or property will ensure that the
    /// listed field and property members have not-null values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed partial class MemberNotNullAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberNotNullAttribute"/>
        /// class with a field or property member.
        /// </summary>
        /// <param name="member">
        /// The field or property member that is promised to be not-null.
        /// </param>
        public MemberNotNullAttribute(string member) => Members = [member];

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberNotNullAttribute"/> class
        /// with the list of field and property members.
        /// </summary>
        /// <param name="members">
        /// The list of field and AttributeTargets.Property members that are promised to be not-null.
        /// </param>
        public MemberNotNullAttribute(params string[] members) => Members = members;

        /// <summary>Gets field or AttributeTargets.Property member names.</summary>
        public string[] Members { get; }
    }
#endif
#if !NET5_0_OR_GREATER
    /// <summary>
    /// Specifies that the method or property will ensure that the listed field and property members
    /// have not-null values when returning with the specified return value condition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed partial class MemberNotNullWhenAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberNotNullWhenAttribute"/> class
        /// with the specified return value condition and a field or property member.
        /// </summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter will not be null.
        /// </param>
        /// <param name="member">The field or property member that is promised to be not-null.</param>
        public MemberNotNullWhenAttribute(bool returnValue, string member)
        {
            ReturnValue = returnValue;
            Members = [member];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberNotNullWhenAttribute"/> class
        /// with the specified return value condition and list of field and property members.
        /// </summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter will not be null.
        /// </param>
        /// <param name="members">
        /// The list of field and property members that are promised to be not-null.
        /// </param>
        public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
        {
            ReturnValue = returnValue;
            Members = members;
        }

        /// <summary>
        /// Gets a value indicating whether the return value condition
        /// is <see langword="true"/> or <see langword="false"/>.
        /// </summary>
        public bool ReturnValue { get; }

        /// <summary>Gets field or property member names.</summary>
        public string[] Members { get; }
    }
#endif
#if !NET7_0_OR_GREATER
    /// <summary>
    /// Specifies that this constructor sets all required members for the current type,
    /// and callers do not need to set any required members themselves.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    sealed partial class SetsRequiredMembersAttribute : Attribute;

    /// <summary>Specifies the syntax used in a string.</summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
    sealed partial class StringSyntaxAttribute : Attribute
    {
        /// <summary>The syntax identifier for strings containing composite formats for string formatting.</summary>
        public const string CompositeFormat = nameof(CompositeFormat);

        /// <summary>The syntax identifier for strings containing date format specifiers.</summary>
        public const string DateOnlyFormat = nameof(DateOnlyFormat);

        /// <summary>The syntax identifier for strings containing date and time format specifiers.</summary>
        public const string DateTimeFormat = nameof(DateTimeFormat);

        /// <summary>The syntax identifier for strings containing <see cref="Enum"/> format specifiers.</summary>
        public const string EnumFormat = nameof(EnumFormat);

        /// <summary>The syntax identifier for strings containing <see cref="Guid"/> format specifiers.</summary>
        public const string GuidFormat = nameof(GuidFormat);

        /// <summary>The syntax identifier for strings containing JavaScript Object Notation (JSON).</summary>
        public const string Json = nameof(Json);

        /// <summary>The syntax identifier for strings containing numeric format specifiers.</summary>
        public const string NumericFormat = nameof(NumericFormat);

        /// <summary>The syntax identifier for strings containing regular expressions.</summary>
        public const string Regex = nameof(Regex);

        /// <summary>The syntax identifier for strings containing time format specifiers.</summary>
        public const string TimeOnlyFormat = nameof(TimeOnlyFormat);

        /// <summary>The syntax identifier for strings containing <see cref="TimeSpan"/> format specifiers.</summary>
        public const string TimeSpanFormat = nameof(TimeSpanFormat);

        /// <summary>The syntax identifier for strings containing URIs.</summary>
        public const string Uri = nameof(Uri);

        /// <summary>The syntax identifier for strings containing XML.</summary>
        public const string Xml = nameof(Xml);

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSyntaxAttribute"/> class
        /// with the identifier of the syntax used.
        /// </summary>
        /// <param name="syntax">The syntax identifier.</param>
        public StringSyntaxAttribute(string syntax)
        {
            Syntax = syntax;
            Arguments = [null];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSyntaxAttribute"/> class
        /// with the identifier of the syntax used.
        /// </summary>
        /// <param name="syntax">The syntax identifier.</param>
        /// <param name="arguments">Optional arguments associated with the specific syntax employed.</param>
        public StringSyntaxAttribute(string syntax, params object?[] arguments)
        {
            Syntax = syntax;
            Arguments = arguments;
        }

        /// <summary>Gets the identifier of the syntax used.</summary>
        public string Syntax { get; }

        /// <summary>Gets the optional arguments associated with the specific syntax employed.</summary>
        public object?[] Arguments { get; }
    }

    /// <summary>Used to indicate a byref escapes and is not scoped.</summary>
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter,
        Inherited = false
    )]
    sealed partial class UnscopedRefAttribute : Attribute;
#endif
#if !NET8_0_OR_GREATER
    /// <summary>Indicates that an API is experimental, and it may change in the future.</summary>
    /// <remarks><para>
    /// This attribute allows call sites to be flagged with a diagnostic that indicates that an experimental
    /// feature is used. Authors can use this attribute to ship preview features in their assemblies.
    /// </para></remarks>
    [AttributeUsage(
        AttributeTargets.Assembly |
        AttributeTargets.Class |
        AttributeTargets.Constructor |
        AttributeTargets.Delegate |
        AttributeTargets.Enum |
        AttributeTargets.Event |
        AttributeTargets.Field |
        AttributeTargets.Interface |
        AttributeTargets.Method |
        AttributeTargets.Module |
        AttributeTargets.Property |
        AttributeTargets.Struct,
        Inherited = false
    )]
    sealed partial class ExperimentalAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExperimentalAttribute"/> class, specifying the
        /// ID that the compiler will use when reporting a use of the API the attribute applies to.
        /// </summary>
        /// <param name="diagnosticId">
        /// The ID that the compiler will use when reporting a use of the API the attribute applies to.
        /// </param>
        public ExperimentalAttribute(string diagnosticId) => DiagnosticId = diagnosticId;

        /// <summary>
        /// Gets the ID that the compiler will use when reporting a use of the API the attribute applies to.
        /// </summary>
        /// <value>The unique diagnostic ID.</value>
        /// <remarks><para>
        /// The diagnostic ID is shown in build output for warnings and errors.
        /// This property represents the unique ID that can be used to suppress the warnings or errors, if needed.
        /// </para></remarks>
        public string DiagnosticId { get; }

        /// <summary>
        /// Gets or sets the URL for corresponding documentation. The API accepts a format string
        /// instead of an actual URL, creating a generic URL that includes the diagnostic ID.
        /// </summary>
        /// <value>The format string that represents a URL to corresponding documentation.</value>
        /// <remarks><para>
        /// An example format string is <c>https://contoso.com/obsoletion-warnings/{0}</c>.
        /// </para></remarks>
        public string? UrlFormat { get; set; }
    }
#endif
}

namespace System.Runtime.CompilerServices
{
    /// <summary>Indicates that a location is intercepted by this method.</summary>
    /// <param name="filePath">The file path to the intercepted location.</param>
    /// <param name="line">The line number to the intercepted location.</param>
    /// <param name="character">The character number to the intercepted location.</param>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
#pragma warning disable CS9113
    sealed class InterceptsLocationAttribute(string filePath, int line, int character) : Attribute;
#pragma warning restore CS9113
#if !NET8_0_OR_GREATER
    /// <summary>
    /// Initialize the attribute to refer to the <paramref name="methodName"/>
    /// method on the <paramref name="builderType"/> type.
    /// </summary>
    /// <param name="builderType">The type of the builder to use to construct the collection.</param>
    /// <param name="methodName">The name of the method on the builder to use to construct the collection.</param>
    /// <remarks><para>
    /// <paramref name="methodName"/> must refer to a static method that accepts a single parameter of
    /// type <c>ReadOnlySpan&lt;T&gt;</c> and returns an instance of the collection being built containing
    /// a copy of the data from that span. In future releases of .NET, additional patterns may be supported.
    /// </para></remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
    sealed class CollectionBuilderAttribute(Type builderType, string methodName) : Attribute
    {
        /// <summary>Gets the type of the builder to use to construct the collection.</summary>
        public Type BuilderType => builderType;

        /// <summary>Gets the name of the method on the builder to use to construct the collection.</summary>
        /// <remarks><para>
        /// This should match the metadata name of the target method.
        /// For example, this might be ".ctor" if targeting the type's constructor.
        /// </para></remarks>
        public string MethodName => methodName;
    }
#endif
#if !NET5_0_OR_GREATER
    /// <summary>
    /// Reserved to be used by the compiler for tracking metadata.
    /// This class should not be used by developers in source code.
    /// </summary>
    [
#if !(NETFRAMEWORK && !NET40_OR_GREATER || NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
        ExcludeFromCodeCoverage,
#endif
        DebuggerNonUserCode]
    static class IsExternalInit;
#endif
#if NETFRAMEWORK || NETSTANDARD && !NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Indicates the type of the async method builder that should be used by a language compiler to
    /// build the attributed async method or to build the attributed type when used as the return type
    /// of an async method.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Struct |
        AttributeTargets.Interface |
        AttributeTargets.Delegate |
        AttributeTargets.Enum |
        AttributeTargets.Method,
        Inherited = false
    )]
    sealed partial class AsyncMethodBuilderAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="AsyncMethodBuilderAttribute"/> class.</summary>
        /// <param name="builderType">The <see cref="Type"/> of the associated builder.</param>
        public AsyncMethodBuilderAttribute(Type builderType) => BuilderType = builderType;

        /// <summary>Gets the <see cref="Type"/> of the associated builder.</summary>
        public Type BuilderType { get; }
    }
#endif
#if NET20 || NET30
    /// <summary>
    /// Indicates that a method is an extension method, or that a class or assembly contains extension methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    sealed partial class ExtensionAttribute : Attribute;
#endif
#if !NET6_0_OR_GREATER
    /// <summary>Indicates the attributed type is to be used as an interpolated string handler.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    sealed partial class InterpolatedStringHandlerAttribute : Attribute;

    /// <summary>
    /// Indicates which arguments to a method involving an interpolated string handler should be passed to that handler.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    sealed partial class InterpolatedStringHandlerArgumentAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterpolatedStringHandlerArgumentAttribute"/> class.
        /// </summary>
        /// <remarks><para>
        /// The empty string may be used as the name of the receiver in an instance method.
        /// </para></remarks>
        /// <param name="argument">The name of the argument that should be passed to the handler.</param>
        public InterpolatedStringHandlerArgumentAttribute(string argument) => Arguments = [argument];

        /// <summary>
        /// Initializes a new instance of the <see cref="InterpolatedStringHandlerArgumentAttribute"/> class.
        /// </summary>
        /// <remarks><para>
        /// The empty string may be used as the name of the receiver in an instance method.
        /// </para></remarks>
        /// <param name="arguments">The names of the arguments that should be passed to the handler.</param>
        public InterpolatedStringHandlerArgumentAttribute(params string[] arguments) => Arguments = arguments;

        /// <summary>Gets the names of the arguments that should be passed to the handler.</summary>
        /// <remarks><para>
        /// The empty string may be used as the name of the receiver in an instance method.
        /// </para></remarks>
        public string[] Arguments { get; }
    }
#endif
#if !NET5_0_OR_GREATER
    /// <summary>
    /// Used to indicate to the compiler that a method should be called in its containing module's initializer.
    /// </summary>
    /// <remarks><para>
    /// When one or more valid methods with this attribute are found in a compilation,
    /// the compiler will emit a module initializer that calls each of the attributed methods.<br />
    /// Certain requirements are imposed on any method targeted with this attribute:
    /// </para><list type="bullet">
    /// <item><description>The method must be <see langword="static"/>.</description></item>
    /// <item><description>
    /// The method must be an ordinary member method, as opposed to a property accessor,
    /// constructor, local function, and so on.
    /// </description></item>
    /// <item><description>The method must be parameterless.</description></item>
    /// <item><description>The method must return <see langword="void"/>.</description></item>
    /// <item><description>The method must not be generic or be contained in a generic type.</description></item>
    /// <item><description>
    /// The method's effective accessibility must be <see langword="internal"/> or <see langword="public"/>.
    /// </description></item>
    /// </list><para>
    /// For more information, see the
    /// <a href="https://github.com/dotnet/runtime/blob/main/docs/design/specs/Ecma-335-Augments.md#module-initializer">
    /// ECMA specification
    /// </a>.
    /// </para></remarks>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed partial class ModuleInitializerAttribute : Attribute;
#endif
#if NETFRAMEWORK
    /// <summary>
    /// Allows you to obtain the full path of the source file that contains the caller.
    /// This is the file path at the time of compile.
    /// </summary>
    /// <remarks><para>
    /// You apply the <see cref="CallerFilePathAttribute"/> attribute to an optional parameter that has a default value.
    /// You must specify an explicit default value for the optional parameter.
    /// You can't apply this attribute to parameters that aren't specified as optional.
    /// </para></remarks>
    [AttributeUsage(AttributeTargets.Parameter)]
    sealed partial class CallerFilePathAttribute : Attribute;

    /// <summary>Allows you to obtain the line number in the source file at which the method is called.</summary>
    /// <remarks><para>
    /// You apply the <see cref="CallerLineNumberAttribute"/> attribute to an optional parameter that
    /// has a default value. You must specify an explicit default value for the optional parameter.
    /// You can't apply this attribute to parameters that aren't specified as optional.
    /// </para></remarks>
    [AttributeUsage(AttributeTargets.Parameter)]
    sealed partial class CallerLineNumberAttribute : Attribute;

    /// <summary>Allows you to obtain the method or property name of the caller to the method.</summary>
    /// <remarks><para>
    /// You apply the <see cref="CallerMemberNameAttribute"/> attribute to an optional parameter that
    /// has a default value. You must specify an explicit default value for the optional parameter.
    /// You can't apply this attribute to parameters that aren't specified as optional.
    /// </para></remarks>
    [AttributeUsage(AttributeTargets.Parameter)]
    sealed partial class CallerMemberNameAttribute : Attribute;
#endif
#if NETFRAMEWORK || NETSTANDARD
    /// <summary>Indicates that a parameter captures the expression passed for another parameter as a string.</summary>
    /// <remarks><para>This attribute is implemented in the compiler for C# 10 and later versions only.</para></remarks>
    [AttributeUsage(AttributeTargets.Parameter)]
    sealed partial class CallerArgumentExpressionAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="CallerArgumentExpressionAttribute"/> class.</summary>
        /// <param name="parameterName">
        /// The name of the parameter whose expression should be captured as a string.
        /// </param>
        public CallerArgumentExpressionAttribute([InvokerParameterName] string parameterName) =>
            ParameterName = parameterName;

        /// <summary>Gets the name of the parameter whose expression should be captured as a string.</summary>
        public string ParameterName { [Pure] get; }
    }
#endif
#if !NET5_0_OR_GREATER
    // ReSharper disable once CommentTypo

    /// <summary>
    /// Used to indicate to the compiler that the <c>.locals init</c> flag should not be set in method headers.
    /// </summary>
    /// <remarks><para>
    /// This attribute is unsafe because it may reveal uninitialized memory to the application in certain
    /// instances (e.g., reading from uninitialized stackalloc'd memory). If applied to a method directly,
    /// the attribute applies to that method and all nested functions (lambdas, local functions) below it.
    /// If applied to a type or module, it applies to all methods nested inside. This attribute is intentionally
    /// not permitted on assemblies. Use at the module level instead to apply to multiple type declarations.
    /// </para></remarks>
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Constructor |
        AttributeTargets.Event |
        AttributeTargets.Interface |
        AttributeTargets.Method |
        AttributeTargets.Module |
        AttributeTargets.Property |
        AttributeTargets.Struct,
        Inherited = false
    )]
    sealed partial class SkipLocalsInitAttribute : Attribute;
#endif
#if !NET6_0_OR_GREATER
    /// <summary>
    /// Indicates that an API is in preview. This attribute allows call sites to be
    /// flagged with a diagnostic that indicates that a preview feature is used.
    /// Authors can use this attribute to ship preview features in their assemblies.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Assembly |
        AttributeTargets.Class |
        AttributeTargets.Constructor |
        AttributeTargets.Delegate |
        AttributeTargets.Enum |
        AttributeTargets.Event |
        AttributeTargets.Field |
        AttributeTargets.Interface |
        AttributeTargets.Method |
        AttributeTargets.Module |
        AttributeTargets.Property |
        AttributeTargets.Struct,
        Inherited = false
    )]
    sealed partial class RequiresPreviewFeaturesAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="RequiresPreviewFeaturesAttribute"/> class.</summary>
        public RequiresPreviewFeaturesAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresPreviewFeaturesAttribute"/> class
        /// with the specified message.
        /// </summary>
        /// <param name="message">An optional message associated with this attribute instance.</param>
        public RequiresPreviewFeaturesAttribute(string? message) => Message = message;

        /// <summary>Gets the optional message associated with this attribute instance.</summary>
        public string? Message { get; }

        /// <summary>Gets or sets the optional URL associated with this attribute instance.</summary>
        public string? Url { get; set; }
    }
#endif
#if !NET7_0_OR_GREATER
    /// <summary>
    /// Indicates that compiler support for a particular feature is
    /// required for the location where this attribute is applied.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    sealed partial class CompilerFeatureRequiredAttribute : Attribute
    {
        /// <summary>The <see cref="FeatureName"/> used for the ref structs C# feature.</summary>
        public const string RefStructs = nameof(RefStructs);

        /// <summary>The <see cref="FeatureName"/> used for the required members C# feature.</summary>
        public const string RequiredMembers = nameof(RequiredMembers);

        /// <summary>Initializes a new instance of the <see cref="CompilerFeatureRequiredAttribute"/> class.</summary>
        /// <param name="featureName">The name of the compiler feature.</param>
        public CompilerFeatureRequiredAttribute(string featureName) => FeatureName = featureName;

        /// <summary>Gets the name of the compiler feature.</summary>
        public string FeatureName { [Pure] get; }

        /// <summary>
        /// Gets or sets a value indicating whether the compiler can choose to allow access to the location
        /// where this attribute is applied if it does not understand <see cref="FeatureName"/>.
        /// </summary>
        public bool IsOptional { [Pure] get; set; }
    }

    /// <summary>Specifies that a type has required members or that a member is required.</summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    sealed partial class RequiredMemberAttribute : Attribute;
#endif
#if !NET8_0_OR_GREATER
    /// <summary>Indicates that the instance's storage is sequentially replicated "length" times.</summary>
    [AttributeUsage(AttributeTargets.Struct), EditorBrowsable(EditorBrowsableState.Never)]
    sealed partial class InlineArrayAttribute(int length) : Attribute
    {
        /// <summary>Gets the length of the inlined array.</summary>
        public int Length { get; } = length;
    }
#endif
#if !NET9_0_OR_GREATER
    /// <summary>
    /// Specifies the priority of a member in overload resolution. When unspecified, the default priority is 0.
    /// </summary>
    /// <param name="priority">
    /// The priority of the attributed member. Higher numbers are prioritized, lower
    /// numbers are deprioritized. 0 is the default if no attribute is present.
    /// </param>
    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property,
        Inherited = false
    )]
    sealed class OverloadResolutionPriorityAttribute(int priority) : Attribute
    {
        /// <summary>Gets the priority of the member.</summary>
        public int Priority { get; } = priority;
    }
#endif
}
#if !NET7_0_OR_GREATER
namespace System.Text.RegularExpressions
{
    /// <summary>
    /// Instructs the System.Text.RegularExpressions source generator to
    /// generate an implementation of the specified regular expression.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The generator associated with this attribute only supports C#. It only supplies an implementation when applied
    /// to static, partial, parameterless, non-generic methods that are typed to return <see cref="Regex"/>.
    /// </para>
    /// <para>
    /// When the <see cref="Regex"/> supports case-insensitive matches (either by passing
    /// <see cref="RegexOptions.IgnoreCase"/> or using the inline <c>(?i)</c> switch in the pattern) the regex engines
    /// will use an internal casing table to transform the passed in pattern into an equivalent case-sensitive one.
    /// For example, given the pattern <c>abc</c>, the engines will transform it to the equivalent pattern
    /// <c>[Aa][Bb][Cc]</c>. The equivalences found in this internal casing table can change over time, for example in
    /// the case new characters are added to a new version of Unicode. When using the source generator, this
    /// transformation happens at compile time, which means the casing table used to find the equivalences will depend
    /// on the target framework at compile time. This differs from the rest of the <see cref="Regex"/> engines, which
    /// perform this transformation at run-time, meaning they will always use casing table for the current runtime.
    /// </para></remarks>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed partial class GeneratedRegexAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedRegexAttribute"/> class with the specified pattern.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        public GeneratedRegexAttribute([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
            : this(pattern, RegexOptions.None) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedRegexAttribute"/>
        /// class with the specified pattern and options.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="options">
        /// A bitwise combination of the enumeration values that modify the regular expression.
        /// </param>
        public GeneratedRegexAttribute(
            [StringSyntax(StringSyntaxAttribute.Regex, nameof(options))] string pattern,
            RegexOptions options
        )
            : this(pattern, options, Timeout.Infinite) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedRegexAttribute"/>
        /// class with the specified pattern and options.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="options">
        /// A bitwise combination of the enumeration values that modify the regular expression.
        /// </param>
        /// <param name="cultureName">
        /// The name of a culture to be used for case-sensitive comparisons.
        /// <paramref name="cultureName"/> is not case-sensitive.
        /// </param>
        /// <remarks><para>
        /// For a list of predefined culture names on Windows systems, see the Language tag column in the
        /// list of language/region names supported by
        /// <a href="https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c">
        /// Windows
        /// </a>.
        /// Culture names follow the standard defined by <a href="https://tools.ietf.org/html/bcp47">BCP 47</a>.
        /// In addition, starting with Windows 10, <paramref name="cultureName"/> can be any valid BCP-47 language tag.
        /// </para><para>
        /// If <paramref name="cultureName"/> is <see cref="string.Empty"/>, the invariant culture will be used.
        /// </para></remarks>
        public GeneratedRegexAttribute(
            [StringSyntax(StringSyntaxAttribute.Regex, nameof(options))] string pattern,
            RegexOptions options,
            string cultureName
        )
            : this(pattern, options, Timeout.Infinite, cultureName) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedRegexAttribute"/>
        /// class with the specified pattern, options, and timeout.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="options">
        /// A bitwise combination of the enumeration values that modify the regular expression.
        /// </param>
        /// <param name="matchTimeoutMilliseconds">
        /// A time-out interval (milliseconds), or <see cref="Timeout.Infinite"/>
        /// to indicate that the method should not time out.
        /// </param>
        public GeneratedRegexAttribute(
            [StringSyntax(StringSyntaxAttribute.Regex, nameof(options))] string pattern,
            RegexOptions options,
            int matchTimeoutMilliseconds
        )
            : this(pattern, options, matchTimeoutMilliseconds, "") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedRegexAttribute"/>
        /// class with the specified pattern, options, and timeout.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="options">
        /// A bitwise combination of the enumeration values that modify the regular expression.
        /// </param>
        /// <param name="matchTimeoutMilliseconds">
        /// A time-out interval (milliseconds), or <see cref="Timeout.Infinite"/>
        /// to indicate that the method should not time out.</param>
        /// <param name="cultureName">
        /// The name of a culture to be used for case-sensitive comparisons.
        /// <paramref name="cultureName"/> is not case-sensitive.
        /// </param>
        /// <remarks><para>
        /// For a list of predefined culture names on Windows systems, see the Language tag column in the
        /// list of language/region names supported by
        /// <a href="https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c">
        /// Windows
        /// </a>.
        /// Culture names follow the standard defined by <a href="https://tools.ietf.org/html/bcp47">BCP 47</a>.
        /// In addition, starting with Windows 10, <paramref name="cultureName"/> can be any valid BCP-47 language tag.
        /// </para><para>
        /// If <paramref name="cultureName"/> is <see cref="string.Empty"/>, the invariant culture will be used.
        /// </para></remarks>
        public GeneratedRegexAttribute(
            [StringSyntax(StringSyntaxAttribute.Regex, nameof(options))] string pattern,
            RegexOptions options,
            int matchTimeoutMilliseconds,
            string cultureName
        )
        {
            Pattern = pattern;
            Options = options;
            MatchTimeoutMilliseconds = matchTimeoutMilliseconds;
            CultureName = cultureName;
        }

        /// <summary>
        /// Gets a time-out interval (milliseconds), or <see cref="Timeout.Infinite"/>
        /// to indicate that the method should not time out.
        /// </summary>
        public int MatchTimeoutMilliseconds { [Pure] get; }

        /// <summary>Gets the name of the culture to be used for case-sensitive comparisons.</summary>
        public string CultureName { [Pure] get; }

        /// <summary>Gets the regular expression pattern to match.</summary>
        public string Pattern { [Pure] get; }

        /// <summary>Gets a bitwise combination of the enumeration values that modify the regular expression.</summary>
        public RegexOptions Options { [Pure] get; }
    }
}
#endif
