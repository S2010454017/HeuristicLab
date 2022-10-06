﻿namespace HeuristicLab.Problems.PTSP.Views {
  partial class ProbabilisticTSPDataView {
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
      this.tabControl = new System.Windows.Forms.TabControl();
      this.coordinatesTabPage = new System.Windows.Forms.TabPage();
      this.probabilitiesTabPage = new System.Windows.Forms.TabPage();
      this.probabilitiesView = new HeuristicLab.Data.Views.StringConvertibleArrayView();
      this.tspDataView = new HeuristicLab.Problems.TravelingSalesman.Views.ITSPDataView();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.coordinatesTabPage.SuspendLayout();
      this.probabilitiesTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.coordinatesTabPage);
      this.tabControl.Controls.Add(this.probabilitiesTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 26);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(351, 392);
      this.tabControl.TabIndex = 2;
      // 
      // coordinatesTabPage
      // 
      this.coordinatesTabPage.Controls.Add(this.tspDataView);
      this.coordinatesTabPage.Location = new System.Drawing.Point(4, 22);
      this.coordinatesTabPage.Name = "coordinatesTabPage";
      this.coordinatesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.coordinatesTabPage.Size = new System.Drawing.Size(343, 366);
      this.coordinatesTabPage.TabIndex = 0;
      this.coordinatesTabPage.Text = "Coordinates";
      this.coordinatesTabPage.UseVisualStyleBackColor = true;
      // 
      // probabilitiesTabPage
      // 
      this.probabilitiesTabPage.Controls.Add(this.probabilitiesView);
      this.probabilitiesTabPage.Location = new System.Drawing.Point(4, 22);
      this.probabilitiesTabPage.Name = "probabilitiesTabPage";
      this.probabilitiesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.probabilitiesTabPage.Size = new System.Drawing.Size(343, 366);
      this.probabilitiesTabPage.TabIndex = 1;
      this.probabilitiesTabPage.Text = "Probabilities";
      this.probabilitiesTabPage.UseVisualStyleBackColor = true;
      // 
      // probabilitiesView
      // 
      this.probabilitiesView.Caption = "StringConvertibleArray View";
      this.probabilitiesView.Content = null;
      this.probabilitiesView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.probabilitiesView.Location = new System.Drawing.Point(3, 3);
      this.probabilitiesView.Name = "probabilitiesView";
      this.probabilitiesView.ReadOnly = false;
      this.probabilitiesView.Size = new System.Drawing.Size(337, 360);
      this.probabilitiesView.TabIndex = 0;
      // 
      // tspDataView
      // 
      this.tspDataView.Caption = "TSP Data View";
      this.tspDataView.Content = null;
      this.tspDataView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tspDataView.Location = new System.Drawing.Point(3, 3);
      this.tspDataView.Name = "tspDataView";
      this.tspDataView.ReadOnly = false;
      this.tspDataView.Size = new System.Drawing.Size(337, 360);
      this.tspDataView.TabIndex = 0;
      // 
      // ProbabilisticTSPDataView
      // 
      this.Controls.Add(this.tabControl);
      this.Name = "ProbabilisticTSPDataView";
      this.Size = new System.Drawing.Size(351, 418);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.coordinatesTabPage.ResumeLayout(false);
      this.probabilitiesTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage coordinatesTabPage;
    private System.Windows.Forms.TabPage probabilitiesTabPage;
    private Data.Views.StringConvertibleArrayView probabilitiesView;
    private TravelingSalesman.Views.ITSPDataView tspDataView;
  }
}
