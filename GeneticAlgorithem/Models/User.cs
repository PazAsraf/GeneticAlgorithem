using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GeneticAlgorithem.Models
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string uid { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string image { get; set; }
    }
}