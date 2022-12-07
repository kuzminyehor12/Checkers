
namespace Checkers.Client.Forms
{
    partial class LeaderboardForm
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Nickname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VictoriesCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Points = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Nickname,
            this.VictoriesCount,
            this.Points});
            this.dataGridView1.Location = new System.Drawing.Point(0, 1);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 29;
            this.dataGridView1.Size = new System.Drawing.Size(783, 400);
            this.dataGridView1.TabIndex = 0;
            // 
            // Nickname
            // 
            this.Nickname.HeaderText = "Nickname";
            this.Nickname.MinimumWidth = 6;
            this.Nickname.Name = "Nickname";
            this.Nickname.ReadOnly = true;
            // 
            // VictoriesCount
            // 
            this.VictoriesCount.HeaderText = "Victories Count";
            this.VictoriesCount.MinimumWidth = 6;
            this.VictoriesCount.Name = "VictoriesCount";
            this.VictoriesCount.ReadOnly = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Checkers.Server.Properties.Resources.pngtree_back_vector_icon_png_image_470452_removebg_preview;
            this.pictureBox1.Location = new System.Drawing.Point(0, 399);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(61, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // Points
            // 
            this.Points.HeaderText = "Points";
            this.Points.MinimumWidth = 6;
            this.Points.Name = "Points";
            this.Points.ReadOnly = true;
            // 
            // LeaderboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 445);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "LeaderboardForm";
            this.Text = "LeaderboardForm";
            this.Load += new System.EventHandler(this.LeaderboardForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nickname;
        private System.Windows.Forms.DataGridViewTextBoxColumn VictoriesCount;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Points;
    }
}