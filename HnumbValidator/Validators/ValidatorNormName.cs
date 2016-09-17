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
    class ValidatorNormName : Validator
    {
        List<string> regs = new List<string>
        {
            @"^Школа №\d+$",
            @"^Гимназия №\d+$",
            @"^Лицей №\d+$",
            @"^Школа-интернат №\d+$",
            @"^Лицей-интернат №\d+$",
            @"^Гимназия-интернат №\d+$",
            @"^Татарская гимназия №\d+$",
            @"^Начальная школа №\d+$"
        };

        public ValidatorNormName()
        {
            FileEnd = "normname";
            Title = "Нормализация названий";

            descriptionForList = "Приведение названий школ к единому виду.";
            descriptionForMap = descriptionForList;
            errors = new List<Error>();
        }

        public override IEnumerable GetTableHead()
        {
            yield return "Название";
            yield return "Нормализованное название";
        }

        public override void ValidateObject( OsmGeo geo )
        {
            if ( !geo.Tags.ContainsKeyValue( "amenity", "school" ) )
                return;

            string value;
            if ( !geo.Tags.TryGetValue( "name", out value ) )
                return;

            foreach ( var regex in regs )
                if ( Regex.IsMatch( value, regex ) )
                    return;

            errors.Add( new Error( geo, value ) );
        }
    }
}
