using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;
using System.Collections;

namespace HnumbValidator
{
    public class ValidatorUncorrectTag : Validator
    {
        Dictionary<List<string>, Dictionary<string, string>> tags;
        List<KeyValuePair<string, string>> ignoreKeyValues;

        public ValidatorUncorrectTag()
        {
            errors = new List<Error>();

            FileEnd = "uncorrect";
            Title = "Не те теги";

            descriptionForList = "Определение типа объекта по названию и отображение недостающих тегов.";
            descriptionForMap = descriptionForList;

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
                        "колледж",
                        "техникум"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "college" }
                    }
                },
                {
                    new List<string>{
                        "школа искусств",
                        "школа исскуств",
                        "школа художественная",
                        "художественная школа"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "training" },
                        { "training", "art" }
                    }
                },
                {
                    new List<string>{
                        "музыкальная школа",
                        "муз школа",
                        "муз. школа"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "training" },
                        { "training", "music" }
                    }
                },
                {
                    new List<string>{
                        "спортивная школа"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "training" },
                        { "training", "sport" }
                    }
                },
                {
                    new List<string>{
                        "школа",
                        "сош",
                        "гимназия"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "school" }
                    }
                },
                {
                    new List<string>{
                        "негосударственный пенсионный"
                    },
                    new Dictionary<string,string>{
                        { "office", "pension_fund" }
                    }
                },
                {
                    new List<string>{
                        "пенсионный"
                    },
                    new Dictionary<string,string>{
                        { "office", "government" },
                        { "government", "pension_fund" }
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
                        "министерство"
                    },
                    new Dictionary<string,string>{
                        { "office", "government" },
                        { "government", "ministry" }
                    }
                },
                {
                    new List<string>{
                        "центр занятости"
                    },
                    new Dictionary<string,string>{
                        { "office", "government" },
                        { "government", "recruitment" }
                    }
                },
                {
                    new List<string>{
                        "налоговая"
                    },
                    new Dictionary<string,string>{
                        { "office", "government" },
                        { "government", "tax" }
                    }
                },
                {
                    new List<string>{
                        "отдел образования"
                    },
                    new Dictionary<string,string>{
                        { "office", "government" },
                        { "government", "education" }
                    }
                },
                {
                    new List<string>{
                        "миграционная"
                    },
                    new Dictionary<string,string>{
                        { "office", "government" },
                        { "government", "migration" }
                    }
                },
                {
                    new List<string>{
                        "приставов",
                        "приставы"
                    },
                    new Dictionary<string,string>{
                        { "office", "government" },
                        { "government", "bailiff" }
                    }
                },
                {
                    new List<string>{
                        "соц защита",
                        "соц. защита",
                        "соцзащита",
                        "соц защиты",
                        "соц. защиты",
                        "соцзащиты"
                    },
                    new Dictionary<string,string>{
                        { "office", "government" },
                        { "government", "social_services" }
                    }
                },
                {
                    new List<string>{
                        "гос статистик",
                        "гос. статистик",
                        "государственная статистик",
                        "государственной статистик"
                    },
                    new Dictionary<string,string>{
                        { "office", "government" },
                        { "government", "statistics" }
                    }
                },
                {
                    new List<string>{
                        "следственный комитет"
                    },
                    new Dictionary<string,string>{
                        { "office", "government" },
                        { "government", "investigation" }
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
                        "нии"
                    },
                    new Dictionary<string,string>{
                        { "office", "research" }
                    }
                },
                {
                    new List<string>{
                        "редакция газеты"
                    },
                    new Dictionary<string,string>{
                        { "office", "newspaper" }
                    }
                },
                {
                    new List<string>{
                        "водоканал"
                    },
                    new Dictionary<string,string>{
                        { "office", "water_utility" }
                    }
                },
                {
                    new List<string>{
                        "театр",
                        "тюз"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "theatre" }
                    }
                },
                {
                    new List<string>{
                        "суд"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "courthouse" }
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
                        "дом культуры",
                        "рдк"
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
                        "баня",
                        "сауна"
                    },
                    new Dictionary<string,string>{
                        { "leisure", "sauna" }
                    }
                },
                {
                    new List<string>{
                        "спортивный центр",
                        "спортивный комплекс",
                        "спорткомплекс"
                    },
                    new Dictionary<string,string>{
                        { "leisure", "sports_centre" }
                    }
                },
                {
                    new List<string>{
                        "военкомат",
                        "военком",
                        "военный комиссариат",
                        "военного комиссариата"
                    },
                    new Dictionary<string,string>{
                        { "military", "office" }
                    }
                },
                {
                    new List<string>{
                        "бти"
                    },
                    new Dictionary<string,string>{
                        { "office", "government" }
                    }
                },
                {
                    new List<string>{
                        "музей"
                    },
                    new Dictionary<string,string>{
                        { "tourism", "museum" }
                    }
                },
                {
                    new List<string>{
                        "сельсовет"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "townhall" }
                    }
                },
                {
                    new List<string>{
                        "автосервис"
                    },
                    new Dictionary<string,string>{
                        { "shop", "car_repair" }
                    }
                },
                {
                    new List<string>{
                        "грузоперевозки"
                    },
                    new Dictionary<string,string>{
                        { "office", "logistics" }
                    }
                },
                {
                    new List<string>{
                        "почта",
                        "почтовое"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "post_office" }
                    }
                },
                {
                    new List<string>{
                        "автошкола"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "driving_school" }
                    }
                },
                {
                    new List<string>{
                        "ветеринарная аптека"
                    },
                    new Dictionary<string,string>{
                        { "shop", "pet" }
                    }
                },
                {
                    new List<string>{
                        "аптека"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "pharmacy" }
                    }
                },
                {
                    new List<string>{
                        "травмпункт"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "doctors" }
                    }
                },
                {
                    new List<string>{
                        "стоматология",
                        "стоматолог"
                    },
                    new Dictionary<string,string>{
                        { "amenity", "dentist" }
                    }
                }
            };

            ignoreKeyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>( "public_transport", "platform" ),
                new KeyValuePair<string, string>( "highway", "bus_stop" ),
                new KeyValuePair<string, string>( "public_transport", "stop_position" ),
                new KeyValuePair<string, string>( "type", "route" ),
                new KeyValuePair<string, string>( "type", "route_master" ),
                new KeyValuePair<string, string>( "type", "public_transport" ),
                new KeyValuePair<string, string>( "type", "boundary" ),
                new KeyValuePair<string, string>( "landuse", "allotments" ),
                new KeyValuePair<string, string>( "amenity", "parking" )
            };
        }

        public override IEnumerable GetTableHead()
        {
            yield return "Название объекта";
            yield return "Недостающие теги";
        }

        public override void ValidateObject( OsmGeo geo )
        {
            string value;
            if ( !geo.Tags.TryGetValue( "name", out value ) )
                return;

            foreach ( var keyvalue in tags )
            {
                if ( value.ToLower().ContainsOneOf( keyvalue.Key ) )
                {
                    if ( Ignore( geo ) )
                        return;

                    var error = new Error( geo, value );
                    foreach ( var tag in keyvalue.Value )
                        if ( !geo.Tags.ContainsKeyValue( tag.Key, tag.Value ) )
                            error.Description += tag.Key + " = " + tag.Value + "<br>";

                    if ( !error.Description.IsEmpty() )
                        errors.Add( error );

                    return;
                }
            }
        }

        private bool Ignore( OsmGeo geo )
        {
            foreach ( var ignoreKeyValue in ignoreKeyValues )
                if ( geo.Tags.ContainsKeyValue( ignoreKeyValue.Key, ignoreKeyValue.Value ) )
                    return true;

            if ( ( geo.Tags.ContainsKeyValue( "landuse", "construction" ) ||
                geo.Tags.ContainsKeyValue( "landuse", "greenfield" ) ||
                geo.Tags.ContainsKeyValue( "landuse", "brownfield" ) ) &&
                geo.Tags.ContainsKey( "opening_date" ) )
                return true;

            return false;
        }

        public override void ValidateEndReadFile()
        {
            errors = errors.OrderBy( x => x.Description ).ThenByDescending( x => x.TimeStump ).ToList();
        }
    }
}
