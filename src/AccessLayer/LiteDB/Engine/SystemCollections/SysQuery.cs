using System.Collections.Generic;

namespace LiteDB.Engine
{
    /// <summary>
    ///     This class implement $query experimental system function to run sub-queries. It's experimental only - possible not be present in final release
    /// </summary>
    internal class SysQuery : SystemCollection
    {
        public SysQuery(ILiteEngine engine) : base("$query")
        {
            _engine = engine;
        }

        #region Fields and Autoproperties

        private readonly ILiteEngine _engine;

        #endregion

        /// <inheritdoc />
        public override IEnumerable<BsonDocument> Input(BsonValue options)
        {
            var query = options?.AsString ??
                        throw new LiteException(0, "Collection $query(sql) requires `sql` string parameter");

            var sql = new SqlParser(_engine, new Tokenizer(query), null);

            using (var reader = sql.Execute())
            {
                while (reader.Read())
                {
                    var value = reader.Current;

                    yield return value.IsDocument ? value.AsDocument : new BsonDocument { ["expr"] = value };
                }
            }
        }
    }
}
