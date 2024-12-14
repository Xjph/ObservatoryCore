using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// <see cref="JsonConverterFactory"/> to convert enums to and from strings, respecting <see cref="EnumMemberAttribute"/> decorations. Supports nullable enums.
    /// </summary>
    public class JsonStringEnumMemberConverter : JsonConverterFactory
    {
        private readonly HashSet<Type>? _EnumTypes;
        private readonly JsonStringEnumMemberConverterOptions? _Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonStringEnumMemberConverter"/> class.
        /// </summary>
        public JsonStringEnumMemberConverter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonStringEnumMemberConverter"/> class.
        /// </summary>
        /// <param name="namingPolicy">
        /// Optional naming policy for writing enum values.
        /// </param>
        /// <param name="allowIntegerValues">
        /// True to allow undefined enum values. When true, if an enum value isn't
        /// defined it will output as a number rather than a string.
        /// </param>
        public JsonStringEnumMemberConverter(JsonNamingPolicy? namingPolicy = null, bool allowIntegerValues = true)
            : this(new JsonStringEnumMemberConverterOptions { NamingPolicy = namingPolicy, AllowIntegerValues = allowIntegerValues })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonStringEnumMemberConverter"/> class.
        /// </summary>
        /// <param name="options"><see cref="JsonStringEnumMemberConverterOptions"/>.</param>
        /// <param name="targetEnumTypes">Optional list of supported enum types to be converted. Specify <see langword="null"/> or empty to convert all enums.</param>
        public JsonStringEnumMemberConverter(JsonStringEnumMemberConverterOptions options, params Type[] targetEnumTypes)
        {
            _Options = options ?? throw new ArgumentNullException(nameof(options));

            if (targetEnumTypes != null && targetEnumTypes.Length > 0)
            {
#if NETSTANDARD2_0
				_EnumTypes = new HashSet<Type>();
#else
                _EnumTypes = new HashSet<Type>(targetEnumTypes.Length);
#endif
                foreach (Type enumType in targetEnumTypes)
                {
                    if (enumType.IsEnum)
                    {
                        _EnumTypes.Add(enumType);
                        continue;
                    }

                    if (enumType.IsGenericType)
                    {
                        (bool IsNullableEnum, Type? UnderlyingType) = TestNullableEnum(enumType);
                        if (IsNullableEnum)
                        {
                            _EnumTypes.Add(UnderlyingType!);
                            continue;
                        }
                    }

                    throw new NotSupportedException($"Type {enumType} is not supported by JsonStringEnumMemberConverter. Only enum types can be converted.");
                }
            }
        }

#pragma warning disable CA1062 // Validate arguments of public methods
        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
        {
            // Don't perform a typeToConvert == null check for performance. Trust our callers will be nice.
            return _EnumTypes != null
                ? _EnumTypes.Contains(typeToConvert)
                : typeToConvert.IsEnum;
        }
#pragma warning restore CA1062 // Validate arguments of public methods

        /// <inheritdoc/>
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            (bool IsNullableEnum, Type? UnderlyingType) = TestNullableEnum(typeToConvert);

            try
            {
                return (JsonConverter)Activator.CreateInstance(
                    typeof(JsonStringEnumMemberConverter<>).MakeGenericType(IsNullableEnum ? UnderlyingType! : typeToConvert),
                    BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    args: new object?[] { _Options },
                    culture: null)!;
            }
            catch (TargetInvocationException targetInvocationEx)
            {
                if (targetInvocationEx.InnerException != null)
                    throw targetInvocationEx.InnerException;
                throw;
            }
        }

        private static (bool IsNullableEnum, Type? UnderlyingType) TestNullableEnum(Type typeToConvert)
        {
            Type? UnderlyingType = Nullable.GetUnderlyingType(typeToConvert);

            return (UnderlyingType?.IsEnum ?? false, UnderlyingType);
        }

        internal static ulong GetEnumValue(TypeCode enumTypeCode, object value)
        {
            return enumTypeCode switch
            {
                TypeCode.Int32 => (ulong)(int)value,
                TypeCode.Int64 => (ulong)(long)value,
                TypeCode.Int16 => (ulong)(short)value,
                TypeCode.Byte => (byte)value,
                TypeCode.UInt32 => (uint)value,
                TypeCode.UInt64 => (ulong)value,
                TypeCode.UInt16 => (ushort)value,
                TypeCode.SByte => (ulong)(sbyte)value,
                _ => throw new NotSupportedException($"Enum '{value}' of {enumTypeCode} type is not supported."),
            };
        }
    }
}
