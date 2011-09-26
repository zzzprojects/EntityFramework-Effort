using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.EntityFrameworkProvider.Caching
{
    public class DbSchemaKey : IEquatable<DbSchemaKey>
    {
        private string[] metadataFiles;

        public DbSchemaKey(string[] metadataFiles)
        {
            this.metadataFiles = metadataFiles
                .Select(f => f ?? string.Empty)
                .Select(f => f.Trim().ToLowerInvariant())
                .OrderBy(f => f)
                .ToArray();
        }

        public bool Equals(DbSchemaKey other)
        {
            if (this.metadataFiles.Length != other.metadataFiles.Length)
            {
                return false;
            }

            for (int i = 0; i < this.metadataFiles.Length; i++)
            {
                if (this.metadataFiles[i] != other.metadataFiles[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            DbSchemaKey other = obj as DbSchemaKey;

            if (obj == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int result = 0;

            for (int i = 0; i < this.metadataFiles.Length; i++)
            {
                result ^= (i + 1) * this.metadataFiles[i].GetHashCode();
            }

            return result;
        }
    }
}
