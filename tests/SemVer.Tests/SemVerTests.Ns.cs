// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.SemVer;

using Newtonsoft.Json;

using vm2;
using vm2.TestUtilities;

using vm2.Serialization.NsJson.SemVer;

public partial class SemVerTests : TestBase
{
    JsonSerializerSettings _settings;

    public SemVerTests(ITestOutputHelper output) : base(output)
    {
        _settings = new JsonSerializerSettings();
        _settings.Converters.Add(new SemVerConverter());
    }

    #region Newtonsoft.Json converter
    [Theory]
    [MemberData(nameof(SemVerComponentCases))]
    public void NsJson_RoundTrip_ShouldPreserveValue(
        int major, int minor, int patch, string preRelease, string buildMetadata)
    {
        var original = new SemVer(major, minor, patch, preRelease, buildMetadata);

        var json = JsonConvert.SerializeObject(original, _settings);
        var deserialized = JsonConvert.DeserializeObject<SemVer>(json, _settings);

        deserialized.Should().Be(original);
    }

    [Fact]
    public void NsJson_Serialize_ShouldWriteQuotedString()
    {
        var version = new SemVer(1, 2, 3, "rc.1", "build.7");

        var json = JsonConvert.SerializeObject(version, _settings);

        json.Should().Be("\"1.2.3-rc.1+build.7\"");
    }

    [Fact]
    public void NsJson_Deserialize_NullableSemVer_WhenJsonIsNull_ShouldReturnNull()
    {
        var result = JsonConvert.DeserializeObject<SemVer?>("null", _settings);

        result.Should().BeNull();
    }

    [Fact]
    public void NsJson_Deserialize_WhenJsonIsInvalidSemVer_ShouldThrowJsonReaderException()
    {
        Action act = () => JsonConvert.DeserializeObject<SemVer>("\"not-a-semver\"", _settings);

        act.Should().Throw<JsonReaderException>();
    }

    [Fact]
    public void NsJson_Deserialize_WhenJsonIsNumber_ShouldThrowJsonReaderException()
    {
        Action act = () => JsonConvert.DeserializeObject<SemVer>("42", _settings);

        act.Should().Throw<JsonReaderException>();
    }

    [Fact]
    public void NsJson_RoundTrip_NullableSemVer_WithValue()
    {
        var original = (SemVer?)new SemVer(1, 2, 3);

        var json = JsonConvert.SerializeObject(original, _settings);
        var deserialized = JsonConvert.DeserializeObject<SemVer?>(json, _settings);

        deserialized.Should().Be(original);
    }

    [Fact]
    public void NsJson_RoundTrip_NullableSemVer_Null()
    {
        SemVer? original = null;

        var json = JsonConvert.SerializeObject(original, _settings);
        var deserialized = JsonConvert.DeserializeObject<SemVer?>(json, _settings);

        json.Should().Be("null");
        deserialized.Should().BeNull();
    }

    [Fact]
    public void NsJson_RoundTrip_ObjectWithSemVerProperty()
    {
        var obj = new NsJsonTestModel { Version = new SemVer(1, 2, 3, "beta.1"), Label = "test" };

        var json = JsonConvert.SerializeObject(obj, _settings);
        var deserialized = JsonConvert.DeserializeObject<NsJsonTestModel>(json, _settings);

        deserialized!.Version.Should().Be(obj.Version);
        deserialized.Label.Should().Be("test");
    }

    [Fact]
    public void NsJson_RoundTrip_ObjectWithNullableSemVerProperty_Null()
    {
        var obj = new NsJsonTestModelNullable { Version = null, Label = "test" };

        var json = JsonConvert.SerializeObject(obj, _settings);
        var deserialized = JsonConvert.DeserializeObject<NsJsonTestModelNullable>(json, _settings);

        deserialized!.Version.Should().BeNull();
    }

    private sealed class NsJsonTestModel
    {
        public SemVer Version { get; set; }
        public string Label { get; set; } = "";
    }

    private sealed class NsJsonTestModelNullable
    {
        public SemVer? Version { get; set; }
        public string Label { get; set; } = "";
    }
    #endregion

    [Fact]
    public void NsJsonConverter_CanConvert_ShouldReturnTrueForSemVer()
    {
        var converter = new SemVerConverter();

        converter.CanConvert(typeof(SemVer)).Should().BeTrue();
    }

    [Fact]
    public void NsJsonConverter_CanConvert_ShouldReturnTrueForNullableSemVer()
    {
        var converter = new SemVerConverter();

        converter.CanConvert(typeof(SemVer?)).Should().BeTrue();
    }

    [Fact]
    public void NsJsonConverter_CanConvert_ShouldReturnFalseForOtherTypes()
    {
        var converter = new SemVerConverter();

        converter.CanConvert(typeof(string)).Should().BeFalse();
        converter.CanConvert(typeof(int)).Should().BeFalse();
        converter.CanConvert(typeof(object)).Should().BeFalse();
    }

    [Fact]
    public void NsJsonConverter_WriteJson_WhenValueIsNull_ShouldWriteNull()
    {
        var converter = new SemVerConverter();
        var sb = new StringWriter();
        var writer = new JsonTextWriter(sb);

        converter.WriteJson(writer, null, JsonSerializer.CreateDefault());
        writer.Flush();

        sb.ToString().Should().Be("null");
    }

    [Fact]
    public void NsJsonConverter_WriteJson_WhenValueIsNotSemVer_ShouldThrow()
    {
        var converter = new SemVerConverter();
        var sb = new StringWriter();
        var writer = new JsonTextWriter(sb);

        Action act = () => converter.WriteJson(writer, "not-a-semver", JsonSerializer.CreateDefault());

        act.Should().Throw<JsonWriterException>();
    }
}
