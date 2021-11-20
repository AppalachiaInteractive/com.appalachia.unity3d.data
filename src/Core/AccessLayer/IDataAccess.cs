using Appalachia.Data.Core.Databases;

namespace Appalachia.Data.Core.AccessLayer
{
    public interface IDataAccess : ICollectionAccess, IDocumentAccess
    {
        TDB CreateDatabase<TDB>()
            where TDB : AppaDatabase<TDB>, new();

        TDB LoadDatabase<TDB>()
            where TDB : AppaDatabase<TDB>, new();

        void SaveDatabase<TDB>(TDB database)
            where TDB : AppaDatabase<TDB>, new();
    }

    public interface IDataAccess<TDB> : IDocumentAccess, ICollectionAccess
        where TDB : AppaDatabase<TDB>, new()
    {
        TDB CreateDatabase();

        TDB LoadDatabase();

        void SaveDatabase(TDB database);
    }
}
