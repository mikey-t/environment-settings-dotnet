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
        private readonly Mock<ISecretVariableProvider> _secretVariableProvider = new();

        public EnvironmentSettingsTest()
        {
            _environmentVariableProvider.Reset();
            _secretVariableProvider.Reset();
            _environmentVariableProvider.Setup(m => m.GetEnvironmentVariable(TestEnvironmentSettings.DONT_START_APP_WITHOUT_ME.ToString())).Returns("anything");
            _envSettings = new EnvironmentSettings(_environmentVariableProvider.Object, _secretVariableProvider.Object);
            _envSettings.AddSettings<TestEnvironmentSettings>();
        }

        [Fact]
        public void AddSettings_NoEnvVars_DefaultSettingsLoaded()
        {
            _environmentVariableProvider.Setup(envVarProvider => envVarProvider.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")).Returns("Development");
            _envSettings = new EnvironmentSettings(_environmentVariableProvider.Object, _secretVariableProvider.Object);
            _envSettings.AddSettings<TestEnvironmentSettings>();

            var someStringSetting = _envSettings.GetString(TestEnvironmentSettings.SOME_SETTING_STRING);
            var someIntSetting = _envSettings.GetInt(TestEnvironmentSettings.SOME_INT_SETTING);
            var someBoolSettingTrue = _envSettings.GetBool(TestEnvironmentSettings.SOME_BOOL_SETTING_TRUE);
            var someBoolSettingFalse = _envSettings.GetBool(TestEnvironmentSettings.SOME_BOOL_SETTING_FALSE);
            var someSecretSettingWithLocalDefaultOnly = _envSettings.GetString(TestEnvironmentSettings.SOME_SECRET_SETTING_WITH_LOCAL_DEFAULT_ONLY);
            var someSecretSettingWithAllEnvironmentsDefault = _envSettings.GetString(TestEnvironmentSettings.SOME_SECRET_SETTING_WITH_ALL_ENVIRONMENTS_DEFAULT);

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
            FluentActions.Invoking(() => _envSettings.GetString(TestEnvironmentSettings.SETTING_WITH_NO_ATTRIBUTE))
                .Should().Throw<ApplicationException>()
                .WithMessage($"Setting not loaded: {TestEnvironmentSettings.SETTING_WITH_NO_ATTRIBUTE}");

            FluentActions.Invoking(() => _envSettings.GetInt(TestEnvironmentSettings.SETTING_WITH_NO_ATTRIBUTE))
                .Should().Throw<ApplicationException>()
                .WithMessage($"Setting not loaded: {TestEnvironmentSettings.SETTING_WITH_NO_ATTRIBUTE}");

            FluentActions.Invoking(() => _envSettings.GetBool(TestEnvironmentSettings.SETTING_WITH_NO_ATTRIBUTE))
                .Should().Throw<ApplicationException>()
                .WithMessage($"Setting not loaded: {TestEnvironmentSettings.SETTING_WITH_NO_ATTRIBUTE}");
        }

        [Fact]
        public void Getters_NoEnvVarAndNoAttributeButDefaultProvided_ReturnsDefault()
        {
            const string expectedString = "some default";
            const int expectedInt = 23;
            const bool expectedBoolTrue = true;
            const bool expectedBoolFalse = false;

            var actualString = _envSettings.GetString(TestEnvironmentSettings.SETTING_WITH_NO_ATTRIBUTE, expectedString);
            var actualInt = _envSettings.GetInt(TestEnvironmentSettings.SETTING_WITH_NO_ATTRIBUTE, expectedInt);
            var actualBoolTrue = _envSettings.GetBool(TestEnvironmentSettings.SETTING_WITH_NO_ATTRIBUTE, expectedBoolTrue);
            var actualBoolFalse = _envSettings.GetBool(TestEnvironmentSettings.SETTING_WITH_NO_ATTRIBUTE, expectedBoolFalse);

            actualString.Should().Be(expectedString);
            actualInt.Should().Be(expectedInt);
            actualBoolTrue.Should().BeTrue();
            actualBoolFalse.Should().BeFalse();
        }

        [Fact]
        public void Getters_TypeMismatch_Throws()
        {
            FluentActions.Invoking(() => _envSettings.GetBool(TestEnvironmentSettings.SOME_INT_SETTING))
                .Should().Throw<ApplicationException>()
                .WithMessage($"Could not parse setting to bool: {TestEnvironmentSettings.SOME_INT_SETTING}");

            FluentActions.Invoking(() => _envSettings.GetInt(TestEnvironmentSettings.SOME_BOOL_SETTING_TRUE))
                .Should().Throw<ApplicationException>()
                .WithMessage($"Could not parse setting to int: {TestEnvironmentSettings.SOME_BOOL_SETTING_TRUE}");
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

            var fromFirst = _envSettings.GetString(TestEnvironmentSettings.SOME_SETTING_STRING);
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

            _environmentVariableProvider.Setup(envProvider => envProvider.GetEnvironmentVariable(TestEnvironmentSettings.SOME_SETTING_STRING.ToName()))
                .Returns(expectedString);
            _environmentVariableProvider.Setup(envProvider => envProvider.GetEnvironmentVariable(TestEnvironmentSettings.SOME_INT_SETTING.ToName()))
                .Returns(expectedInt.ToString());
            _environmentVariableProvider.Setup(envProvider => envProvider.GetEnvironmentVariable(TestEnvironmentSettings.SOME_BOOL_SETTING_TRUE.ToName()))
                .Returns(expectedBool.ToString());

            var actualString = _envSettings.GetString(TestEnvironmentSettings.SOME_INT_SETTING);
            var actualInt = _envSettings.GetString(TestEnvironmentSettings.SOME_INT_SETTING);
            var actualBool = _envSettings.GetString(TestEnvironmentSettings.SOME_BOOL_SETTING_TRUE);

            actualString.Should().Be(actualString);
            actualInt.Should().Be(actualInt);
            actualBool.Should().Be(actualBool);
        }

        [Fact]
        public void GetAllAsSafeLogString_PropOptionsRespected()
        {
            var safeLogString = _envSettings.GetAllAsSafeLogString();

            safeLogString.Should().NotBeEmpty();
            safeLogString.Should().Contain($"{TestEnvironmentSettings.SETTING_WITH_NO_ATTRIBUTE}=");
            safeLogString.Should().Contain($"{TestEnvironmentSettings.SOME_SETTING_STRING}=some string");
            safeLogString.Should().Contain($"{TestEnvironmentSettings.SOME_INT_SETTING}=42");
            safeLogString.Should().Contain($"{TestEnvironmentSettings.SOME_BOOL_SETTING_TRUE}=true");
            safeLogString.Should().Contain($"{TestEnvironmentSettings.SOME_BOOL_SETTING_FALSE}=false");
            safeLogString.Should().Contain($"{TestEnvironmentSettings.SETTING_WITH_NO_DEFAULT_VALUE}=");
            safeLogString.Should().Contain($"{TestEnvironmentSettings.SOME_SECRET_SETTING_WITH_LOCAL_DEFAULT_ONLY}=");
            safeLogString.Should().Contain($"{TestEnvironmentSettings.SOME_SECRET_SETTING_WITH_ALL_ENVIRONMENTS_DEFAULT}=");
            safeLogString.Should().NotContainAny("test_secret_local_only_default", "test_secret_all_environment_default");
        }

        [Fact]
        public void GetString_NotLocalWithLocalOnlyDefault_Throws()
        {
            _environmentVariableProvider.Setup(envVarProvider => envVarProvider.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")).Returns("Production");
            _envSettings = new EnvironmentSettings(_environmentVariableProvider.Object, _secretVariableProvider.Object);
            _envSettings.AddSettings<TestEnvironmentSettings>();

            FluentActions.Invoking(() => _envSettings.GetString(TestEnvironmentSettings.SOME_SECRET_SETTING_WITH_LOCAL_DEFAULT_ONLY))
                .Should().Throw<ApplicationException>()
                .WithMessage($"Setting not loaded: {TestEnvironmentSettings.SOME_SECRET_SETTING_WITH_LOCAL_DEFAULT_ONLY}");
        }

        [Fact]
        public void GetString_NotLocalWithAllEnvironmentsDefault_Throws()
        {
            var actual = _envSettings.GetString(TestEnvironmentSettings.SOME_SECRET_SETTING_WITH_ALL_ENVIRONMENTS_DEFAULT);
            actual.Should().Be("test_secret_all_environment_default");
        }

        [Fact]
        public void GetString_IsSecret_SecretReturned()
        {
            const string secretValue = "super_secret";
            _secretVariableProvider.Setup(m => m.GetEnvironmentVariable(TestEnvironmentSettings.SOME_SECRET.ToString())).Returns(secretValue);
            _envSettings = new EnvironmentSettings(_environmentVariableProvider.Object, _secretVariableProvider.Object);
            _envSettings.AddSettings<TestEnvironmentSettings>();

            var actual = _envSettings.GetString(TestEnvironmentSettings.SOME_SECRET);

            actual.Should().Be(secretValue);
        }

        [Fact]
        public void GetString_NotSetAndThrowIfNotSetTrue_Throws()
        {
            _environmentVariableProvider.Reset();
            _envSettings = new EnvironmentSettings(_environmentVariableProvider.Object, _secretVariableProvider.Object);
            FluentActions.Invoking(() => _envSettings.AddSettings<TestEnvironmentSettings>())
                .Should().Throw<ApplicationException>()
                .WithMessage($"Missing required environment setting: {TestEnvironmentSettings.DONT_START_APP_WITHOUT_ME}");
        }
    }
}
