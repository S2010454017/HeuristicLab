﻿namespace HeuristicLab.PluginInfrastructure.Advanced {
  partial class LocalPluginManager {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Active Plugins", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Inactive Plugins", System.Windows.Forms.HorizontalAlignment.Left);
      this.imageListForLocalItems = new System.Windows.Forms.ImageList(this.components);
      this.localPluginsListView = new HeuristicLab.PluginInfrastructure.Advanced.MultiSelectListView();
      this.nameHeader = new System.Windows.Forms.ColumnHeader();
      this.versionHeader = new System.Windows.Forms.ColumnHeader();
      this.descriptionHeader = new System.Windows.Forms.ColumnHeader();
      this.SuspendLayout();
      // 
      // imageListForLocalItems
      // 
      this.imageListForLocalItems.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
      this.imageListForLocalItems.ImageSize = new System.Drawing.Size(13, 13);
      this.imageListForLocalItems.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // localPluginsListView
      // 
      this.localPluginsListView.CheckBoxes = true;
      this.localPluginsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameHeader,
            this.versionHeader,
            this.descriptionHeader});
      this.localPluginsListView.Dock = System.Windows.Forms.DockStyle.Fill;
      listViewGroup1.Header = "Active Plugins";
      listViewGroup1.Name = "activePluginsGroup";
      listViewGroup2.Header = "Inactive Plugins";
      listViewGroup2.Name = "inactivePluginsGroup";
      this.localPluginsListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
      this.localPluginsListView.Location = new System.Drawing.Point(0, 0);
      this.localPluginsListView.Name = "localPluginsListView";
      this.localPluginsListView.Size = new System.Drawing.Size(570, 547);
      this.localPluginsListView.StateImageList = this.imageListForLocalItems;
      this.localPluginsListView.SuppressItemCheckedEvents = false;
      this.localPluginsListView.TabIndex = 9;
      this.localPluginsListView.UseCompatibleStateImageBehavior = false;
      this.localPluginsListView.View = System.Windows.Forms.View.Details;
      this.localPluginsListView.ItemActivate += new System.EventHandler(this.localPluginsListView_ItemActivate);
      this.localPluginsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.pluginsListView_ItemChecked);
      // 
      // nameHeader
      // 
      this.nameHeader.Text = "Name";
      this.nameHeader.Width = 155;
      // 
      // versionHeader
      // 
      this.versionHeader.Text = "Version";
      this.versionHeader.Width = 92;
      // 
      // descriptionHeader
      // 
      this.descriptionHeader.Text = "Description";
      this.descriptionHeader.Width = 316;
      // 
      // LocalPluginManager
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.localPluginsListView);
      this.Name = "LocalPluginManager";
      this.Size = new System.Drawing.Size(570, 547);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ImageList imageListForLocalItems;
    private MultiSelectListView localPluginsListView;
    private System.Windows.Forms.ColumnHeader nameHeader;
    private System.Windows.Forms.ColumnHeader versionHeader;
    private System.Windows.Forms.ColumnHeader descriptionHeader;
  }
}
