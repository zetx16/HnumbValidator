using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace HouseNumberValidator
{
    [Serializable]
    public class StatValidator
    {
        public string ErrorType;
        public int CountNewErrors;
        public int CountOldErrors;

        public void UpdateCountErrors( int newcount )
        {
            CountOldErrors = CountNewErrors;
            CountNewErrors = newcount;
        }

        public void UpdateCountErrorsAndNoUpdateOldCount( int newcount )
        {
            CountNewErrors = newcount;
        }



        public StatValidator( string type, int cnew, int cold )
        {
            ErrorType = type;
            CountNewErrors = cnew;
            CountOldErrors = cold;
        }

        public StatValidator( string type, int cnew )
            : this( type, cnew, -1 )
        { }

        public StatValidator()
        { }
    }

    [Serializable]
    public class StatRegion
    {
        public string Region;
        public DateTime Stamp;
        public List<StatValidator> Stats;

        public void AddStatForValidator( StatValidator stat )
        {
            Stats.Add( stat );
        }

        public void AddResult( string type, int count )
        {
            var i = Stats.FindIndex( x => x.ErrorType == type );

            if ( i < 0 )
            {
                Stats.Add( new StatValidator( type, count ) );
                return;
            }

            Stats[ i ].UpdateCountErrors( count );
        }

        public bool UpdateDate( DateTime stamp )
        {
            if ( Stamp == stamp.Date )
                return false;

            Stamp = stamp.Date;
            return true;
        }


        public StatRegion( string region, DateTime stamp, List<StatValidator> stats )
        {
            Region = region;
            Stamp  = stamp.Date;
            Stats  = stats;
        }
        public StatRegion( string region, DateTime stamp )
            : this( region, stamp, new List<StatValidator>() )
        { }

        public StatRegion()
        {
            Stats = new List<StatValidator>();
        }
    }

    [Serializable]
    public class StatRegionList
    {
        public List<StatRegion> StatRegions;

        public void UpdateOrAddRegion( string region, DateTime stamp, List<StatValidator> stats )
        {
            var i = StatRegions.FindIndex( x => x.Region == region );

            if ( i < 0 )
            {
                StatRegions.Add( new StatRegion( region, stamp, stats ) );
                return;
            }

            bool dateUpdated = false;
            foreach ( var stat in stats )
            {
                int iStatValidator = StatRegions[ i ].Stats.FindIndex( x => x.ErrorType == stat.ErrorType );
                if ( iStatValidator >= 0 )
                {
                    if ( !dateUpdated )
                        dateUpdated = StatRegions[ i ].UpdateDate( stamp );
                    if ( dateUpdated )
                        StatRegions[ i ].Stats[ iStatValidator ].UpdateCountErrors( stat.CountNewErrors );
                    else
                        StatRegions[ i ].Stats[ iStatValidator ].UpdateCountErrorsAndNoUpdateOldCount( stat.CountNewErrors );
                }
                else
                    StatRegions[ i ].Stats.Add( new StatValidator( stat.ErrorType, stat.CountNewErrors ) );
            }
        }

        public StatRegion GetRegion( string region )
        {
            return StatRegions.Find( x => x.Region == region );
        }


        public StatRegionList( List<StatRegion> statRegions )
        {
            StatRegions = statRegions;
        }
        public StatRegionList()
        {
            StatRegions = new List<StatRegion>();
        }
    }

    public static class Reports
    {
        public static StatRegionList RegionList;

        static Reports()
        {
            if ( !File.Exists( Paths.FileReportXml ) )
            {
                RegionList = new StatRegionList();
                return;
            }

            XmlSerializer formatter = new XmlSerializer( typeof( StatRegionList ) );

            using ( FileStream fs = new FileStream( Paths.FileReportXml, FileMode.OpenOrCreate ) )
                RegionList = (StatRegionList)formatter.Deserialize( fs );

            //RegionList.StatRegions.Sort( ( x, y ) => Regions.RegionsDict[x.Region].CompareTo( Regions.RegionsDict[y.Region] ) );
        }

        public static void Save()
        {
            XmlSerializer formatter = new XmlSerializer( typeof( StatRegionList ) );

            using ( FileStream fs = new FileStream( Paths.FileReportXml, FileMode.Create ) )
                formatter.Serialize( fs, RegionList );
        }
    }
}
