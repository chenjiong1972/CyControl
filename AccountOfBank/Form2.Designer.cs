namespace UnvaryingSagacity.AccountOfBank
{
    partial class Form2
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
            this.cyEditor1 = new UnvaryingSagacity.Core.CyEditor();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cyEditor1
            // 
            this.cyEditor1.BackColor = System.Drawing.Color.White;
            this.cyEditor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cyEditor1.CyTextFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cyEditor1.CyTitleFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cyEditor1.Decimals = 2;
            this.cyEditor1.DisplayZero = false;
            this.cyEditor1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cyEditor1.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cyEditor1.Location = new System.Drawing.Point(80, 38);
            this.cyEditor1.Mode = UnvaryingSagacity.Core.DisplayMode.全部;
            this.cyEditor1.Name = "cyEditor1";
            this.cyEditor1.ReadOnly = false;
            this.cyEditor1.Size = new System.Drawing.Size(352, 77);
            this.cyEditor1.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(294, 184);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 1;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 261);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.cyEditor1);
            this.KeyPreview = true;
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Core.CyEditor cyEditor1;
        private System.Windows.Forms.TextBox textBox1;
    }
}