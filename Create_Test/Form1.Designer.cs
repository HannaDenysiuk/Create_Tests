
namespace Create_Test
{
    partial class FormTests
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
            this.add = new System.Windows.Forms.Button();
            this.edit = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Honeydew;
            this.groupBox1.Controls.Add(this.edit);
            this.groupBox1.Controls.Add(this.add);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(47, 49);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(612, 221);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Test";
            // 
            // add
            // 
            this.add.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.add.ForeColor = System.Drawing.Color.Green;
            this.add.Location = new System.Drawing.Point(23, 99);
            this.add.Name = "add";
            this.add.Size = new System.Drawing.Size(242, 71);
            this.add.TabIndex = 0;
            this.add.Text = "add new test";
            this.add.UseVisualStyleBackColor = false;
            this.add.Click += new System.EventHandler(this.add_Click);
            // 
            // edit
            // 
            this.edit.BackColor = System.Drawing.Color.LightSteelBlue;
            this.edit.ForeColor = System.Drawing.Color.Navy;
            this.edit.Location = new System.Drawing.Point(328, 99);
            this.edit.Name = "edit";
            this.edit.Size = new System.Drawing.Size(242, 71);
            this.edit.TabIndex = 1;
            this.edit.Text = "edit test";
            this.edit.UseVisualStyleBackColor = false;
            this.edit.Click += new System.EventHandler(this.edit_Click);
            // 
            // FormTests
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.ClientSize = new System.Drawing.Size(713, 336);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormTests";
            this.Text = "FormTests";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button edit;
        private System.Windows.Forms.Button add;
    }
}

