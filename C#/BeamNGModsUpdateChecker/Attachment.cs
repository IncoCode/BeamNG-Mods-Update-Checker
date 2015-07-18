#region Using

using System;
using System.Collections.Generic;

#endregion

namespace BeamNGModsUpdateChecker
{
    public class Attachment : IEquatable<Attachment>, IEqualityComparer<Attachment>
    {
        public string Name { get; set; }
        public string Size { get; set; }

        #region Constructors

        public Attachment()
        {
        }

        public Attachment( string name, string size )
        {
            this.Name = name;
            this.Size = size;
        }

        #endregion

        #region Members

        public bool Equals( Attachment obj )
        {
            if ( obj == null )
            {
                return false;
            }
            return this.Name.Equals( obj.Name ) && this.Size.Equals( obj.Size );
        }

        public bool Equals( Attachment x, Attachment y )
        {
            if ( ReferenceEquals( x, y ) )
            {
                return true;
            }

            if ( ReferenceEquals( x, null ) || ReferenceEquals( y, null ) )
            {
                return false;
            }

            return x.Name == y.Name && x.Size == y.Size;
        }

        public override bool Equals( object obj )
        {
            Attachment attachment = obj as Attachment;
            return attachment != null && this.Equals( attachment );
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.Size.GetHashCode();
        }

        public int GetHashCode( Attachment attachment )
        {
            if ( ReferenceEquals( attachment, null ) )
            {
                return 0;
            }

            int hashName = attachment.Name == null ? 0 : attachment.Name.GetHashCode();

            int hashSize = attachment.Size.GetHashCode();

            return hashName ^ hashSize;
        }

        #endregion
    }
}