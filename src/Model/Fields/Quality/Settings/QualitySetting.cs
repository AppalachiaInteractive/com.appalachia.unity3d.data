using System;
using Appalachia.Data.Core.Fields;
using Unity.Profiling;

namespace Appalachia.Data.Model.Fields.Quality.Settings
{
    public abstract class QualitySetting<T> : AppaField
    {
        #region Fields and Autoproperties

        public T VeryLow { get; set; }
        public T Low { get; set; }
        public T Medium { get; set; }
        public T High { get; set; }
        public T VeryHigh { get; set; }
        public T Ultra { get; set; }
        public T Custom { get; set; }

        #endregion

        public T Get(QualitySettingsPresetType preset)
        {
            using (_PRF_Get.Auto())
            {
                switch (preset)
                {
                    case QualitySettingsPresetType.VeryLow:
                        return VeryLow;
                    case QualitySettingsPresetType.Low:
                        return Low;
                    case QualitySettingsPresetType.Medium:
                        return Medium;
                    case QualitySettingsPresetType.High:
                        return High;
                    case QualitySettingsPresetType.VeryHigh:
                        return VeryHigh;
                    case QualitySettingsPresetType.Ultra:
                        return Ultra;
                    case QualitySettingsPresetType.Custom:
                        return Custom;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(preset), preset, null);
                }
            }
        }

        public void Set(QualitySettingsPresetType preset, T value)
        {
            using (_PRF_Set.Auto())
            {
               MarkFieldModified();

                switch (preset)
                {
                    case QualitySettingsPresetType.VeryLow:
                        VeryLow = value;
                        break;
                    case QualitySettingsPresetType.Low:
                        Low = value;
                        break;
                    case QualitySettingsPresetType.Medium:
                        Medium = value;
                        break;
                    case QualitySettingsPresetType.High:
                        High = value;
                        break;
                    case QualitySettingsPresetType.VeryHigh:
                        VeryHigh = value;
                        break;
                    case QualitySettingsPresetType.Ultra:
                        Ultra = value;
                        break;
                    case QualitySettingsPresetType.Custom:
                        Custom = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(preset), preset, null);
                }
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(QualitySetting<T>) + ".";

        private static readonly ProfilerMarker _PRF_Get = new ProfilerMarker(_PRF_PFX + nameof(Get));

        private static readonly ProfilerMarker _PRF_Set = new ProfilerMarker(_PRF_PFX + nameof(Set));

        #endregion
    }
}
