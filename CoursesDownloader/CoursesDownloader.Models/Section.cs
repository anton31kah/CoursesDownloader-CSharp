using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoursesDownloader.IModels;
using CoursesDownloader.IModels.ILinks;

namespace CoursesDownloader.Models
{
    [DebuggerDisplay("Section(header={Header}, urls={Links})")]
    public class Section : ISection, IEquatable<Section>
    {
        public IHeader Header { get; }
        public IList<IDownloadableLink> Links { get; }

        public ILink this[int key] => Links[key];
        public ILink this[string key] => Links.First(l => l.Name == key || l.Url == key);
        public int Count => Links.Count;

        public Section(Header header = null, List<IDownloadableLink> links = null)
        {
            Header = header ?? new Header();
            Links = links ?? new List<IDownloadableLink>();
        }

        public Section(string header, List<IDownloadableLink> links = null) 
            : this(new Header(header), links) {}

        public bool Contains(ILink link)
        {
            return Links.Contains(link);
        }

        #region Equality

        public bool Equals(Section other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Header, other.Header) && Equals(Links, other.Links);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Section) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Header != null ? Header.GetHashCode() : 0) * 397) ^ (Links != null ? Links.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Section left, Section right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Section left, Section right)
        {
            return !Equals(left, right);
        }

        #endregion

        public override string ToString()
        {
            return Header.Name;
        }
    }
}