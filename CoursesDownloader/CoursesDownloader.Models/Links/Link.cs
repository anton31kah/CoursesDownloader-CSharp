using System;
using System.Diagnostics;
using CoursesDownloader.IModels.ILinks;

namespace CoursesDownloader.Models.Links
{
    [DebuggerDisplay("Link(name={Name}, url={Url})")]
    public abstract class Link : ILink, IEquatable<Link>
    {
        public string Name { get; }
        public string Url { get; }

        protected Link(string name = "", string url = "")
        {
            Name = name;
            Url = url;
        }

        #region Equality

        public bool Equals(Link other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && string.Equals(Url, other.Url);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((Link) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Url != null ? Url.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Link left, Link right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Link left, Link right)
        {
            return !Equals(left, right);
        }

        #endregion

        public override string ToString()
        {
            return Name;
        }
    }
}
