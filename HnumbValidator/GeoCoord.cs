using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HnumbValidator
{
    public struct NdCoord : IComparable<NdCoord>
    {
        public long id;
        public float lat;
        public float lon;

        public NdCoord( long i ) :
            this( i, 0.0f, 0.0f )
        { }

        public NdCoord( long i, float lt, float ln )
        {
            id = i;
            lat = lt;
            lon = ln;
        }

        public int CompareTo( NdCoord o )
        {
            return id.CompareTo( o.id );
        }
    }

    public struct WayCoord : IComparable<WayCoord>
    {
        public long id;
        public long ndId;

        public WayCoord( long i, long nd )
        {
            id = i;
            ndId = nd;
        }

        public int CompareTo( WayCoord o )
        {
            return id.CompareTo( o.id );
        }
    }

    public struct RelCoord : IComparable<RelCoord>
    {
        public long id;
        public long mbId;
        public OsmGeoType mbTp;

        public RelCoord( long i, long memberId, OsmGeoType memberType )
        {
            id = i;
            mbId = memberId;
            mbTp = memberType;
        }

        public int CompareTo( RelCoord o )
        {
            return id.CompareTo( o.id );
        }
    }
}

