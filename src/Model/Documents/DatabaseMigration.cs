using System;
using Appalachia.Data.Core.Documents;
using Appalachia.Data.Model.Collections;

namespace Appalachia.Data.Model.Documents
{
    [Serializable]
    public class DatabaseMigration : AppaDocument<DatabaseMigration, DatabaseMigrationCollection>
    {
        #region Fields and Autoproperties

        public DateTime DateStarted { get; set; }
        public DateTime DateCompleted { get; set; }
        public bool CompletedSuccessfully { get; set; }

        #endregion

        /// <inheritdoc />
        protected override void SetDefaults()
        {
        }
    }
}
