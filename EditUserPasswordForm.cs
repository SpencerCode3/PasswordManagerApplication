using System;
using System.Windows.Forms;

namespace PasswordManagerApplication
{
    /* 
     * EditPasswordForm:
     * This is a Form used to allow users to edit an existing password entry.
     */
    public partial class EditPasswordForm : Form
    {
        public string Site { get; private set; } // Stores the updated website name
        public string Password { get; private set; } // Stores the updated password value

        /*
         * Constructor for the form.
         * Parameters:
         * currentSite: the site value that should appear in the form for editing
         * currentPassword: the password value that should appear in the form
         */
        public EditPasswordForm(string currentSite, string currentPassword)
        {
            InitializeComponent();
            txtSite.Text = currentSite; // Sets the text of the site input textbox to the current value for display
            txtPassword.Text = currentPassword; // Sets the password textbox text to the current value
        }

        /*
         * btnSave_Click:
         * Validates input and returns the updated data to the calling form.
         */
        private void btnSave_Click(object sender, EventArgs e)
        {
            Site = txtSite.Text.Trim(); // Retrieves text from the Site textbox 
            Password = txtPassword.Text.Trim(); // Retrieves text from the Password textbox

            // Validate input to ensure that neither field is empty
            if (string.IsNullOrEmpty(Site) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Please enter both site and password."); // Show warning message if fields are incomplete
                return;  // Stop execution and do not close the form
            }

            this.DialogResult = DialogResult.OK; // Sets the result of the dialog to OK indicating successful input
            this.Close(); // Closes the form window
        }

        /*
         * btnCancel_Click:
         * This closes the form without saving any changes.
         */
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;  // Indicates that the user canceled the operation
            this.Close();
        }
    }
}




