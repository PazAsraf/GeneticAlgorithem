using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GeneticAlgorithem.Models;

namespace GeneticAlgorithem.Controllers
{
    public class GAController : ApiController
    {
        // GET api/values/5
        public string Get(string boardId)
        {
            GA.GetAllBoardData(boardId);

            GA.SplitTasks();

            return "value";
        }
    }
}
