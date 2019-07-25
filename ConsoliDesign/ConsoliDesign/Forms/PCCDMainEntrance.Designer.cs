namespace ConsoliDesign.Forms
{
    partial class PCCDMainEntrance
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PCCDMainEntrance));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openCADFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.startANewSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editAnExistingSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pCCDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findFeasibleGroupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findSecondaryGroupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.partConsolidationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redesignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optimizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topologyOptimizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.latticeInfillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(131, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 29);
            this.label1.TabIndex = 3;
            this.label1.Text = "Welcome";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(26, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(348, 154);
            this.label2.TabIndex = 4;
            this.label2.Text = resources.GetString("label2.Text");
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 277);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Copyright @ADML";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(238, 276);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(135, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Developed by Sheng Yang";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.pCCDToolStripMenuItem,
            this.partConsolidationToolStripMenuItem,
            this.optimizationToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(394, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openCADFileToolStripMenuItem,
            this.openXMLToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "File";
            // 
            // openCADFileToolStripMenuItem
            // 
            this.openCADFileToolStripMenuItem.Name = "openCADFileToolStripMenuItem";
            this.openCADFileToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.openCADFileToolStripMenuItem.Text = "OpenCADFile";
            this.openCADFileToolStripMenuItem.Click += new System.EventHandler(this.openCADFileToolStripMenuItem_Click);
            // 
            // openXMLToolStripMenuItem
            // 
            this.openXMLToolStripMenuItem.Name = "openXMLToolStripMenuItem";
            this.openXMLToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.openXMLToolStripMenuItem.Text = "OpenXML";
            this.openXMLToolStripMenuItem.Click += new System.EventHandler(this.openXMLToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startANewSessionToolStripMenuItem,
            this.editAnExistingSessionToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(72, 20);
            this.toolStripMenuItem2.Text = "Info setup";
            // 
            // startANewSessionToolStripMenuItem
            // 
            this.startANewSessionToolStripMenuItem.Name = "startANewSessionToolStripMenuItem";
            this.startANewSessionToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.startANewSessionToolStripMenuItem.Text = "Start a new session";
            this.startANewSessionToolStripMenuItem.Click += new System.EventHandler(this.startANewSessionToolStripMenuItem_Click);
            // 
            // editAnExistingSessionToolStripMenuItem
            // 
            this.editAnExistingSessionToolStripMenuItem.Name = "editAnExistingSessionToolStripMenuItem";
            this.editAnExistingSessionToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.editAnExistingSessionToolStripMenuItem.Text = "Edit an existing session";
            this.editAnExistingSessionToolStripMenuItem.Click += new System.EventHandler(this.editAnExistingSessionToolStripMenuItem_Click_1);
            // 
            // pCCDToolStripMenuItem
            // 
            this.pCCDToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findFeasibleGroupsToolStripMenuItem,
            this.findSecondaryGroupsToolStripMenuItem});
            this.pCCDToolStripMenuItem.Name = "pCCDToolStripMenuItem";
            this.pCCDToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.pCCDToolStripMenuItem.Text = "Screening";
            // 
            // findFeasibleGroupsToolStripMenuItem
            // 
            this.findFeasibleGroupsToolStripMenuItem.Name = "findFeasibleGroupsToolStripMenuItem";
            this.findFeasibleGroupsToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.findFeasibleGroupsToolStripMenuItem.Text = "Find primary groups";
            this.findFeasibleGroupsToolStripMenuItem.Click += new System.EventHandler(this.findFeasibleGroupsToolStripMenuItem_Click);
            // 
            // findSecondaryGroupsToolStripMenuItem
            // 
            this.findSecondaryGroupsToolStripMenuItem.Name = "findSecondaryGroupsToolStripMenuItem";
            this.findSecondaryGroupsToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.findSecondaryGroupsToolStripMenuItem.Text = "Find secondary groups";
            // 
            // partConsolidationToolStripMenuItem
            // 
            this.partConsolidationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.redesignToolStripMenuItem});
            this.partConsolidationToolStripMenuItem.Name = "partConsolidationToolStripMenuItem";
            this.partConsolidationToolStripMenuItem.Size = new System.Drawing.Size(88, 20);
            this.partConsolidationToolStripMenuItem.Text = "Embodiment";
            // 
            // redesignToolStripMenuItem
            // 
            this.redesignToolStripMenuItem.Name = "redesignToolStripMenuItem";
            this.redesignToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.redesignToolStripMenuItem.Text = "Redesign";
            this.redesignToolStripMenuItem.Click += new System.EventHandler(this.redesignToolStripMenuItem_Click);
            // 
            // optimizationToolStripMenuItem
            // 
            this.optimizationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.topologyOptimizationToolStripMenuItem,
            this.latticeInfillToolStripMenuItem});
            this.optimizationToolStripMenuItem.Name = "optimizationToolStripMenuItem";
            this.optimizationToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.optimizationToolStripMenuItem.Text = "Refinement";
            // 
            // topologyOptimizationToolStripMenuItem
            // 
            this.topologyOptimizationToolStripMenuItem.Name = "topologyOptimizationToolStripMenuItem";
            this.topologyOptimizationToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.topologyOptimizationToolStripMenuItem.Text = "Topology optimization";
            // 
            // latticeInfillToolStripMenuItem
            // 
            this.latticeInfillToolStripMenuItem.Name = "latticeInfillToolStripMenuItem";
            this.latticeInfillToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.latticeInfillToolStripMenuItem.Text = "IntraLattice";
            // 
            // PCCDMainEntrance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 299);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PCCDMainEntrance";
            this.Text = "ConsolidDesign assistant";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openCADFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem editAnExistingSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startANewSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pCCDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findFeasibleGroupsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem partConsolidationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redesignToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optimizationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem latticeInfillToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findSecondaryGroupsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topologyOptimizationToolStripMenuItem;

    }
}