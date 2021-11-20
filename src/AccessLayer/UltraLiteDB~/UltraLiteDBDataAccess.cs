using System.Collections.Generic;
using Appalachia.Data.Core;
using Appalachia.Data.Core.AccessLayer;
using Appalachia.Data.Documents;

namespace Appalachia.Data.AccessLayer
{
    public class UltraLiteDBConnectionWrapper : DbConnectionWrapper<UltraLiteDatabase>
    {
        internal UltraLiteDBConnectionWrapper(string filePath, bool encrypted, int initialSizeInKB = 512) : base(
            new UltraLiteDatabase(
                GetConnectionString(
                    filePath,
                    "direct",
                    encrypted ? APPASTR.LDB : null,
                    initialSizeInKB,
                    false,
                    false
                )
            )
        )
        {
            _filePath = filePath;
            _encrypted = encrypted;
            _initialSizeInKb = initialSizeInKB;
        }

        #region Fields

        private readonly bool _encrypted;

        private readonly int _initialSizeInKb;

        private readonly string _filePath;

        #endregion

        public override DatabaseStyle DatabaseStyle => DatabaseStyle.Document;

        private static string GetConnectionString(
            string filename,
            string connection,
            string password,
            int initialSizeInKB,
            bool readOnly,
            bool upgrade)
        {
            return $"Filename:{filename}; " +
                   $"Connection:{connection}; " +
                   (password == null ? "" : $"Password:{password}; ") +
                   $"InitialSize:{initialSizeInKB}KB; " +
                   $"ReadOnly:{readOnly}; " +
                   $"Upgrade:{upgrade};";
        }

        public override void AddOrUpdate(IEnumerable<AppaDocument> values)
        {
            throw new System.NotImplementedException();
        }

        public override void AddOrUpdateSingleton(AppaDocument value)
        {
            throw new System.NotImplementedException();
        }

        public override void Load<T>(ref T database)
        {
            throw new System.NotImplementedException();
        }

        public override void MigrateSchema()
        {
        }

        public override void Save<T>(T database)
        {
            throw new System.NotImplementedException();
        }

        protected override string GetFilePath()
        {
            return _filePath;
        }

        protected override void OnCreate()
        {
        }

        /*
         * // Open database (or create if doesn't exist)
using(var db = new UltraLiteDatabase(@"C:\Temp\MyData.db"))
{
    // Get a collection (or create, if doesn't exist)
    var col = db.GetCollection<Customer>("customers");

    // Create your new customer instance
    var customer = new Customer
    { 
        Name = "John Doe", 
        Phones = new string[] { "8000-0000", "9000-0000" }, 
        IsActive = true
    };
	
    // Insert new customer document (Id will be auto-incremented)
    col.Insert(customer);
	
    // Update a document inside a collection
    customer.Name = "Jane Doe";
	
    col.Update(customer);
	
    // Index document using document Name property
    col.EnsureIndex(x => x.Name);
	
    // Use LINQ to query documents (filter, sort, transform)
    var results = col.Query()
        .Where(x => x.Name.StartsWith("J"))
        .OrderBy(x => x.Name)
        .Select(x => new { x.Name, NameUpper = x.Name.ToUpper() })
        .Limit(10)
        .ToList();

    // Let's create an index in phone numbers (using expression). It's a multikey index
    col.EnsureIndex(x => x.Phones); 

    // and now we can query phones
    var r = col.FindOne(x => x.Phones.Contains("8888-5555"));
}
         */
    }
}
