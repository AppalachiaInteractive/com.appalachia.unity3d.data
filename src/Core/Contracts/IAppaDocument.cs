using System;

namespace Appalachia.Data.Core.Contracts
{
    public interface IAppaDocument
    {
        bool RequiresMigration { get; }

        bool AutoPopulated { get; set; }
        bool IsDirty { get; set; }

        DateTime DateCreated { get; set; }

        DateTime DateUpdated { get; set; }

        int CurrentVersion { get; set; }

        int InitialVersion { get; set; }

        public void MarkAsModified();
    }
}
