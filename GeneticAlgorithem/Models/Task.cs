using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GAF;
using GAF.Operators;
using GAF.Extensions;
using MongoDB.Driver;
using System.Web.Configuration;

namespace GeneticAlgorithem.Models
{
    [DataContract]
    public class Task
    {
        [DataMember]
        public string boardId { get; set; }

        [DataMember]
        public string taskId { get; set; }

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public string boardName { get; set; }

        [DataMember]
        public string status { get; set; }

        [DataMember]
        public double overallTime { get; set; }

        [DataMember]
        public double remainingTime { get; set; }

        [DataMember]
        public User owner { get; set; }

        public void RunGA()
        {
            const double crossoverProbability = 0.8;
            const double mutationProbability = 0.02;
            const int elitismPercentage = 5;

            //create the population
            var population = new Population();

            //create the chromosomes
            foreach (User currUser in GA.currBoard.boardMembers)
            {
                var chromosome = new Chromosome();
                chromosome.Genes.Add(new Gene(currUser));
                chromosome.Genes.ShuffleFast();
                population.Solutions.Add(chromosome);
            }

            //create the genetic operators 
            var elite = new Elite(elitismPercentage);

            var crossover = new Crossover(crossoverProbability, true)
            {
                CrossoverType = CrossoverType.SinglePoint
            };

            var mutation = new BinaryMutate(mutationProbability, true);

            //create the GA itself 
            var ga = new GeneticAlgorithm(population, CalculateFitness);

            //subscribe to the GAs Generation Complete event 
            //ga.OnGenerationComplete += ga_OnGenerationComplete;
            ga.OnRunComplete += ga_OnRunComplete;

            //add the operators to the ga process pipeline 
            ga.Operators.Add(elite);
            ga.Operators.Add(crossover);
            ga.Operators.Add(mutation);

            //run the GA 
            ga.Run(TerminateAlgorithm);
        }

        private double CalculateFitness(Chromosome chromosome)
        {
            //get the current user specified in the chromosome
            User currUser = (User)chromosome.Genes[0].ObjectValue;

            // Get the user calender
            Calendar userCalendar = GA.allCalenders.FirstOrDefault(c => c.uid == currUser.uid);

            // Create event for the task
            // the event is start in 9am and end in 9+remaining time of the task
            Event newEvent = new Event()
            {
                title = this.title,
                eventId = System.Guid.NewGuid().ToString(),
                startDate = GA.currBoard.startDate.ToLocalTime().Date.AddHours(Globals.workStartHour),
                endDate = GA.currBoard.startDate.ToLocalTime().Date.AddHours(Globals.workStartHour).AddHours(this.remainingTime)
            };

            // Check if time is ok 
            // start date is today or forword
            if (newEvent.startDate.Date < DateTime.Today.Date)
            {
                newEvent.startDate = DateTime.Today.Date.AddHours(Globals.workStartHour);
                newEvent.endDate = newEvent.startDate.AddHours(this.remainingTime);
            }
            
            // Check event fitness in the calender
            if (userCalendar == null)
            {
                // the user has nothing in the calender so his fitness is high!
                // TODO - return fitness highest! ?? 
                currUser.taskBestTiming[this.taskId] = newEvent;

                return 1;
            }
            else
            {
                // Get only event in the time of the calender
                userCalendar.events = userCalendar.events.Where(c => c.startDate >= DateTime.Today && c.endDate <= GA.currBoard.endDate).ToList();

                // The user has events in the calender so we need to check when is the best time to put this task-event
                foreach (Event existEvent in userCalendar.events.OrderBy(x => x.startDate))
                {
                    if (newEvent.doesEventsOverlapping(existEvent))
                    {
                        // if event is overlapping 
                        newEvent.startDate = existEvent.endDate.ToLocalTime();
                        newEvent.endDate = newEvent.startDate.AddHours(this.remainingTime);

                        // make sure that the timing is in not after the work time
                        if (newEvent.endDate.Hour > Globals.workEndtHour)
                        {
                            newEvent.startDate = newEvent.startDate.Date.AddDays(1).AddDays(Globals.workStartHour);
                            newEvent.endDate = newEvent.startDate.AddHours(this.remainingTime);
                        }

                        // decreas fitness score
                    }
                    else
                    {
                        break;
                    }
                }

                if (newEvent.endDate > GA.currBoard.endDate)
                {
                    // fintess 0
                    return 0;
                }
                else
                {
                    currUser.taskBestTiming[this.taskId] = newEvent;
                    TimeSpan fit = newEvent.startDate - GA.currBoard.startDate;
                    
                    return (1 / (fit.TotalHours.Equals(0) ? 1 : fit.TotalHours));
                }
            }

            // return default fitness
            //return 0;
        }
        
        public bool TerminateAlgorithm(Population population,
        int currentGeneration, long currentEvaluation)
        {
            return currentGeneration > 10;
        }

        private void ga_OnRunComplete(object sender, GaEventArgs e)
        {
            //get the best solution 
            //get the current user specified in the chromosome
            User currUser = (User)e.Population.GetTop(1)[0].Genes[0].ObjectValue;

            // get the event best timing
            Event bestEvent = currUser.taskBestTiming[this.taskId];

            // save event in program
            Calendar userCalendar = GA.allCalenders.FirstOrDefault(c => c.uid == currUser.uid);

            if (userCalendar == null)
            {
                userCalendar = new Calendar()
                {
                    uid = currUser.uid
                };
            }

            userCalendar.events.Add(bestEvent);
            this.owner = currUser;
            // save to the db
            MongoClient conn = new MongoClient(Globals.Connection_String);

            IMongoDatabase database = conn.GetDatabase(WebConfigurationManager.AppSettings[Globals.Web_Config_Key_DB]);

            IMongoCollection<Calendar> calendersCollection = database.GetCollection<Calendar>(Globals.CalendersCollection);

            // upsert to the db the current user calendar- TODO
        }
    }
}