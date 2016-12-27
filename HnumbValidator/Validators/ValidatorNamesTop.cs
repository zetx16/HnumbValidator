using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HnumbValidator
{
    class ValidatorNamesTop : Validator
    {
        Dictionary<string, int> names;
        List<string> keys = new List<string>
        {
            "highway",
            "place",
            "route",
            "waterway"
        };

        public ValidatorNamesTop()
        {
            FileEnd = "topnames";
            Title = "Частые названия";
            descriptionForList = descriptionForMap = "";

            errors = new List<Error>();
            names = new Dictionary<string, int>();
        }

        public override void ValidateObject( OsmGeo geo )
        {
            if ( geo.Tags.ContainsOneOfKeys( keys ) )
                return;
            string name;
            if ( !geo.Tags.TryGetValue( "name", out name ) )
                return;
            foreach ( var word in name.ToLower().Split( ' ' ) )
            {
                if ( names.ContainsKey( word ) )
                    names[ word ]++;
                else
                    names.Add( word, 1 );
            }
        }

        public override void ValidateEndReadFile()
        {
            descriptionForList = names.Count.ToString();
            var ordernames = names.Take( 5000 ).OrderByDescending( x => x.Value ).ThenBy( x => x.Key );
            foreach ( var name in ordernames )
            {
                var err = new Error( OsmGeoType.Node, 1, name.Key );
                err.Description = name.Value.ToString();
                errors.Add( err );
            }
        }
    }
}
