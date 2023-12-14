using UserAccess;
namespace PaeoniaTechSpectroMeter.Model
{
    public class UserAccessControl : AccessControl
    {
        public static string PositionTeachingFeatureName = "Position Teaching";
        public static string CommonValueEntryFeatureName = "Commom Value Entry";
        public static string DelaySettingFeatureName = "Delay Setting";
        public static string DeviceConfigFeatureName = "Device Config";
        public static string ProductionOptionsFeatureName = "Production Options";
        public static string SuperUserAccessFeatureName = "Super User Access";
        public static string MotionProfileFeatureName = "Motion Profile";
        public static string CalibrationSetupFeatureName = "Calibration Setup";
        public static string ZSlowdownFeatureName = "Z Slow Down Options";
        public static string DisableInterLockFeatureName = "Disable Interlock";
        public static string SystemConfigurationFeatureName = "System Configuration";
        AccessConfig systemConfig = new AccessConfig(SystemConfigurationFeatureName, AccessLevel.ENGINEER);
        public AccessConfig SystemConfig { get { return systemConfig; } }

        AccessConfig posTeaching = new AccessConfig(PositionTeachingFeatureName, AccessLevel.ENGINEER);
        public AccessConfig PosTeaching { get { return posTeaching; } }

        AccessConfig disableMotionInterlock = new AccessConfig(DisableInterLockFeatureName, AccessLevel.ENGINEER);
        public AccessConfig DisableMotionInterlock { get { return disableMotionInterlock; } }


        AccessConfig deviceConfig = new AccessConfig(DeviceConfigFeatureName, AccessLevel.ENGINEER);
        public AccessConfig DeviceConfig { get { return deviceConfig; } }


        AccessConfig zSlowDownOptions = new AccessConfig(ZSlowdownFeatureName, AccessLevel.ENGINEER);
        public AccessConfig ZlowDownOptions { get { return zSlowDownOptions; } }

        AccessConfig delaySetting = new AccessConfig(DelaySettingFeatureName, AccessLevel.ENGINEER);
        public AccessConfig DelaySetting { get { return delaySetting; } }

        //   AccessConfig bondSetting = new AccessConfig(BondSettingFeatureName, AccessLevel.ENGINEER);
        // public AccessConfig BondSetting { get { return bondSetting; } }

        AccessConfig motionProfileConfig = new AccessConfig(MotionProfileFeatureName, AccessLevel.ENGINEER);
        public AccessConfig MotionProfileConfig { get { return motionProfileConfig; } }

        AccessConfig calibrationConfig = new AccessConfig(CalibrationSetupFeatureName, AccessLevel.ENGINEER);
        public AccessConfig CalibrationConfig { get { return calibrationConfig; } }

        AccessConfig productionConfig = new AccessConfig(ProductionOptionsFeatureName, AccessLevel.ENGINEER);
        public AccessConfig ProductionConfig { get { return productionConfig; } }

        AccessConfig commonValueEntryConfig = new AccessConfig(CommonValueEntryFeatureName, AccessLevel.ENGINEER, true);
        public AccessConfig CommonValueEntryConfig { get { return commonValueEntryConfig; } }


        AccessConfig superUserAccessFeatures = new AccessConfig(SuperUserAccessFeatureName, AccessLevel.SUPERUSER, true);
        public AccessConfig SuperUserAccessFeatures { get { return superUserAccessFeatures; } }

        //Delay Setting

        public UserAccessControl() { }
        public new void Init(string exeName)
        {
            base.Init(exeName);
            //
            RegisterNewAccessConfig(systemConfig);
            RegisterNewAccessConfig(posTeaching);
            RegisterNewAccessConfig(productionConfig);
            RegisterNewAccessConfig(motionProfileConfig);
            RegisterNewAccessConfig(delaySetting);
            RegisterNewAccessConfig(disableMotionInterlock);
            RegisterNewAccessConfig(calibrationConfig);
            RegisterNewAccessConfig(zSlowDownOptions);
            RegisterNewAccessConfig(commonValueEntryConfig);
            RegisterNewAccessConfig(deviceConfig);
            RegisterNewAccessConfig(superUserAccessFeatures);
            //
            LoadConfigsValues();
            superUserAccessFeatures.ConfiguredValue = (byte)AccessLevel.SUPERUSER;
            commonValueEntryConfig.ConfiguredValue = (byte)AccessLevel.ENGINEER;
        }

    }
}
