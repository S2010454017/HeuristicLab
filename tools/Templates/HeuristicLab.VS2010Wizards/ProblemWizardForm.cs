﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.VS2010Wizards {
  public partial class ProblemWizardForm : Form {
    public string ProblemName {
      get;
      private set;
    }
    public string ProblemDescription {
      get;
      private set;
    }
    public bool IsMultiObjective {
      get;
      private set;
    }
    public string ParameterProperties {
      get;
      private set;
    }
    public string Properties {
      get;
      private set;
    }
    public string ParameterInitializers {
      get;
      private set;
    }

    public ProblemWizardForm() {
      InitializeComponent();
    }

    private void finishButton_Click(object sender, System.EventArgs e) {
      SetProperties();
      DialogResult = System.Windows.Forms.DialogResult.OK;
      Close();
    }

    private void cancelButton_Click(object sender, System.EventArgs e) {
      DialogResult = System.Windows.Forms.DialogResult.Cancel;
      Close();
    }

    private void SetProperties() {
      ProblemName = problemNameTextBox.Text;
      ProblemDescription = problemDescriptionTextBox.Text;
      IsMultiObjective = isMultiObjectiveCheckBox.Checked;
      ParameterProperties = parametersControl.GetParameterProperties("private");
      Properties = parametersControl.GetProperties("public");
      ParameterInitializers = parametersControl.GetInitializers();
    }

    private void nextButton_Click(object sender, EventArgs e) {
      page1Panel.Visible = false;
      page2Panel.Visible = true;
      nextButton.Enabled = false;
      previousButton.Enabled = true;
    }

    private void previousButton_Click(object sender, EventArgs e) {
      page2Panel.Visible = false;
      page1Panel.Visible = true;
      previousButton.Enabled = false;
      nextButton.Enabled = true;
    }
  }
}
