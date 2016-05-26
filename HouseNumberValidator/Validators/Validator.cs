using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HouseNumberValidator
{
    public abstract class Validator
    {
        protected List<Error> errors;
        public List<Error> Errors { get { return errors; } }

        public abstract void ValidateObject( OsmGeo geo );

        public virtual void ValidateEndReadFile()
        {
            errors = errors.OrderByDescending( x => x.TimeStump ).ToList();
        }

        //-----------Отчеты Список--------------

        protected string fileListEnd;
        protected string description;

        public string DescriptionForList { get { return description; } }

        public string GetPathList( string directory, string region )
        {
            return directory + region + fileListEnd;
        }

        //-----------Отчеты Карта---------------

        protected string fileMapEnd;
        protected string descriptionForMap;

        public string DescriptionForMap { get { return descriptionForMap; } }

        public string GetPathMap( string directory, string region )
        {
            return directory + region + fileMapEnd;
        }

         public abstract string[] GetTableHead();
    }
}
