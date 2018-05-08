using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeneticAlgorithem.Models
{
    public static class Globals
    {
        public const string Connection_String = "mongodb://localhost:27017";
        public const string BoardCollection = "Boards";
        public const string CalendersCollection = "Calendars";
        public const string Web_Config_Key_DB = "DB";

        public const double workStartHour = 9;
        public const double workEndtHour = 17;
    }
}