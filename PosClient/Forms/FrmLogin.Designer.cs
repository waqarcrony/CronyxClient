namespace PosClient
{
    partial class FrmLogin
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            TxtLicense = new TextBox();
            label1 = new Label();
            label2 = new Label();
            TxtMacAddr = new TextBox();
            BtnSave = new Button();
            SuspendLayout();
            // 
            // TxtLicense
            // 
            TxtLicense.Font = new Font("Times New Roman", 15.75F, FontStyle.Bold);
            TxtLicense.Location = new Point(29, 62);
            TxtLicense.Margin = new Padding(4);
            TxtLicense.MaxLength = 100;
            TxtLicense.Name = "TxtLicense";
            TxtLicense.Size = new Size(381, 32);
            TxtLicense.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 30);
            label1.Name = "label1";
            label1.Size = new Size(112, 19);
            label1.TabIndex = 1;
            label1.Text = "License Number:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(27, 124);
            label2.Name = "label2";
            label2.Size = new Size(94, 19);
            label2.TabIndex = 3;
            label2.Text = "Mac Address:";
            // 
            // TxtMacAddr
            // 
            TxtMacAddr.Enabled = false;
            TxtMacAddr.Font = new Font("Times New Roman", 15.75F, FontStyle.Bold);
            TxtMacAddr.Location = new Point(29, 156);
            TxtMacAddr.Margin = new Padding(4);
            TxtMacAddr.MaxLength = 100;
            TxtMacAddr.Name = "TxtMacAddr";
            TxtMacAddr.Size = new Size(381, 32);
            TxtMacAddr.TabIndex = 2;
            // 
            // BtnSave
            // 
            BtnSave.BackColor = Color.SteelBlue;
            BtnSave.FlatStyle = FlatStyle.Popup;
            BtnSave.Font = new Font("Times New Roman", 14F);
            BtnSave.ForeColor = Color.White;
            BtnSave.Location = new Point(277, 217);
            BtnSave.Name = "BtnSave";
            BtnSave.Size = new Size(133, 46);
            BtnSave.TabIndex = 4;
            BtnSave.Text = "Authenticate";
            BtnSave.UseVisualStyleBackColor = false;
            BtnSave.Click += BtnSave_Click;
            // 
            // FrmLogin
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(462, 296);
            Controls.Add(BtnSave);
            Controls.Add(label2);
            Controls.Add(TxtMacAddr);
            Controls.Add(label1);
            Controls.Add(TxtLicense);
            Font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4);
            Name = "FrmLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            Shown += FrmLogin_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox TxtLicense;
        private Label label1;
        private Label label2;
        private TextBox TxtMacAddr;
        private Button BtnSave;
    }
}
