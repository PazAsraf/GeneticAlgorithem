using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GAF;
using GAF.Operators;
using GAF.Extensions;

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
            ga.OnGenerationComplete += ga_OnGenerationComplete;

            //add the operators to the ga process pipeline 
            ga.Operators.Add(elite);
            ga.Operators.Add(crossover);
            ga.Operators.Add(mutation);

            //run the GA 
            ga.Run(TerminateAlgorithm);
        }

        private double CalculateFitness(Chromosome chromosome)
        {
            //run through each city in the order specified in the chromosome
            User currUser = (User)chromosome.Genes[0].ObjectValue;

            Calendar userCalendar = GA.allCalenders.FirstOrDefault(c => c.userId == currUser.uid);

            Event newEvent = new Event()
            {
                title = this.title,
                eventId = System.Guid.NewGuid().ToString(),
                startDate = GA.currBoard.startDate.Date.AddHours(Globals.workStartHour),
                endDate = GA.currBoard.startDate.Date.AddHours(Globals.workStartHour).AddHours(this.remainingTime)
            };

            // Check if time is ok - today and forword

            // Check if event is ok in the calender
            if (userCalendar == null)
            {
                // it OK
            }
            else
            {
                foreach (Event existEvent in userCalendar.events)
                {
                    if (newEvent.doesEventsOverlapping(existEvent))
                    {
                        // if event is overlapping 
                        // increment time with half hour until end day time
                        // call foreach again and decreas fitness score
                        break;
                    }
                }
            }

            // fitness
            return 0;
        }
        
        public bool TerminateAlgorithm(Population population,
        int currentGeneration, long currentEvaluation)
        {
            return currentGeneration > 400;
        }

        private void ga_OnGenerationComplete(object sender, GaEventArgs e)
        {
            //get the best solution 
            var chromosome = e.Population.GetTop(1)[0];

            //decode chromosome

            //get x and y from the solution 
            var x1 = Convert.ToInt32(chromosome.ToBinaryString(0, chromosome.Count / 2), 2);
            var y1 = Convert.ToInt32(chromosome.ToBinaryString(chromosome.Count / 2, chromosome.Count / 2), 2);

            //Adjust range to -100 to +100 
            var rangeConst = 200 / (System.Math.Pow(2, chromosome.Count / 2) - 1);
            var x = (x1 * rangeConst) - 100;
            var y = (y1 * rangeConst) - 100;

            //display the X, Y and fitness of the best chromosome in this generation 
            Console.WriteLine("x:{0} y:{1} Fitness{2}", x, y, e.Population.MaximumFitness);
        }
    }
}