using Sitecore.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Sitecore.Support
{
  public static class ExistingRolesFinder
  {
    public static IEnumerable<string> GetExistingRoles(System.Web.UI.WebControls.ListItemCollection providedRoles)
    {
      List<string> existingRoles = new List<string>();
      var roleNames = new string[providedRoles.Count];
      for (int i = 0; i < providedRoles.Count; i++)
      {
        roleNames[i] = $"\'{providedRoles[i].Value}\'";
      }
      string selectRolesQuery = $"SELECT RoleName FROM dbo.aspnet_Roles WHERE RoleName IN ({String.Join(",", roleNames)});";
      using (SqlConnection connection = new SqlConnection(Settings.GetConnectionString(Factory.GetDatabase("core").ConnectionStringName)))
      {
        connection.Open();
        SqlCommand command = new SqlCommand(selectRolesQuery, connection);
        using (SqlDataReader reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            existingRoles.Add((string)reader[0]);
          }
        }
      }

      return existingRoles;
    }
  }
}