﻿using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Roles
{
    public class RoleManager
  {
    private readonly Dictionary<string, int> Rights;

    public RoleManager()
    {
      this.Rights = new Dictionary<string, int>();
    }

    public void Init()
    {
            this.Rights.Clear();
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT fuse, rank FROM fuserights;");
                DataTable table1 = dbClient.GetTable();
                if (table1 != null)
                {
                    foreach (DataRow dataRow in table1.Rows)
                        this.Rights.Add((string)dataRow["fuse"], Convert.ToInt32(dataRow["rank"]));
                }
            }
    }

    public bool RankHasRight(int RankId, string Fuse)
    {
      if (!this.ContainsRight(Fuse))
        return false;
      int num = this.Rights[Fuse];
      return RankId >= num;
    }

    public bool ContainsRight(string Right)
    {
      return this.Rights.ContainsKey(Right);
    }
  }
}
