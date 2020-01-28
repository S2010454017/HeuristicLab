﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class JsonItemBaseControl : UserControl {
    private JsonItemVMBase VM { get; set; }

    private JsonItemBaseControl() {
      InitializeComponent();
    }

    public JsonItemBaseControl(JsonItemVMBase vm) {
      InitializeComponent();
      VM = vm;

      checkBoxActive.DataBindings.Add("Checked", VM, nameof(JsonItemVMBase.Selected));
      textBoxName.DataBindings.Add("Text", VM, nameof(JsonItemVMBase.Name));
      textBoxActualName.DataBindings.Add("Text", VM, nameof(JsonItemVMBase.ActualName));

      //checkBoxActive.Checked = VM.Selected;
      //textBoxName.Text = VM.Item.Name;
      if (string.IsNullOrWhiteSpace(VM.Item.ActualName))
        textBoxActualName.ReadOnly = true;
      else
        textBoxActualName.Text = VM.Item.ActualName;
    }
  }
}
