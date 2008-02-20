#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.PluginInfrastructure.GUI;
using System.Threading;

namespace HeuristicLab {
  public partial class MainForm : Form {

    private ListViewItem pluginManagerListViewItem;

    public MainForm() {
      SplashScreen splashScreen = new SplashScreen(1000, "Loading HeuristicLab...");
      splashScreen.Owner = this;
      splashScreen.Show();

      Application.DoEvents();
      this.Enabled = false;

      PluginManager.Manager.Action += new PluginManagerActionEventHandler(splashScreen.Manager_Action);
      PluginManager.Manager.Initialize();

      InitializeComponent();

      RefreshApplicationsList();

      this.Enabled = true;
      this.Visible = true;
    }

    private void RefreshApplicationsList() {
      applicationsListView.Clear();

      pluginManagerListViewItem = new ListViewItem("Plugin Manager", 0);
      pluginManagerListViewItem.Group = applicationsListView.Groups["Plugin Management"];
      pluginManagerListViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(pluginManagerListViewItem, "-"));
      pluginManagerListViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(pluginManagerListViewItem, "Install, upgrade or delete plugins"));
      pluginManagerListViewItem.ToolTipText = "Install, upgrade or delete plugins";

      applicationsListView.Items.Add(pluginManagerListViewItem);

      foreach(ApplicationInfo info in PluginManager.Manager.InstalledApplications) {
        ListViewItem item = new ListViewItem(info.Name, 0);
        item.Tag = info;
        item.Group = applicationsListView.Groups["Applications"];
        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, info.Version.ToString()));
        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, info.Description));
        item.ToolTipText = info.Description;
        applicationsListView.Items.Add(item);
      }
    }

    private void applicationsListView_ItemActivate(object sender, EventArgs e) {
      if(applicationsListView.SelectedItems.Count > 0) {        
        ListViewItem selected = applicationsListView.SelectedItems[0];
        if(selected == pluginManagerListViewItem) {
          ManagerForm form = new ManagerForm();
          this.Visible = false;
          form.ShowDialog();
          RefreshApplicationsList();
          this.Visible = true;
        } else {
          ApplicationInfo app = (ApplicationInfo)applicationsListView.SelectedItems[0].Tag;
          SplashScreen splashScreen = new SplashScreen(1000, "Loading " + app.Name);
          splashScreen.Owner = this;
          splashScreen.Show();
          this.Visible = false;
          PluginManager.Manager.Action += new PluginManagerActionEventHandler(splashScreen.Manager_Action);
          PluginManager.Manager.Run(app);
          this.Visible = true;
        }
      }
    }

    private void applicationsListBox_SelectedIndexChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
      if(e.IsSelected) {
        startButton.Enabled = true;
      } else {
        startButton.Enabled = false;
      }
    }

    private void largeIconsButton_Click(object sender, EventArgs e) {
      applicationsListView.View = View.LargeIcon;
    }

    private void listButton_Click(object sender, EventArgs e) {
      applicationsListView.View = View.List;
    }

    private void detailsButton_Click(object sender, EventArgs e) {
      applicationsListView.View = View.Details;
    }

  }
}
