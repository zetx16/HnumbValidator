using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HnumbValidator
{
    class ValidatorDoubleTag : Validator
    {
        List<Error> tempSchool;
        List<Error> tempKindergarten;
        List<string> ignoreWords;

        public ValidatorDoubleTag()
        {
            ListableReport = false;
            FileEnd = "double";
            Title = "Дублирование";
            descriptionForList = "Дублирования тега amenity=school и amenity=kindergarten, т.е. когда тег висит и на территории и на здании.";
            descriptionForMap = descriptionForList + "<br><br>"
                            + @"<div class=""info-colour"" style=""background-color:#f60;""></div> - школы<br>"
                            + @"<div class=""info-colour"" style=""background-color:orange;""></div> - детские сады<br>";

            tempSchool = new List<Error>();
            tempKindergarten = new List<Error>();
            errors = new List<Error>();
            ignoreWords = new List<string>{
                "художественная",
				"музыкальная",
                "спортивная",
                "искусств"
			};
        }

        public override void ValidateObject( OsmGeo geo )
        {
            string value;
            if ( geo.Tags.ContainsKeyValue( "amenity", "school" ) )
            {
                geo.Tags.TryGetValue( "name", out value );
                if ( ignoreWords.Any( x => value.ToLower().Contains( x ) ) )
                    return;
                var school = new Error( geo, value );
                tempSchool.Add( school );
            }
            if ( geo.Tags.ContainsKeyValue( "amenity", "kindergarten" ) )
            {
                geo.Tags.TryGetValue( "name", out value );
                var kindergarten = new Error( geo, value, ErrorLevel.Level5 );
                tempKindergarten.Add( kindergarten );
            }
        }

        public override void ValidateEndReadFile()
        {
            ValidateSchool();
            ValidateKindergarten();
            base.ValidateEndReadFile();
        }

        private void ValidateSchool()
        {
            float square = 0.001f;
            StringComparison nocs = StringComparison.OrdinalIgnoreCase;

            tempSchool = tempSchool.OrderBy( x => x.lat ).ToList();
            for ( int i = 0; i < tempSchool.Count - 1; i++ )
            {
                var refi = new string( tempSchool[ i ].Value.Where( x => Char.IsDigit( x ) ).ToArray() );
                var containi = tempSchool[ i ].Value.ToLower().Contains( "школа" ) || tempSchool[ i ].Value.ToLower().Contains( "сош" );

                for ( int j = i + 1; j < tempSchool.Count; j++ )
                {
                    if ( Math.Abs( tempSchool[ i ].lat - tempSchool[ j ].lat ) > square )
                        break;
                    if ( Math.Abs( tempSchool[ i ].lon - tempSchool[ j ].lon ) > square )
                        continue;
                    var dist = GeoCollections.Distance( tempSchool[ i ], tempSchool[ j ] );
                    if ( dist > 200 )
                        continue;

                    var refj = new string( tempSchool[ j ].Value.Where( x => Char.IsDigit( x ) ).ToArray() );
                    var containj = tempSchool[ j ].Value.ToLower().Contains( "школа" ) || tempSchool[ j ].Value.ToLower().Contains( "сош" );

                    if ( refi == refj ||
                        ( ( ( containi && refi == string.Empty ) || tempSchool[ i ].Value == string.Empty ) && refj != string.Empty ) ||
                        ( refi != string.Empty && ( ( refj == string.Empty && containj ) || tempSchool[ j ].Value == string.Empty ) )
                        )
                    {
                        errors.Add( tempSchool[ i ] );
                        errors.Add( tempSchool[ j ] );
                    }
                }
            }
            errors = errors.Distinct().ToList();
        }

        private void ValidateKindergarten()
        {
            float square = 0.001f;
            StringComparison nocs = StringComparison.OrdinalIgnoreCase;
            var regex = new Regex( @"[""«](.*?)[""»]", RegexOptions.Compiled );

            tempKindergarten = tempKindergarten.OrderBy( x => x.lat ).ToList();
            for ( int i = 0; i < tempKindergarten.Count - 1; i++ )
            {
                string refi = new string( tempKindergarten[ i ].Value.Where( x => Char.IsDigit( x ) ).ToArray() );
                bool containi = tempKindergarten[ i ].Value.ToLower().Contains( "детский сад" ) || tempKindergarten[ i ].Value.ToLower().Contains( "доу" );
                var nameMatch = regex.Match( tempKindergarten[ i ].Value );
                string namei = nameMatch.Success ? nameMatch.Groups[ 1 ].Value.ToLower() : "";

                for ( int j = i + 1; j < tempKindergarten.Count; j++ )
                {
                    if ( Math.Abs( tempKindergarten[ i ].lat - tempKindergarten[ j ].lat ) > square )
                        break;
                    if ( Math.Abs( tempKindergarten[ i ].lon - tempKindergarten[ j ].lon ) > square )
                        continue;
                    var dist = GeoCollections.Distance( tempKindergarten[ i ], tempKindergarten[ j ] );
                    if ( dist > 200 )
                        continue;

                    string refj = new string( tempKindergarten[ j ].Value.Where( x => Char.IsDigit( x ) ).ToArray() );
                    bool containj = tempKindergarten[ j ].Value.ToLower().Contains( "детский сад" ) || tempKindergarten[ j ].Value.ToLower().Contains( "доу" );
                    nameMatch = regex.Match( tempKindergarten[ j ].Value );
                    string namej = nameMatch.Success ? nameMatch.Groups[ 1 ].Value.ToLower() : "";

                    if ( namei != namej && !namei.IsEmpty() && !namej.IsEmpty() )
                        continue;
                    
                    if ( ( refi == refj ) ||
                        ( !refj.IsEmpty() && ( ( containi && refi.IsEmpty() ) || tempKindergarten[ i ].Value.IsEmpty() ) ) ||
                        ( !refi.IsEmpty() && ( ( containj && refj.IsEmpty() ) || tempKindergarten[ j ].Value.IsEmpty() ) )
                        )
                    {
                        errors.Add( tempKindergarten[ i ] );
                        errors.Add( tempKindergarten[ j ] );
                    }
                }
            }
            errors = errors.Distinct().ToList();
        }
    }
}
