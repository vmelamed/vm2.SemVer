// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2;

/// <summary>
/// Represents a semantic version as defined by the SemVer 2.0.0 specification.
/// </summary>
[Newtonsoft.Json.JsonConverter(typeof(SemVerNsConverter))]
[System.Text.Json.Serialization.JsonConverter(typeof(SemVerSysConverter))]
public readonly partial struct SemVer :
    IEquatable<SemVer>,
    IComparable<SemVer>,
    ISpanParsable<SemVer>,
    IUtf8SpanParsable<SemVer>,
    IFormattable,
    ISpanFormattable,
    IUtf8SpanFormattable,
    IEqualityOperators<SemVer, SemVer, bool>,
    IComparisonOperators<SemVer, SemVer, bool>
{
    #region Properties
    /// <summary>
    /// The major version number, which is incremented when there are incompatible API changes.
    /// </summary>
    public int Major { get; } = 0;

    /// <summary>
    /// The minor version number, which is incremented when functionality is added in a backwards-compatible manner.
    /// </summary>
    public int Minor { get; } = 0;

    /// <summary>
    /// The patch version number, which is incremented when backwards-compatible bug fixes are made.
    /// </summary>
    public int Patch { get; } = 0;

    /// <summary>
    /// The pre-release version, which is an optional identifier that indicates a version is unstable and may not satisfy the
    /// intended compatibility requirements as denoted by its associated normal version.
    /// </summary>
    public string PreRelease { get => field ?? ""; } = "";

    /// <summary>
    /// The build metadata, which is an optional identifier that provides additional build information about a version. Build
    /// metadata does not affect version precedence and is ignored when determining version compatibility.
    /// </summary>
    public string BuildMetadata { get => field ?? ""; } = "";

    /// <summary>
    /// Indicates whether the semantic version is a pre-release version.
    /// </summary>
    public bool IsPreRelease => PreRelease.Length > 0;

    /// <summary>
    /// Indicates whether the semantic version is a stable version.
    /// </summary>
    public bool IsStable => PreRelease.Length == 0;

    /// <summary>
    /// Gets the core version of the semantic version, which consists of the major, minor, and patch components. The core
    /// version is used for determining version precedence and compatibility, while the pre-release and build metadata
    /// components are considered additional information that does not affect version precedence. The core version is
    /// represented as a new <see cref="SemVer"/> instance with the same major, minor, and patch values as the original instance,
    /// but with empty pre-release and build metadata components.
    /// </summary>
    public SemVer Core => new(Major, Minor, Patch);

    /// <summary>
    /// The length of the string representation of the semantic version, which is calculated based on the lengths of the major,
    /// minor, patch, pre-release, and build metadata components, as well as the separators used in the string representation
    /// (e.g. ".", "-", "+"). The minimum length is 5, corresponding to "0.0.0".
    /// </summary>
    public int Length { get => field < 5 ? 5 : field; } = 5;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="SemVer"/> struct with default values for all components (major, minor, patch, pre-release, and build metadata).
    /// This constructor is primarily used for deserialization purposes, allowing the creation of a <see cref="SemVer"/> instance with default values that can be populated during the deserialization process.
    /// The resulting <see cref="SemVer"/> instance will have a major version of 0, a minor version of 0, a patch version of 0, an empty pre-release version, and an empty build metadata.
    /// </summary>
    public SemVer()
    {
        Major         = 0;
        Minor         = 0;
        Patch         = 0;
        PreRelease    = "";
        BuildMetadata = "";
        Length        = CalculateLength();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemVer"/> struct with the specified major, minor, patch, pre-release, and
    /// build metadata components.
    /// </summary>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    /// <param name="patch">The patch version number.</param>
    /// <param name="preRelease">The pre-release version.</param>
    /// <param name="buildMetadata">The build metadata.</param>
    public SemVer(int major, int minor, int patch, string? preRelease = null, string? buildMetadata = null)
    {
        if (major < 0)
            throw new ArgumentOutOfRangeException(nameof(major), "SemVer core components must be non-negative integers.");
        if (minor < 0)
            throw new ArgumentOutOfRangeException(nameof(minor), "SemVer core components must be non-negative integers.");
        if (patch < 0)
            throw new ArgumentOutOfRangeException(nameof(patch), "SemVer core components must be non-negative integers.");

        preRelease    = string.IsNullOrWhiteSpace(preRelease)    ? "" : preRelease.Trim();
        buildMetadata = string.IsNullOrWhiteSpace(buildMetadata) ? "" : buildMetadata.Trim();

        if (preRelease is not "" && !PreReleaseIdentifier().IsMatch(preRelease))
            throw new ArgumentException("Pre-release version must consist of dot-separated identifiers containing only alphanumeric characters and hyphens.", nameof(preRelease));
        if (buildMetadata is not "" && !BuildIdentifier().IsMatch(buildMetadata))
            throw new ArgumentException("Build metadata must consist of dot-separated identifiers containing only alphanumeric characters and hyphens.", nameof(buildMetadata));

        Major         = major;
        Minor         = minor;
        Patch         = patch;
        PreRelease    = preRelease;
        BuildMetadata = buildMetadata;
        Length        = CalculateLength();
    }

    private int CalculateLength()
    {
        static int LengthOfInt(int value)
            => value switch
            {
                < 10 => 1,
                < 100 => 2,
                < 1000 => 3,
                < 10000 => 4,
                < 100000 => 5,
                < 1000000 => 6,
                < 10000000 => 7,
                < 100000000 => 8,
                < 1000000000 => 9,
                _ => 10,
            };

        var length = 0;

        length += LengthOfInt(Major) + 1;
        length += LengthOfInt(Minor) + 1;
        length += LengthOfInt(Patch);

        return length +
               (PreRelease.Length    > 0 ? 1 + PreRelease.Length    : 0) +
               (BuildMetadata.Length > 0 ? 1 + BuildMetadata.Length : 0);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemVer"/> struct by parsing a string representation of a semantic version.
    /// The input string must follow the SemVer 2.0.0 specification.
    /// </summary>
    /// <param name="semVerString">
    /// The string representation of the semantic version.
    /// </param>
    public SemVer(string semVerString)
    {
        var semVer = Parse(semVerString);

        Major         = semVer.Major;
        Minor         = semVer.Minor;
        Patch         = semVer.Patch;
        PreRelease    = semVer.PreRelease;
        BuildMetadata = semVer.BuildMetadata;
        Length        = semVer.Length;
    }
    #endregion

    /// <summary>
    /// Returns a string representation of the semantic version, following the format defined by the SemVer 2.0.0 specification.
    /// </summary>
    /// <returns>
    /// A string representation of the semantic version.
    /// </returns>
    public override string ToString() => ToString(null, null);

    #region IEquatable<SemVer>
    /// <summary>
    /// Determines whether the specified <see cref="SemVer"/> instance is equal to the current instance.
    /// </summary>
    /// <param name="other">The <see cref="SemVer"/> instance to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="SemVer"/> instance is equal to the current instance; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(SemVer other)
        => Major == other.Major
        && Minor == other.Minor
        && Patch == other.Patch
        && string.Equals(PreRelease, other.PreRelease, StringComparison.Ordinal);
    #endregion

    /// <summary>
    /// Determines whether the specified object is equal to the current semantic version instance. Two semantic version
    /// instances are considered equal if they have the same major, minor, patch, and pre-release. Build metadata components are
    /// ignored in equality comparisons.
    /// </summary>
    /// <param name="obj">The object to compare with the current semantic version instance.</param>
    /// <returns>
    /// <c>true</c> if the specified object is equal to the current semantic version instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
        => obj is SemVer other && Equals(other);

    /// <summary>
    /// Returns a hash code for the current semantic version instance. The hash code is computed based on the major, minor, patch,
    /// produce the same hash code, while different semantic version instances are likely to produce different hash codes.
    /// </summary>
    /// <returns>
    /// A hash code for the current semantic version instance.
    /// </returns>
    public override int GetHashCode()
        => HashCode.Combine(Major, Minor, Patch, PreRelease);

    #region Methods
    /// <summary>
    /// Determines whether the specified string is a valid semantic version.
    /// </summary>
    /// <param name="s">The string to validate.</param>
    /// <returns>
    /// <c>true</c> if the specified string is a valid semantic version; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsValid(string? s) => s is not null && SemVer20().IsMatch(s.Trim());

    /// <summary>
    /// Returns a new <see cref="SemVer"/> instance with the major version number incremented by one and the minor and patch
    /// version numbers reset to zero.
    /// </summary>
    /// <param name="preRelease">
    /// The pre-release version to apply to the bumped version. If this parameter is null or empty, the resulting
    /// <see cref="SemVer"/> instance will have an empty pre-release component.
    /// </param>
    /// <param name="buildMetadata">
    /// The build metadata to apply to the bumped version. If this parameter is null or empty, the resulting
    /// <see cref="SemVer"/> instance will have an empty build metadata component.
    /// </param>
    /// <returns>
    /// A new <see cref="SemVer"/> instance with the major version number incremented by one and the minor and patch version
    /// numbers reset to zero, using the specified pre-release and build metadata components.
    /// </returns>
    public SemVer BumpMajor(string? preRelease = null, string? buildMetadata = null)
        => new (checked(Major + 1), 0, 0, preRelease, buildMetadata);

    /// <summary>
    /// Returns a new <see cref="SemVer"/> instance with the minor version number incremented by one and the patch version number
    /// reset to zero, while the major version number remains unchanged.
    /// </summary>
    /// <param name="preRelease">
    /// The pre-release version to apply to the bumped version. If this parameter is null or empty, the resulting
    /// <see cref="SemVer"/> instance will have an empty pre-release component.
    /// </param>
    /// <param name="buildMetadata">
    /// The build metadata to apply to the bumped version. If this parameter is null or empty, the resulting
    /// <see cref="SemVer"/> instance will have an empty build metadata component.
    /// </param>
    /// <returns>
    /// A new <see cref="SemVer"/> instance with the minor version number incremented by one and the patch version number reset
    /// to zero, while the major version number remains unchanged, using the specified pre-release and build metadata
    /// components.
    /// </returns>
    public SemVer BumpMinor(string? preRelease = null, string? buildMetadata = null)
        => new (Major, checked(Minor + 1), 0, preRelease, buildMetadata);

    /// <summary>
    /// Returns a new <see cref="SemVer"/> instance with the patch version number incremented by one, while the major and minor
    /// version numbers remain unchanged.
    /// </summary>
    /// <param name="preRelease">
    /// The pre-release version to apply to the bumped version. If this parameter is null or empty, the resulting
    /// <see cref="SemVer"/> instance will have an empty pre-release component.
    /// </param>
    /// <param name="buildMetadata">
    /// The build metadata to apply to the bumped version. If this parameter is null or empty, the resulting
    /// <see cref="SemVer"/> instance will have an empty build metadata component.
    /// </param>
    /// <returns>
    /// A new <see cref="SemVer"/> instance with the patch version number incremented by one, while the major and minor version
    /// numbers remain unchanged, using the specified pre-release and build metadata components.
    /// </returns>
    public SemVer BumpPatch(string? preRelease = null, string? buildMetadata = null)
        => new (Major, Minor, checked(Patch + 1), preRelease, buildMetadata);

    /// <summary>
    /// Returns a new <see cref="SemVer"/> instance with the specified pre-release version, while the major, minor, and patch
    /// version numbers remain unchanged. If the provided pre-release version is null or empty, the resulting
    /// <see cref="SemVer"/> instance will have an empty pre-release component, indicating that it is a stable version. The build
    /// metadata remains unchanged.
    /// </summary>
    /// <param name="preRelease">
    /// The pre-release version to set. If this parameter is null or empty, the resulting <see cref="SemVer"/> instance will
    /// have an empty pre-release component, indicating that it is a stable version. The build metadata remains unchanged.
    /// </param>
    /// <returns>
    /// A new <see cref="SemVer"/> instance with the specified pre-release version, while the major, minor, and patch version
    /// numbers remain unchanged. If the provided pre-release version is null or empty, the resulting <see cref="SemVer"/>
    /// instance will have an empty pre-release component, indicating that it is a stable version. The build metadata remains
    /// unchanged.
    /// </returns>
    public SemVer WithPreRelease(string? preRelease)
    {
        preRelease = string.IsNullOrWhiteSpace(preRelease) ? null : preRelease?.Trim();
        if (preRelease is not null && !PreReleaseIdentifier().IsMatch(preRelease))
            throw new ArgumentException("Pre-release version must consist of dot-separated identifiers containing only alphanumeric characters and hyphens, and must not be empty.", nameof(preRelease));

        return new SemVer(Major, Minor, Patch, preRelease, BuildMetadata);
    }

    /// <summary>
    /// Returns a new <see cref="SemVer"/> instance with the specified build metadata, while the major, minor, patch version
    /// numbers, and pre-release version remain unchanged. If the provided build metadata is null or empty, the resulting
    /// <see cref="SemVer"/> instance will have an empty build metadata component. The build metadata does not affect version
    /// precedence and is ignored when determining version compatibility, so changing the build metadata does not affect the
    /// version's precedence.
    /// </summary>
    /// <param name="buildMetadata">
    /// The build metadata to set. If this parameter is null or empty, the resulting <see cref="SemVer"/> instance will have an
    /// empty build metadata component.
    /// </param>
    /// <returns>
    /// A new <see cref="SemVer"/> instance with the specified build metadata, while the major, minor, patch version numbers,
    /// and pre-release version remain unchanged.
    /// </returns>
    public SemVer WithBuildMetadata(string? buildMetadata)
    {
        buildMetadata = string.IsNullOrWhiteSpace(buildMetadata) ? null : buildMetadata?.Trim();
        if (buildMetadata is not null && !BuildIdentifier().IsMatch(buildMetadata))
            throw new ArgumentException("Build metadata must consist of dot-separated identifiers containing only alphanumeric characters and hyphens, and must not be empty.", nameof(buildMetadata));

        return new SemVer(Major, Minor, Patch, PreRelease, buildMetadata);
    }

    /// <summary>
    /// Returns a new <see cref="SemVer"/> instance with the same major, minor, and patch version numbers as the current instance,
    /// but with empty pre-release and build metadata components, indicating that it is a stable release version
    /// This method is useful for obtaining the stable release version corresponding to a pre-release version.
    /// </summary>
    /// <returns>
    /// A new <see cref="SemVer"/> instance with the same major, minor, and patch version numbers as the current instance,
    /// but with empty pre-release and build metadata components, indicating that it is a stable release version.
    /// </returns>
    public SemVer Release()
        => new (Major, Minor, Patch);
    #endregion

    #region IComparable<SemVer>
    /// <summary>
    /// Compares the current semantic version instance with another semantic version instance and returns an integer that indicates
    /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the other instance.
    /// The comparison is based on the major, minor, patch, pre-release, and build metadata components of the semantic versions,
    /// following the precedence rules defined by the SemVer 2.0.0 specification.
    /// </summary>
    /// <param name="other">
    /// The semantic version instance to compare with the current instance.
    /// </param>
    /// <returns>
    /// A value less than zero if the current instance precedes <paramref name="other"/> in the sort order; zero if the current
    /// instance occurs in the same position in the sort order as <paramref name="other"/>; a value greater than zero if the
    /// current instance follows <paramref name="other"/> in the sort order.
    /// </returns>
    public int CompareTo(SemVer other)
    {
        var d = Major - other.Major;
        if (d != 0)
            return d;

        d = Minor - other.Minor;
        if (d != 0)
            return d;

        d = Patch - other.Patch;
        if (d != 0)
            return d;

        var thisHasPreRelease =  PreRelease.Length > 0;
        var otherHasPreRelease = other.PreRelease.Length > 0;

        if (!thisHasPreRelease && !otherHasPreRelease)
            return 0; // Both versions are equal

        if (!thisHasPreRelease)
            return 1; // This version is greater (no pre-release means higher precedence)

        if (!otherHasPreRelease)
            return -1; // This version is lesser (pre-release means lower precedence)

        // both have pre-release, compare them
        return CompareSpans(PreRelease, other.PreRelease);
    }

    private static int CompareSpans(
        ReadOnlySpan<char> str1,
        ReadOnlySpan<char> str2)
    {
        var e1 = str1.Split('.').GetEnumerator();
        var e2 = str2.Split('.').GetEnumerator();
        bool moved1, moved2;

        do
        {
            moved1 = e1.MoveNext();
            moved2 = e2.MoveNext();

            if (!moved1 && !moved2)
                return 0;   // Reached the end of both identifier lists
            if (!moved1)
                return -1;  // str1 has fewer identifiers, so str2 has higher precedence
            if (!moved2)
                return 1;   // str2 has fewer identifiers, so str1 has higher precedence

            var id1 = str1[e1.Current];
            var id2 = str2[e2.Current];

            bool isNum1 = id1.Length > 0;

            if (isNum1)
                for (int i = 0; i < id1.Length; i++)
                    if (!char.IsDigit(id1[i]))
                    {
                        isNum1 = false;
                        break;
                    }

            bool isNum2 = id2.Length > 0;

            if (isNum2)
                for (int i = 0; i < id2.Length; i++)
                    if (!char.IsDigit(id2[i]))
                    {
                        isNum2 = false;
                        break;
                    }

            if (isNum1 && isNum2) {
                if (id1.Length != id2.Length)
                    return id1.Length < id2.Length ? -1 : 1; // the longer identifier is bigger, i.e. has higher precedence
                var comp = id1.CompareTo(id2, StringComparison.Ordinal);
                if (comp != 0)
                    return comp; // Both identifiers are numeric and equal in length, compare lexically
                // else continue to the next identifiers
            }
            else
            if (!isNum1 && !isNum2)
            {
                var comp = id1.CompareTo(id2, StringComparison.Ordinal);
                if (comp != 0)
                    return comp; // Both identifiers are non-numeric and different, compare lexically
                // else continue to the next identifiers
            }
            else
                // If one is numeric and the other is not, the numeric identifier has lower precedence
                return isNum1 ? -1 : 1;
        }
        while (true);
    }
    #endregion

    #region IParsable<SemVer>
    /// <summary>
    /// Parses a string representation of a semantic version and returns the corresponding <see cref="SemVer"/> instance. The
    /// input string must follow the format defined by the SemVer 2.0.0 specification. If the input string is not in a valid
    /// semantic version format, a <see cref="FormatException"/> is thrown.
    /// </summary>
    /// <param name="s">
    /// The string representation of the semantic version to parse. The string must follow the format defined by the SemVer 2.0.0 specification.
    /// </param>
    /// <param name="_">
    /// An optional format provider. This parameter is not used and can be set to <c>null</c>.
    /// </param>
    /// <returns>
    /// The <see cref="SemVer"/> instance corresponding to the parsed string.
    /// </returns>
    public static SemVer Parse(string s, IFormatProvider? _ = null)
    {
        if (TryParse(s, _, out var result))
            return result;

        throw new FormatException($"Input string '{s}' is not in a valid semantic version format. See the SemVer 2.0.0 specification for more details: https://semver.org/.");
    }

    /// <summary>
    /// Tries to parse a string representation of a semantic version and returns a boolean indicating whether the parsing was
    /// successful. If the input string is in a valid semantic version format, the method returns <c>true</c> and outputs the
    /// corresponding <see cref="SemVer"/> instance; otherwise, it returns <c>false</c> and outputs the default value of
    /// <see cref="SemVer"/>.
    /// </summary>
    /// <param name="s">
    /// The string representation of the semantic version to parse. The string must follow the format defined by the SemVer
    /// 2.0.0 specification.
    /// </param>
    /// <param name="_">
    /// An optional format provider. This parameter is not used and can be set to <c>null</c>.
    /// </param>
    /// <param name="result">
    /// When this method returns, contains the <see cref="SemVer"/> instance equivalent to the semantic version contained in
    /// <paramref name="s"/>, if the conversion succeeded, or the default value if the conversion failed.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <paramref name="s"/> parameter was converted successfully; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? _,
        [MaybeNullWhen(false)] out SemVer result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(s))
            return false;

        var match = SemVer20().Match(s.Trim());
        if (!match.Success)
            return false;

        var major         = int.Parse(match.Groups[MajorGr].ValueSpan);
        var minor         = int.Parse(match.Groups[MinorGr].ValueSpan);
        var patch         = int.Parse(match.Groups[PatchGr].ValueSpan);
        var preRelease    = match.Groups[PreReleaseGr].Success ? match.Groups[PreReleaseGr].Value : "";
        var buildMetadata = match.Groups[BuildGr].Success      ? match.Groups[BuildGr].Value      : "";

        result = new SemVer(major, minor, patch, preRelease, buildMetadata);
        return true;
    }
    #endregion

    #region ISpanParsable<SemVer>
    /// <summary>
    /// Parses a span of characters representing a semantic version and returns the corresponding <see cref="SemVer"/> instance.
    /// The input span must follow the format defined by the SemVer 2.0.0 specification. If the input span is not in a valid
    /// semantic version format, a <see cref="FormatException"/> is thrown.
    /// </summary>
    /// <param name="s">The span of characters representing the semantic version.</param>
    /// <param name="_">
    /// An object that supplies culture-specific formatting information. This parameter is not used in the parsing process and can be set to <c>null</c>.
    /// </param>
    /// <returns>The <see cref="SemVer"/> instance equivalent to the semantic version contained in the input span.</returns>
    /// <exception cref="FormatException">Thrown when the input span is not in a valid semantic version format.</exception>
    public static SemVer Parse(
        ReadOnlySpan<char> s,
        IFormatProvider? _ = null)
    {
        if (TryParse(s, _, out var result))
            return result;

        throw new FormatException($"Input string '{s}' is not in a valid semantic version format. See the SemVer 2.0.0 specification for more details: https://semver.org/.");
    }

    /// <summary>
    /// Tries to parse a span of characters representing a semantic version and returns a boolean indicating whether the parsing
    /// was successful. If the input span is in a valid semantic version format, the method returns <c>true</c> and outputs the
    /// corresponding <see cref="SemVer"/> instance; otherwise, it returns <c>false</c> and outputs the default value.
    /// </summary>
    /// <param name="s">The span of characters representing the semantic version.</param>
    /// <param name="_">
    /// An object that supplies culture-specific formatting information. This parameter is not used in the parsing process and can be set to <c>null</c>.
    /// </param>
    /// <param name="result">
    /// When this method returns, contains the <see cref="SemVer"/> instance equivalent to the semantic version contained in the
    /// input span, if the parsing succeeded; otherwise, the default value.
    /// </param>
    /// <returns>
    /// <c>true</c> if the input span was successfully parsed; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? _,
        [MaybeNullWhen(false)] out SemVer result)
    {
        result = default;

        if (s.IsEmpty || s.IsWhiteSpace())
            return false;

        s = s.Trim();

        var match = SemVer20().Match(s.ToString());
        if (!match.Success)
            return false;

        var major         = int.Parse(match.Groups[MajorGr].ValueSpan);
        var minor         = int.Parse(match.Groups[MinorGr].ValueSpan);
        var patch         = int.Parse(match.Groups[PatchGr].ValueSpan);
        var preRelease    = match.Groups[PreReleaseGr].Success ? match.Groups[PreReleaseGr].Value : "";
        var buildMetadata = match.Groups[BuildGr].Success      ? match.Groups[BuildGr].Value      : "";

        result = new SemVer(major, minor, patch, preRelease, buildMetadata);
        return true;
    }
    #endregion

    #region IUtf8SpanParsable<SemVer>
    /// <summary>
    /// Parses a span of UTF-8 encoded bytes representing a semantic version and returns the corresponding <see cref="SemVer"/>
    /// instance. The input span must follow the format defined by the SemVer 2.0.0 specification. If the input span is not in a
    /// valid semantic version format, a <see cref="FormatException"/> is thrown.
    /// </summary>
    /// <param name="utf8Text">The span of UTF-8 encoded bytes representing the semantic version.</param>
    /// <param name="_">
    /// An object that supplies culture-specific formatting information. This parameter is not used in the parsing process and can be set to <c>null</c>.
    /// </param>
    /// <returns>The <see cref="SemVer"/> instance equivalent to the semantic version contained in the input span.</returns>
    /// <exception cref="FormatException">
    /// Thrown when the input span is not in a valid semantic version format.
    /// </exception>
    public static SemVer Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? _ = null)
    {
        if (TryParse(utf8Text, _, out var result))
            return result;

        throw new FormatException($"Input string '{Encoding.UTF8.GetString(utf8Text)}' is not in a valid semantic version format. See the SemVer 2.0.0 specification for more details: https://semver.org/.");
    }

    /// <summary>
    /// Tries to parse a span of UTF-8 encoded bytes representing a semantic version and returns a boolean indicating whether the parsing was successful. If the input span is in a valid semantic version format, the method returns <c>true</c> and outputs the corresponding <see cref="SemVer"/> instance; otherwise, it returns <c>false</c> and outputs the default value.
    /// </summary>
    /// <param name="utf8Text">The span of UTF-8 encoded bytes representing the semantic version.</param>
    /// <param name="_">
    /// An object that supplies culture-specific formatting information. This parameter is not used in the parsing process and can be set to <c>null</c>.
    /// </param>
    /// <param name="result">When this method returns, contains the <see cref="SemVer"/> instance equivalent to the semantic version contained in the input span, if the parsing succeeded; otherwise, the default value.</param>
    /// <returns><c>true</c> if the input span was successfully parsed; otherwise, <c>false</c>.</returns>
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? _,
        [MaybeNullWhen(false)] out SemVer result)
    {
        result = default;

        if (utf8Text.IsEmpty)
            return false;

        Span<char> chars = utf8Text.Length < 1024 ? stackalloc char[utf8Text.Length] : new char[utf8Text.Length];

        if (!Encoding.UTF8.TryGetChars(utf8Text, chars, out var charsUsed))
            return false;

        chars = chars[..charsUsed].Trim();

        var match = SemVer20().Match(chars.ToString());

        if (!match.Success)
            return false;

        var major         = int.Parse(match.Groups[MajorGr].ValueSpan);
        var minor         = int.Parse(match.Groups[MinorGr].ValueSpan);
        var patch         = int.Parse(match.Groups[PatchGr].ValueSpan);
        var preRelease    = match.Groups[PreReleaseGr].Success ? match.Groups[PreReleaseGr].Value : "";
        var buildMetadata = match.Groups[BuildGr].Success      ? match.Groups[BuildGr].Value      : "";

        result = new SemVer(major, minor, patch, preRelease, buildMetadata);
        return true;
    }
    #endregion

    #region Practical TryParse overloads
    /// <summary>
    /// Tries to parse a string representation of a semantic version and returns a boolean indicating whether the parsing was
    /// successful. If the input string is in a valid semantic version format, the method returns <c>true</c> and outputs the
    /// corresponding <see cref="SemVer"/> instance; otherwise, it returns <c>false</c> and outputs the default value of
    /// <see cref="SemVer"/>.
    /// </summary>
    /// <param name="s">
    /// The string representation of the semantic version to parse. The string must follow the format defined by the SemVer
    /// 2.0.0 specification.
    /// </param>
    /// <param name="result">
    /// When this method returns, contains the <see cref="SemVer"/> instance equivalent to the semantic version contained in
    /// <paramref name="s"/>, if the conversion succeeded, or the default value if the conversion failed.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <paramref name="s"/> parameter was converted successfully; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        [MaybeNullWhen(false)] out SemVer result) => TryParse(s, null, out result);

    /// <summary>
    /// Tries to parse a span of characters representing a semantic version and returns a boolean indicating whether the parsing
    /// was successful. If the input span is in a valid semantic version format, the method returns <c>true</c> and outputs the
    /// corresponding <see cref="SemVer"/> instance; otherwise, it returns <c>false</c> and outputs the default value.
    /// </summary>
    /// <param name="s">The span of characters representing the semantic version.</param>
    /// <param name="result">
    /// When this method returns, contains the <see cref="SemVer"/> instance equivalent to the semantic version contained in the
    /// input span, if the parsing succeeded; otherwise, the default value.
    /// </param>
    /// <returns>
    /// <c>true</c> if the input span was successfully parsed; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(
        ReadOnlySpan<char> s,
        [MaybeNullWhen(false)] out SemVer result) => TryParse(s, null, out result);

    /// <summary>
    /// Tries to parse a span of UTF-8 encoded bytes representing a semantic version and returns a boolean indicating whether the parsing was successful. If the input span is in a valid semantic version format, the method returns <c>true</c> and outputs the corresponding <see cref="SemVer"/> instance; otherwise, it returns <c>false</c> and outputs the default value.
    /// </summary>
    /// <param name="utf8Text">The span of UTF-8 encoded bytes representing the semantic version.</param>
    /// <param name="result">When this method returns, contains the <see cref="SemVer"/> instance equivalent to the semantic version contained in the input span, if the parsing succeeded; otherwise, the default value.</param>
    /// <returns><c>true</c> if the input span was successfully parsed; otherwise, <c>false</c>.</returns>
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        [MaybeNullWhen(false)] out SemVer result) => TryParse(utf8Text, null, out result);
    #endregion

    #region IFormattable
    /// <summary>
    /// Returns a string representation of the semantic version, following the format defined by the SemVer 2.0.0 specification.
    /// The <paramref name="format"/> parameter is not used and must be null or an empty string; otherwise, a
    /// <see cref="FormatException"/> is thrown. The <paramref name="_"/> parameter is also not used and can be set to
    /// <c>null</c>.
    /// </summary>
    /// <param name="format">
    /// The format string. This parameter is not used and must be null or an empty string; otherwise, a
    /// <see cref="FormatException"/> is thrown.
    /// </param>
    /// <param name="_">
    /// An object that supplies culture-specific formatting information. This parameter is not used and can be set to <c>null</c>.
    /// </param>
    /// <returns>
    /// A string representation of the semantic version.
    /// </returns>
    /// <exception cref="FormatException"></exception>
    public string ToString(string? format, IFormatProvider? _)
    {
        if (!string.IsNullOrEmpty(format))
            throw new FormatException($"The format string '{format}' is not supported. The only supported format is the default format, which can be specified by passing null or an empty string as the format parameter.");

        Span<char> buffer = Length < 1024 ? stackalloc char[Length] : new char[Length];
        var written = 0;

        TryFormat(buffer, out written);
        return new(buffer[..written]);
    }
    #endregion

    #region ISpanFormattable
    /// <summary>
    /// Formats the current semantic version instance into the provided destination span of characters, following the format
    /// defined by the SemVer 2.0.0 specification. The <paramref name="format"/> parameter is not used and can be null or an
    /// empty string; otherwise, a <see cref="FormatException"/> is thrown. The <paramref name="_"/> parameter is also not used
    /// and can be set to <c>null</c>.
    /// </summary>
    /// <param name="destination">
    /// The span of characters to which the semantic version will be formatted. The method will attempt to format the semantic
    /// version into this span, and if the span is not large enough to hold the formatted string, the method will return
    /// <c>false</c> and set <paramref name="charsWritten"/> to zero.
    /// </param>
    /// <param name="charsWritten">
    /// The number of characters written to the destination span. If the span is not large enough, this will be set to zero.
    /// </param>
    /// <param name="format">
    /// The format string. This parameter is not used and must be null or an empty string; otherwise, a
    /// <see cref="FormatException"/> is thrown.
    /// </param>
    /// <param name="_">
    /// An object that supplies culture-specific formatting information. This parameter is not used and can be set to <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the semantic version was successfully formatted; otherwise, <c>false</c>.
    /// </returns>
    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format = "",
        IFormatProvider? _ = null)
    {
        charsWritten = 0;

        if (!format.IsEmpty)
            throw new FormatException($"The format string '{format}' is not supported. The only supported format is the default format, which can be specified by passing null or an empty string as the format parameter.");

        if (destination.Length < Length)
            return false; // Not enough space in the destination span or unsupported format string

        var pos = 0;

        Major.TryFormat(destination[pos..], out var written);
        pos += written;
        destination[pos++] = '.';

        Minor.TryFormat(destination[pos..], out written);
        pos += written;
        destination[pos++] = '.';

        Patch.TryFormat(destination[pos..], out written);
        pos += written;

        if (PreRelease.Length > 0)
        {
            destination[pos++] = '-';
            PreRelease.AsSpan().CopyTo(destination[pos..]);
            pos += PreRelease.Length;
        }

        if (BuildMetadata.Length > 0)
        {
            destination[pos++] = '+';
            BuildMetadata.AsSpan().CopyTo(destination[pos..]);
            pos += BuildMetadata.Length;
        }

        charsWritten = pos;
        return true;
    }
    #endregion

    #region IUtf8SpanFormattable
    /// <summary>
    /// Formats the current semantic version instance into the provided destination span of UTF-8 encoded bytes following the
    /// format defined by the SemVer 2.0.0 specification. The <paramref name="format"/> parameter is not used and can be null or
    /// an empty string; otherwise, a <see cref="FormatException"/> is thrown. The <paramref name="provider"/> parameter is also not
    /// used and can be set to <c>null</c>.
    /// </summary>
    /// <param name="utf8Destination">
    /// The span of UTF-8 encoded bytes to which the semantic version will be formatted. The method will attempt to format the semantic version into this span, and if the span is not large enough to hold the formatted string, the method will return <c>false</c> and set <paramref name="bytesWritten"/> to zero.
    /// </param>
    /// <param name="bytesWritten">
    /// The number of bytes written to the destination span. If the span is not large enough, this will be set to zero.
    /// </param>
    /// <param name="format">
    /// The format string. This parameter is not used and must be null or an empty string; otherwise, a <see cref="FormatException"/> is thrown.
    /// </param>
    /// <param name="provider">
    /// The format provider. This parameter is not used and can be set to <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the semantic version was successfully formatted; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="FormatException"></exception>
    public bool TryFormat(
        Span<byte> utf8Destination,
        out int bytesWritten,
        ReadOnlySpan<char> format = "",
        IFormatProvider? provider = null)
    {
        bytesWritten = 0;

        if (!format.IsEmpty)
            throw new FormatException($"The format string '{format}' is not supported. The only supported format is the default format, which can be specified by passing null or an empty string as the format parameter.");

        if (utf8Destination.Length < Length)
            return false; // Not enough space in the destination span or unsupported format string

        var pos = 0;

        if (!Major.TryFormat(utf8Destination[pos..], out var written))
            return false;
        pos += written;
        utf8Destination[pos++] = (byte)'.';

        if (!Minor.TryFormat(utf8Destination[pos..], out written))
            return false;
        pos += written;
        utf8Destination[pos++] = (byte)'.';

        if (!Patch.TryFormat(utf8Destination[pos..], out written))
            return false;
        pos += written;

        if (PreRelease.Length > 0)
        {
            utf8Destination[pos++] = (byte)'-';
            if (!Encoding.UTF8.TryGetBytes(PreRelease.AsSpan(), utf8Destination[pos..], out written))
                return false;
            pos += written;
        }

        if (BuildMetadata.Length > 0)
        {
            utf8Destination[pos++] = (byte)'+';
            if (!Encoding.UTF8.TryGetBytes(BuildMetadata.AsSpan(), utf8Destination[pos..], out written))
                return false;
            pos += written;
        }

        bytesWritten = pos;
        return true;
    }
    #endregion

    #region IEqualityOperators<SemVer>
    /// <summary>
    /// Determines whether two <see cref="SemVer"/> instances are equal. Two semantic version instances are considered equal if
    /// they have the same major, minor, patch, and pre-release components.
    /// </summary>
    /// <param name="left">The first <see cref="SemVer"/> instance to compare.</param>
    /// <param name="right">The second <see cref="SemVer"/> instance to compare.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="SemVer"/> instances are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(SemVer left, SemVer right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="SemVer"/> instances are not equal. Two semantic version instances are considered not
    /// equal if they differ in any of their major, minor, patch, or pre-release components.
    /// </summary>
    /// <param name="left">The first <see cref="SemVer"/> instance to compare.</param>
    /// <param name="right">The second <see cref="SemVer"/> instance to compare.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="SemVer"/> instances are not equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(SemVer left, SemVer right) => !left.Equals(right);

    /// <summary>
    /// Determines whether the first <see cref="SemVer"/> instance is less than the second <see cref="SemVer"/> instance.
    /// Two semantic version instances are compared based on their major, minor, patch, and pre-release components,
    /// following the precedence rules defined by the SemVer 2.0.0 specification.
    /// </summary>
    /// <param name="left">The first <see cref="SemVer"/> instance to compare.</param>
    /// <param name="right">The second <see cref="SemVer"/> instance to compare.</param>
    /// <returns>
    /// <c>true</c> if the first <see cref="SemVer"/> instance is less than the second <see cref="SemVer"/> instance; otherwise,
    /// <c>false</c>.
    /// </returns>
    public static bool operator <(SemVer left, SemVer right) => left.CompareTo(right)<0;

    /// <summary>
    /// Determines whether the first <see cref="SemVer"/> instance is less than or equal to the second <see cref="SemVer"/>
    /// instance. Two semantic version instances are compared based on their major, minor, patch, and pre-release components,
    /// following the precedence rules defined by the SemVer 2.0.0 specification.
    /// </summary>
    /// <param name="left">The first <see cref="SemVer"/> instance to compare.</param>
    /// <param name="right">The second <see cref="SemVer"/> instance to compare.</param>
    /// <returns>
    /// <c>true</c> if the first <see cref="SemVer"/> instance is less than or equal to the second <see cref="SemVer"/>
    /// instance; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator <=(SemVer left, SemVer right) => left.CompareTo(right)<=0;

    /// <summary>
    /// Determines whether the first <see cref="SemVer"/> instance is greater than the second <see cref="SemVer"/> instance. Two
    /// semantic version instances are compared based on their major, minor, patch, and pre-release components,
    /// following the precedence rules defined by the SemVer 2.0.0 specification.
    /// </summary>
    /// <param name="left">The first <see cref="SemVer"/> instance to compare.</param>
    /// <param name="right">The second <see cref="SemVer"/> instance to compare.</param>
    /// <returns>
    /// <c>true</c> if the first <see cref="SemVer"/> instance is greater than the second <see cref="SemVer"/> instance;
    /// otherwise, <c>false</c>.
    /// </returns>
    public static bool operator >(SemVer left, SemVer right) => left.CompareTo(right)>0;

    /// <summary>
    /// Determines whether the first <see cref="SemVer"/> instance is greater than or equal to the second <see cref="SemVer"/>
    /// instance. Two semantic version instances are compared based on their major, minor, patch, and pre-release components,
    /// following the precedence rules defined by the SemVer 2.0.0 specification.
    /// </summary>
    /// <param name="left">The first <see cref="SemVer"/> instance to compare.</param>
    /// <param name="right">The second <see cref="SemVer"/> instance to compare.</param>
    /// <returns>
    /// <c>true</c> if the first <see cref="SemVer"/> instance is greater than or equal to the second <see cref="SemVer"/>
    /// instance; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator >=(SemVer left, SemVer right) => left.CompareTo(right)>=0;
    #endregion
}
