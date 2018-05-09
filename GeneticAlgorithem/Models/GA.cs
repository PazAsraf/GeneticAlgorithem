using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GeneticAlgorithem.Models;
using System.Web.Configuration;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Core;

namespace GeneticAlgorithem.Models
{
    public static class GA
    {
        public static Board currBoard;
        public static List<Calendar> allCalenders;
        private static MongoClient conn = new MongoClient(Globals.Connection_String);

        public static void GetAllBoardData(string boardId)
        {
            IMongoDatabase database = conn.GetDatabase(WebConfigurationManager.AppSettings[Globals.Web_Config_Key_DB]);

            IMongoCollection<Board> boardsCollection = database.GetCollection<Board>(Globals.BoardCollection);

            currBoard = boardsCollection.Find(x => x.boardId == boardId).FirstOrDefault<Board>();

            // Load users calenders
            IMongoCollection<Calendar> calendersCollection = database.GetCollection<Calendar>(Globals.CalendersCollection);

            allCalenders = calendersCollection.Find(FilterDefinition<Calendar>.Empty).ToList<Calendar>();
        }

        public static void SplitTasks()
        {
            foreach (Task currTask in currBoard.tasks)
            {
                // TODO: Check who should do this by the algo
                currTask.RunGA();
            }

            // save board/return board
        }
    }
}