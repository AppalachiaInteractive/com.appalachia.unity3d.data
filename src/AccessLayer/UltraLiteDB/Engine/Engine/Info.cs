using System.Linq;

namespace UltraLiteDB
{
    public partial class UltraLiteEngine
    {
        /// <summary>
        /// Get internal information about database. Can filter collections
        /// </summary>
        public BsonDocument Info()
        {
            var header = _pager.GetPage<HeaderPage>(0);
            var collections = new BsonArray();

            foreach(var colName in header.CollectionPages.Keys)
            {
                var col = this.GetCollectionPage(colName, false);

                var colDoc = new BsonDocument
                {
                    { "name", col.CollectionName },
                    { "pageID", (double)col.PageID },
                    { "count", col.DocumentCount },
                    { "sequence", col.Sequence },
                    { "indexes", new BsonArray(
                        col.Indexes.Where(x => !x.IsEmpty).Select(i => new BsonDocument
                        {
                            {  "slot", i.Slot },
                            {  "field", i.Field },
                            {  "unique", i.Unique }
                        }))
                    }
                };

                collections.Add(colDoc);
            }

            return new BsonDocument
            {
                { "userVersion", (int)header.UserVersion },
                { "encrypted", header.Password.Any(x => x > 0) },
                { "changeID", (int)header.ChangeID },
                { "lastPageID", (int)header.LastPageID },
                { "fileSize", BasePage.GetSizeOfPages(header.LastPageID + 1) },
                { "collections", collections }
            };
        }
        
    }
}