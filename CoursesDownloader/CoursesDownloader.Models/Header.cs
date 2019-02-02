using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using CoursesDownloader.IModels;

namespace CoursesDownloader.Models
{
    [DebuggerDisplay("Header(name={Name}, order={Order}, anchorId={AnchorId})")]
    public class Header : IHeader, IEquatable<Header>
    {
        public string Name { get; }
        public string Order { get; }
        public string AnchorId { get; }

        public Header(string name = "", string order = "", string anchorId = "")
        {
            Name = name;
            var tagOrder = Regex.Match(order, @"\d+").Value;
            Order = tagOrder;
            AnchorId = anchorId;
        }

        #region Equality

        public bool Equals(Header other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Header)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(Header left, Header right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Header left, Header right)
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
