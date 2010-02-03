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

namespace HeuristicLab.Parameters.Views {
  partial class ItemParameterView<T> {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (typeSelectorDialog != null) typeSelectorDialog.Dispose();
        if (components != null) components.Dispose();
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
      this.localValueGroupBox = new System.Windows.Forms.GroupBox();
      this.localValuePanel = new System.Windows.Forms.Panel();
      this.viewHost = new HeuristicLab.Core.Views.ViewHost();
      this.clearLocalValueButton = new System.Windows.Forms.Button();
      this.setLocalValueButton = new System.Windows.Forms.Button();
      this.actualNameTextBox = new System.Windows.Forms.TextBox();
      this.actualNameLabel = new System.Windows.Forms.Label();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.localValueGroupBox.SuspendLayout();
      this.localValuePanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // dataTypeLabel
      // 
      this.dataTypeLabel.Location = new System.Drawing.Point(3, 123);
      this.dataTypeLabel.TabIndex = 6;
      // 
      // dataTypeTextBox
      // 
      this.dataTypeTextBox.Location = new System.Drawing.Point(80, 120);
      this.dataTypeTextBox.Size = new System.Drawing.Size(306, 20);
      this.dataTypeTextBox.TabIndex = 7;
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(80, 0);
      this.nameTextBox.Size = new System.Drawing.Size(306, 20);
      // 
      // descriptionLabel
      // 
      this.descriptionLabel.Location = new System.Drawing.Point(3, 55);
      this.descriptionLabel.TabIndex = 4;
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Location = new System.Drawing.Point(80, 52);
      this.descriptionTextBox.Size = new System.Drawing.Size(306, 62);
      this.descriptionTextBox.TabIndex = 5;
      // 
      // localValueGroupBox
      // 
      this.localValueGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.localValueGroupBox.Controls.Add(this.localValuePanel);
      this.localValueGroupBox.Controls.Add(this.clearLocalValueButton);
      this.localValueGroupBox.Controls.Add(this.setLocalValueButton);
      this.localValueGroupBox.Location = new System.Drawing.Point(0, 146);
      this.localValueGroupBox.Name = "localValueGroupBox";
      this.localValueGroupBox.Size = new System.Drawing.Size(386, 169);
      this.localValueGroupBox.TabIndex = 10;
      this.localValueGroupBox.TabStop = false;
      this.localValueGroupBox.Text = "Local &Value:";
      // 
      // localValuePanel
      // 
      this.localValuePanel.AllowDrop = true;
      this.localValuePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.localValuePanel.Controls.Add(this.viewHost);
      this.localValuePanel.Location = new System.Drawing.Point(6, 48);
      this.localValuePanel.Name = "localValuePanel";
      this.localValuePanel.Size = new System.Drawing.Size(374, 115);
      this.localValuePanel.TabIndex = 0;
      this.localValuePanel.DragOver += new System.Windows.Forms.DragEventHandler(this.localValuePanel_DragEnterOver);
      this.localValuePanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.localValuePanel_DragDrop);
      this.localValuePanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.localValuePanel_DragEnterOver);
      // 
      // viewHost
      // 
      this.viewHost.Content = null;
      this.viewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.viewHost.Location = new System.Drawing.Point(0, 0);
      this.viewHost.Name = "viewHost";
      this.viewHost.Size = new System.Drawing.Size(374, 115);
      this.viewHost.TabIndex = 0;
      // 
      // clearLocalValueButton
      // 
      this.clearLocalValueButton.Enabled = false;
      this.clearLocalValueButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Remove;
      this.clearLocalValueButton.Location = new System.Drawing.Point(35, 19);
      this.clearLocalValueButton.Name = "clearLocalValueButton";
      this.clearLocalValueButton.Size = new System.Drawing.Size(23, 23);
      this.clearLocalValueButton.TabIndex = 9;
      this.toolTip.SetToolTip(this.clearLocalValueButton, "Clear Value");
      this.clearLocalValueButton.UseVisualStyleBackColor = true;
      this.clearLocalValueButton.Click += new System.EventHandler(this.clearLocalValueButton_Click);
      // 
      // setLocalValueButton
      // 
      this.setLocalValueButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Add;
      this.setLocalValueButton.Location = new System.Drawing.Point(6, 19);
      this.setLocalValueButton.Name = "setLocalValueButton";
      this.setLocalValueButton.Size = new System.Drawing.Size(23, 23);
      this.setLocalValueButton.TabIndex = 8;
      this.toolTip.SetToolTip(this.setLocalValueButton, "Set Value");
      this.setLocalValueButton.UseVisualStyleBackColor = true;
      this.setLocalValueButton.Click += new System.EventHandler(this.setLocalValueButton_Click);
      // 
      // actualNameTextBox
      // 
      this.actualNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.actualNameTextBox.Location = new System.Drawing.Point(80, 26);
      this.actualNameTextBox.Name = "actualNameTextBox";
      this.actualNameTextBox.Size = new System.Drawing.Size(306, 20);
      this.actualNameTextBox.TabIndex = 3;
      this.actualNameTextBox.Validated += new System.EventHandler(this.actualNameTextBox_Validated);
      // 
      // actualNameLabel
      // 
      this.actualNameLabel.AutoSize = true;
      this.actualNameLabel.Location = new System.Drawing.Point(3, 29);
      this.actualNameLabel.Name = "actualNameLabel";
      this.actualNameLabel.Size = new System.Drawing.Size(71, 13);
      this.actualNameLabel.TabIndex = 2;
      this.actualNameLabel.Text = "&Actual Name:";
      // 
      // ItemParameterView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.localValueGroupBox);
      this.Controls.Add(this.actualNameTextBox);
      this.Controls.Add(this.actualNameLabel);
      this.Name = "ItemParameterView";
      this.Size = new System.Drawing.Size(386, 315);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.dataTypeTextBox, 0);
      this.Controls.SetChildIndex(this.actualNameLabel, 0);
      this.Controls.SetChildIndex(this.dataTypeLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.actualNameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.localValueGroupBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.localValueGroupBox.ResumeLayout(false);
      this.localValuePanel.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.GroupBox localValueGroupBox;
    protected System.Windows.Forms.TextBox actualNameTextBox;
    protected System.Windows.Forms.Label actualNameLabel;
    protected System.Windows.Forms.Panel localValuePanel;
    protected HeuristicLab.Core.Views.ViewHost viewHost;
    protected System.Windows.Forms.Button setLocalValueButton;
    protected System.Windows.Forms.ToolTip toolTip;
    protected System.Windows.Forms.Button clearLocalValueButton;
  }
}
