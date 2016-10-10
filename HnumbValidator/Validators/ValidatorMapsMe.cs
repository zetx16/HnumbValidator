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
            @"^(.*)?родители( .*)?$"
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
            string value;
            if ( !geo.Tags.TryGetValue( "name", out value ) )
                return;

            foreach ( var regex in regs )
                if ( !Regex.IsMatch( value, regex ) )
                    return;

            var error = new Error( geo, value );

            errors.Add( error );
        }
    }
}
