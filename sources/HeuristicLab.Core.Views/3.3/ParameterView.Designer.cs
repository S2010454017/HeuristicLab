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

namespace HeuristicLab.Core.Views {
  partial class ParameterView {
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
      this.dataTypeLabel = new System.Windows.Forms.Label();
      this.dataTypeTextBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.nameTextBox.Location = new System.Drawing.Point(80, 0);
      this.nameTextBox.Size = new System.Drawing.Size(313, 20);
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.descriptionTextBox.Location = new System.Drawing.Point(80, 26);
      this.descriptionTextBox.Size = new System.Drawing.Size(313, 62);
      // 
      // dataTypeLabel
      // 
      this.dataTypeLabel.AutoSize = true;
      this.dataTypeLabel.Location = new System.Drawing.Point(3, 97);
      this.dataTypeLabel.Name = "dataTypeLabel";
      this.dataTypeLabel.Size = new System.Drawing.Size(60, 13);
      this.dataTypeLabel.TabIndex = 4;
      this.dataTypeLabel.Text = "Data &Type:";
      // 
      // dataTypeTextBox
      // 
      this.dataTypeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataTypeTextBox.Location = new System.Drawing.Point(80, 94);
      this.dataTypeTextBox.Name = "dataTypeTextBox";
      this.dataTypeTextBox.ReadOnly = true;
      this.dataTypeTextBox.Size = new System.Drawing.Size(313, 20);
      this.dataTypeTextBox.TabIndex = 5;
      // 
      // ParameterBaseView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.dataTypeTextBox);
      this.Controls.Add(this.dataTypeLabel);
      this.Name = "ParameterBaseView";
      this.Size = new System.Drawing.Size(393, 117);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      this.Controls.SetChildIndex(this.dataTypeLabel, 0);
      this.Controls.SetChildIndex(this.dataTypeTextBox, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Label dataTypeLabel;
    protected System.Windows.Forms.TextBox dataTypeTextBox;

  }
}
