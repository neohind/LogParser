namespace LogParse.DockCtrls
{
    partial class CtrlLoadAndAppendLogs
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.cmbLogTypes = new System.Windows.Forms.ComboBox();
            this.listSources = new System.Windows.Forms.ListBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(10, 133);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(41, 21, 41, 21);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(200, 24);
            this.progressBar1.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(41, 0, 41, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 15);
            this.label1.TabIndex = 19;
            this.label1.Text = "Log Type";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(10, 65);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(41, 21, 41, 21);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(200, 26);
            this.btnLoad.TabIndex = 16;
            this.btnLoad.Text = "Load Logfile";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // cmbLogTypes
            // 
            this.cmbLogTypes.FormattingEnabled = true;
            this.cmbLogTypes.Location = new System.Drawing.Point(10, 32);
            this.cmbLogTypes.Margin = new System.Windows.Forms.Padding(41, 21, 41, 21);
            this.cmbLogTypes.Name = "cmbLogTypes";
            this.cmbLogTypes.Size = new System.Drawing.Size(200, 23);
            this.cmbLogTypes.TabIndex = 18;
            // 
            // listSources
            // 
            this.listSources.FormattingEnabled = true;
            this.listSources.HorizontalScrollbar = true;
            this.listSources.ItemHeight = 15;
            this.listSources.Location = new System.Drawing.Point(10, 190);
            this.listSources.Margin = new System.Windows.Forms.Padding(41, 24, 41, 24);
            this.listSources.Name = "listSources";
            this.listSources.Size = new System.Drawing.Size(200, 199);
            this.listSources.TabIndex = 21;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(10, 405);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(200, 23);
            this.btnRemove.TabIndex = 22;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // CtrlLoadAndAppendLogs
            // 
            this.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.listSources);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.cmbLogTypes);
            this.Margin = new System.Windows.Forms.Padding(41, 21, 41, 21);
            this.Name = "CtrlLoadAndAppendLogs";
            this.Size = new System.Drawing.Size(222, 565);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ComboBox cmbLogTypes;
        private System.Windows.Forms.ListBox listSources;
        private System.Windows.Forms.Button btnRemove;
    }
}
