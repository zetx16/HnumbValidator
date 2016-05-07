using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HouseNumberValidator
{
    static class Regexes
    {
        private static string numbPattern;
        public static Regex numb;

        private static string defisPattern;
        public static Regex defis;

        private static string nchPattern;
        public static Regex nch;

        private static string rimPattern;
        public static Regex rim;

        private static string zyabPattern;
        public static Regex zyab;

        private static string cPattern;
        public static Regex c;

        private static string zgradPattern;
        public static Regex zgrad;
        
        private static string latinPattern;
        public static Regex latin;
        
        private static string korpPattern;
        public static string korpReplace;
        public static Regex korp;
        
        private static string stroyPattern;
        public static string stroyReplace;
        public static Regex stroy;
        
        private static string spacePattern;
        public static string spaceReplace;
        public static Regex space;
        
        private static string soorPattern;
        public static string soorReplace;
        public static Regex soor;

        private static string fullPattern;
        //public static string fullReplace;
        public static Regex full;

        private static string full2Pattern;
        //public static string fullReplace;
        public static Regex full2;

        private static string downPattern;
        public static Regex down;


        private static string flatPattern;
        public static Regex flat;

        static Regexes()
        {
            string korpus = @"(?: к[0-9А-Я]+)?";
            string stroen = @"(?: с[0-9А-Я]+)?";
            string sooryz = @"(?: соор[0-9А-Я]+)?";
            string litera = @"(?: лит[0-9А-Я]+)?";

            numbPattern = @"(?:вл)?[0-9]+[А-Яа-я]?";
            numbPattern += @"(?:/[0-9]+[А-Яа-я]?)?";
            numbPattern += korpus + stroen + sooryz + litera;
            numb = new Regex( numbPattern, RegexOptions.Compiled );

            defisPattern = @"[0-9]+-[0-9]+";//(?:-[0-9]+)?";
            defisPattern += korpus + stroen + sooryz + litera;
            defis = new Regex( defisPattern, RegexOptions.Compiled );

            nchPattern = @"[0-9]+[А-Я]?[/-][0-9]+[А-Я]?[/-][0-9]+[А-Я]?";
            nch = new Regex( nchPattern, RegexOptions.Compiled );

            rimPattern = @"(([0-9]+[А-Я]?[IXV]*)|[IXV]*)[/-]([0-9]+[А-Я]?|[IXV]*)";
            rim = new Regex( rimPattern, RegexOptions.Compiled );

            zyabPattern = @"ЗЯБ-[0-9]+";
            zyab = new Regex( zyabPattern, RegexOptions.Compiled );

            cPattern = @"С-([0-9]+[А-Я]?(?:/[0-9]+[А-Я]?)?|[IXV]*)";
            c = new Regex( cPattern, RegexOptions.Compiled );

            zgradPattern = @"к[0-9А-Я]+";
            zgradPattern += stroen + sooryz + litera;
            zgrad = new Regex( zgradPattern, RegexOptions.Compiled );

            latinPattern = @"[A-Za-z]";
            latin = new Regex( latinPattern, RegexOptions.Compiled );



            korpPattern = @"([0-9]+[А-Яа-я]?) ?[кК]о?р?п?у?с?[. ]*([0-9А-Я]+)";
            korpReplace = @"$1 к$2";
            korp = new Regex( korpPattern, RegexOptions.Compiled );

            stroyPattern = @"([0-9]+[А-Яа-я]?(?: ?[кК][0-9А-Я]+)?) ?[сc]т?р?о?е?н?и?е?[. ]*([0-9А-Я]+)";
            stroyReplace = @"$1 с$2";
            stroy = new Regex( stroyPattern, RegexOptions.Compiled );

            soorPattern = @"([0-9]+[А-Яа-я]?(?: ?[кК][0-9А-Я]+)?(?: ?[с][0-9А-Я]+)?)";
            soorPattern += @" ?соору?ж?е?н?и?е?[. ]*([0-9]+[А-Я]?)";
            soorReplace = @"$1 соор$2";
            soor = new Regex( soorPattern, RegexOptions.Compiled );

            spacePattern = @"([0-9]+) ([А-Яа-я])";
            spaceReplace = @"$1$2";
            space = new Regex( spacePattern, RegexOptions.Compiled );

            fullPattern = @"([0-9]+[А-Яа-я]??)(?: ?[кК]о?р?п?у?с?[. ]*([0-9А-Я]+))?(?: ?[сc]т?р?о?е?н?и?е?[. ]*([0-9А-Я]+))?(?: ?соору?ж?е?н?и?е?[. ]*([0-9А-Я]+))?";
            full = new Regex( fullPattern, RegexOptions.Compiled );


            string korpus2 = @"[кК]о?р?п?у?с?[\. ]*";
            string stroen2 = @"[сc]т?р?о?е?н?и?е?[\. ]*";
            string sooryz2 = @"соору?ж?е?н?и?е?[\. ]*";
            string numb2 = @"[0-9А-Я]+";

            full2Pattern = String.Format( @"д?\.? ?([0-9]+(?:(?:(?!{0}{3}|{1}{3}|{2}{3}))[А-Яа-я])?)(?:,? ?{0}({3}))?(?: ?{1}({3}))?(?: ?{2}({3}))?",
                korpus2,
                stroen2,
                sooryz2,
                numb2 );
            full2 = new Regex( full2Pattern, RegexOptions.Compiled );


            downPattern = @"([0-9]+)([а-жи-нп-я])(\s.*)?";
            down = new Regex( downPattern, RegexOptions.Compiled );


            flatPattern = @"[0-9]+[А-Я]?(?:(?:; ?|-)[0-9]+[А-Я]?)*";
            flat = new Regex( flatPattern, RegexOptions.Compiled );
        }

        public static bool CheckPattern( Regex rgx, string str )
        {
            Match res = rgx.Match( str );
            if ( res.Success && res.Length == str.Length )
                return true;
            else
                return false;
        }

        public static bool MatchSuccess( Regex rgx, string str )
        {
            Match res = rgx.Match( str );
            if ( res.Success )
                return true;
            else
                return false;
        }
    }
}
