﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Views.Symbolic.Symbols {
  partial class LaggedVariableView {
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
      this.minTimeOffsetLabel = new System.Windows.Forms.Label();
      this.maxTimeOffsetLabel = new System.Windows.Forms.Label();
      this.minTimeOffsetTextBox = new System.Windows.Forms.TextBox();
      this.maxTimeOffsetTextBox = new System.Windows.Forms.TextBox();
      this.initializationGroupBox.SuspendLayout();
      this.mutationGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // weightMuLabel
      // 
      this.toolTip.SetToolTip(this.weightMuLabel, "The mu (mean) parameter of the normal distribution to use for initial weights.");
      // 
      // weightInitializationMuTextBox
      // 
      this.weightInitializationMuTextBox.Location = new System.Drawing.Point(93, 19);
      this.weightInitializationMuTextBox.Size = new System.Drawing.Size(309, 20);
      this.toolTip.SetToolTip(this.weightInitializationMuTextBox, "The mu (mean) parameter of the normal distribution from which to sample the initi" +
              "al weights.");
      // 
      // initializationGroupBox
      // 
      this.initializationGroupBox.Location = new System.Drawing.Point(0, 104);
      this.initializationGroupBox.Size = new System.Drawing.Size(408, 73);
      this.initializationGroupBox.TabIndex = 9;
      // 
      // weightSigmaLabel
      // 
      this.toolTip.SetToolTip(this.weightSigmaLabel, "The sigma parameter for the normal distribution to use for the initial weights.");
      // 
      // weightInitializationSigmaTextBox
      // 
      this.weightInitializationSigmaTextBox.Location = new System.Drawing.Point(93, 45);
      this.weightInitializationSigmaTextBox.Size = new System.Drawing.Size(309, 20);
      this.toolTip.SetToolTip(this.weightInitializationSigmaTextBox, "The sigma parameter for the normal distribution from which to sample the initial " +
              "weights.");
      // 
      // mutationGroupBox
      // 
      this.mutationGroupBox.Location = new System.Drawing.Point(0, 183);
      this.mutationGroupBox.Size = new System.Drawing.Size(408, 73);
      this.mutationGroupBox.TabIndex = 10;
      // 
      // multiplicativeWeightChangeLabel
      // 
      this.toolTip.SetToolTip(this.multiplicativeWeightChangeLabel, "The sigma parameter for the normal distribution to use to sample a multiplicative" +
              " change in weight.");
      // 
      // multiplicativeWeightChangeSigmaTextBox
      // 
      this.multiplicativeWeightChangeSigmaTextBox.Size = new System.Drawing.Size(201, 20);
      this.toolTip.SetToolTip(this.multiplicativeWeightChangeSigmaTextBox, "The sigma (std.dev.) parameter for the normal distribution to sample a multiplica" +
              "tive change in weight.");
      // 
      // additiveWeightChangeLabel
      // 
      this.toolTip.SetToolTip(this.additiveWeightChangeLabel, "The sigma (std.dev.) parameter for the normal distribution to sample an additive " +
              "change in weight.");
      // 
      // additiveWeightChangeSigmaTextBox
      // 
      this.additiveWeightChangeSigmaTextBox.Size = new System.Drawing.Size(201, 20);
      this.toolTip.SetToolTip(this.additiveWeightChangeSigmaTextBox, "The sigma (std.dev.) parameter for the normal distribution to sample an additive " +
              "change in weight.");
      // 
      // initialFrequencyLabel
      // 
      this.toolTip.SetToolTip(this.initialFrequencyLabel, "Relative frequency of the symbol in randomly created trees");
      // 
      // initialFrequencyTextBox
      // 
      this.errorProvider.SetIconAlignment(this.initialFrequencyTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.initialFrequencyTextBox.Size = new System.Drawing.Size(315, 20);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(290, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(389, 3);
      // 
      // minTimeOffsetLabel
      // 
      this.minTimeOffsetLabel.AutoSize = true;
      this.minTimeOffsetLabel.Location = new System.Drawing.Point(3, 55);
      this.minTimeOffsetLabel.Name = "minTimeOffsetLabel";
      this.minTimeOffsetLabel.Size = new System.Drawing.Size(81, 13);
      this.minTimeOffsetLabel.TabIndex = 5;
      this.minTimeOffsetLabel.Text = "Min. time offset:";
      // 
      // maxTimeOffsetLabel
      // 
      this.maxTimeOffsetLabel.AutoSize = true;
      this.maxTimeOffsetLabel.Location = new System.Drawing.Point(3, 81);
      this.maxTimeOffsetLabel.Name = "maxTimeOffsetLabel";
      this.maxTimeOffsetLabel.Size = new System.Drawing.Size(84, 13);
      this.maxTimeOffsetLabel.TabIndex = 7;
      this.maxTimeOffsetLabel.Text = "Max. time offset:";
      // 
      // minTimeOffsetTextBox
      // 
      this.minTimeOffsetTextBox.Location = new System.Drawing.Point(93, 52);
      this.minTimeOffsetTextBox.Name = "minTimeOffsetTextBox";
      this.minTimeOffsetTextBox.Size = new System.Drawing.Size(315, 20);
      this.minTimeOffsetTextBox.TabIndex = 6;
      this.minTimeOffsetTextBox.TextChanged += new System.EventHandler(this.minTimeOffsetTextBox_TextChanged);
      // 
      // maxTimeOffsetTextBox
      // 
      this.maxTimeOffsetTextBox.Location = new System.Drawing.Point(93, 78);
      this.maxTimeOffsetTextBox.Name = "maxTimeOffsetTextBox";
      this.maxTimeOffsetTextBox.Size = new System.Drawing.Size(315, 20);
      this.maxTimeOffsetTextBox.TabIndex = 8;
      this.maxTimeOffsetTextBox.TextChanged += new System.EventHandler(this.maxTimeOffsetTextBox_TextChanged);
      // 
      // LaggedVariableView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.maxTimeOffsetTextBox);
      this.Controls.Add(this.minTimeOffsetTextBox);
      this.Controls.Add(this.maxTimeOffsetLabel);
      this.Controls.Add(this.minTimeOffsetLabel);
      this.Name = "LaggedVariableView";
      this.Size = new System.Drawing.Size(408, 260);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.initializationGroupBox, 0);
      this.Controls.SetChildIndex(this.initialFrequencyTextBox, 0);
      this.Controls.SetChildIndex(this.initialFrequencyLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.mutationGroupBox, 0);
      this.Controls.SetChildIndex(this.minTimeOffsetLabel, 0);
      this.Controls.SetChildIndex(this.maxTimeOffsetLabel, 0);
      this.Controls.SetChildIndex(this.minTimeOffsetTextBox, 0);
      this.Controls.SetChildIndex(this.maxTimeOffsetTextBox, 0);
      this.initializationGroupBox.ResumeLayout(false);
      this.initializationGroupBox.PerformLayout();
      this.mutationGroupBox.ResumeLayout(false);
      this.mutationGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label minTimeOffsetLabel;
    private System.Windows.Forms.Label maxTimeOffsetLabel;
    private System.Windows.Forms.TextBox minTimeOffsetTextBox;
    private System.Windows.Forms.TextBox maxTimeOffsetTextBox;

  }
}
