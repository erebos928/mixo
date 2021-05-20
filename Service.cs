using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqMixo
{
    public class Service : List<Table>
    {
        public List<Visit> ComputeVisits()
        {
            List<Visit> result = new List<Visit>();
            foreach (Table table in this)
            {
                result.AddRange(table.ComputeVisits());
            }
            return result;
        }
        public void ChangePlaces(int p1,int p2)
        {
            Table table1 = null;
            Table table2 = null;
            foreach (Table table in this){
                if (table.Contains(p1))
                    table1 = table;
                else
                    if (table.Contains(p2))
                    table2 = table;
            }
            if (table1 != null && table2 != null && table1 != table2)
            {
                int k = table1.IndexOf(p1);
                table1.RemoveAt(k);
                table1.Add(p2);
                k = table2.IndexOf(p2);
                table2.RemoveAt(k);
                table2.Add(p1);
            }
        }
        private int GetTableIndex(int p)//returns the index of table in which exists p
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i].Contains(p))
                    return i;
            return -1;
        }
        public Table GetTable(int p)//get table on which sits p
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i].Contains(p))
                    return this[i];
            return null;
        }
        public int GetP2(int p1,Random rnd)
        {
            int tableIndex = GetTableIndex(p1);//the table on which p1 is present
            if (tableIndex != -1)
            {
                int selectedTable = tableIndex;
                while (selectedTable == tableIndex)
                    selectedTable = rnd.Next(this.Count);
                Table table = this[selectedTable];//a table other than one on which p1 is present
                int p2Index = rnd.Next(table.Count);//select a person on this table
                return table[p2Index];
            }
            else
                throw new Exception("Invalid parameter");
        }
        public List<int> GetP2List(int p1)
        {
            List<int> result = new List<int>();
            int tableIndex = GetTableIndex(p1);//the table on which p1 is present
            if (tableIndex != -1)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (i != tableIndex)
                    {
                        result.AddRange(this[i]);//all members who are not with p1 on the same table are eligible
                    }//in other words all persons on other tables
                }
                return result;
            }
            else
                throw new Exception("Invalid parameter");
        }
        public override string ToString()
        {
            String result = "";
            for (int i = 0; i < this.Count; i++)
            {
                result += "\r\nTable " + i + "\r\n";
                result += this[i].ToString();
            }
            return result;
        }
    }
}
