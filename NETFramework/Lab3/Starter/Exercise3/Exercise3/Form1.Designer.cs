namespace Exercise3
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.FileSignTextBox = new System.Windows.Forms.TextBox();
            this.KeyFileTextBox = new System.Windows.Forms.TextBox();
            this.SignButton = new System.Windows.Forms.Button();
            this.BrowseSignatureButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.FileVerifyTextBox = new System.Windows.Forms.TextBox();
            this.VerifyButton = new System.Windows.Forms.Button();
            this.KeyFileVerifyTextBox = new System.Windows.Forms.TextBox();
            this.BrowseSignaturFileVerifyButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SignatureFileTextBox = new System.Windows.Forms.TextBox();
            this.BrowseKeyFileButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.SignatureFileVerifyTextBox = new System.Windows.Forms.TextBox();
            this.BrowseKeyFileVerifyButton = new System.Windows.Forms.Button();
            this.BrowseFileButton = new System.Windows.Forms.Button();
            this.BrowseFileVerifyButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BrowseFileButton);
            this.groupBox1.Controls.Add(this.BrowseKeyFileButton);
            this.groupBox1.Controls.Add(this.SignatureFileTextBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.FileSignTextBox);
            this.groupBox1.Controls.Add(this.KeyFileTextBox);
            this.groupBox1.Controls.Add(this.SignButton);
            this.groupBox1.Controls.Add(this.BrowseSignatureButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(21, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(568, 170);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sign";
            // 
            // FileSignTextBox
            // 
            this.FileSignTextBox.Location = new System.Drawing.Point(88, 100);
            this.FileSignTextBox.Name = "FileSignTextBox";
            this.FileSignTextBox.Size = new System.Drawing.Size(352, 20);
            this.FileSignTextBox.TabIndex = 5;
            // 
            // KeyFileTextBox
            // 
            this.KeyFileTextBox.Location = new System.Drawing.Point(88, 25);
            this.KeyFileTextBox.Name = "KeyFileTextBox";
            this.KeyFileTextBox.Size = new System.Drawing.Size(352, 20);
            this.KeyFileTextBox.TabIndex = 4;
            // 
            // SignButton
            // 
            this.SignButton.Location = new System.Drawing.Point(469, 134);
            this.SignButton.Name = "SignButton";
            this.SignButton.Size = new System.Drawing.Size(80, 30);
            this.SignButton.TabIndex = 3;
            this.SignButton.Text = "Sign";
            this.SignButton.UseVisualStyleBackColor = true;
            this.SignButton.Click += new System.EventHandler(this.SignButton_Click);
            // 
            // BrowseSignatureButton
            // 
            this.BrowseSignatureButton.Location = new System.Drawing.Point(469, 58);
            this.BrowseSignatureButton.Name = "BrowseSignatureButton";
            this.BrowseSignatureButton.Size = new System.Drawing.Size(80, 30);
            this.BrowseSignatureButton.TabIndex = 2;
            this.BrowseSignatureButton.Text = "Browse...";
            this.BrowseSignatureButton.UseVisualStyleBackColor = true;
            this.BrowseSignatureButton.Click += new System.EventHandler(this.BrowseSignatureFileButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "File";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "KeyFile";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BrowseFileVerifyButton);
            this.groupBox2.Controls.Add(this.BrowseKeyFileVerifyButton);
            this.groupBox2.Controls.Add(this.SignatureFileVerifyTextBox);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.FileVerifyTextBox);
            this.groupBox2.Controls.Add(this.VerifyButton);
            this.groupBox2.Controls.Add(this.KeyFileVerifyTextBox);
            this.groupBox2.Controls.Add(this.BrowseSignaturFileVerifyButton);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(21, 236);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(565, 162);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Verify";
            // 
            // FileVerifyTextBox
            // 
            this.FileVerifyTextBox.Location = new System.Drawing.Point(86, 95);
            this.FileVerifyTextBox.Name = "FileVerifyTextBox";
            this.FileVerifyTextBox.Size = new System.Drawing.Size(352, 20);
            this.FileVerifyTextBox.TabIndex = 5;
            // 
            // VerifyButton
            // 
            this.VerifyButton.Location = new System.Drawing.Point(467, 126);
            this.VerifyButton.Name = "VerifyButton";
            this.VerifyButton.Size = new System.Drawing.Size(80, 30);
            this.VerifyButton.TabIndex = 3;
            this.VerifyButton.Text = "Verify";
            this.VerifyButton.UseVisualStyleBackColor = true;
            this.VerifyButton.Click += new System.EventHandler(this.VerifyButton_Click);
            // 
            // KeyFileVerifyTextBox
            // 
            this.KeyFileVerifyTextBox.Location = new System.Drawing.Point(86, 23);
            this.KeyFileVerifyTextBox.Name = "KeyFileVerifyTextBox";
            this.KeyFileVerifyTextBox.Size = new System.Drawing.Size(352, 20);
            this.KeyFileVerifyTextBox.TabIndex = 4;
            // 
            // BrowseSignaturFileVerifyButton
            // 
            this.BrowseSignaturFileVerifyButton.Location = new System.Drawing.Point(467, 50);
            this.BrowseSignaturFileVerifyButton.Name = "BrowseSignaturFileVerifyButton";
            this.BrowseSignaturFileVerifyButton.Size = new System.Drawing.Size(80, 30);
            this.BrowseSignaturFileVerifyButton.TabIndex = 2;
            this.BrowseSignaturFileVerifyButton.Text = "Browse...";
            this.BrowseSignaturFileVerifyButton.UseVisualStyleBackColor = true;
            this.BrowseSignaturFileVerifyButton.Click += new System.EventHandler(this.BrowseSignatureFileVerifyButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "File";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "KeyFile";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "SignatureFile";
            // 
            // SignatureFileTextBox
            // 
            this.SignatureFileTextBox.Location = new System.Drawing.Point(88, 64);
            this.SignatureFileTextBox.Name = "SignatureFileTextBox";
            this.SignatureFileTextBox.Size = new System.Drawing.Size(352, 20);
            this.SignatureFileTextBox.TabIndex = 7;
            // 
            // BrowseKeyFileButton
            // 
            this.BrowseKeyFileButton.Location = new System.Drawing.Point(469, 22);
            this.BrowseKeyFileButton.Name = "BrowseKeyFileButton";
            this.BrowseKeyFileButton.Size = new System.Drawing.Size(80, 30);
            this.BrowseKeyFileButton.TabIndex = 8;
            this.BrowseKeyFileButton.Text = "Browse...";
            this.BrowseKeyFileButton.UseVisualStyleBackColor = true;
            this.BrowseKeyFileButton.Click += new System.EventHandler(this.BrowseKeyFileButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 59);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "SignatureFile";
            // 
            // SignatureFileVerifyTextBox
            // 
            this.SignatureFileVerifyTextBox.Location = new System.Drawing.Point(86, 56);
            this.SignatureFileVerifyTextBox.Name = "SignatureFileVerifyTextBox";
            this.SignatureFileVerifyTextBox.Size = new System.Drawing.Size(352, 20);
            this.SignatureFileVerifyTextBox.TabIndex = 9;
            // 
            // BrowseKeyFileVerifyButton
            // 
            this.BrowseKeyFileVerifyButton.Location = new System.Drawing.Point(467, 17);
            this.BrowseKeyFileVerifyButton.Name = "BrowseKeyFileVerifyButton";
            this.BrowseKeyFileVerifyButton.Size = new System.Drawing.Size(80, 30);
            this.BrowseKeyFileVerifyButton.TabIndex = 10;
            this.BrowseKeyFileVerifyButton.Text = "Browse...";
            this.BrowseKeyFileVerifyButton.UseVisualStyleBackColor = true;
            this.BrowseKeyFileVerifyButton.Click += new System.EventHandler(this.BrowseKeyFileVerifyButton_Click);
            // 
            // BrowseFileButton
            // 
            this.BrowseFileButton.Location = new System.Drawing.Point(467, 94);
            this.BrowseFileButton.Name = "BrowseFileButton";
            this.BrowseFileButton.Size = new System.Drawing.Size(80, 30);
            this.BrowseFileButton.TabIndex = 9;
            this.BrowseFileButton.Text = "Browse...";
            this.BrowseFileButton.UseVisualStyleBackColor = true;
            this.BrowseFileButton.Click += new System.EventHandler(this.BrowseFileButton_Click);
            // 
            // BrowseFileVerifyButton
            // 
            this.BrowseFileVerifyButton.Location = new System.Drawing.Point(467, 89);
            this.BrowseFileVerifyButton.Name = "BrowseFileVerifyButton";
            this.BrowseFileVerifyButton.Size = new System.Drawing.Size(80, 30);
            this.BrowseFileVerifyButton.TabIndex = 11;
            this.BrowseFileVerifyButton.Text = "Browse...";
            this.BrowseFileVerifyButton.UseVisualStyleBackColor = true;
            this.BrowseFileVerifyButton.Click += new System.EventHandler(this.BrowseFileVerifyButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 403);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox FileSignTextBox;
        private System.Windows.Forms.TextBox KeyFileTextBox;
        private System.Windows.Forms.Button SignButton;
        private System.Windows.Forms.Button BrowseSignatureButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox FileVerifyTextBox;
        private System.Windows.Forms.Button VerifyButton;
        private System.Windows.Forms.TextBox KeyFileVerifyTextBox;
        private System.Windows.Forms.Button BrowseSignaturFileVerifyButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button BrowseKeyFileButton;
        private System.Windows.Forms.TextBox SignatureFileTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button BrowseKeyFileVerifyButton;
        private System.Windows.Forms.TextBox SignatureFileVerifyTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button BrowseFileButton;
        private System.Windows.Forms.Button BrowseFileVerifyButton;
    }
}

