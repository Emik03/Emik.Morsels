// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <inheritdoc cref="Assert"/>
abstract partial class Assert
{
    /// <summary>Represents the way an assertion be formatted.</summary>
    /// <param name="template">The template that is formatted and shown when the declaring member fails.</param>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed partial class FormatAttribute(string template) : Attribute
    {
        /// <summary>The value that is substituted for the function body of the assertion.</summary>
        public const string Assertion = "!!";

        /// <summary>The value that is substituted for the function body of the first parameter's factory.</summary>
        public const string XFactory = "@x";

        /// <summary>The value that is substituted for first parameter.</summary>
        public const string XValue = "#x";

        /// <summary>The value that is substituted for the function body of the second parameter's factory.</summary>
        public const string YFactory = "@y";

        /// <summary>The value that is substituted for second parameter.</summary>
        public const string YValue = "#y";

        /// <summary>Returns the formatted <see cref="Template"/> by inserting the parameter.</summary>
        /// <param name="assertion">The value to replace <see cref="Assertion"/> with.</param>
        [Pure]
        public string this[string assertion] => Template.Replace(Assertion, assertion);

        /// <summary>Returns the formatted <see cref="Template"/> by inserting the parameters.</summary>
        /// <param name="assertion">The value to replace <see cref="Assertion"/> with.</param>
        /// <param name="xFactory">The value to replace <see cref="XFactory"/> with.</param>
        /// <param name="xValue">The value to replace <see cref="XValue"/> with.</param>
        [Pure]
        public string this[string assertion, string xFactory, string xValue] =>
            this[assertion, xFactory, xValue, xFactory, xValue];

        /// <summary>Returns the formatted <see cref="Template"/> by inserting the parameters.</summary>
        /// <param name="assertion">The value to replace <see cref="Assertion"/> with.</param>
        /// <param name="xFactory">The value to replace <see cref="XFactory"/> with.</param>
        /// <param name="xValue">The value to replace <see cref="XValue"/> with.</param>
        /// <param name="yFactory">The value to replace <see cref="YFactory"/> with.</param>
        /// <param name="yValue">The value to replace <see cref="YValue"/> with.</param>
        [Pure]
        public string this[string assertion, string xFactory, string xValue, string? yFactory, string? yValue] =>
            new StringBuilder(Template)
               .Replace(Assertion, assertion)
               .Replace(XFactory, xFactory)
               .Replace(XValue, xValue)
               .Replace(YFactory, yFactory)
               .Replace(YValue, yValue)
               .ToString();

        /// <summary>Gets the template, before any substitution occurs.</summary>
        [Pure]
        public string Template => template;
    }
}