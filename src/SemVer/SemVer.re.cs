// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2;

public readonly partial struct SemVer
{
    // Abbreviations and conventions used for building regular expressions:
    //
    // RE: regular expression.
    //
    // Charset: a set of characters (that is, a character-class fragment).
    //
    // Charsets are strings that contain literal characters (for example, "abc") and/or character ranges (for example, "A-Z").
    // Charsets can be concatenated, subject to .NET regex syntax rules, to build larger charsets, for example:
    // `alphanumericChars = $"{letterChars}{digitChars}";`.
    // A charset may represent a BNF term that is defined as a set of characters, for example:
    // `letter ::= "A" | "B" | "C" | ... | "Z" | "a" | "b" | "c" | ... | "z"`.
    // A charset is not a regular expression by itself; it is typically wrapped in square brackets to form one, for example:
    // `[letterChars]`.
    // By convention, non-public charset constants use camelCase and the "Chars" suffix, for example `letterChars`.
    //
    // Non-public constants in camelCase without a suffix represent regex fragments.
    // Most of these are valid regex patterns on their own, but they are intended for composition rather than standalone use.
    //
    // Whitespace rule:
    // If a fragment includes readability spaces around operators (for example, around `|`), every regex that includes that
    // fragment MUST be compiled or generated with RegexOptions.IgnorePatternWhitespace.
    //
    // Rex vs Regex:
    // - `*Rex` constants are generally unanchored patterns intended for composition or searching within larger strings.
    // - `*Regex` constants are full-string validation patterns, typically anchored with `^` and `$`.
    //
    // Only `*Regex` constants get public Regex factory methods.
    // These methods are named after the constant without the "Regex" suffix (for example, `SemVer20()`), and are generated
    // via `GeneratedRegexAttribute` using the corresponding `*Regex` pattern.
    //
    // Quick convention table:
    // - Non-public `*Chars` (camelCase): character-class fragments (not standalone regex patterns).
    // - Non-public camelCase without suffix: regex fragments for composition.
    // - Public `*Rex` (PascalCase): generally unanchored public patterns.
    // - Public `*Regex` (PascalCase): anchored full-string validation patterns.
    // - Public methods named like `*Regex` without suffix: `GeneratedRegex` factories for `*Regex` constants.

    /// <summary>
    /// Charset containing the letters in identifiers.
    /// letter = 'A' | 'B' | 'C' | ... | 'Z' | 'a' | 'b' | 'c' | ... | 'z'
    /// </summary>
    const string letterChars = "A-Za-z";

    /// <summary>
    /// Charset containing the positive digits in identifiers.
    /// positive_digit = '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9'
    /// </summary>
    const string positiveDigitChars = "1-9";

    /// <summary>
    /// RE that matches a positive digit in identifiers.
    /// positive_digit = '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9'
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "1", "2", "3", etc.
    /// </example>
    const string positiveDigit = $"[ {positiveDigitChars} ]";

    /// <summary>
    /// Charset containing the digits in identifiers.
    /// digit = '0' | positive_digit
    /// </summary>
    const string digitChars = $"0-9"; // <=> $"0{positiveDigitChars}";

    /// <summary>
    /// RE that matches a digit in identifiers.
    /// digit = '0' | positive_digit
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "0", "1", "2", etc.
    /// </example>
    const string digit = $"[ {digitChars} ]";

    /// <summary>
    /// digits = digit+
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "0", "1", "2", etc.
    /// </example>
    const string digits = $"{digit}+";

    /// <summary>
    /// Charset containing the non-digit characters in identifiers.
    /// non_digit_chars = letter | '-'
    /// </summary>
    const string nonDigitChars = $"{letterChars}-";

    /// <summary>
    /// RE that matches a non-digit character in identifiers.
    /// non_digit = letter | '-'   or non_digit_set
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "A", "B", etc. and "a", "b", etc. and "-" etc.
    /// </example>
    const string nonDigit = $"[ {nonDigitChars} ]";

    /// <summary>
    /// Charset containing the characters that can be used in identifiers.
    /// identifier_character_chars = digit | non_digit
    /// </summary>
    const string identifierCharacterChars = $"{digitChars}{nonDigitChars}";

    /// <summary>
    /// RE that matches an identifier character in identifiers.
    /// identifier_character = 1 { digit | non_digit }  or  1{ identifier_character_set }
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "0", "1", "2", etc. and "A", "B", "C", etc. and "a", "b", "c", etc. and "-" etc.
    /// </example>
    const string identifierCharacter = $"[ {identifierCharacterChars} ]";

    /// <summary>
    /// identifier_characters = +{ identifier_character }
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "0", "A", "-", etc. and combinations of these characters like "01", "Ab", "1A-", "a-2", etc.
    /// </example>
    const string identifierCharacters = $"{identifierCharacter}+";

    /// <summary>
    /// numericIdentifier = '0' | positive_digit digit*
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "0", "1", "2", "10", "23", "107", etc.
    /// </example>
    const string numericIdentifier = $"(?: 0 | {positiveDigit}{digit}* )";

    /// <summary>
    /// numericIdentifier = '0' | positive_digit digit*
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "0", "1", "2", "10", "23", "107", etc.
    /// </example>
    /// <remarks>
    /// The regex must be run/compiled with the option <see cref="RegexOptions.IgnorePatternWhitespace"/> for correct performance.
    /// </remarks>
    public const string NumericIdentifierRex = numericIdentifier;

    /// <summary>
    /// numericIdentifier = '0' | positive_digit digit*
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "0", "1", "2", "10", "23", "107", etc.
    /// </example>
    /// <remarks>
    /// The regex must be run/compiled with the option <see cref="RegexOptions.IgnorePatternWhitespace"/> for correct performance.
    /// </remarks>
    public const string NumericIdentifierRegex = $"^(?: {NumericIdentifierRex} )$";

    /// <summary>
    /// Method returns a <see cref="Regex"/> instance that can be used to validate numeric identifier strings against the SemVer 2.0.0 specification.
    /// </summary>
    [GeneratedRegex(NumericIdentifierRegex, RegexOptions.IgnorePatternWhitespace)]
    public static partial Regex NumericIdentifier();

    /// <summary>
    /// alphanumeric_identifier = { non_digit } | { non_digit }{ identifier_characters } | { identifier_characters } { non_digit } | { identifier_characters } { non_digit } { identifier_characters }
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "A", "1A", "A1", "A1B", "1A1", "1-A", "A-1", "A-1B", etc. But not "1", "01", etc.
    /// </example>
    const string alphanumericIdentifier = $"(?: {identifierCharacter}* {nonDigit} {identifierCharacter}* )";    // has to have at least one non-digit character

    /// <summary>
    /// buildIdentifier = { alphanumeric_identifier } | { digits }
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "A", "1A", "A1", "A1B", "01A1", "1-A", "A-1", "A-1B", etc. and "0", "1", "02", "10", "23", "00107", etc.
    /// </example>
    const string buildIdentifier = $"(?> {alphanumericIdentifier} | {digits} )";

    /// <summary>
    /// preReleaseIdentifier = { alphanumeric_identifier } | { numericIdentifier }
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "A", "1A", "A1", "A1B", "1-A", "A-1", "A-1B", etc. and "0", "1", "2", "10", "023", "107", etc. But not "01", etc.
    /// </example>
    const string preReleaseIdentifier = $"(?> {alphanumericIdentifier} | {numericIdentifier} )";

    /// <summary>
    /// dotSeparatedBuildIdentifiers = buildIdentifier ('.' buildIdentifier)*
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "A", "1A", "A1", "A1B", "01A1", "1-A", "A-1", "A-1B", etc. and "0", "1", "2", "10", "023", "107", etc. and combinations of these separated by dots like "A.1A.01A1" or "1.10.023" etc.
    /// </example>
    const string dotSeparatedBuildIdentifiers = @$"(?> {buildIdentifier} (?:\.{buildIdentifier})* )";

    /// <summary>
    /// build = dotSeparatedBuildIdentifiers
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "A", "1A", "A1", "A1B", "01A1", "1-A", "A-1", "A-1B", etc. and "0", "1", "2", "10", "023", "107", etc. and combinations of these separated by dots like "A.1A.01A1" or "1.10.023" etc.
    /// </example>
    const string build = dotSeparatedBuildIdentifiers;

    /// <summary>
    /// dotSeparatedPreReleaseIdentifiers = preReleaseIdentifier ('.' preReleaseIdentifier)*
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "A", "1A", "A1", "A1B", "1-A", "A-1", "A-1B", etc. and "0", "1", "2", "10", "023", "107", etc. But not "01", etc. and combinations of these separated by dots like "A.1A.01A1" or "1.10.023" etc.
    /// </example>
    const string dotSeparatedPreReleaseIdentifiers = @$"(?> {preReleaseIdentifier} (?:\.{preReleaseIdentifier})* )";

    /// <summary>
    /// preRelease = dotSeparatedPreReleaseIdentifiers
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "A", "1A", "A1", "A1B", "1-A", "A-1", "A-1B", etc. and "0", "1", "2", "10", "023", "107", etc. But not "01", etc. and combinations of these separated by dots like "A.1A.01A1" or "1.10.023" etc.
    /// </example>
    const string preRelease = dotSeparatedPreReleaseIdentifiers;

    /// <summary>
    /// Matches a build metadata identifier in a string.
    /// </summary>
    /// <remarks>
    /// The regex must be run/compiled with the option <see cref="RegexOptions.IgnorePatternWhitespace"/> for correct performance.
    /// </remarks>
    public const string BuildRex = dotSeparatedBuildIdentifiers;

    /// <summary>
    /// Matches a string if it is a SemVer build metadata identifier.
    /// </summary>
    /// <remarks>
    /// The regex must be run/compiled with the option <see cref="RegexOptions.IgnorePatternWhitespace"/> for correct performance.
    /// </remarks>
    public const string BuildRegex = $"^{BuildRex}$";

    /// <summary>
    /// The method returns a <see cref="Regex"/> instance that can be used to validate build metadata strings against the SemVer 2.0.0 specification.
    /// </summary>
    [GeneratedRegex(BuildRegex, RegexOptions.IgnorePatternWhitespace)]
    public static partial Regex BuildIdentifier();

    /// <summary>
    /// Matches a pre-release version in a string.
    /// </summary>
    /// <remarks>
    /// The regex must be run/compiled with the option <see cref="RegexOptions.IgnorePatternWhitespace"/> for correct performance.
    /// </remarks>
    public const string PreReleaseRex = preRelease;

    /// <summary>
    /// Matches a string if it is a SemVer pre-release version.
    /// </summary>
    /// <remarks>
    /// The regex must be run/compiled with the option <see cref="RegexOptions.IgnorePatternWhitespace"/> for correct performance.
    /// </remarks>
    public const string PreReleaseRegex = $"^{PreReleaseRex}$";

    /// <summary>
    /// The method returns a <see cref="Regex"/> instance that can be used to validate pre-release version strings against the SemVer 2.0.0 specification.
    /// </summary>
    [GeneratedRegex(PreReleaseRegex, RegexOptions.IgnorePatternWhitespace)]
    public static partial Regex PreReleaseIdentifier();

    /// <summary>
    /// patch = numericIdentifier
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "0", "1", "2", "10", "23", "107", etc. But not "01", "023", etc.
    /// </example>
    const string patch = NumericIdentifierRex;

    /// <summary>
    /// minor = numericIdentifier
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "0", "1", "2", "10", "23", "107", etc. But not "01", etc.
    /// </example>
    const string minor = NumericIdentifierRex;
    /// <summary>
    /// major = numericIdentifier
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "0", "1", "2", "10", "23", "107", etc. But not "01", "023", etc.
    /// </example>
    const string major = NumericIdentifierRex;

    #region Capturing group names
    /// <summary>
    /// The name of a matching group representing the major version in a SemVer.
    /// </summary>
    public const string MajorGr = "major";

    /// <summary>
    /// The name of a matching group representing the minor version in a SemVer.
    /// </summary>
    public const string MinorGr = "minor";

    /// <summary>
    /// The name of a matching group representing the patch version in a SemVer.
    /// </summary>
    public const string PatchGr = "patch";

    /// <summary>
    /// The name of a matching group representing the core version in a SemVer (maj.min.patch only).
    /// </summary>
    public const string CoreGr = "core";

    /// <summary>
    /// The name of a matching group representing the pre-release version in a SemVer (maj.min.patch only).
    /// </summary>
    public const string PreReleaseGr = "pre";

    /// <summary>
    /// The name of a matching group representing the build meta-data in a SemVer.
    /// </summary>
    public const string BuildGr = "build";
    #endregion

    /// <summary>
    /// versionCore = major '.' minor '.' patch
    /// </summary>
    /// <example>
    /// Strings that match the pattern: "0.0.0", "1.2.3", "10.20.30", etc. But not "01.2.3", "1.02.3", "1.2.03", etc.
    /// </example>
    const string versionCore = @$"(?<{MajorGr}> {major} ) \. (?<{MinorGr}> {minor} ) \. (?<{PatchGr}> {patch} )";

    /// <summary>
    /// Regular expression that matches semantic version (SemVer) in a string.
    /// <see href="https://semver.org/#backusnaur-form-grammar-for-valid-semver-versions"/>
    /// </summary>
    /// <remarks>
    /// The regex must be run/compiled with the option <see cref="RegexOptions.IgnorePatternWhitespace"/> for correct performance.
    /// </remarks>
    public const string ValidSemVerRex = @$"(?<{CoreGr}> {versionCore} )(?: - (?<{PreReleaseGr}> {preRelease} ) )? (?: \+ (?<{BuildGr}> {build} ) )?";

    /// <summary>
    /// Regular expression that matches if a string is a valid semantic version (SemVer).
    /// <see href="https://semver.org/#backusnaur-form-grammar-for-valid-semver-versions"/>
    /// </summary>
    /// <remarks>
    /// The regex must be run/compiled with the option <see cref="RegexOptions.IgnorePatternWhitespace"/> for correct performance.
    /// </remarks>
    public const string ValidSemVerRegex = @$"^{ValidSemVerRex}$";

    /// <summary>
    /// Method returns a <see cref="Regex"/> instance that can be used to validate semantic version strings against the SemVer 2.0.0 specification.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex"/> instance that can be used to validate semantic version strings.
    /// </returns>
    [GeneratedRegex(ValidSemVerRegex, RegexOptions.IgnorePatternWhitespace)]
    public static partial Regex SemVer20();
}
