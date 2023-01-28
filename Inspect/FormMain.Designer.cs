namespace Inspect
{
    partial class FormMain
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.labelRecipeCurrent = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelSN = new System.Windows.Forms.Label();
            this.labelPanelCode = new System.Windows.Forms.Label();
            this.labelRecipeInsp = new System.Windows.Forms.Label();
            this.labelPath = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            //this.algoServViews1 = new Inspect.Views.AlgoServView();
            //this.algoServViews2 = new Inspect.Views.AlgoServView();
            //this.algoServViews3 = new Inspect.Views.AlgoServView();
            //this.algoServViews4 = new Inspect.Views.AlgoServView();
            //this.algoServViews5 = new Inspect.Views.AlgoServView();
            //this.algoServViews6 = new Inspect.Views.AlgoServView();
            //this.algoServViews7 = new Inspect.Views.AlgoServView();
            //this.algoServViews8 = new Inspect.Views.AlgoServView();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelRecipeCurrent, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelSN, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelPanelCode, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelRecipeInsp, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelPath, 4, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(526, 80);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "当前配方";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelRecipeCurrent
            // 
            this.labelRecipeCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRecipeCurrent.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelRecipeCurrent.Location = new System.Drawing.Point(62, 6);
            this.labelRecipeCurrent.Margin = new System.Windows.Forms.Padding(3);
            this.labelRecipeCurrent.Name = "labelRecipeCurrent";
            this.labelRecipeCurrent.Size = new System.Drawing.Size(110, 27);
            this.labelRecipeCurrent.TabIndex = 1;
            this.labelRecipeCurrent.Text = "-1";
            this.labelRecipeCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 54);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "检测请求";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelSN
            // 
            this.labelSN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSN.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelSN.Location = new System.Drawing.Point(62, 46);
            this.labelSN.Margin = new System.Windows.Forms.Padding(3);
            this.labelSN.Name = "labelSN";
            this.labelSN.Size = new System.Drawing.Size(110, 27);
            this.labelSN.TabIndex = 1;
            this.labelSN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPanelCode
            // 
            this.labelPanelCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPanelCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelPanelCode.Location = new System.Drawing.Point(178, 46);
            this.labelPanelCode.Margin = new System.Windows.Forms.Padding(3);
            this.labelPanelCode.Name = "labelPanelCode";
            this.labelPanelCode.Size = new System.Drawing.Size(110, 27);
            this.labelPanelCode.TabIndex = 1;
            this.labelPanelCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelRecipeInsp
            // 
            this.labelRecipeInsp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRecipeInsp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelRecipeInsp.Location = new System.Drawing.Point(294, 46);
            this.labelRecipeInsp.Margin = new System.Windows.Forms.Padding(3);
            this.labelRecipeInsp.Name = "labelRecipeInsp";
            this.labelRecipeInsp.Size = new System.Drawing.Size(110, 27);
            this.labelRecipeInsp.TabIndex = 1;
            this.labelRecipeInsp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPath
            // 
            this.labelPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPath.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelPath.Location = new System.Drawing.Point(410, 46);
            this.labelPath.Margin = new System.Windows.Forms.Padding(3);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(113, 27);
            this.labelPath.TabIndex = 1;
            this.labelPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            //this.tableLayoutPanel2.Controls.Add(this.algoServViews1, 0, 0);
            //this.tableLayoutPanel2.Controls.Add(this.algoServViews2, 0, 1);
            //this.tableLayoutPanel2.Controls.Add(this.algoServViews3, 0, 2);
            //this.tableLayoutPanel2.Controls.Add(this.algoServViews4, 0, 3);
            //this.tableLayoutPanel2.Controls.Add(this.algoServViews5, 0, 4);
            //this.tableLayoutPanel2.Controls.Add(this.algoServViews6, 0, 5);
            //this.tableLayoutPanel2.Controls.Add(this.algoServViews7, 0, 6);
            //this.tableLayoutPanel2.Controls.Add(this.algoServViews8, 0, 7);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 98);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 8;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(526, 343);
            this.tableLayoutPanel2.TabIndex = 1;
            //// 
            //// algoServViews1
            //// 
            //this.algoServViews1._Name = "label1";
            //this.algoServViews1._Port = "label1";
            //this.algoServViews1._Status = false;
            //this.algoServViews1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            //this.algoServViews1.Location = new System.Drawing.Point(3, 3);
            //this.algoServViews1.Name = "algoServViews1";
            //this.algoServViews1.Size = new System.Drawing.Size(520, 36);
            //this.algoServViews1.TabIndex = 0;
            //// 
            //// algoServViews2
            //// 
            //this.algoServViews2._Name = "label1";
            //this.algoServViews2._Port = "label1";
            //this.algoServViews2._Status = false;
            //this.algoServViews2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            //this.algoServViews2.Location = new System.Drawing.Point(3, 45);
            //this.algoServViews2.Name = "algoServViews2";
            //this.algoServViews2.Size = new System.Drawing.Size(520, 36);
            //this.algoServViews2.TabIndex = 0;
            //// 
            //// algoServViews3
            //// 
            //this.algoServViews3._Name = "label1";
            //this.algoServViews3._Port = "label1";
            //this.algoServViews3._Status = false;
            //this.algoServViews3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            //this.algoServViews3.Location = new System.Drawing.Point(3, 87);
            //this.algoServViews3.Name = "algoServViews3";
            //this.algoServViews3.Size = new System.Drawing.Size(520, 36);
            //this.algoServViews3.TabIndex = 0;
            //// 
            //// algoServViews4
            //// 
            //this.algoServViews4._Name = "label1";
            //this.algoServViews4._Port = "label1";
            //this.algoServViews4._Status = false;
            //this.algoServViews4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            //this.algoServViews4.Location = new System.Drawing.Point(3, 129);
            //this.algoServViews4.Name = "algoServViews4";
            //this.algoServViews4.Size = new System.Drawing.Size(520, 36);
            //this.algoServViews4.TabIndex = 0;
            //// 
            //// algoServViews5
            //// 
            //this.algoServViews5._Name = "label1";
            //this.algoServViews5._Port = "label1";
            //this.algoServViews5._Status = false;
            //this.algoServViews5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            //this.algoServViews5.Location = new System.Drawing.Point(3, 171);
            //this.algoServViews5.Name = "algoServViews5";
            //this.algoServViews5.Size = new System.Drawing.Size(520, 36);
            //this.algoServViews5.TabIndex = 0;
            //// 
            //// algoServViews6
            //// 
            //this.algoServViews6._Name = "label1";
            //this.algoServViews6._Port = "label1";
            //this.algoServViews6._Status = false;
            //this.algoServViews6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            //this.algoServViews6.Location = new System.Drawing.Point(3, 213);
            //this.algoServViews6.Name = "algoServViews6";
            //this.algoServViews6.Size = new System.Drawing.Size(520, 36);
            //this.algoServViews6.TabIndex = 0;
            //// 
            //// algoServViews7
            //// 
            //this.algoServViews7._Name = "label1";
            //this.algoServViews7._Port = "label1";
            //this.algoServViews7._Status = false;
            //this.algoServViews7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            //this.algoServViews7.Location = new System.Drawing.Point(3, 255);
            //this.algoServViews7.Name = "algoServViews7";
            //this.algoServViews7.Size = new System.Drawing.Size(520, 36);
            //this.algoServViews7.TabIndex = 0;
            //// 
            //// algoServViews8
            //// 
            //this.algoServViews8._Name = "label1";
            //this.algoServViews8._Port = "label1";
            //this.algoServViews8._Status = false;
            //this.algoServViews8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            //this.algoServViews8.Location = new System.Drawing.Point(3, 300);
            //this.algoServViews8.Name = "algoServViews8";
            //this.algoServViews8.Size = new System.Drawing.Size(520, 36);
            //this.algoServViews8.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 453);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormMain";
            this.Text = "检测端";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelRecipeCurrent;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelSN;
        private System.Windows.Forms.Label labelPanelCode;
        private System.Windows.Forms.Label labelRecipeInsp;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private Views.AlgoServView algoServViews1;
        private Views.AlgoServView algoServViews2;
        private Views.AlgoServView algoServViews3;
        private Views.AlgoServView algoServViews4;
        private Views.AlgoServView algoServViews5;
        private Views.AlgoServView algoServViews6;
        private Views.AlgoServView algoServViews7;
        private Views.AlgoServView algoServViews8;
    }
}

