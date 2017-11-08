using Sitecore.Diagnostics;
using Sitecore.Security;
using Sitecore.Security.Accounts;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sitecore.Support.Applications.Security.EditUser
{
  /// Handles a click on the OK button.
  /// </summary>
  /// <remarks>
  /// When the user clicks OK, the dialog is closed by calling
  /// the <see cref="M:Sitecore.Web.UI.Sheer.ClientResponse.CloseWindow">CloseWindow</see> method.
  public class EditUserPage : Sitecore.Shell.Applications.Security.EditUser.EditUserPage
  {
    /// <summary>
    /// Type object of the base type
    /// </summary>
    protected Type _editUserType = typeof(Sitecore.Shell.Applications.Security.EditUser.EditUserPage);

    protected override void OK_Click()
    {
      if (!(bool)this._editUserType.GetMethod("Validate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, null)
        || !(bool)this._editUserType.GetMethod("ValidateTicket", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, null))
      {
        return;
      }
      User user = this._editUserType.GetMethod("GetUser", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null) as User;
      Assert.IsNotNull(user, typeof(Sitecore.Security.Accounts.User), "User not found", new object[0]);
      UserProfile profile = user.Profile;
      Assert.IsNotNull(profile, typeof(UserProfile));
      try
      {
        // Run single select quiery to extract all existing roles for the user.
        var existingRoles = ExistingRolesFinder.GetExistingRoles(this.Roles.Items);
        IEnumerable<Role> roles = existingRoles.Select(roleName => Role.FromName(roleName));
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
        if (Sitecore.Context.IsAdministrator && profile.IsAdministrator != this.IsAdministrator.Checked)
        {
          profile.IsAdministrator = this.IsAdministrator.Checked;
          flag = true;
        }
        if (this.HasChanged(profile.FullName, this.FullName.Text))
        {
          profile.FullName = this.FullName.Text;
          flag = true;
        }
        if (this.HasChanged(profile.Comment, this.Description.Text))
        {
          profile.Comment = this.Description.Text;
          flag = true;
        }
        if (this.HasChanged(profile.Email, this.Email.Text))
        {
          profile.Email = this.Email.Text;
          flag = true;
        }
        if (this.HasChanged(profile.StartUrl, text))
        {
          profile.StartUrl = text;
          flag = true;
        }
        if (this.HasChanged(profile.ClientLanguage, this.ClientLanguage.SelectedValue) && (!string.IsNullOrEmpty(profile.ClientLanguage) || !string.IsNullOrEmpty(this.ClientLanguage.SelectedValue)))
        {
          profile.ClientLanguage = this.ClientLanguage.SelectedValue;
          flag = true;
        }
        if (this.HasChanged(profile.RegionalIsoCode, this.RegionalISOCode.SelectedValue) && (!string.IsNullOrEmpty(profile.RegionalIsoCode) || !string.IsNullOrEmpty(this.RegionalISOCode.SelectedValue)))
        {
          profile.RegionalIsoCode = this.RegionalISOCode.SelectedValue;
          flag = true;
        }
        if (this.HasChanged(profile.ContentLanguage, this.ContentLanguage.SelectedValue))
        {
          profile.ContentLanguage = this.ContentLanguage.SelectedValue;
          flag = true;
        }
        if (this.HasChanged(profile.Portrait, this.Portrait.Text))
        {
          profile.Portrait = this.Portrait.Text;
          flag = true;
        }
        if (this.HasChanged(profile.ManagedDomainNames, this.ManagedDomainsValue.Value))
        {
          SecurityAudit.LogManagedDomainChanged(this, user.Name, profile.ManagedDomainNames, this.ManagedDomainsValue.Value);
          profile.ManagedDomainNames = this.ManagedDomainsValue.Value;
          flag = true;
        }
        var args = new object[] { profile, flag };
        this._editUserType.GetMethod("SaveCustomProperties", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, args);
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
      // Call base ethod of the Sitecore.Shell.Applications.Security.EditUser.EditUserPage class
      SheerResponse.CloseWindow();
    }

    protected bool HasChanged(string profileValue, string controlValue)
    {
      return (bool)this._editUserType.GetMethod("HasChanged", BindingFlags.Static | BindingFlags.NonPublic)
        .Invoke(null, new object[] { profileValue, controlValue });
    }
  }
}