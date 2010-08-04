﻿namespace HeuristicLab.VS2010Wizards {
  partial class ViewWizardForm {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.baseClassLabel = new System.Windows.Forms.Label();
      this.baseClassTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.isDefaultViewCheckBox = new System.Windows.Forms.CheckBox();
      this.viewContentTypeTextBox = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Location = new System.Drawing.Point(10, 146);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 6;
      this.okButton.Text = "Ok";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Location = new System.Drawing.Point(92, 146);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 7;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // baseClassLabel
      // 
      this.baseClassLabel.AutoSize = true;
      this.baseClassLabel.Location = new System.Drawing.Point(7, 15);
      this.baseClassLabel.Name = "baseClassLabel";
      this.baseClassLabel.Size = new System.Drawing.Size(59, 13);
      this.baseClassLabel.TabIndex = 0;
      this.baseClassLabel.Text = "BaseClass:";
      // 
      // baseClassTextBox
      // 
      this.baseClassTextBox.Location = new System.Drawing.Point(113, 12);
      this.baseClassTextBox.Name = "baseClassTextBox";
      this.baseClassTextBox.Size = new System.Drawing.Size(100, 20);
      this.baseClassTextBox.TabIndex = 1;
      this.baseClassTextBox.Text = "ItemView";
      this.baseClassTextBox.TextChanged += new System.EventHandler(this.baseClassTextBox_TextChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(7, 41);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(100, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "View Content Type:";
      // 
      // isDefaultViewCheckBox
      // 
      this.isDefaultViewCheckBox.AutoSize = true;
      this.isDefaultViewCheckBox.Location = new System.Drawing.Point(113, 64);
      this.isDefaultViewCheckBox.Name = "isDefaultViewCheckBox";
      this.isDefaultViewCheckBox.Size = new System.Drawing.Size(97, 17);
      this.isDefaultViewCheckBox.TabIndex = 4;
      this.isDefaultViewCheckBox.Text = "Is Default View";
      this.isDefaultViewCheckBox.UseVisualStyleBackColor = true;
      this.isDefaultViewCheckBox.CheckedChanged += new System.EventHandler(this.isDefaultViewCheckBox_CheckedChanged);
      // 
      // viewContentTypeTextBox
      // 
      this.viewContentTypeTextBox.Location = new System.Drawing.Point(113, 38);
      this.viewContentTypeTextBox.Name = "viewContentTypeTextBox";
      this.viewContentTypeTextBox.Size = new System.Drawing.Size(162, 20);
      this.viewContentTypeTextBox.TabIndex = 3;
      this.viewContentTypeTextBox.Text = "Item";
      this.viewContentTypeTextBox.TextChanged += new System.EventHandler(this.viewContentTypeTextBox_TextChanged);
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(110, 84);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(165, 60);
      this.label2.TabIndex = 5;
      this.label2.Text = "Warning: Only one view can be default for a certain content type. If there is mor" +
          "e than one default view, the application will crash.";
      // 
      // ViewWizardForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(286, 180);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.viewContentTypeTextBox);
      this.Controls.Add(this.isDefaultViewCheckBox);
      this.Controls.Add(this.baseClassTextBox);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.baseClassLabel);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.Name = "ViewWizardForm";
      this.Text = "New View Wizard";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Label baseClassLabel;
    private System.Windows.Forms.TextBox baseClassTextBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.CheckBox isDefaultViewCheckBox;
    private System.Windows.Forms.TextBox viewContentTypeTextBox;
    private System.Windows.Forms.Label label2;

  }
}