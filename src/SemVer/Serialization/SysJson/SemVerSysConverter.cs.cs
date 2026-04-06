// SPDX-License-Identifier: MIT
// Copyright (c) 2025 Val Melamed

namespace vm2.SemVerSerialization.SysJson;

using System.Text.Json;
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
    /// <param name="value">The <see cref="SemVer"/> value to write.</param>
    /// <param name="_">The <see cref="JsonSerializerOptions"/> to use during serialization. This parameter is not used in this
    /// implementation but is required by the method signature.</param>
    public override void Write(
        Utf8JsonWriter writer,
        SemVer value,
        JsonSerializerOptions _)
    {
        Span<byte> utf8Chars = stackalloc byte[SemVerStringLength];
        var success = value.TryWrite(utf8Chars);

        if (!success)
            // Debug.Assert(false, "This should never happen because SemVer.TryWrite should only return false if the buffer is too small, and we are providing a buffer of the correct size.");
            throw new JsonException("Could not serialize SemVer value.");

        writer.WriteStringValue(utf8Chars);
    }

    /// <summary>
    /// Reads and converts the JSON representation of a SemVer (Universally Unique Lexicographically Sortable Identifier).
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read the JSON data from.</param>
    /// <param name="_">The type of the object to convert. This parameter is ignored as this method always converts to a <see
    /// cref="SemVer"/>.</param>
    /// <param name="__">The serializer __ to use during deserialization. This parameter is not used in this implementation.</param>
    /// <returns>The <see cref="SemVer"/> value parsed from the JSON data.</returns>
    /// <exception cref="JsonException">Thrown if the JSON data does not represent a valid SemVer.</exception>
    public override SemVer Read(ref Utf8JsonReader reader, Type _, JsonSerializerOptions __)
    {
        try
        {
            return TryParse(reader.ValueSpan, out var SemVer)
                        ? SemVer
                        : throw new JsonException("Could not parse SemVer value.");
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            // Debug.Assert(false, "This should never happen because TryParse should only throw if the input is invalid, and we are catching that case and throwing a JsonException.");
            throw new JsonException("Could not parse SemVer value.", ex);
        }
    }
}
