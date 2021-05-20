using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqMixo
{
    public class Session : List<Service>
    {
        List<Visit> _visits;
        List<Friends> FriendSets = new List<Friends>();
        List<Enemies> EnemySets = new List<Enemies>();
        List<Visit> Visits { get { return (_visits == null) ? ComputeVisits() : _visits; } }
        List<Visit> FriendPairs = new List<Visit>();
        List<Visit> EnemyPairs = new List<Visit>();
        Random rnd;
        private List<SppStruct> SppRepository = new List<SppStruct>();
        public Session()
        {
            rnd = new Random();
        }
        public void SetFriendSets(List<Friends> sets)
        {
            foreach (Friends set in sets)
            {
                set.Sort();
            }
            FriendSets = sets;
            FriendPairs = CalculateFriendsPairs();
            if (EnemyPairs.Count > 0)
            {
               CheckConflicts();//checks if two persons are enemies and also friends, throws exception
            }
        }
        public void SetEnemySets(List<Enemies> sets)
        {
            foreach (Enemies set in sets)
            {
                set.Sort();
            }
            EnemySets = sets;
            EnemyPairs = CalculateEnemyPairs();
            if (FriendPairs.Count > 0)
            {
                CheckConflicts();
            }
        }
        private void CheckConflicts()
        {
            List<Visit> intersectionList = FriendPairs.Intersect(EnemyPairs).ToList();
            if (intersectionList.Count > 0)
            {
                string plist = "";
                intersectionList.ForEach(p => plist += "," + p.ToString());
                plist.TrimStart(',');
                throw new ConflictException(String.Format("{0} are friends and enemy at the same time",plist));
            }
        }
        public List<Visit> CalculateFriendsPairs()
        {
            List<Visit> friendPairs = new List<Visit>();
            foreach(Friends friends in FriendSets)
            {
                friendPairs.AddRange(ComputeVisits(friends));
            }
            return friendPairs;
        }
        public void Initialize()
        {
            if (FriendSets.Count() > 0)
                ArrangeFriends();
            //if (EnemySets.Count() > 0)
              //  AarrangeEnemies();
        }
        private void ArrangeFriends()
        {
            foreach(Visit pair in FriendPairs)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    Table tableP1 = this[i].GetTable(pair.Visitor1);
                    Table tableP2 = this[i].GetTable(pair.Visitor2);
                    List<int> p1Friends = GetFriends(pair.Visitor1);
                    List<int> p1Enemies = GetEnemies(pair.Visitor1);
                    List<int> p2Friends = GetFriends(pair.Visitor2);
                    List<int> p2Enemies = GetEnemies(pair.Visitor2);

                    if (tableP1 != tableP2)
                    {
                        //select a member of tableP1 that is not friend of p1 and has no enemy on tableP2
                        List<int> eligiblePersonsTable1 = tableP1.Select(p => p)
                                .Where(p => !p1Friends.Contains(p) && !HasEnemyOnTableExceptPerson(p,pair.Visitor2,tableP2)).ToList();

                        //select a member of tableP2 that is not friend of p2 and has no enemy on tableP1
                        List<int> eligiblePersonsTable2 = tableP2.Select(p => p)
                                .Where(p => !p2Friends.Contains(p) && !HasEnemyOnTableExceptPerson(p, pair.Visitor1, tableP1)).ToList();
                        if (eligiblePersonsTable1.Count > 0 && eligiblePersonsTable2.Count > 0)
                        {
                            int p2 = eligiblePersonsTable2[rnd.Next(eligiblePersonsTable2.Count)];
                            int p1 = eligiblePersonsTable1[rnd.Next(eligiblePersonsTable1.Count)];
                            ChangePlaces(i, p1, p2);
                        }
                        else
                            throw new ImpossibleMoveException("Cannot bring {0} near {1}");

                    }
                }
            }
        }
        private bool HasEnemyOnTableExceptPerson(int p,int person,Table t)
        {//checks if p has any enemy on table t except person
            List<int> list = GetEnemies(p).Intersect(t).ToList();
            list.RemoveAll(pr => pr == person);
            return (list.Count > 0);
        }
        public List<Visit> ComputeVisits(List<int> list)
        {
            List<Visit> visits = new List<Visit>();
            for (int i = 0; i < list.Count; i++)
                for (int j = i + 1; j < list.Count; j++)
                    visits.Add(new Visit(list[i], list[j], true));
            return visits;

        }
        public List<Visit> CalculateEnemyPairs()
        {
            List<Visit> enemyPairs = new List<Visit>();
            foreach (Enemies enemies in EnemySets)
            {
                enemyPairs.AddRange(ComputeVisits(enemies));
            }
            return enemyPairs;
        }

        public List<Visit> ComputeVisits()
        {
            List<Visit> result = new List<Visit>();
                foreach (Service service in this)
                {
                    result.AddRange(service.ComputeVisits());
                }
                return result;
        }
        public int GetScore()
        {
            var score = Visits.GroupBy(visit => new { visit.Visitor1, visit.Visitor2 })
                  .Select(g => g.Count())
                  .Where(repeatition => repeatition > 1)
                  .Aggregate(0, (total, currentCount) => total + currentCount);
            return score;
        }
        public int ReEvaluateScore()
        {
            _visits = null;
            return GetScore();
        }
        public void CheckSpp(SppStruct spp)//check spp to see if it is possible. otherwise raises exception
        {
            CanPickupPerson(spp.s, spp.p1);
            CanPickupPerson(spp.s, spp.p2);
            CanMoveTo(spp.s, spp.p1, spp.p2);//Checks if p1 can go in place of p2
            CanMoveTo(spp.s, spp.p2, spp.p1);//checks if p2 can go near p1
        }
        public bool CanPickupPerson(int service,int person)//check if the person can be picked up from its place(it depends if he has any friend in his place)
        {
            List<int> pFriends = GetFriends(person);
            Table p1table = this[service].GetTable(person);
            int t = this[service].IndexOf(p1table);
            List<int> friendsOnTable = p1table.Intersect(pFriends).ToList();
            if (friendsOnTable.Count > 0)
            {
                string plist = "";
                friendsOnTable.ForEach(p => plist += "," + p.ToString());
                plist.TrimStart(',');
                throw new ImpossibleMoveException(String.Format("{0} cannot move. It is attached to {1} on table {2}", person, plist, t));
            }
            else
                return true;
        }
        public List<int> GetFriends(int person)//returns the person's friends
        {
            List<int> pFriends = new List<int>();
            FriendSets.Select(friends => friends)
                .Where(f => f.Contains(person))
                .Aggregate(pFriends, (flist, friendSet) => { flist.AddRange(friendSet); return flist; });
            pFriends.RemoveAll(p => p == person);//the p1's friends
            return pFriends;
        }
        public bool CanMoveTo(int service,int p1, int p2)//when is possible that p1 has no enemy on the p2's table (except that p2)
        {
            List<int> pEnemies = GetEnemies(p1);
            pEnemies.RemoveAll(p => p == p1 || p == p2);
            Table p2table = this[service].GetTable(p2);
            int t = this[service].IndexOf(p2table);
            List<int> enemiesOnTable = p2table.Intersect(pEnemies).ToList();
            if (enemiesOnTable.Count > 0)
            {
                string plist = "";
                enemiesOnTable.ForEach(p => plist += "," + p.ToString());
                plist.TrimStart(',');
                throw new ImpossibleMoveException(String.Format("{0} cannot move. It has these enemies:( {1} ) on table {2}", p1, plist, t));
            }
            else
                return true;
        }
        private List<int> GetEnemies(int person)
        {
            List<int> pEnemies = new List<int>();//p1 enemies
            EnemySets.Select(enemies => enemies)
                .Where(e => e.Contains(person))
                .Aggregate(pEnemies, (elist, enemySet) => { elist.AddRange(enemySet); return elist; });
            return pEnemies;

        }
        public void ChangePlaces(int selectedService, int p1, int p2)
        {
            
            Service service = this[selectedService];
            service.ChangePlaces(p1, p2);
        }
        private List<int> GetServicesList()
        {
            List<int> serviceIds = new List<int>();
            for (int i = 0; i < this.Count; i++)
                serviceIds.Add(i);
            return serviceIds;
        }
        private List<int> GetP1List()
        {
            List<int> result = new List<int>();
            var visg = Visits.GroupBy(visit => new { visit.Visitor1, visit.Visitor2 }, visit => visit,
                (key, groupValues) => new { count = groupValues.Count(), pair = groupValues.First() })
                .Where(g => g.count > 1)
                .Aggregate(result, (buffer, val) => { result.Add(val.pair.Visitor1); result.Add(val.pair.Visitor2); return result; })
                .Distinct();
            return visg.ToList();
        }
        public SppStruct GetSPP()//service-person-person
        {
            if (SppRepository.Count > 0)
            {
                int r = rnd.Next(SppRepository.Count);
                SppStruct temp = SppRepository[r];
                SppRepository.RemoveAt(r);
                return temp;
            }
            else
                throw new HaltException("No more change possible");
        }
        private List<int> GetP2List(int service,int p1)
        {
            return this[service].GetP2List(p1);
        }
        public void Success()//called when the state is favorable
        {
            ReloadSppRepository();
        }
        public void ReloadSppRepository()
        {
            SppRepository.Clear();
            List<int> S = GetServicesList();//all services ids
            List<int> P1 = GetP1List();//the first component of all visits that repeated more than 1
            var sppQuery = S.SelectMany(s => P1, (s, p) => new { s, p })
                .SelectMany(sp => GetP2List(sp.s,sp.p), (sp, pp) => new SppStruct(sp.s, sp.p, pp ));//(service, p1, all persons that are not on same table with p1 in this service)
            SppRepository.AddRange(sppQuery.ToList());
        }
        public override string ToString()
        {
            String result = "";
            for(int i = 0; i < this.Count; i++)
            {
                result += "\r\nService " + i + "\r\n";
                result += this[i].ToString();
                
            }
            return result;
        }
    }
}
