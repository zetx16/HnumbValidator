using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;
using System.Collections;

namespace HnumbValidator
{
    class ValidatorOpeningDate: Validator
    {
        public ValidatorOpeningDate()
        {
            FileEnd = "opendate";
            Title = "Дата открытия";

            descriptionForList = "Строящиеся объекты у которых дата открытия прошла.";
            descriptionForMap = descriptionForList;
            errors = new List<Error>();
        }

        public override IEnumerable GetTableHead()
        {
            yield return "Дата открытия";
            yield return "Доп. информация";
        }

        public override void ValidateObject( OsmGeo geo )
        {
            string value;
            if ( !geo.Tags.TryGetValue( "opening_date", out value ) )
                return;

            DateTime opendate;
            int openyear;

            if ( DateTime.TryParse( value, out opendate ) )
            {
                if ( opendate <= DateTime.Now )
                    errors.Add( new Error( geo, value, "Дата открытия прошла." ));
                else if ( ( opendate - DateTime.Now ).Days < 7 )
                    errors.Add( new Error( geo, value, "До открытия меньше недели." ));
            }
            else if ( int.TryParse( value, out openyear ) )
            {
                if ( openyear <= DateTime.Now.Year )
                    errors.Add( new Error( geo, value, "Дата открытия прошла." ));
            }
            else
                errors.Add( new Error( geo, value, "Дата не распознана." ));
        }
        
        public override void ValidateEndReadFile()
        {
            errors = errors.OrderByDescending( x => x.Value ).ToList();
        }
    }
}
