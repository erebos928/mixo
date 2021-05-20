using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinqMixo
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * 3*[5|5|5|7]
             * [5|5|7|8][5|5|7|8]
             */
            Console.WriteLine("Please enter the number of services: ");
            String str = Console.ReadLine();
            int serviceCount = int.Parse(str);
            List<List<int>> formule = new List<List<int>>();
            /* the commented section is for the cases that there is different table number with different capacity in services
            for (int i = 0; i < serviceCount; i++)
            {
                Console.WriteLine("Please enter the number of tables on service " + i + " : ");
                str = Console.ReadLine();
                int tableCount = int.Parse(str);
                List<int> tableCapacities = new List<int>();
                for (int j = 0; j < tableCount; j++)
                {
                    Console.WriteLine("Please enter the capacity of table {0} of service {1}", j, i);
                    str = Console.ReadLine();
                    int t = int.Parse(str);
                    tableCapacities.Add(t);
                }
                formule.Add(tableCapacities);
            }*/
            Console.Write("Please enter the number of tables in each service: ");
            str = Console.ReadLine();
            int tableCount = int.Parse(str);
            Console.Write("Please enter the capacity of tables: ");
            str = Console.ReadLine();
            int tableCapacity = int.Parse(str);
            int friendCount = tableCapacity + 1;
            while (friendCount > tableCapacity)
            {
                Console.Write("Please enter the number of friends (ones who want to sit together): ");
                str = Console.ReadLine();
                friendCount = int.Parse(str);
                if (friendCount > tableCapacity)
                    Console.Write("Should be less than table capacity ({0}) ",tableCapacity);
            }
            int enemyCount = tableCount + 1;
            while (enemyCount > tableCount)
            {
                Console.Write("Please enter the number of enemies (ones who dont want to sit together): ");
                str = Console.ReadLine();
                enemyCount = int.Parse(str);
                if (enemyCount > tableCount)
                    Console.Write("Should be less than table count ({0}) ", tableCount);
            }
            Random rnd = new Random();
            Session session = new Session();
            for (int i = 0; i < serviceCount; i++)
            {
                Service service = new Service();
                for (int j = 0; j < tableCount; j++)
                    service.Add(new Table());
                session.Add(service);
            }
            for (int k = 0; k < serviceCount; k++)
                for (int i = 0; i < friendCount; i++)
                    session[k][0].Add(i);//add friends on service k table 0
            for (int k = 0; k < serviceCount; k++)
                for (int i = friendCount; i < enemyCount && session[k][i].Count < tableCapacity; i++)
                    session[k][i].Add(i);
            int start = friendCount + enemyCount;
            int total = tableCount * tableCapacity;
            for (int i = 0; i < serviceCount; i++)
            {
                int j = 0;
                for (int k = start; k < total; k++)//rest of persons for each service
                {
                    if (session[i][j].Count < tableCapacity)
                        session[i][j].Add(k);
                    else
                    {
                        j++;
                        session[i][j].Add(k);
                    }
                }
            }

            //session = LoadSession();
            //List<Visit> rencontres = session.ComputeVisits();
            /*List<int> p1Candidates = new List<int>();
            var collisions = rencontres.GroupBy(visit => new { visit.Visitor1, visit.Visitor2 },
                                        visit => visit,
                                        (visKey, visits) => new { A = visits.First().Visitor1, B = visits.First().Visitor2, Count = visits.Count() })
                        .Where(g => g.Count > 1)
                        
                        .Distinct();
            foreach (var col in collisions)
            {
                Console.WriteLine(col);
            }
            */
            int minScore = serviceCount * friendCount * (friendCount - 1) / 2;
            //session.Initialize();
            int score = session.ReEvaluateScore();
            bool accepted = false;
            int minIterations = 1000;
            int iterationsCount = minIterations;
            session.ReloadSppRepository();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool mayExists = true;
            int scoreBeforeLoop = score;
            int counter = 0; //counts the number of series of loops without success
            int iterationsSofar = 0;
            int pointCounter = 0;//counts the number of points written on console after conflict count
            try
            {
                while (mayExists)
                {

                    for (int i = 0; i < iterationsCount && score > minScore; i++)
                    {
                        iterationsSofar++;
                        SppStruct spp = session.GetSPP();
                        accepted = false;// boolean shows if spp is accepted by the session
                        while (!accepted)//always enters
                        {
                            try
                            {
                                session.CheckSpp(spp);
                                session.ChangePlaces(spp.s, spp.p1, spp.p2);
                                accepted = true;//if no exception means accepted
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    spp = session.GetSPP();
                                }
                                catch (Exception exp)//no more change
                                {
                                    throw exp;
                                }
                            }
                        }
                        int newscore = session.ReEvaluateScore();
                        //Console.WriteLine("newscore{0}", newscore);
                        if (newscore > score)
                        {
                            //Console.WriteLine("refused");
                            session.ChangePlaces(spp.s, spp.p2, spp.p1);
                        }
                        else
                        {
                            Console.Write("\r Conflicts {0} iteration {1} from {2} {3} ", newscore,i,iterationsCount, GetPoints(pointCounter));
                            score = newscore;
                            session.Success();
                        }
                        if (pointCounter >= 5)
                            pointCounter = 0;
                        pointCounter++;
                    }
                    if (score == minScore)
                        mayExists = false;
                    else
                    {
                        if (scoreBeforeLoop == score)
                        {
                            if (counter < 1)
                            {
                                counter++;
                                iterationsCount = (minIterations > iterationsSofar) ? minIterations : iterationsSofar;
                            }
                            else
                                mayExists = false;
                        }
                        else//had progression
                        {
                            if (iterationsSofar > minIterations)
                                iterationsCount = iterationsSofar;
                            else
                                iterationsCount = minIterations;
                            scoreBeforeLoop = score;
                            counter = 0;
                        }
                    }
                }
            }
            catch(HaltException exp)
            {
                Console.WriteLine(session);
            }
            Console.WriteLine(session);
            Console.ReadKey();
        }
        static String GetPoints(int count)
        {
            String result = "";
            for (int i = 0; i < count; i++)
                result += ".";
            for (int i = count; i < 5; i++)
                result += " ";
            return result;
        }
        static List<Visit> ComputeVisits(int[] serviceTable)
        {
            List<Visit> visits = new List<Visit>();
            for (int i = 0; i < serviceTable.Length; i++)
                for (int j = i + 1; j < serviceTable.Length; j++)
                    visits.Add(new Visit(serviceTable[i], serviceTable[j],true));
            return visits;
        }
        static List<Visit> ComputeVisits(Table serviceTable)
        {
            List<Visit> visits = new List<Visit>();
            for (int i = 0; i < serviceTable.Count; i++)
                for (int j = i + 1; j < serviceTable.Count; j++)
                    visits.Add(new Visit(serviceTable[i], serviceTable[j],true));
            return visits;
        }
        static Session LoadSession()
        {
            FileStream stream = new FileStream("c:\\temp\\result.txt", FileMode.Open);
            StreamReader streamReader = new StreamReader(stream);
            Session session = new Session();
            while (!streamReader.EndOfStream)
            {
                String line = streamReader.ReadLine();
                while (line.StartsWith("SERVICE"))
                {
                    Service service = new Service();
                    String innerLine = streamReader.ReadLine();
                    while (innerLine.StartsWith("Table"))
                    {
                        Table table = new Table();
                        String tableData = streamReader.ReadLine();
                        String[] members = tableData.Split(',');
                        foreach (String member in members)
                        {
                            if (member != "")
                            {
                                int k = int.Parse(member);
                                table.Add(k);
                            }
                        }
                        service.Add(table);
                        innerLine = streamReader.ReadLine();
                    }
                    session.Add(service);
                    line = streamReader.ReadLine();
                    if (line == null)
                        break;
                }
            }
            return session;
        }

    }

}
