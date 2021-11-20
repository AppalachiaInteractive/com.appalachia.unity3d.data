using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Data.Core.Bson.Document.DataReader;
using Appalachia.Data.Core.Bson.Document.Json;
using LiteDB.Engine;
using static LiteDB.Constants;

namespace LiteDB
{
    internal partial class SqlParser
    {
        /// <summary>
        /// SHRINK
        /// </summary>
        private BsonDataReader ParseRebuild()
        {
            _tokenizer.ReadToken().Expect("REBUILD");

            var options = new RebuildOptions();

            // read <eol> or ;
            var next = _tokenizer.LookAhead();

            if (next.Type == TokenType.EOF || next.Type == TokenType.SemiColon)
            {
                options = null;

                _tokenizer.ReadToken();
            }
            else
            {
                var reader = new JsonReader(_tokenizer);
                var json = reader.Deserialize();

                if (json.IsDocument == false) throw DataException.UnexpectedToken(next);

                if (json["password"].IsString)
                {
                    options.Password = json["password"];
                }

                if (json["collation"].IsString)
                {
                    options.Collation = new Collation(json["collation"].AsString);
                }
            }

            var diff = _engine.Rebuild(options);

            return new BsonDataReader((int)diff);
        }
    }
}