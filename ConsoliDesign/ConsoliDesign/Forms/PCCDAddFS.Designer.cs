namespace ConsoliDesign.Forms
{
    partial class PCCDAddFS
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
            this.label3 = new System.Windows.Forms.Label();
            this.combo_FSType = new System.Windows.Forms.ComboBox();
            this.listBox_Functions = new System.Windows.Forms.ListBox();
            this.txtBox_flag = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtbox_Name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label_ID = new System.Windows.Forms.Label();
            this.txtbox_ID = new System.Windows.Forms.TextBox();
            this.Add_surface = new System.Windows.Forms.Button();
            this.btn_save = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.Next = new System.Windows.Forms.Button();
            this.Function = new System.Windows.Forms.GroupBox();
            this.btn_DeletFSs = new System.Windows.Forms.Button();
            this.btn_addfunction = new System.Windows.Forms.Button();
            this.Function.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 97);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "Type of FI";
            // 
            // combo_FSType
            // 
            this.combo_FSType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_FSType.FormattingEnabled = true;
            this.combo_FSType.Items.AddRange(new object[] {
            "Part-environment",
            "Part-user",
            "Part-Part",
            "System Boundary"});
            this.combo_FSType.Location = new System.Drawing.Point(78, 94);
            this.combo_FSType.Margin = new System.Windows.Forms.Padding(2);
            this.combo_FSType.Name = "combo_FSType";
            this.combo_FSType.Size = new System.Drawing.Size(156, 21);
            this.combo_FSType.TabIndex = 27;
            // 
            // listBox_Functions
            // 
            this.listBox_Functions.FormattingEnabled = true;
            this.listBox_Functions.Location = new System.Drawing.Point(4, 16);
            this.listBox_Functions.Margin = new System.Windows.Forms.Padding(2);
            this.listBox_Functions.Name = "listBox_Functions";
            this.listBox_Functions.Size = new System.Drawing.Size(155, 43);
            this.listBox_Functions.TabIndex = 25;
            // 
            // txtBox_flag
            // 
            this.txtBox_flag.Location = new System.Drawing.Point(101, 68);
            this.txtBox_flag.Margin = new System.Windows.Forms.Padding(2);
            this.txtBox_flag.Name = "txtBox_flag";
            this.txtBox_flag.ReadOnly = true;
            this.txtBox_flag.Size = new System.Drawing.Size(47, 20);
            this.txtBox_flag.TabIndex = 24;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 71);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Selection Status";
            // 
            // txtbox_Name
            // 
            this.txtbox_Name.Location = new System.Drawing.Point(52, 35);
            this.txtbox_Name.Margin = new System.Windows.Forms.Padding(2);
            this.txtbox_Name.Name = "txtbox_Name";
            this.txtbox_Name.Size = new System.Drawing.Size(183, 20);
            this.txtbox_Name.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Name";
            // 
            // label_ID
            // 
            this.label_ID.AutoSize = true;
            this.label_ID.Location = new System.Drawing.Point(13, 14);
            this.label_ID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_ID.Name = "label_ID";
            this.label_ID.Size = new System.Drawing.Size(18, 13);
            this.label_ID.TabIndex = 20;
            this.label_ID.Text = "ID";
            // 
            // txtbox_ID
            // 
            this.txtbox_ID.Location = new System.Drawing.Point(52, 11);
            this.txtbox_ID.Margin = new System.Windows.Forms.Padding(2);
            this.txtbox_ID.Name = "txtbox_ID";
            this.txtbox_ID.ReadOnly = true;
            this.txtbox_ID.Size = new System.Drawing.Size(183, 20);
            this.txtbox_ID.TabIndex = 19;
            // 
            // Add_surface
            // 
            this.Add_surface.Location = new System.Drawing.Point(179, 68);
            this.Add_surface.Margin = new System.Windows.Forms.Padding(2);
            this.Add_surface.Name = "Add_surface";
            this.Add_surface.Size = new System.Drawing.Size(55, 19);
            this.Add_surface.TabIndex = 18;
            this.Add_surface.Text = "Select Surface";
            this.Add_surface.UseVisualStyleBackColor = true;
            this.Add_surface.Click += new System.EventHandler(this.Add_surface_Click);
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(91, 198);
            this.btn_save.Margin = new System.Windows.Forms.Padding(2);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(46, 19);
            this.btn_save.TabIndex = 17;
            this.btn_save.Text = "Save";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(179, 198);
            this.Cancel.Margin = new System.Windows.Forms.Padding(2);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(56, 19);
            this.Cancel.TabIndex = 16;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Next
            // 
            this.Next.Location = new System.Drawing.Point(137, 198);
            this.Next.Margin = new System.Windows.Forms.Padding(2);
            this.Next.Name = "Next";
            this.Next.Size = new System.Drawing.Size(46, 19);
            this.Next.TabIndex = 15;
            this.Next.Text = "Next";
            this.Next.UseVisualStyleBackColor = true;
            this.Next.Click += new System.EventHandler(this.Next_Click);
            // 
            // Function
            // 
            this.Function.Controls.Add(this.btn_DeletFSs);
            this.Function.Controls.Add(this.btn_addfunction);
            this.Function.Controls.Add(this.listBox_Functions);
            this.Function.Location = new System.Drawing.Point(13, 123);
            this.Function.Margin = new System.Windows.Forms.Padding(2);
            this.Function.Name = "Function";
            this.Function.Padding = new System.Windows.Forms.Padding(2);
            this.Function.Size = new System.Drawing.Size(221, 71);
            this.Function.TabIndex = 26;
            this.Function.TabStop = false;
            this.Function.Text = "Functional info";
            // 
            // btn_DeletFSs
            // 
            this.btn_DeletFSs.Location = new System.Drawing.Point(160, 37);
            this.btn_DeletFSs.Margin = new System.Windows.Forms.Padding(2);
            this.btn_DeletFSs.Name = "btn_DeletFSs";
            this.btn_DeletFSs.Size = new System.Drawing.Size(56, 19);
            this.btn_DeletFSs.TabIndex = 15;
            this.btn_DeletFSs.Text = "Delete";
            this.btn_DeletFSs.UseVisualStyleBackColor = true;
            this.btn_DeletFSs.Click += new System.EventHandler(this.btn_DeletFSs_Click);
            // 
            // btn_addfunction
            // 
            this.btn_addfunction.Location = new System.Drawing.Point(160, 14);
            this.btn_addfunction.Margin = new System.Windows.Forms.Padding(2);
            this.btn_addfunction.Name = "btn_addfunction";
            this.btn_addfunction.Size = new System.Drawing.Size(56, 19);
            this.btn_addfunction.TabIndex = 11;
            this.btn_addfunction.Text = "Add function";
            this.btn_addfunction.UseVisualStyleBackColor = true;
            this.btn_addfunction.Click += new System.EventHandler(this.btn_addfunction_Click);
            // 
            // PCCDAddFS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(248, 226);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.combo_FSType);
            this.Controls.Add(this.txtBox_flag);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtbox_Name);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_ID);
            this.Controls.Add(this.txtbox_ID);
            this.Controls.Add(this.Add_surface);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Next);
            this.Controls.Add(this.Function);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "PCCDAddFS";
            this.Text = "FunctionalInterf Editor";
            this.Function.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox combo_FSType;
        private System.Windows.Forms.ListBox listBox_Functions;
        private System.Windows.Forms.TextBox txtBox_flag;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtbox_Name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_ID;
        private System.Windows.Forms.TextBox txtbox_ID;
        private System.Windows.Forms.Button Add_surface;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button Next;
        private System.Windows.Forms.GroupBox Function;
        private System.Windows.Forms.Button btn_DeletFSs;
        private System.Windows.Forms.Button btn_addfunction;
    }
}