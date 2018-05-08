using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using MongoDB.Bson;

namespace GeneticAlgorithem.Models
{
    [DataContract]
    public class Board
    {
        [DataMember]
        public ObjectId _id { get; set; }

        [DataMember]
        public string boardId { get; set; }

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public DateTime startDate { get; set; }

        [DataMember]
        public DateTime endDate { get; set; }

        [DataMember]
        public User boardOwner { get; set; }

        [DataMember]
        public List<User> boardMembers { get; set; }
        
        [DataMember]
        public List<Task> tasks { get; set; }

    }
}