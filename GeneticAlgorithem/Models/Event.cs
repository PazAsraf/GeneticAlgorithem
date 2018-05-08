using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GeneticAlgorithem.Models
{
    [DataContract]
    public class Event
    {
        [DataMember]
        public string eventId { get; set; }

        [DataMember]
        public DateTime startDate { get; set; }

        [DataMember]
        public DateTime endDate { get; set; }

        [DataMember]
        public string title { get; set; }

        public bool doesEventsOverlapping(Event otherEvent)
        {
            // if the events start time is the same
            if (DateTime.Compare(this.startDate, otherEvent.startDate) == 0)
            {
                return false;
            }
            // if this event start before the other
            else if (DateTime.Compare(this.startDate, otherEvent.startDate) < 0)
            {
                // so we have to make sure it end before or in the same time the other one start
                return DateTime.Compare(this.endDate, otherEvent.startDate) <= 0;
            }
            // if this event start after the other one
            else if (DateTime.Compare(this.startDate, otherEvent.startDate) > 0)
            {
                // so we have to make sure it end before or in the same time the other one start
                return DateTime.Compare(otherEvent.endDate, this.startDate) <= 0;
            }
            else
            {
                return false;
            }
        }
    }
}