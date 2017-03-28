namespace DistributedDesktopApp
{
    partial class Main
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
            this.manageConnection = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.connectionMessage = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // manageConnection
            // 
            this.manageConnection.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.manageConnection.BackColor = System.Drawing.Color.ForestGreen;
            this.manageConnection.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.manageConnection.ForeColor = System.Drawing.Color.White;
            this.manageConnection.Location = new System.Drawing.Point(243, 478);
            this.manageConnection.Name = "manageConnection";
            this.manageConnection.Size = new System.Drawing.Size(306, 50);
            this.manageConnection.TabIndex = 0;
            this.manageConnection.Text = "Connect to QuickBooks";
            this.manageConnection.UseVisualStyleBackColor = false;
            this.manageConnection.Click += new System.EventHandler(this.manageConnection_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 34);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(767, 438);
            this.dataGridView1.TabIndex = 1;
            // 
            // connectionMessage
            // 
            this.connectionMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionMessage.BackColor = System.Drawing.SystemColors.Info;
            this.connectionMessage.Location = new System.Drawing.Point(12, 8);
            this.connectionMessage.Name = "connectionMessage";
            this.connectionMessage.Size = new System.Drawing.Size(767, 20);
            this.connectionMessage.TabIndex = 3;
            this.connectionMessage.Text = "You\'re not connected!  Use the Connect to QuickBooks button below .";
            this.connectionMessage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 537);
            this.Controls.Add(this.connectionMessage);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.manageConnection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QuickBooks Customer Viewer";
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button manageConnection;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox connectionMessage;
    }
}