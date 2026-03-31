namespace CronyxApp
{
    partial class FrmSelectStore
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
            label1 = new Label();
            BtnSave = new Button();
            CmbStore = new ComboBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(27, 21);
            label1.Name = "label1";
            label1.Size = new Size(57, 19);
            label1.TabIndex = 1;
            label1.Text = "Stores:";
            // 
            // BtnSave
            // 
            BtnSave.BackColor = Color.SteelBlue;
            BtnSave.FlatStyle = FlatStyle.Popup;
            BtnSave.Font = new Font("Times New Roman", 14F);
            BtnSave.ForeColor = Color.White;
            BtnSave.Location = new Point(277, 123);
            BtnSave.Name = "BtnSave";
            BtnSave.Size = new Size(133, 46);
            BtnSave.TabIndex = 4;
            BtnSave.Text = "Proceed";
            BtnSave.UseVisualStyleBackColor = false;
            BtnSave.Click += BtnSave_Click;
            // 
            // CmbStore
            // 
            CmbStore.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbStore.Font = new Font("Times New Roman", 15.75F);
            CmbStore.FormattingEnabled = true;
            CmbStore.Location = new Point(27, 54);
            CmbStore.Name = "CmbStore";
            CmbStore.Size = new Size(383, 31);
            CmbStore.TabIndex = 5;
            // 
            // FrmSelectStore
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(461, 194);
            Controls.Add(CmbStore);
            Controls.Add(BtnSave);
            Controls.Add(label1);
            Font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4);
            Name = "FrmSelectStore";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Cronyx - Login";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Button BtnSave;
        private ComboBox CmbStore;
    }
}
