using System;
using FluentAssertions;
using MikeyT.EnvironmentSettingsNS.Enums;
using MikeyT.EnvironmentSettingsNS.Interface;
using MikeyT.EnvironmentSettingsNS.Logic;
using Moq;
using Xunit;

namespace MikeyT.EnvironmentSettingsNS.Tests
{
    public class EnvironmentSettingsTest
    {
        private EnvironmentSettings _envSettings;
        private readonly Mock<IEnvironmentVariableProvider> _environmentVariableProvider = new();

        public EnvironmentSettingsTest()
        {
            _environmentVariableProvider.Reset();
            _envSettings = new EnvironmentSettings(_environmentVariableProvider.Object);
            _envSettings.AddSettings<TestEnvironmentSettings>();
        }

        [Fact]
        public void AddSettings_NoEnvVars_DefaultSettingsLoaded()
        {
            _environmentVariableProvider.Setup(envVarProvider => envVarProvider.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")).Returns("Development");
            _envSettings = new EnvironmentSettings(_environmentVariableProvider.Object);
            _envSettings.AddSettings<TestEnvironmentSettings>();

            var someStringSetting = _envSettings.GetString(TestEnvironmentSettings.SomeStringSetting);
            var someIntSetting = _envSettings.GetInt(TestEnvironmentSettings.SomeIntSetting);
            var someBoolSettingTrue = _envSettings.GetBool(TestEnvironmentSettings.SomeBoolSettingTrue);
            var someBoolSettingFalse = _envSettings.GetBool(TestEnvironmentSettings.SomeBoolSettingFalse);
            var someSecretSettingWithLocalDefaultOnly = _envSettings.GetString(TestEnvironmentSettings.SomeSecretSettingWithLocalDefaultOnly);
            var someSecretSettingWithAllEnvironmentsDefault = _envSettings.GetString(TestEnvironmentSettings.SomeSecretSettingWithAllEnvironmentsDefault);

            someStringSetting.Should().Be("some string");
            someIntSetting.Should().Be(42);
            someBoolSettingTrue.Should().BeTrue();
            someBoolSettingFalse.Should().BeFalse();
            someSecretSettingWithLocalDefaultOnly.Should().Be("test_secret_local_only_default");
            someSecretSettingWithAllEnvironmentsDefault.Should().Be("test_secret_all_environment_default");
        }

        [Fact]
        public void Getters_NoEnvVarAndNoAttribute_Throws()
        {
            FluentActions.Invoking(() => _envSettings.GetString(TestEnvironmentSettings.SettingWithNoAttribute))
                .Should().Throw<ApplicationException>()
                .WithMessage("Setting not loaded: SettingWithNoAttribute");

            FluentActions.Invoking(() => _envSettings.GetInt(TestEnvironmentSettings.SettingWithNoAttribute))
                .Should().Throw<ApplicationException>()
                .WithMessage("Setting not loaded: SettingWithNoAttribute");

            FluentActions.Invoking(() => _envSettings.GetBool(TestEnvironmentSettings.SettingWithNoAttribute))
                .Should().Throw<ApplicationException>()
                .WithMessage("Setting not loaded: SettingWithNoAttribute");
        }

        [Fact]
        public void Getters_NoEnvVarAndNoAttributeButDefaultProvided_ReturnsDefault()
        {
            const string expectedString = "some default";
            const int expectedInt = 23;
            const bool expectedBoolTrue = true;
            const bool expectedBoolFalse = false;

            var actualString = _envSettings.GetString(TestEnvironmentSettings.SettingWithNoAttribute, expectedString);
            var actualInt = _envSettings.GetInt(TestEnvironmentSettings.SettingWithNoAttribute, expectedInt);
            var actualBoolTrue = _envSettings.GetBool(TestEnvironmentSettings.SettingWithNoAttribute, expectedBoolTrue);
            var actualBoolFalse = _envSettings.GetBool(TestEnvironmentSettings.SettingWithNoAttribute, expectedBoolFalse);

            actualString.Should().Be(expectedString);
            actualInt.Should().Be(expectedInt);
            actualBoolTrue.Should().BeTrue();
            actualBoolFalse.Should().BeFalse();
        }

        [Fact]
        public void Getters_TypeMismatch_Throws()
        {
            FluentActions.Invoking(() => _envSettings.GetBool(TestEnvironmentSettings.SomeIntSetting))
                .Should().Throw<ApplicationException>()
                .WithMessage("Could not parse setting to bool: SomeIntSetting");

            FluentActions.Invoking(() => _envSettings.GetInt(TestEnvironmentSettings.SomeBoolSettingTrue))
                .Should().Throw<ApplicationException>()
                .WithMessage("Could not parse setting to int: SomeBoolSettingTrue");
        }

        [Fact]
        public void GetString_NoEnum_Throws()
        {
            FluentActions.Invoking(() => _envSettings.GetString("DoesNotExist"))
                .Should().Throw<ApplicationException>()
                .WithMessage("Setting not loaded: DoesNotExist");
        }

        [Fact]
        public void GetString_NoEnumButDefaultProvided_ReturnsDefault()
        {
            const string expected = "some default";
            var actual = _envSettings.GetString("DoesNotExist", expected);

            actual.Should().Be(expected);
        }

        [Fact]
        public void AddSettings_WithSettingsAlreadyLoaded_HasBothSetsOfSettings()
        {
            _envSettings.AddSettings<TestEnvironmentSettingsAlt>();

            var fromFirst = _envSettings.GetString(TestEnvironmentSettings.SomeStringSetting);
            var fromAlt = _envSettings.GetString(TestEnvironmentSettingsAlt.SomeStringSettingAlt);

            fromFirst.Should().Be("some string");
            fromAlt.Should().Be("foo");
        }

        [Fact]
        public void Getters_EnvVarsExist_ReturnsEnvVarValues()
        {
            const string expectedString = "string from env var";
            const int expectedInt = 12345;
            const bool expectedBool = true;

            _environmentVariableProvider.Setup(envProvider => envProvider.GetEnvironmentVariable(TestEnvironmentSettings.SomeStringSetting.ToName()))
                .Returns(expectedString);
            _environmentVariableProvider.Setup(envProvider => envProvider.GetEnvironmentVariable(TestEnvironmentSettings.SomeIntSetting.ToName()))
                .Returns(expectedInt.ToString());
            _environmentVariableProvider.Setup(envProvider => envProvider.GetEnvironmentVariable(TestEnvironmentSettings.SomeBoolSettingTrue.ToName()))
                .Returns(expectedBool.ToString());

            var actualString = _envSettings.GetString(TestEnvironmentSettings.SomeIntSetting);
            var actualInt = _envSettings.GetString(TestEnvironmentSettings.SomeIntSetting);
            var actualBool = _envSettings.GetString(TestEnvironmentSettings.SomeBoolSettingTrue);

            actualString.Should().Be(actualString);
            actualInt.Should().Be(actualInt);
            actualBool.Should().Be(actualBool);
        }

        [Fact]
        public void GetAllAsSafeLogString_PropOptionsRespected()
        {
            var safeLogString = _envSettings.GetAllAsSafeLogString();

            safeLogString.Should().NotBeEmpty();
            safeLogString.Should().Contain("SettingWithNoAttribute=");
            safeLogString.Should().Contain("SomeStringSetting=some string");
            safeLogString.Should().Contain("SomeIntSetting=42");
            safeLogString.Should().Contain("SomeBoolSettingTrue=true");
            safeLogString.Should().Contain("SomeBoolSettingFalse=false");
            safeLogString.Should().Contain("SettingWithNoDefaultValue=");
            safeLogString.Should().Contain("SomeSecretSettingWithLocalDefaultOnly=");
            safeLogString.Should().Contain("SomeSecretSettingWithAllEnvironmentsDefault=");
            safeLogString.Should().NotContainAny("test_secret_local_only_default", "test_secret_all_environment_default");
        }

        [Fact]
        public void GetString_NotLocalWithLocalOnlyDefault_Throws()
        {
            _environmentVariableProvider.Setup(envVarProvider => envVarProvider.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")).Returns("Production");
            _envSettings = new EnvironmentSettings(_environmentVariableProvider.Object);
            _envSettings.AddSettings<TestEnvironmentSettings>();

            FluentActions.Invoking(() => _envSettings.GetString(TestEnvironmentSettings.SomeSecretSettingWithLocalDefaultOnly))
                .Should().Throw<ApplicationException>()
                .WithMessage("Setting not loaded: SomeSecretSettingWithLocalDefaultOnly");
        }

        [Fact]
        public void GetString_NotLocalWithAllEnvironmentsDefault_Throws()
        {
            var actual = _envSettings.GetString(TestEnvironmentSettings.SomeSecretSettingWithAllEnvironmentsDefault);
            actual.Should().Be("test_secret_all_environment_default");
        }
    }
}