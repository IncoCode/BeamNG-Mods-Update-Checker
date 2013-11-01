using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeamNGModsUpdateChecker
{
    public class Attachment : IEquatable<Attachment>, IEqualityComparer<Attachment>
    {
        public string Name { get; set; }
        public string Size { get; set; }

        #region Constructors

        public Attachment() { }

        public Attachment( string Name, string Size )
        {
            this.Name = Name;
            this.Size = Size;
        }

        #endregion

        public bool Equals( Attachment obj )
        {
            return this.Name.Equals( obj.Name ) && this.Size.Equals( obj.Size );
        }

        public bool Equals( Attachment x, Attachment y )
        {
            if ( Object.ReferenceEquals( x, y ) ) return true;

            if ( Object.ReferenceEquals( x, null ) || Object.ReferenceEquals( y, null ) )
                return false;

            return x.Name == y.Name && x.Size == y.Size;
        }

        public int GetHashCode( Attachment product )
        {
            if ( Object.ReferenceEquals( product, null ) ) return 0;

            int hashName = product.Name == null ? 0 : product.Name.GetHashCode();

            int hashSize = product.Size.GetHashCode();

            return hashName ^ hashSize;
        }
    }
}
