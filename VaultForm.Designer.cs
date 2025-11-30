namespace PasswordManagerApplication
{
    partial class VaultForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VaultForm));
            this.lstPasswords = new System.Windows.Forms.ListBox();
            this.txtSite = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.Site = new System.Windows.Forms.Label();
            this.Password = new System.Windows.Forms.Label();
            this.btnToggleVisibility = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.btnEdit = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSortOptions = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStarSelected = new System.Windows.Forms.Button();
            this.btnShowFavorites = new System.Windows.Forms.Button();
            this.lstCategories = new System.Windows.Forms.ListBox();
            this.btnDeleteCategory = new System.Windows.Forms.Button();
            this.btnAssignCategory = new System.Windows.Forms.Button();
            this.btnFilterByCategory = new System.Windows.Forms.Button();
            this.btnShowAll = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.btnRemoveCategoryFromPassword = new System.Windows.Forms.Button();
            this.btnAddCategoryPopup = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.pnlSidebar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lstPasswords
            // 
            this.lstPasswords.Font = new System.Drawing.Font("Arial Narrow", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstPasswords.FormattingEnabled = true;
            this.lstPasswords.ItemHeight = 31;
            this.lstPasswords.Location = new System.Drawing.Point(659, 230);
            this.lstPasswords.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.lstPasswords.Name = "lstPasswords";
            this.lstPasswords.Size = new System.Drawing.Size(531, 159);
            this.lstPasswords.TabIndex = 0;
            // 
            // txtSite
            // 
            this.txtSite.Font = new System.Drawing.Font("Arial Narrow", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSite.Location = new System.Drawing.Point(887, 437);
            this.txtSite.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.txtSite.Name = "txtSite";
            this.txtSite.Size = new System.Drawing.Size(303, 38);
            this.txtSite.TabIndex = 1;
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Arial Narrow", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(885, 502);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(305, 38);
            this.txtPassword.TabIndex = 2;
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("Arial Narrow", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.Location = new System.Drawing.Point(963, 567);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(227, 50);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add new password";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(1311, 427);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(227, 50);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // Site
            // 
            this.Site.AutoSize = true;
            this.Site.BackColor = System.Drawing.Color.Transparent;
            this.Site.Font = new System.Drawing.Font("Arial Narrow", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Site.ForeColor = System.Drawing.Color.White;
            this.Site.Location = new System.Drawing.Point(653, 437);
            this.Site.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Site.Name = "Site";
            this.Site.Size = new System.Drawing.Size(213, 33);
            this.Site.TabIndex = 5;
            this.Site.Text = "Website/application:";
            // 
            // Password
            // 
            this.Password.AutoSize = true;
            this.Password.BackColor = System.Drawing.Color.Transparent;
            this.Password.ForeColor = System.Drawing.Color.White;
            this.Password.Location = new System.Drawing.Point(653, 502);
            this.Password.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(130, 35);
            this.Password.TabIndex = 6;
            this.Password.Text = "Password:";
            // 
            // btnToggleVisibility
            // 
            this.btnToggleVisibility.Font = new System.Drawing.Font("Arial Narrow", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnToggleVisibility.Location = new System.Drawing.Point(1311, 288);
            this.btnToggleVisibility.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.btnToggleVisibility.Name = "btnToggleVisibility";
            this.btnToggleVisibility.Size = new System.Drawing.Size(227, 50);
            this.btnToggleVisibility.TabIndex = 7;
            this.btnToggleVisibility.Text = "Show/Hide";
            this.btnToggleVisibility.UseVisualStyleBackColor = true;
            this.btnToggleVisibility.Click += new System.EventHandler(this.btnToggleVisibility_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Font = new System.Drawing.Font("Arial Narrow", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopy.Location = new System.Drawing.Point(1311, 352);
            this.btnCopy.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(227, 50);
            this.btnCopy.TabIndex = 8;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(1311, 58);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(227, 50);
            this.btnLogout.TabIndex = 9;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Font = new System.Drawing.Font("Arial Narrow", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerate.Location = new System.Drawing.Point(659, 567);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(227, 50);
            this.btnGenerate.TabIndex = 11;
            this.btnGenerate.Text = "Generate password";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Arial Narrow", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(766, 168);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(293, 38);
            this.txtSearch.TabIndex = 13;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.BackColor = System.Drawing.Color.Transparent;
            this.lblSearch.Font = new System.Drawing.Font("Arial Narrow", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearch.ForeColor = System.Drawing.Color.White;
            this.lblSearch.Location = new System.Drawing.Point(653, 168);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(93, 33);
            this.lblSearch.TabIndex = 14;
            this.lblSearch.Text = "Search:";
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(1311, 494);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(227, 50);
            this.btnEdit.TabIndex = 16;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(653, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(527, 94);
            this.label2.TabIndex = 17;
            this.label2.Text = "Passwords Vault";
            // 
            // cmbSortOptions
            // 
            this.cmbSortOptions.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSortOptions.FormattingEnabled = true;
            this.cmbSortOptions.Items.AddRange(new object[] {
            "Alphabetical (A–Z)",
            "Alphabetical (Z–A)",
            "Password Length (Shortest → Longest)",
            "Password Length (Longest → Shortest)"});
            this.cmbSortOptions.Location = new System.Drawing.Point(0, 725);
            this.cmbSortOptions.Name = "cmbSortOptions";
            this.cmbSortOptions.Size = new System.Drawing.Size(247, 35);
            this.cmbSortOptions.TabIndex = 20;
            this.cmbSortOptions.SelectedIndexChanged += new System.EventHandler(this.cmbSortOptions_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(-8, 695);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 27);
            this.label3.TabIndex = 21;
            this.label3.Text = "Sort vault by:";
            // 
            // btnStarSelected
            // 
            this.btnStarSelected.BackColor = System.Drawing.Color.White;
            this.btnStarSelected.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStarSelected.Location = new System.Drawing.Point(0, 153);
            this.btnStarSelected.Name = "btnStarSelected";
            this.btnStarSelected.Size = new System.Drawing.Size(253, 71);
            this.btnStarSelected.TabIndex = 22;
            this.btnStarSelected.Text = "Favorite / Unfavorite password";
            this.btnStarSelected.UseVisualStyleBackColor = false;
            this.btnStarSelected.Click += new System.EventHandler(this.btnStarSelected_Click);
            // 
            // btnShowFavorites
            // 
            this.btnShowFavorites.BackColor = System.Drawing.Color.White;
            this.btnShowFavorites.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowFavorites.Location = new System.Drawing.Point(0, 221);
            this.btnShowFavorites.Name = "btnShowFavorites";
            this.btnShowFavorites.Size = new System.Drawing.Size(253, 71);
            this.btnShowFavorites.TabIndex = 23;
            this.btnShowFavorites.Text = "Show favorites";
            this.btnShowFavorites.UseVisualStyleBackColor = false;
            this.btnShowFavorites.Click += new System.EventHandler(this.btnShowFavorites_Click);
            // 
            // lstCategories
            // 
            this.lstCategories.Font = new System.Drawing.Font("Arial Narrow", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstCategories.FormattingEnabled = true;
            this.lstCategories.ItemHeight = 31;
            this.lstCategories.Location = new System.Drawing.Point(269, 610);
            this.lstCategories.Name = "lstCategories";
            this.lstCategories.Size = new System.Drawing.Size(292, 128);
            this.lstCategories.TabIndex = 24;
            // 
            // btnDeleteCategory
            // 
            this.btnDeleteCategory.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteCategory.Location = new System.Drawing.Point(0, 425);
            this.btnDeleteCategory.Name = "btnDeleteCategory";
            this.btnDeleteCategory.Size = new System.Drawing.Size(253, 71);
            this.btnDeleteCategory.TabIndex = 27;
            this.btnDeleteCategory.Text = "Delete category";
            this.btnDeleteCategory.UseVisualStyleBackColor = true;
            this.btnDeleteCategory.Click += new System.EventHandler(this.btnDeleteCategory_Click);
            // 
            // btnAssignCategory
            // 
            this.btnAssignCategory.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAssignCategory.Location = new System.Drawing.Point(-3, 559);
            this.btnAssignCategory.Name = "btnAssignCategory";
            this.btnAssignCategory.Size = new System.Drawing.Size(253, 71);
            this.btnAssignCategory.TabIndex = 28;
            this.btnAssignCategory.Text = "Assign category to password";
            this.btnAssignCategory.UseVisualStyleBackColor = true;
            this.btnAssignCategory.Click += new System.EventHandler(this.btnAssignCategory_Click);
            // 
            // btnFilterByCategory
            // 
            this.btnFilterByCategory.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilterByCategory.Location = new System.Drawing.Point(0, 494);
            this.btnFilterByCategory.Name = "btnFilterByCategory";
            this.btnFilterByCategory.Size = new System.Drawing.Size(253, 71);
            this.btnFilterByCategory.TabIndex = 29;
            this.btnFilterByCategory.Text = "Filter vault by category\r\n";
            this.btnFilterByCategory.UseVisualStyleBackColor = true;
            this.btnFilterByCategory.Click += new System.EventHandler(this.btnFilterByCategory_Click);
            // 
            // btnShowAll
            // 
            this.btnShowAll.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowAll.Location = new System.Drawing.Point(-3, 288);
            this.btnShowAll.Name = "btnShowAll";
            this.btnShowAll.Size = new System.Drawing.Size(253, 71);
            this.btnShowAll.TabIndex = 30;
            this.btnShowAll.Text = "Show all passwords";
            this.btnShowAll.UseVisualStyleBackColor = true;
            this.btnShowAll.Click += new System.EventHandler(this.btnShowAll_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Arial Narrow", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(286, 571);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(230, 33);
            this.label4.TabIndex = 31;
            this.label4.Text = "Password categories:";
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.White;
            this.pnlSidebar.Controls.Add(this.btnRemoveCategoryFromPassword);
            this.pnlSidebar.Controls.Add(this.btnDeleteCategory);
            this.pnlSidebar.Controls.Add(this.btnFilterByCategory);
            this.pnlSidebar.Controls.Add(this.btnAssignCategory);
            this.pnlSidebar.Controls.Add(this.btnAddCategoryPopup);
            this.pnlSidebar.Controls.Add(this.pictureBox2);
            this.pnlSidebar.Controls.Add(this.cmbSortOptions);
            this.pnlSidebar.Controls.Add(this.label3);
            this.pnlSidebar.Controls.Add(this.btnShowAll);
            this.pnlSidebar.Controls.Add(this.btnStarSelected);
            this.pnlSidebar.Controls.Add(this.btnShowFavorites);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(250, 1051);
            this.pnlSidebar.TabIndex = 33;
            // 
            // btnRemoveCategoryFromPassword
            // 
            this.btnRemoveCategoryFromPassword.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveCategoryFromPassword.Location = new System.Drawing.Point(-3, 621);
            this.btnRemoveCategoryFromPassword.Name = "btnRemoveCategoryFromPassword";
            this.btnRemoveCategoryFromPassword.Size = new System.Drawing.Size(253, 71);
            this.btnRemoveCategoryFromPassword.TabIndex = 36;
            this.btnRemoveCategoryFromPassword.Text = "Delete category from password";
            this.btnRemoveCategoryFromPassword.UseVisualStyleBackColor = true;
            this.btnRemoveCategoryFromPassword.Click += new System.EventHandler(this.btnRemoveCategoryFromPassword_Click);
            // 
            // btnAddCategoryPopup
            // 
            this.btnAddCategoryPopup.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddCategoryPopup.Location = new System.Drawing.Point(0, 356);
            this.btnAddCategoryPopup.Name = "btnAddCategoryPopup";
            this.btnAddCategoryPopup.Size = new System.Drawing.Size(253, 71);
            this.btnAddCategoryPopup.TabIndex = 35;
            this.btnAddCategoryPopup.Text = "Add category";
            this.btnAddCategoryPopup.UseVisualStyleBackColor = true;
            this.btnAddCategoryPopup.Click += new System.EventHandler(this.btnAddCategoryPopup_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(23, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(192, 152);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 35;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(412, 60);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(0, 0);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 34;
            this.pictureBox1.TabStop = false;
            // 
            // btnHelp
            // 
            this.btnHelp.Font = new System.Drawing.Font("Arial Narrow", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHelp.Location = new System.Drawing.Point(1311, 135);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(227, 50);
            this.btnHelp.TabIndex = 35;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // VaultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 35F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1435, 1051);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pnlSidebar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lstCategories);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnToggleVisibility);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.Site);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtSite);
            this.Controls.Add(this.lstPasswords);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.Name = "VaultForm";
            this.Text = "Vault";
            this.pnlSidebar.ResumeLayout(false);
            this.pnlSidebar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstPasswords;
        private System.Windows.Forms.TextBox txtSite;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label Site;
        private System.Windows.Forms.Label Password;
        private System.Windows.Forms.Button btnToggleVisibility;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSortOptions;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnStarSelected;
        private System.Windows.Forms.Button btnShowFavorites;
        private System.Windows.Forms.ListBox lstCategories;
        private System.Windows.Forms.Button btnDeleteCategory;
        private System.Windows.Forms.Button btnAssignCategory;
        private System.Windows.Forms.Button btnFilterByCategory;
        private System.Windows.Forms.Button btnShowAll;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btnAddCategoryPopup;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnRemoveCategoryFromPassword;
    }
}