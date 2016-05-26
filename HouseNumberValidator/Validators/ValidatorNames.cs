using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HouseNumberValidator
{
    class ValidatorNames: Validator
    {
        List<string> keys = new List<string>{
				"highway",
				"amenity",
				"place",
				"waterway",
				"shop",
				"natural",
				"landuse",
				"religion",
				"boundary",
                "railway",
                "route",
                "tourism",
                "power",
                "historic",
                "route_master",
                "public_transport",
                "office",
                "traffic_sign",
                "aeroway",
                "craft",
                "leisure",
                "emergency",
                "man_made",
                "social_facility",
                "attraction"
			};

        Dictionary<string, string> tags = new Dictionary<string, string>{
                { "associatedStreet", "type" },
                { "collection", "type" },
                { "bridge", "type" },
                { "train_station", "building" }
            };

        public ValidatorNames()
        {
            fileListEnd = ".names.html";
            fileMapEnd = ".names.map.html";

            errors = new List<Error>();

            description = "Список объектов с тегом name, у которых нет ни одного из данных тегов: ";
            descriptionForMap = "Объекты с тегом name, у которых нет ни одного из данных тегов: ";

            foreach ( var key in keys.OrderBy( x => x ).ToList() )
            {
                description += key + ", ";
                descriptionForMap += key + ", ";
            }
            foreach ( var tag in tags.OrderBy( x => x.Value ).ToList() )
            {
                description += tag.Value + "=" + tag.Key + ", ";
                descriptionForMap += tag.Value + "=" + tag.Key + ", ";
            }

            description = description.TrimEnd( new char[] { ',', ' ' } );
            descriptionForMap = descriptionForMap.TrimEnd( new char[] { ',', ' ' } );
            description += ".<br>"
                + "Например: есть объект с name=\"Школа №2\", но нет тега amenity=school.<br>"
                + "В колонке с дополнительной информацией перечислены все ключи данного объкта.<br>";
            descriptionForMap += ".<br>" + "Например: есть объект с name=\"Школа №2\", но нет тега amenity=school.<br>";
        }

        public override void ValidateObject(OsmGeo geo)
        {
            string value;

            if ( geo.Tags.TryGetValue( "name", out value ) && !ValidateVoidNames( geo ) )
            {
                Error name = new Error( geo, value );

                string alltags = "";
                foreach ( var tg in geo.Tags )
                {
                    if ( !tg.Key.StartsWith( "addr" ) && !tg.Key.StartsWith( "building:" ) && !tg.Key.StartsWith( "name" ) )
                        alltags += tg.Key + " | ";
                }
                name.Description += alltags.TrimEnd( new char[ 2 ] { '|', ' ' } ) + "<br>";

                GeoOperations.GetCoordinates( geo, name );
                errors.Add( name );
            }
        }

        public override void ValidateEndReadFile()
        {
            errors = errors.OrderBy( x => x.Value ).ToList();
        }

        private bool ValidateVoidNames( OsmGeo geo )
        {
            if ( geo.Tags.ContainsOneOfKeys( keys ) )
                return true;

            foreach ( var tag in tags )
                if ( geo.Tags.ContainsKeyValue( tag.Value, tag.Key ) )
                    return true;

            return false;
        }

        private bool ValidateVoidNames( Error hn, OsmGeo geo )
        {
            StringComparison nocs = StringComparison.OrdinalIgnoreCase;
            if ( geo.Tags.ContainsOneOfKeys( keys ) )
                return true;

            string value;
            geo.Tags.TryGetValue( "name", out value );
            /*
            if ( value.IndexOf( "азс", nocs ) >= 0 || value.IndexOf( "заправ", nocs ) >= 0 )
            {
                hn.Description += "Автозаправка<br>";
                return false;
            }
            if ( value.IndexOf( "школа", nocs ) >= 0 || value.IndexOf( "сош", nocs ) >= 0 )
            {
                hn.Description += "Школа<br>";
                return false;
            }
            if ( value.IndexOf( "доу", nocs ) >= 0 || value.IndexOf( "сад", nocs ) >= 0 )
            {
                hn.Description += "Детский сад<br>";
                return false;
            }*/
            if ( value.IndexOf( "молочная кухня", nocs ) >= 0 )
            {
                hn.Description += @"amenity=<a href=""http://wiki.openstreetmap.org/wiki/RU:Key:social_facility"">social_facility</a> & social_facility=dairy_kitchen<br>";
                return false;
            }
            if ( value.IndexOf( "детский сад", nocs ) >= 0 || value.IndexOf( "д/с", nocs ) >= 0 )
            {
                hn.Description += @"amenity=<a href=""http://wiki.openstreetmap.org/wiki/Tag:amenity%3Dkindergarten"">kindergarten</a><br>";
                return false;
            }

            foreach ( var tag in tags )
                if ( geo.Tags.ContainsKeyValue( tag.Value, tag.Key ) )
                    return true;

            return false;
        }

        public override string[] GetTableHead()
        {
            string[] result = new string[ 2 ];
            result[ 0 ] = "Ошибка";
            result[ 1 ] = "Доп. информация";
            return result;
        }

        public IEnumerable<string[]> GetTableTr()
        {
            string[] result = new string[ 3 ];

            foreach ( var error in errors )
            {
                result[ 0 ] = String.Format( @"<a href=""http://127.0.0.1:8111/load_object?objects={0}{1}"" onClick=""open_josm('{0}{1}');return false;""><img src=icon_to_josm.png></a>", error.Type, error.Osmid );
                result[ 1 ] = String.Format( @"<a href=""http://osm.org/{0}/{1}"">{2}</a>", error.Type, error.Osmid, error.Value );
                result[ 2 ] = error.Description;
                yield return result;
            }
        }
    }
}
