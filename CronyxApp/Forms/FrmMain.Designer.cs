namespace CronyxApp
{
    partial class FrmMain
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
            LstDevices = new ListView();
            SuspendLayout();
            // 
            // LstDevices
            // 
            LstDevices.Location = new Point(69, 68);
            LstDevices.Name = "LstDevices";
            LstDevices.Size = new Size(1189, 450);
            LstDevices.TabIndex = 0;
            LstDevices.UseCompatibleStateImageBehavior = false;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1351, 590);
            Controls.Add(LstDevices);
            Font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4);
            Name = "FrmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Main";
            Shown += FrmMain_Shown;
            ResumeLayout(false);
        }

        #endregion

        private ListView LstDevices;
    }
}
