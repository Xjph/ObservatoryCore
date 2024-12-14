namespace System.Text.Json.Serialization
{
    /// <summary>
    /// When placed on an enum type specifies the options for the <see
    /// cref="JsonStringEnumMemberConverter"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public sealed class JsonStringEnumMemberConverterOptionsAttribute : Attribute
    {
        /// <summary>
        /// Gets the <see cref="JsonStringEnumMemberConverterOptions"/>
        /// generated for the attribute.
        /// </summary>
        public JsonStringEnumMemberConverterOptions? Options { get; }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="JsonStringEnumMemberConverterOptionsAttribute"/> class.
        /// </summary>
        /// <param name="namingPolicyType">
        /// Optional type of a <see cref="JsonNamingPolicy"/> to use for writing
        /// enum values. Type must expose a public parameterless constructor.
        /// </param>
        /// <param name="allowIntegerValues">
        /// True to allow undefined enum values. When true, if an enum value
        /// isn't defined it will output as a number rather than a string.
        /// </param>
        /// <param name="deserializationFailureFallbackValue">
        /// Optional default value to use when a json string does not match
        /// anything defined on the target enum. If not specified a <see
        /// cref="JsonException"/> is thrown for all failures.
        /// </param>
        public JsonStringEnumMemberConverterOptionsAttribute(
            Type? namingPolicyType = null,
            bool allowIntegerValues = true,
            object? deserializationFailureFallbackValue = null)
        {
            Options = new JsonStringEnumMemberConverterOptions
            {
                AllowIntegerValues = allowIntegerValues,
                DeserializationFailureFallbackValue = deserializationFailureFallbackValue
            };

            if (namingPolicyType != null)
            {
                if (!typeof(JsonNamingPolicy).IsAssignableFrom(namingPolicyType))
                    throw new InvalidOperationException($"Supplied namingPolicyType {namingPolicyType} does not derive from JsonNamingPolicy.");
                if (namingPolicyType.GetConstructor(Type.EmptyTypes) == null)
                    throw new InvalidOperationException($"Supplied namingPolicyType {namingPolicyType} does not expose a public parameterless constructor.");

                Options.NamingPolicy = (JsonNamingPolicy)Activator.CreateInstance(namingPolicyType)!;
            }
        }
    }
}
