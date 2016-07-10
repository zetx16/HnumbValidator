using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HnumbValidator
{
    public abstract class Validator
    {
        protected List<Error> errors;
        public List<Error> Errors { get { return errors; } }

        public abstract void ValidateObject( OsmGeo geo );

        /// <summary>
        /// Действие после прочтения всего файла
        /// </summary>
        public virtual void ValidateEndReadFile()
        {
            errors = errors.OrderByDescending( x => x.TimeStump ).ToList();
        }

        //--------------Отчеты------------------

        public string FileEnd;

        //-----------Отчеты Список--------------

        public bool ListableReport = true;

        public string Title;

        protected string descriptionForList;

        public string DescriptionForList { get { return descriptionForList; } }

        public string GetPathList( string directory, string region )
        {
            return directory + region + "." + FileEnd + ".html";
        }

        public IEnumerable GetTableHead()
        {
            yield return "Ошибка";
            yield return "Доп. информация";
        }

        //-----------Отчеты Карта---------------

        public bool MapableReport = true;

        protected string descriptionForMap;

        public string DescriptionForMap { get { return descriptionForMap; } }

        public string GetPathMap( string directory, string region )
        {
            return directory + region + "." + FileEnd + ".map.html";
        }
    }
}
