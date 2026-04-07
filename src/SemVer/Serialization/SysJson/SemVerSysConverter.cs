// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.SemVerSerialization.SysJson;

using System.Text.Json.Serialization;

/// <summary>
/// Provides functionality to convert <see cref="SemVer"/> values to and from JSON format.
/// Implements the System.Text.Json.Serialization.<see cref="JsonConverter{T}"/>".
/// </summary>
/// <remarks>
/// This converter is used to serialize and deserialize <see cref="SemVer"/> values when working with JSON. It ensures that
/// <see cref="SemVer"/> instances are correctly represented as strings in JSON and parsed back into <see cref="SemVer"/> objects
/// during deserialization.
/// </remarks>
public class SemVerSysConverter : JsonConverter<SemVer>
{
    /// <summary>
    /// Writes the specified <see cref="SemVer"/> value as a raw JSON string using the provided <see cref="Utf8JsonWriter"/>.
    /// </summary>
    /// <param name="writer">
    /// The <see cref="Utf8JsonWriter"/> to which the <see cref="SemVer"/> value will be written. Cannot be <c>null</c>.
    /// </param>
    /// <param name="value">
    /// The <see cref="SemVer"/> value to write.
    /// </param>
    /// <param name="_">
    /// The <see cref="JsonSerializerOptions"/> to use during serialization. This parameter is not used in this implementation
    /// but is required by the method signature.
    /// </param>
    public override void Write(
        Utf8JsonWriter writer,
        SemVer value,
        JsonSerializerOptions _)
    {
        Span<byte> bytes = value.Length <= 1024 ? stackalloc byte[value.Length] : new byte[value.Length];
        value.TryFormat(bytes, out var bytesWritten);

        writer.WriteStringValue(bytes[..bytesWritten]);
    }

    /// <summary>
    /// Reads and converts the JSON representation of a SemVer 2.0.0 value.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="Utf8JsonReader"/> to read the JSON data from.
    /// </param>
    /// <param name="_">
    /// The type of the object to convert. This parameter is ignored as this method always converts to a <see cref="SemVer"/>.
    /// </param>
    /// <param name="__">
    /// The serializer __ to use during deserialization. This parameter is not used in this implementation.
    /// </param>
    /// <returns>
    /// The <see cref="SemVer"/> value parsed from the JSON data.
    /// </returns>
    /// <exception cref="JsonException">
    /// Thrown if the JSON data does not represent a valid SemVer.
    /// </exception>
    public override SemVer Read(ref Utf8JsonReader reader, Type _, JsonSerializerOptions __)
    {
        try
        {
            if (reader.TokenType == JsonTokenType.Null)
                return default;

            // Cannot use reader.ValueSpan directly: Utf8JsonWriter escapes '+' as '\u002B',
            // and ValueSpan returns the raw (still-escaped) bytes. CopyString unescapes first.
            int rawLen = reader.HasValueSequence
                            ? checked((int)reader.ValueSequence.Length)
                            : reader.ValueSpan.Length;
            Span<byte> buffer = rawLen <= 1024 ? stackalloc byte[rawLen] : new byte[rawLen];
            int written = reader.CopyString(buffer);

            return SemVer.TryParse(buffer[..written], null, out var semVer)
                        ? semVer
                        : throw new JsonException("Could not parse SemVer value.");
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new JsonException("Could not parse SemVer value.", ex);
        }
    }
}
