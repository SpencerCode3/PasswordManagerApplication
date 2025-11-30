using System;
using System.Windows.Forms;


namespace PasswordManagerApplication
{
    /*
     * Form1 represents the login screen of the application.
     * It is the entry point for users to log in or navigate to registration or password recovery.
     */
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /*
        * Triggered when the user clicks the "Login" button to attempt authentication.
        */
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim(); // Reads and trims the username input to remove extra whitespace.
            string password = txtPassword.Text; // Reads the master password directly (not trimmed to preserve exact characters).

            /*
             * ValidateUser checks if the provided credentials are correct.
             * If valid, it returns true and outputs the user's unique ID into userId.
             */

            if (DatabaseHelper.ValidateUser(username, password, out int userId))
            {
                this.Hide(); // Hides the login form to transition smoothly without closing the app.

                // Creates a new VaultForm instance and passes the userId and password (used for decryption of vault data).
                VaultForm vault = new VaultForm(userId, password);
                vault.Show();// Displays the vault interface where user passwords are stored and managed.
            }
            else
            {
                // Displays an error message if credentials are invalid.
                MessageBox.Show("Invalid username or password.");
            }
        }

        /*
         * Opens the registration form to allow new users to sign up.
         */
        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Creates a RegisterForm instance and passes the current form (login form) to return to after registration.
            RegisterForm registerForm = new RegisterForm(this);
            registerForm.Show();
            this.Hide(); // Hide login while registering
        }


        /*
         * Opens the ResetPasswordForm in a modal dialog, preventing interaction with other forms until closed.
         */
        private void btnForgotPassword_Click(object sender, EventArgs e)
        {
            using (var resetForm = new ResetPasswordForm()) // 'using' ensures resources are disposed properly after closing.
            {
                resetForm.ShowDialog();  // ShowDialog is used to block other windows until the reset process is completed.
            }
        }

        
    }
}





