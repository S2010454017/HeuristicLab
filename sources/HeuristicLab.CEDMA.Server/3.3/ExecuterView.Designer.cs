﻿namespace HeuristicLab.CEDMA.Server {
  partial class ExecuterView {
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
      this.jobsList = new System.Windows.Forms.ListBox();
      this.maxJobsLabel = new System.Windows.Forms.Label();
      this.maxActiveJobs = new System.Windows.Forms.NumericUpDown();
      ((System.ComponentModel.ISupportInitialize)(this.maxActiveJobs)).BeginInit();
      this.SuspendLayout();
      // 
      // jobsList
      // 
      this.jobsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.jobsList.FormattingEnabled = true;
      this.jobsList.Location = new System.Drawing.Point(3, 29);
      this.jobsList.Name = "jobsList";
      this.jobsList.Size = new System.Drawing.Size(327, 264);
      this.jobsList.TabIndex = 0;
      // 
      // maxJobsLabel
      // 
      this.maxJobsLabel.AutoSize = true;
      this.maxJobsLabel.Location = new System.Drawing.Point(5, 5);
      this.maxJobsLabel.Name = "maxJobsLabel";
      this.maxJobsLabel.Size = new System.Drawing.Size(87, 13);
      this.maxJobsLabel.TabIndex = 1;
      this.maxJobsLabel.Text = "Max. active jobs:";
      // 
      // maxActiveJobs
      // 
      this.maxActiveJobs.Location = new System.Drawing.Point(98, 3);
      this.maxActiveJobs.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
      this.maxActiveJobs.Name = "maxActiveJobs";
      this.maxActiveJobs.Size = new System.Drawing.Size(120, 20);
      this.maxActiveJobs.TabIndex = 2;
      // 
      // ExecuterView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.maxActiveJobs);
      this.Controls.Add(this.maxJobsLabel);
      this.Controls.Add(this.jobsList);
      this.Name = "ExecuterView";
      this.Size = new System.Drawing.Size(333, 294);
      ((System.ComponentModel.ISupportInitialize)(this.maxActiveJobs)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox jobsList;
    private System.Windows.Forms.Label maxJobsLabel;
    private System.Windows.Forms.NumericUpDown maxActiveJobs;
  }
}
