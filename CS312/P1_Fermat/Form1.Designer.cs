namespace P1_Fermat
{
    partial class Form1
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
            this.m_bSolve = new System.Windows.Forms.Button();
            this.m_tbInput = new System.Windows.Forms.TextBox();
            this.m_tbK = new System.Windows.Forms.TextBox();
            this.m_tbOutput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_bSolve
            // 
            this.m_bSolve.Location = new System.Drawing.Point(159, 131);
            this.m_bSolve.Name = "m_bSolve";
            this.m_bSolve.Size = new System.Drawing.Size(75, 23);
            this.m_bSolve.TabIndex = 0;
            this.m_bSolve.Text = "Solve!";
            this.m_bSolve.UseVisualStyleBackColor = true;
            this.m_bSolve.Click += new System.EventHandler(this.On_SolveClick);
            // 
            // m_tbInput
            // 
            this.m_tbInput.Location = new System.Drawing.Point(144, 53);
            this.m_tbInput.Name = "m_tbInput";
            this.m_tbInput.Size = new System.Drawing.Size(100, 20);
            this.m_tbInput.TabIndex = 1;
            // 
            // m_tbK
            // 
            this.m_tbK.Location = new System.Drawing.Point(144, 79);
            this.m_tbK.Name = "m_tbK";
            this.m_tbK.Size = new System.Drawing.Size(100, 20);
            this.m_tbK.TabIndex = 2;
            // 
            // m_tbOutput
            // 
            this.m_tbOutput.Location = new System.Drawing.Point(144, 105);
            this.m_tbOutput.Name = "m_tbOutput";
            this.m_tbOutput.Size = new System.Drawing.Size(100, 20);
            this.m_tbOutput.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Input";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(84, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "k";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(84, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Output";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 277);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_tbOutput);
            this.Controls.Add(this.m_tbK);
            this.Controls.Add(this.m_tbInput);
            this.Controls.Add(this.m_bSolve);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_bSolve;
        private System.Windows.Forms.TextBox m_tbInput;
        private System.Windows.Forms.TextBox m_tbK;
        private System.Windows.Forms.TextBox m_tbOutput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

