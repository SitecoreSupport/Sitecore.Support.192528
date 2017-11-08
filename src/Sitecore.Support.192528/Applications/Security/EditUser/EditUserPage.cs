using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.Applications.Security.EditUser
{
  public class EditUserPage : Sitecore.Shell.Applications.Security.EditUser.EditUserPage
  {
    protected override void OK_Click()
    {
      if (!this.Validate() || !this.ValidateTicket())
      {
        return;
      }
      User user = EditUserPage.GetUser();
      Assert.IsNotNull(user, typeof(User), "User not found", new object[0]);
      UserProfile profile = user.Profile;
      Assert.IsNotNull(profile, typeof(UserProfile));
      try
      {
        System.Collections.Generic.IEnumerable<Role> roles = from System.Web.UI.WebControls.ListItem item in this.Roles.Items
                                                             where System.Web.Security.Roles.RoleExists(item.Value)
                                                             select Role.FromName(item.Value);
        System.Web.HttpContext current = System.Web.HttpContext.Current;
        Assert.IsNotNull(current, typeof(System.Web.HttpContext));
        string text = string.Empty;
        foreach (string text2 in current.Request.Form.Keys)
        {
          if (text2 != null && text2.EndsWith("StartUrlSelector", System.StringComparison.InvariantCulture))
          {
            text = current.Request.Form[text2];
            break;
          }
        }
        if (text == "Default")
        {
          text = string.Empty;
        }
        if (text == "Custom")
        {
          text = this.StartUrl.Text;
        }
        user.Roles.Replace(roles);
        bool flag = false;
        if (Context.IsAdministrator && profile.IsAdministrator != this.IsAdministrator.Checked)
        {
          profile.IsAdministrator = this.IsAdministrator.Checked;
          flag = true;
        }
        if (EditUserPage.HasChanged(profile.FullName, this.FullName.Text))
        {
          profile.FullName = this.FullName.Text;
          flag = true;
        }
        if (EditUserPage.HasChanged(profile.Comment, this.Description.Text))
        {
          profile.Comment = this.Description.Text;
          flag = true;
        }
        if (EditUserPage.HasChanged(profile.Email, this.Email.Text))
        {
          profile.Email = this.Email.Text;
          flag = true;
        }
        if (EditUserPage.HasChanged(profile.StartUrl, text))
        {
          profile.StartUrl = text;
          flag = true;
        }
        if (EditUserPage.HasChanged(profile.ClientLanguage, this.ClientLanguage.SelectedValue) && (!string.IsNullOrEmpty(profile.ClientLanguage) || !string.IsNullOrEmpty(this.ClientLanguage.SelectedValue)))
        {
          profile.ClientLanguage = this.ClientLanguage.SelectedValue;
          flag = true;
        }
        if (EditUserPage.HasChanged(profile.RegionalIsoCode, this.RegionalISOCode.SelectedValue) && (!string.IsNullOrEmpty(profile.RegionalIsoCode) || !string.IsNullOrEmpty(this.RegionalISOCode.SelectedValue)))
        {
          profile.RegionalIsoCode = this.RegionalISOCode.SelectedValue;
          flag = true;
        }
        if (EditUserPage.HasChanged(profile.ContentLanguage, this.ContentLanguage.SelectedValue))
        {
          profile.ContentLanguage = this.ContentLanguage.SelectedValue;
          flag = true;
        }
        if (EditUserPage.HasChanged(profile.Portrait, this.Portrait.Text))
        {
          profile.Portrait = this.Portrait.Text;
          flag = true;
        }
        if (EditUserPage.HasChanged(profile.ManagedDomainNames, this.ManagedDomainsValue.Value))
        {
          SecurityAudit.LogManagedDomainChanged(this, user.Name, profile.ManagedDomainNames, this.ManagedDomainsValue.Value);
          profile.ManagedDomainNames = this.ManagedDomainsValue.Value;
          flag = true;
        }
        this.SaveCustomProperties(profile, ref flag);
        if (flag)
        {
          profile.Save();
          Log.Audit(this, "Edit user: {0}", new string[]
          {
        user.Name
          });
        }
      }
      catch (System.Exception ex)
      {
        SheerResponse.Alert(string.Format("An error occurred while updating the user:\n\n{0}", ex.Message), new string[0]);
        return;
      }
      base.OK_Click();
    }
  }
}