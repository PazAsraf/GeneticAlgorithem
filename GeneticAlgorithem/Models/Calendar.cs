using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using MongoDB.Bson;

namespace GeneticAlgorithem.Models
{
    [DataContract]
    public class Calendar
    {

        [DataMember]
        public ObjectId _id { get; set; }

        [DataMember]
        public string userId { get; set; }

        [DataMember]
        public List<Event> events { get; set; }
    }
}