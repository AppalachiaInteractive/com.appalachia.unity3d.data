using System.Text.RegularExpressions;

namespace UltraLiteDB
{
    /// <summary>
    ///     A StringScanner is state machine used in text parsers based on regular expressions
    /// </summary>
    public class StringScanner
    {
        /// <summary>
        ///     Initialize scanner with a string to be parsed
        /// </summary>
        public StringScanner(string source)
        {
            Source = source;
            Index = 0;
        }

        #region Fields and Autoproperties

        public int Index { get; set; }
        public string Source { get; private set; }

        #endregion

        /// <summary>
        ///     Indicate that cursor is EOF
        /// </summary>
        public bool HasTerminated => Index >= Source.Length;

        /// <inheritdoc />
        public override string ToString()
        {
            return HasTerminated ? "<EOF>" : Source.Substring(Index);
        }

        /// <summary>
        ///     Match if pattern is true in current cursor position. Do not change cursor position
        /// </summary>
        public bool Match(string pattern)
        {
            return Match(
                new Regex(
                    (pattern.StartsWith("^") ? "" : "^") + pattern,
                    RegexOptions.IgnorePatternWhitespace
                )
            );
        }

        /// <summary>
        ///     Match if pattern is true in current cursor position. Do not change cursor position
        /// </summary>
        public bool Match(Regex regex)
        {
            var match = regex.Match(Source, Index, Source.Length - Index);
            return match.Success;
        }

        /// <summary>
        ///     Reset cursor position
        /// </summary>
        public void Reset()
        {
            Index = 0;
        }

        /// <summary>
        ///     Scan in current cursor position for this patterns. If found, returns string and run with cursor
        /// </summary>
        public string Scan(string pattern)
        {
            return Scan(
                new Regex(
                    (pattern.StartsWith("^") ? "" : "^") + pattern,
                    RegexOptions.IgnorePatternWhitespace
                )
            );
        }

        /// <summary>
        ///     Scan in current cursor position for this patterns. If found, returns string and run with cursor
        /// </summary>
        public string Scan(Regex regex)
        {
            var match = regex.Match(Source, Index, Source.Length - Index);

            if (match.Success)
            {
                Index += match.Length;
                return match.Value;
            }

            return string.Empty;
        }

        /// <summary>
        ///     Scan pattern and returns group string index 1 based
        /// </summary>
        public string Scan(string pattern, int group)
        {
            return Scan(
                new Regex(
                    (pattern.StartsWith("^") ? "" : "^") + pattern,
                    RegexOptions.IgnorePatternWhitespace
                ),
                group
            );
        }

        public string Scan(Regex regex, int group)
        {
            var match = regex.Match(Source, Index, Source.Length - Index);

            if (match.Success)
            {
                Index += match.Length;
                return group >= match.Groups.Count ? "" : match.Groups[group].Value;
            }

            return string.Empty;
        }

        /// <summary>
        ///     Skip cursor position in string source
        /// </summary>
        public void Seek(int length)
        {
            Index += length;
        }

        /// <summary>
        ///     Throw syntax exception if not terminate string
        /// </summary>
        public void ThrowIfNotFinish()
        {
            Scan(@"\s*");

            if (!HasTerminated)
            {
                throw UltraLiteException.SyntaxError(this);
            }
        }
    }
}
