// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.SemVer;

using System.Text.Json;

using vm2;
using vm2.TestUtilities;

public partial class SemVerTests : TestBase
{
    [Theory]
    [MemberData(nameof(SemVerComponentCases))]
    public void SysJson_RoundTrip_ShouldPreserveValue(
        int major, int minor, int patch, string preRelease, string buildMetadata)
    {
        var original = new SemVer(major, minor, patch, preRelease, buildMetadata);

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<SemVer>(json);

        deserialized.Should().Be(original);
    }

    [Fact]
    public void SysJson_Serialize_ShouldWriteQuotedString()
    {
        var version = new SemVer(1, 2, 3, "rc.1", "build.7");

        var json = JsonSerializer.Serialize(version);

        // Utf8JsonWriter escapes '+' as '\u002B' by default
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.GetString().Should().Be("1.2.3-rc.1+build.7");
    }

    [Fact]
    public void SysJson_Serialize_CoreVersion_ShouldWriteQuotedString()
    {
        var version = new SemVer(1, 2, 3);

        var json = JsonSerializer.Serialize(version);

        json.Should().Be("\"1.2.3\"");
    }

    [Fact]
    public void SysJson_Deserialize_NullableSemVer_WhenJsonIsNull_ShouldReturnNull()
    {
        var result = JsonSerializer.Deserialize<SemVer?>("null");

        result.Should().BeNull();
    }

    [Fact]
    public void SysJson_Deserialize_WhenJsonIsInvalidSemVer_ShouldThrowJsonException()
    {
        Action act = () => JsonSerializer.Deserialize<SemVer>("\"not-a-semver\"");

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void SysJson_Deserialize_WhenJsonIsNumber_ShouldThrowJsonException()
    {
        Action act = () => JsonSerializer.Deserialize<SemVer>("42");

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void SysJson_RoundTrip_NullableSemVer_WithValue()
    {
        var original = (SemVer?)new SemVer(1, 2, 3);

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<SemVer?>(json);

        deserialized.Should().Be(original);
    }

    [Fact]
    public void SysJson_RoundTrip_NullableSemVer_Null()
    {
        SemVer? original = null;

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<SemVer?>(json);

        json.Should().Be("null");
        deserialized.Should().BeNull();
    }

    [Fact]
    public void SysJson_RoundTrip_ObjectWithSemVerProperty()
    {
        var obj = new SysJsonTestModel { Version = new SemVer(1, 2, 3, "beta.1"), Label = "test" };

        var json = JsonSerializer.Serialize(obj);
        var deserialized = JsonSerializer.Deserialize<SysJsonTestModel>(json);

        deserialized!.Version.Should().Be(obj.Version);
        deserialized.Label.Should().Be("test");
    }

    [Fact]
    public void SysJson_RoundTrip_ObjectWithNullableSemVerProperty_Null()
    {
        var obj = new SysJsonTestModelNullable { Version = null, Label = "test" };

        var json = JsonSerializer.Serialize(obj);
        var deserialized = JsonSerializer.Deserialize<SysJsonTestModelNullable>(json);

        deserialized!.Version.Should().BeNull();
    }

    [Fact]
    public void SysJson_Deserialize_WhenJsonIsNull_NonNullable_ShouldThrowJsonException()
    {
        // Directly deserializing "null" into non-nullable SemVer should throw
        Action act = () => JsonSerializer.Deserialize<SemVer>("null");

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void SysJson_RoundTrip_WhenLengthExceedsThreshold_ShouldPreserveValue()
    {
        var original = MakeLargeSemVer();

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<SemVer>(json);

        deserialized.Should().Be(original);
    }
}
