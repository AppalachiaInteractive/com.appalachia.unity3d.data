using System;
using System.Collections.Generic;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Data.Core.Collections;
using Appalachia.Utility.Async;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Profiling;

namespace Appalachia.Data.Core.Databases
{
    [Serializable]
    public abstract class AppaDatabaseBase : DataObject, IDisposable
    {
        #region Fields and Autoproperties

        [NonSerialized, JsonProperty]
        protected List<AppaCollectionBase> _collections;

        #endregion

        [ShowInInspector, JsonIgnore]
        public abstract DatabaseType Type { get; }

        [ShowInInspector, BoxGroup(nameof(Collections))]
        public IReadOnlyList<AppaCollectionBase> Collections => _collections;

        protected virtual void Dispose(bool disposing)
        {
            using (_PRF_Dispose.Auto())
            {
                if (disposing)
                {
                }
            }
        }

        protected override async AppaTask Initialize(Initializer initializer)
        {
            using (_PRF_Initialize.Auto())
            {
                await base.Initialize(initializer);

                await initializer.Do(
                    this,
                    nameof(AppaCollectionBase),
                    Initializer.TagState.NonSerialized,
                    () => { _collections = new List<AppaCollectionBase>(); }
                );
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            using (_PRF_Dispose.Auto())
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region Profiling

        private const string _PRF_PFX = nameof(AppaDatabaseBase) + ".";

        private static readonly ProfilerMarker _PRF_Initialize =
            new ProfilerMarker(_PRF_PFX + nameof(Initialize));

        private static readonly ProfilerMarker
            _PRF_OnEnable = new ProfilerMarker(_PRF_PFX + nameof(OnEnable));

        private static readonly ProfilerMarker _PRF_Dispose = new ProfilerMarker(_PRF_PFX + nameof(Dispose));

        #endregion
    }
}
