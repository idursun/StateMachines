namespace StateMachine.Designer
{
    partial class FrmMain
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Graph.Compatibility.AlwaysCompatible alwaysCompatible1 = new Graph.Compatibility.AlwaysCompatible();
            this.graphControl1 = new Graph.GraphControl();
            this.SuspendLayout();
            // 
            // graphControl1
            // 
            this.graphControl1.BackColor = System.Drawing.Color.Gray;
            this.graphControl1.CompatibilityStrategy = alwaysCompatible1;
            this.graphControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphControl1.FocusElement = null;
            this.graphControl1.HighlightCompatible = false;
            this.graphControl1.LargeGridStep = 0F;
            this.graphControl1.LargeStepGridColor = System.Drawing.Color.LightGray;
            this.graphControl1.Location = new System.Drawing.Point(0, 0);
            this.graphControl1.Name = "graphControl1";
            this.graphControl1.ShowLabels = false;
            this.graphControl1.Size = new System.Drawing.Size(668, 454);
            this.graphControl1.SmallGridStep = 0F;
            this.graphControl1.SmallStepGridColor = System.Drawing.Color.Gray;
            this.graphControl1.TabIndex = 0;
            this.graphControl1.Text = "graphControl1";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 454);
            this.Controls.Add(this.graphControl1);
            this.Name = "FrmMain";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Graph.GraphControl graphControl1;
    }
}

