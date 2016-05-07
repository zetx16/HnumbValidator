using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HouseNumberValidator
{
    public class ValidatorUnorrectTag : IValidator
    {
        string fileListEnd = ".uncorrect.html";
        string fileMapEnd = ".uncorrect.map.html";

        List<Error> errors;
        public List<Error> Errors { get { return errors; } }

        string description = "";
        public string DescriptionForList { get { return description; } }

        string descriptionForMap = "";
        public string DescriptionForMap { get { return descriptionForMap; } }

        Dictionary<List<string>, Dictionary<string, string>> tags;

        public ValidatorUnorrectTag()
        {
            errors = new List<Error>();

            tags = new Dictionary<List<string>, Dictionary<string, string>>
            {
                {
                    new List<string>{
                        "детский дом",
                        "детдом",
                        "дет дом"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "social_facility" },
                        { "social_facility", "group_home" },
                        { "social_facility:for", "orphan" }
                    }
                },
                {
                    new List<string>{
                        "престарелых"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "social_facility" },
                        { "social_facility", "group_home" },
                        { "social_facility:for", "senior" }
                    }
                },
                {
                    new List<string>{
                        "молочная кухня"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "social_facility" },
                        { "social_facility", "dairy_kitchen" }
                    }
                },
                {
                    new List<string>{
                        "детский сад",
                        "доу",
                        "детсад"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "kindergarten" }
                    }
                },
                {
                    new List<string>{
                        "школа",
                        "сош"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "school" }
                    }
                },
                {
                    new List<string>{
                        "загс"
                    },
                    new Dictionary<string,string>{
                        { "office", "government" },
                        { "government", "register_office" }
                    }
                },
                {
                    new List<string>{
                        "прокуратура",
                        "прокурор"
                    },
                    new Dictionary<string,string>{
                        { "office", "government" },
                        { "government", "prosecutor" }
                    }
                },
                {
                    new List<string>{
                        "нотариус",
                        "нотариальная"
                    },
                    new Dictionary<string,string>{
                        { "office", "lawyer" },
                        { "lawyer", "notary" }
                    }
                },
                {
                    new List<string>{
                        "адвокат"
                    },
                    new Dictionary<string,string>{
                        { "office", "lawyer" },
                        { "lawyer", "advocate" }
                    }
                },
                {
                    new List<string>{
                        "мэрия"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "townhall" }
                    }
                },
                {
                    new List<string>{
                        "дк",
                        "дом культуры"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "community_centre" }
                    }
                },
                {
                    new List<string>{
                        "лагерь"
                    },
                    new Dictionary<string,string>{
                        { "leisure", "resort" },
                        { "resort", "kids_camp" }
                    }
                },
                {
                    new List<string>{
                        "санаторий"
                    },
                    new Dictionary<string,string>{
                        { "leisure", "resort" },
                        { "resort", "sanatorium" }
                    }
                },
                {
                    new List<string>{
                        "пансионат"
                    },
                    new Dictionary<string,string>{
                        { "leisure", "resort" },
                        { "resort", "pension" }
                    }
                },
                {
                    new List<string>{
                        "база отдыха",
                        "б.о.",
                        "б/о"
                    },
                    new Dictionary<string,string>{
                        { "leisure", "resort" },
                        { "resort", "recreation_center" }
                    }
                },
                {
                    new List<string>{
                        "поликлиника"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "clinic" }
                    }
                }
            };
        }

        public void ValidateObject( OsmGeo geo )
        {
            if ( !geo.Tags.ContainsKey( "name" ) )
                return;

            string value;
            geo.Tags.TryGetValue( "name", out value );

            foreach (var keyvalue in tags)
            {
                if ( value.ContainsOneOf( keyvalue.Key ) )
                {
                    var error = new Error( geo, value );
                    foreach ( var tag in keyvalue.Value )
                        if ( !geo.Tags.ContainsKeyValue( tag.Key, tag.Value ) )
                            error.Description += tag.Key + " = " + tag.Value + "<br>";
                    if ( !error.Description.IsEmpty() )
                        errors.Add( error );
                }
            }

        }


        public void ValidateEndReadFile()
        {
            errors = errors.OrderByDescending( x => x.TimeStump ).ToList();
        }

        public string GetPathList( string directory, string region )
        {
            return directory + region + fileListEnd;
        }
        public string GetPathMap( string directory, string region )
        {
            return directory + region + fileMapEnd;
        }

        public string[] GetTableHead()
        {
            string[] result = new string[ 2 ];
            result[ 0 ] = "Ошибка";
            result[ 1 ] = "Доп. информация";
            return result;
        }
    }
}
