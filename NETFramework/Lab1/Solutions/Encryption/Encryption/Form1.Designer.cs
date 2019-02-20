namespace Encryption
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.BrowseEncryptButton = new System.Windows.Forms.Button();
            this.EncryptButton = new System.Windows.Forms.Button();
            this.BrowseDecryptButton = new System.Windows.Forms.Button();
            this.DecryptButton = new System.Windows.Forms.Button();
            this.PasswordEncryptTextBox = new System.Windows.Forms.TextBox();
            this.FileEncryptTextBox = new System.Windows.Forms.TextBox();
            this.PasswordDecryptTextBox = new System.Windows.Forms.TextBox();
            this.FileDecryptTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.FileEncryptTextBox);
            this.groupBox1.Controls.Add(this.PasswordEncryptTextBox);
            this.groupBox1.Controls.Add(this.EncryptButton);
            this.groupBox1.Controls.Add(this.BrowseEncryptButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(21, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(568, 132);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Encrypt";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.FileDecryptTextBox);
            this.groupBox2.Controls.Add(this.DecryptButton);
            this.groupBox2.Controls.Add(this.PasswordDecryptTextBox);
            this.groupBox2.Controls.Add(this.BrowseDecryptButton);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(23, 169);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(565, 131);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Decrypt";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "File";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 62);
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
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Password";
            // 
            // BrowseEncryptButton
            // 
            this.BrowseEncryptButton.Location = new System.Drawing.Point(469, 50);
            this.BrowseEncryptButton.Name = "BrowseEncryptButton";
            this.BrowseEncryptButton.Size = new System.Drawing.Size(80, 30);
            this.BrowseEncryptButton.TabIndex = 2;
            this.BrowseEncryptButton.Text = "Browse...";
            this.BrowseEncryptButton.UseVisualStyleBackColor = true;
            this.BrowseEncryptButton.Click += new System.EventHandler(this.BrowseEncryptButton_Click);
            // 
            // EncryptButton
            // 
            this.EncryptButton.Location = new System.Drawing.Point(469, 86);
            this.EncryptButton.Name = "EncryptButton";
            this.EncryptButton.Size = new System.Drawing.Size(80, 30);
            this.EncryptButton.TabIndex = 3;
            this.EncryptButton.Text = "Encrypt";
            this.EncryptButton.UseVisualStyleBackColor = true;
            this.EncryptButton.Click += new System.EventHandler(this.EncryptButton_Click);
            // 
            // BrowseDecryptButton
            // 
            this.BrowseDecryptButton.Location = new System.Drawing.Point(467, 53);
            this.BrowseDecryptButton.Name = "BrowseDecryptButton";
            this.BrowseDecryptButton.Size = new System.Drawing.Size(80, 30);
            this.BrowseDecryptButton.TabIndex = 2;
            this.BrowseDecryptButton.Text = "Browse...";
            this.BrowseDecryptButton.UseVisualStyleBackColor = true;
            this.BrowseDecryptButton.Click += new System.EventHandler(this.BrowseDecryptButton_Click);
            // 
            // DecryptButton
            // 
            this.DecryptButton.Location = new System.Drawing.Point(467, 89);
            this.DecryptButton.Name = "DecryptButton";
            this.DecryptButton.Size = new System.Drawing.Size(80, 30);
            this.DecryptButton.TabIndex = 3;
            this.DecryptButton.Text = "Decrypt";
            this.DecryptButton.UseVisualStyleBackColor = true;
            this.DecryptButton.Click += new System.EventHandler(this.DecryptButton_Click);
            // 
            // PasswordEncryptTextBox
            // 
            this.PasswordEncryptTextBox.Location = new System.Drawing.Point(88, 25);
            this.PasswordEncryptTextBox.Name = "PasswordEncryptTextBox";
            this.PasswordEncryptTextBox.Size = new System.Drawing.Size(92, 20);
            this.PasswordEncryptTextBox.TabIndex = 4;
            // 
            // FileEncryptTextBox
            // 
            this.FileEncryptTextBox.Location = new System.Drawing.Point(88, 56);
            this.FileEncryptTextBox.Name = "FileEncryptTextBox";
            this.FileEncryptTextBox.Size = new System.Drawing.Size(352, 20);
            this.FileEncryptTextBox.TabIndex = 5;
            // 
            // PasswordDecryptTextBox
            // 
            this.PasswordDecryptTextBox.Location = new System.Drawing.Point(86, 23);
            this.PasswordDecryptTextBox.Name = "PasswordDecryptTextBox";
            this.PasswordDecryptTextBox.Size = new System.Drawing.Size(92, 20);
            this.PasswordDecryptTextBox.TabIndex = 4;
            // 
            // FileDecryptTextBox
            // 
            this.FileDecryptTextBox.Location = new System.Drawing.Point(86, 59);
            this.FileDecryptTextBox.Name = "FileDecryptTextBox";
            this.FileDecryptTextBox.Size = new System.Drawing.Size(352, 20);
            this.FileDecryptTextBox.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 306);
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
        private System.Windows.Forms.TextBox FileEncryptTextBox;
        private System.Windows.Forms.TextBox PasswordEncryptTextBox;
        private System.Windows.Forms.Button EncryptButton;
        private System.Windows.Forms.Button BrowseEncryptButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox FileDecryptTextBox;
        private System.Windows.Forms.Button DecryptButton;
        private System.Windows.Forms.TextBox PasswordDecryptTextBox;
        private System.Windows.Forms.Button BrowseDecryptButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

