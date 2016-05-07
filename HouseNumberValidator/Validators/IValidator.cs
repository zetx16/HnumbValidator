using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HouseNumberValidator
{
    interface IValidator
    {
        List<Error> Errors { get; }
        string DescriptionForList { get; }
        string DescriptionForMap { get; }

        void ValidateObject( OsmGeo geo );
        void ValidateEndReadFile();

        string GetPathList( string directory, string region );
        string GetPathMap( string directory, string region );
        string[] GetTableHead();
        //IEnumerable<string[]> GetTableTr();
    }
}
