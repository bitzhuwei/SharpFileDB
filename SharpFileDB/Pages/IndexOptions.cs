using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpFileDB.Pages
{
    /// <summary>
    /// A class that represent all index options used on a index creation
    /// </summary>
    public class IndexOptions : IEquatable<IndexOptions>
    {
        public bool Unique { get; set; }

        public bool IgnoreCase { get; set; }

        public bool TrimWhitespace { get; set; }

        public bool EmptyStringToNull { get; set; }

        public bool RemoveAccents { get; set; }

        public IndexOptions()
        {
            this.Unique = false;
            this.IgnoreCase = true;
            this.TrimWhitespace = true;
            this.EmptyStringToNull = true;
            this.RemoveAccents = true;
        }

        public bool Equals(IndexOptions other)
        {
            return this.Unique == other.Unique &&
                this.IgnoreCase == other.IgnoreCase &&
                this.TrimWhitespace == other.TrimWhitespace &&
                this.EmptyStringToNull == other.EmptyStringToNull &&
                this.RemoveAccents == other.RemoveAccents;
        }

        public IndexOptions Clone()
        {
            return new IndexOptions
            {
                Unique = this.Unique,
                IgnoreCase = this.IgnoreCase,
                TrimWhitespace = this.TrimWhitespace,
                EmptyStringToNull = this.EmptyStringToNull,
                RemoveAccents = this.RemoveAccents
            };
        }
    }
}
