using System;
using System.Collections.Generic;
using System.IO;
using Appalachia.Utility.Strings;

namespace LiteDB.Engine
{
    internal class SysFile : SystemCollection
    {
        public SysFile() : base("$file")
        {
        }

        #region Fields and Autoproperties

        private Dictionary<string, SystemCollection> _formats =
            new Dictionary<string, SystemCollection>(StringComparer.OrdinalIgnoreCase)
            {
                ["json"] = new SysFileJson(), ["csv"] = new SysFileCsv()
            };

        #endregion

        /// <inheritdoc />
        public override IEnumerable<BsonDocument> Input(BsonValue options)
        {
            var format = GetFormat(options);

            if (_formats.TryGetValue(format, out var factory))
            {
                return factory.Input(options);
            }

            throw new LiteException(0, ZString.Format("Unknow file format in $file: `{0}`", format));
        }

        /// <inheritdoc />
        public override int Output(IEnumerable<BsonDocument> source, BsonValue options)
        {
            var format = GetFormat(options);

            if (_formats.TryGetValue(format, out var factory))
            {
                return factory.Output(source, options);
            }

            throw new LiteException(0, ZString.Format("Unknow file format in $file: `{0}`", format));
        }

        private string GetFormat(BsonValue options)
        {
            var filename = GetOption(options, "filename")?.AsString ??
                           throw new LiteException(
                               0,
                               "Collection $file requires string as 'filename' or a document field 'filename'"
                           );
            var format = GetOption(options, "format", Path.GetExtension(filename)).AsString;

            return format.StartsWith(".") ? format.Substring(1) : format;
        }
    }
}
