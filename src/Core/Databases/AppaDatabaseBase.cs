using System;
using System.Collections.Generic;
using Appalachia.Data.Core.Collections;
using Appalachia.Utility.Execution;
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

        protected abstract void OnInitialize();

        protected virtual void Dispose(bool disposing)
        {
            using (_PRF_Dispose.Auto())
            {
                if (disposing)
                {
                }
            }
        }

        private static readonly ProfilerMarker _PRF_OnEnable = new ProfilerMarker(_PRF_PFX + nameof(OnEnable));
        
        protected override void OnEnable()
        {
            using (_PRF_OnEnable.Auto())
            {
                base.OnEnable();

                InitializeInternal();
            }
        }

        protected void InitializeInternal()
        {
            using (_PRF_Initialize.Auto())
            {
                initializationData.Initialize(
                    this,
                    nameof(AppaCollectionBase),
                    Initializer.TagState.NonSerialized,
                    () =>
                    {
                        _collections = new List<AppaCollectionBase>();

                        OnInitialize();
                    }
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
            new ProfilerMarker(_PRF_PFX + nameof(InitializeInternal));

        private static readonly ProfilerMarker _PRF_Dispose = new ProfilerMarker(_PRF_PFX + nameof(Dispose));

        #endregion
    }
}
