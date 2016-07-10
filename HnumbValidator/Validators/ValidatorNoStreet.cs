using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HnumbValidator
{
    class ValidatorNoStreet: Validator
    {
        List<string> addrkeys;

        public ValidatorNoStreet()
        {
            FileEnd = "nostreet";
            Title = "Нет улицы";

            descriptionForList = @"Список номеров домов без ""addr:street"" и ""addr:place""<br>";
            descriptionForMap = @"Адреса без ""addr:street"" и ""addr:place""<br><br>"
                        + @"<div class=""info-colour"" style=""background-color:orange;""></div> - есть ""addr:city"" или ""addr:suburb""<br>";

            errors = new List<Error>();
            addrkeys = new List<string>{
				"addr:street",
				"addr:place",
                //"addr:quarter",
                //"addr:neighbourhood",
                "is_in:neighbourhood"
			};
        }

        public override void ValidateObject( OsmGeo geo )
        {
            string value;
            if ( geo.Tags.TryGetValue( "addr:housenumber", out value ) && !geo.Tags.ContainsOneOfKeys( addrkeys ) )
            {
                if ( !GeoCollections.CountryRu( geo ) )
                    return;

                Error nostreet = new Error( geo, value );

                string city;

                if ( geo.Tags.TryGetValue( "addr:city", out city ) )
                    nostreet.Description += "<font color=\"gray\">city:</font> " + city;
                if ( geo.Tags.TryGetValue( "addr:suburb", out city ) )
                    if ( nostreet.Description != String.Empty )
                        nostreet.Description += "  |  " + "<font color=\"gray\">suburb: </font>" + city;
                    else
                        nostreet.Description += "<font color=\"gray\">suburb: </font>" + city;
                errors.Add( nostreet );
            }
        }

        public override void ValidateEndReadFile()
        {
            errors = errors.Except( GeoCollections.members ).ToList();
            errors = errors.OrderByDescending( x => x.TimeStump ).OrderBy( x => x.Description ).ToList();
        }
    }
}
