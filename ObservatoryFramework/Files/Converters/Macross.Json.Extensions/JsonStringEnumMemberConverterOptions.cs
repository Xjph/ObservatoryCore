namespace System.Text.Json.Serialization
{
    /// <summary>
    /// Stores options for <see cref="JsonStringEnumMemberConverter"/>.
    /// </summary>
    public class JsonStringEnumMemberConverterOptions
    {
        private object? _DeserializationFailureFallbackValue;

        /// <summary>
        /// Gets or sets the optional <see cref="JsonNamingPolicy"/> for writing
        /// enum values.
        /// </summary>
        public JsonNamingPolicy? NamingPolicy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether integer values are allowed
        /// for reading enum values. Default value: <see langword="true"/>.
        /// </summary>
        public bool AllowIntegerValues { get; set; } = true;

        /// <summary>
        /// Gets or sets the optional default value to use when a json string
        /// does not match anything defined on the target enum. If not specified
        /// a <see cref="JsonException"/> is thrown for all failures.
        /// </summary>
        public object? DeserializationFailureFallbackValue
        {
            get => _DeserializationFailureFallbackValue;
            set
            {
                _DeserializationFailureFallbackValue = value;
                ConvertedDeserializationFailureFallbackValue = ConvertEnumValueToUInt64(value);
            }
        }

        internal ulong? ConvertedDeserializationFailureFallbackValue { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonStringEnumMemberConverterOptions"/> class.
        /// </summary>
        /// <param name="namingPolicy">
        /// Optional naming policy for writing enum values.
        /// </param>
        /// <param name="allowIntegerValues">
        /// True to allow undefined enum values. When true, if an enum value isn't
        /// defined it will output as a number rather than a string.
        /// </param>
        /// <param name="deserializationFailureFallbackValue">
        /// Optional default value to use when a json string does not match
        /// anything defined on the target enum. If not specified a <see
        /// cref="JsonException"/> is thrown for all failures.
        /// </param>
        public JsonStringEnumMemberConverterOptions(
            JsonNamingPolicy? namingPolicy = null,
            bool allowIntegerValues = true,
            object? deserializationFailureFallbackValue = null)
        {
            NamingPolicy = namingPolicy;
            AllowIntegerValues = allowIntegerValues;
            DeserializationFailureFallbackValue = deserializationFailureFallbackValue;
        }

        private static ulong? ConvertEnumValueToUInt64(object? deserializationFailureFallbackValue)
        {
            if (deserializationFailureFallbackValue == null)
                return null;

            ulong? value = deserializationFailureFallbackValue switch
            {
                int intVal => (ulong)intVal,
                long longVal => (ulong)longVal,
                byte byteVal => byteVal,
                short shortVal => (ulong)shortVal,
                uint uintVal => uintVal,
                ulong ulongVal => ulongVal,
                sbyte sbyteVal => (ulong)sbyteVal,
                ushort ushortVal => ushortVal,
                _ => null,
            };

            return value ?? (deserializationFailureFallbackValue is not Enum enumValue
                ? throw new InvalidOperationException("Supplied deserializationFailureFallbackValue parameter is not an enum value.")
                : JsonStringEnumMemberConverter.GetEnumValue(Type.GetTypeCode(enumValue.GetType()), deserializationFailureFallbackValue));
        }
    }
}
