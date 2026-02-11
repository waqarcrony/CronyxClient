namespace CronyxApp
{
    partial class Status
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
            label2 = new Label();
            txtToken = new TextBox();
            label3 = new Label();
            cmdActive = new Button();
            button1 = new Button();
            lblAppStatus = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(31, 71);
            label1.Name = "label1";
            label1.Size = new Size(87, 15);
            label1.TabIndex = 1;
            label1.Text = "[Service Status]";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(31, 27);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 2;
            label2.Text = "Token";
            // 
            // txtToken
            // 
            txtToken.Location = new Point(31, 45);
            txtToken.Name = "txtToken";
            txtToken.Size = new Size(740, 23);
            txtToken.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(31, 86);
            label3.Name = "label3";
            label3.Size = new Size(95, 15);
            label3.TabIndex = 4;
            label3.Text = "[Service Process]";
            // 
            // cmdActive
            // 
            cmdActive.Location = new Point(641, 74);
            cmdActive.Name = "cmdActive";
            cmdActive.Size = new Size(130, 37);
            cmdActive.TabIndex = 5;
            cmdActive.Text = "Activate";
            cmdActive.UseVisualStyleBackColor = true;
            cmdActive.Visible = false;
            cmdActive.Click += cmdActive_Click;
            // 
            // button1
            // 
            button1.Location = new Point(505, 75);
            button1.Name = "button1";
            button1.Size = new Size(130, 37);
            button1.TabIndex = 6;
            button1.Text = "Test";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // lblAppStatus
            // 
            lblAppStatus.AutoSize = true;
            lblAppStatus.Location = new Point(684, 27);
            lblAppStatus.Name = "lblAppStatus";
            lblAppStatus.Size = new Size(72, 15);
            lblAppStatus.TabIndex = 7;
            lblAppStatus.Text = "[App Status]";
            // 
            // Status
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblAppStatus);
            Controls.Add(button1);
            Controls.Add(cmdActive);
            Controls.Add(label3);
            Controls.Add(txtToken);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Status";
            StartPosition = FormStartPosition.WindowsDefaultBounds;
            Text = "CronyX  Service Status";
            WindowState = FormWindowState.Minimized;
            Load += Status_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox txtToken;
        private Label label3;
        private Button cmdActive;
        private Button button1;
        private Label lblAppStatus;
    }
}
