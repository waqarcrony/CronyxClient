namespace CronyxApp
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
            TxtEmail = new TextBox();
            label1 = new Label();
            label2 = new Label();
            TxtPass = new TextBox();
            BtnSave = new Button();
            SuspendLayout();
            // 
            // TxtEmail
            // 
            TxtEmail.Font = new Font("Times New Roman", 15.75F);
            TxtEmail.Location = new Point(29, 64);
            TxtEmail.Margin = new Padding(4);
            TxtEmail.MaxLength = 100;
            TxtEmail.Name = "TxtEmail";
            TxtEmail.Size = new Size(381, 32);
            TxtEmail.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 32);
            label1.Name = "label1";
            label1.Size = new Size(107, 19);
            label1.TabIndex = 1;
            label1.Text = "Email/Username";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(27, 126);
            label2.Name = "label2";
            label2.Size = new Size(69, 19);
            label2.TabIndex = 3;
            label2.Text = "Password";
            // 
            // TxtPass
            // 
            TxtPass.Font = new Font("Times New Roman", 15.75F);
            TxtPass.Location = new Point(29, 158);
            TxtPass.Margin = new Padding(4);
            TxtPass.MaxLength = 100;
            TxtPass.Name = "TxtPass";
            TxtPass.Size = new Size(381, 32);
            TxtPass.TabIndex = 2;
            // 
            // BtnSave
            // 
            BtnSave.BackColor = Color.SteelBlue;
            BtnSave.FlatStyle = FlatStyle.Popup;
            BtnSave.Font = new Font("Times New Roman", 14F);
            BtnSave.ForeColor = Color.White;
            BtnSave.Location = new Point(277, 219);
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
            ClientSize = new Size(461, 312);
            Controls.Add(BtnSave);
            Controls.Add(label2);
            Controls.Add(TxtPass);
            Controls.Add(label1);
            Controls.Add(TxtEmail);
            Font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4);
            Name = "FrmLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Cronyx - Login";
            //FormClosed += FrmLogin_FormClosed;
            Load += FrmLogin_Load;
            Shown += FrmLogin_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox TxtEmail;
        private Label label1;
        private Label label2;
        private TextBox TxtPass;
        private Button BtnSave;
    }
}
