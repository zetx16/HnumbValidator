using OsmSharp.Osm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HnumbValidator
{
    class ValidatorMapsMe : Validator
    {
        List<string> regs = new List<string>
        {
            @"^(мой |частный )?дом(ик)?$",
            @"^(моя )?работа$",
            @"^магазин(чик)?$",
            @"^кафе(шка)?$",
            @"^т(е|ё|ьо)щ(а|енька)$",
            @"^мама$",
            @"^(.*)?родители( .*)?$",
            @"^дача$"
        };

        public ValidatorMapsMe()
        {
            FileEnd = "mapsme";
            Title = "Ошибки новичков";

            descriptionForList = "Типичные ошибки новичков.";
            descriptionForMap = descriptionForList;
            errors = new List<Error>();
        }

        public override void ValidateObject( OsmGeo geo )
        {
            if ( geo.Tags.ContainsKeyValue( "public_transport", "platform" ) ||
                geo.Tags.ContainsKeyValue( "highway", "bus_stop" ) ||
                geo.Tags.ContainsKeyValue( "public_transport", "stop_position" ) ||
                geo.Tags.ContainsKeyValue( "type", "route" ) ||
                geo.Tags.ContainsKeyValue( "type", "route_master" ) ||
                geo.Tags.ContainsKeyValue( "type", "public_transport" ) ||
                geo.Tags.ContainsKeyValue( "type", "boundary" ) ||
                ( ( geo.Tags.ContainsKeyValue( "landuse", "construction" ) ||
                geo.Tags.ContainsKeyValue( "landuse", "greenfield" ) ||
                geo.Tags.ContainsKeyValue( "landuse", "brownfield" ) ) &&
                geo.Tags.ContainsKey( "opening_date" ) )
                )
                return;

            string value;
            if ( !geo.Tags.TryGetValue( "name", out value ) )
                return;

            bool match = false;
            foreach ( var regex in regs )
                if ( Regex.IsMatch( value.ToLower(), regex ) )
                    match = true;

            if ( !match )
            {
                if ( !geo.Tags.TryGetValue( "name:ru", out value ) )
                    return;

                foreach ( var regex in regs )
                    if ( Regex.IsMatch( value.ToLower(), regex ) )
                        match = true;
            }

            if ( !match )
                return;

            var error = new Error( geo, value );

            errors.Add( error );
        }
    }
}
