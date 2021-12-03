using System;
using Appalachia.CI.Integration.Attributes;
using Appalachia.Data.Core.Attributes;
using Unity.Profiling;

namespace Appalachia.Data.Core.Fields
{
    [DoNotReorderFields]
    public abstract class AppaField
    {
        protected internal AppaField()
        {
            using (_PRF_AppaField.Auto())
            {
                var now = DateTime.UtcNow;

                DateCreated = now;
                DateUpdated = now;

                SetDefaults();
            }
        }

        #region Fields and Autoproperties

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        [DataIgnore] public bool HasBeenModified { get; set; }

        #endregion

        public void MarkFieldModified()
        {
            using (_PRF_MarkModified.Auto())
            {
                HasBeenModified = true;
                DateUpdated = DateTime.UtcNow;
            }
        }

        protected virtual void SetDefaults()
        {
        }

        #region Profiling

        private const string _PRF_PFX = nameof(AppaField) + ".";

        private static readonly ProfilerMarker _PRF_AppaField =
            new ProfilerMarker(_PRF_PFX + nameof(AppaField));

        private static readonly ProfilerMarker
            _PRF_MarkModified = new ProfilerMarker(_PRF_PFX + nameof(MarkFieldModified));

        #endregion
    }
}
