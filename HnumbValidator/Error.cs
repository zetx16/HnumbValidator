using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HnumbValidator
{
    public enum ErrorLevel
    {
        Level0,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Level6,
        Level7,
        Level8,
        Level9
    }

    public class Error
    {
        public OsmGeoType Type { get; set; }
        public string TypeString
        {
            get { return Type == OsmGeoType.Node ? "node" : Type == OsmGeoType.Way ? "way" : "relation"; }
        }
        public string TypeStringShort
        {
            get { return Type == OsmGeoType.Node ? "n" : Type == OsmGeoType.Way ? "w" : "r"; }
        }
        public long Osmid { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }

        public float lat { get; set; }
        public float lon { get; set; }

        public DateTime TimeStump { get; set; }
        public ErrorLevel Level { get; set; }

        public Error( OsmGeo geo ) 
            : this( geo, String.Empty ) { }

        public Error( OsmGeo geo, string value ) 
            : this( geo, value, ErrorLevel.Level0 ) { }

        public Error( OsmGeo geo, string value, ErrorLevel lvl )
        {
            Osmid = (long)geo.Id;
            Type = geo.Type;
            TimeStump = (DateTime)geo.TimeStamp;
            Value = value;

            GeoCollections.GetCoordinates( geo, this );

            Description = String.Empty;
            Level = lvl;
        }

        public Error( OsmGeoType type, long id )
            : this( type, id, String.Empty ) { }

        public Error( OsmGeoType type, long id, string value )
        {
            Type = type;
            Osmid = id;
            Value = value;
            Description = String.Empty;
            lat = 0;
            lon = 0;
            Level = ErrorLevel.Level0;
        }


        public override bool Equals( object obj )
        {
            var err = obj as Error;
            if ( err == null )
                return false;

            return this.Type == err.Type && this.Osmid == err.Osmid;
        }
        public bool Equals( Error obj )
        {
            if ( obj == null )
                return false;

            return this.Type == obj.Type && this.Osmid == obj.Osmid;
        }
        public override int GetHashCode()
        {
            return this.Type.GetHashCode() ^ this.Osmid.GetHashCode();
        }
    }
}
