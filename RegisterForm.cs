using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PasswordManagerApplication
{
    public partial class RegisterForm : Form
    {
        /* 
         * CLASS OVERVIEW:
         * This form allows new users to create an account.
         * It collects:
         *   - A username
         *   - A master password
         *   - 3 unique security questions + answers
         * 
         * The data is validated and passed to DatabaseHelper.RegisterUser()
         * for storage in the local SQLite database.
         */
        private Form1 _loginForm;

        public RegisterForm(Form1 loginForm)
        {
            InitializeComponent();
            _loginForm = loginForm;

            /* 
             * SECURITY QUESTIONS INITIALIZATION
             * Each ComboBox (dropdown) is populated with a set of potential security questions.
             * These are grouped differently across three ComboBoxes to encourage distinct and varied questions.
             */

            // Populate ComboBox #1 with personal/family-based questions
            cmbQuestion1.Items.AddRange(new string[]
            {
                "What is your mother’s maiden name?",
                "What is your father’s middle name?",
                "What is the first name of your oldest cousin?",
                "Where did your parents meet?",
                "What was the name of your first boyfriend/girlfriend?",
                "What is your grandmother’s first name?",
                "In what city did your parents get married?",
                "What is the name of your eldest child?"
            });

            // Populate ComboBox #2 with memory/favorites-based questions
            cmbQuestion2.Items.AddRange(new string[]
            {
                "What was the first country you visited?",
                "What was the first movie you saw in a theater?",
                "What was your favorite childhood TV show?",
                "What is your favorite book?",
                "What is your favorite movie?",
                "What is your favorite food?",
                "What is your favorite sports team?",
                "What was your first job?",
                "What was the name of the company of your first job?",
                "Where was your first vacation destination?"
            });

            // Populate ComboBox #3 with technology/game-related questions
            cmbQuestion3.Items.AddRange(new string[]
            {
                "What was the first video game you played?",
                "What is the name of your favorite childhood video game character?",
                "What is your favorite board game?",
                "What is the first website you remember visiting?",
                "What is the name of your favorite teacher?",
                "What was the name of your first email provider?"
            });

            /* 
             * SET DEFAULT SELECTIONS + VALIDATION EVENT HOOKS
             * Default indices are set to avoid empty ComboBoxes.
             * Event handlers ensure that duplicate questions cannot be chosen across ComboBoxes.
             */
            cmbQuestion1.SelectedIndex = 0;
            cmbQuestion2.SelectedIndex = 0;
            cmbQuestion3.SelectedIndex = 0;

            // Attach validation event to all ComboBoxes
            cmbQuestion1.SelectedIndexChanged += ValidateUniqueQuestions;
            cmbQuestion2.SelectedIndexChanged += ValidateUniqueQuestions;
            cmbQuestion3.SelectedIndexChanged += ValidateUniqueQuestions;
        }

        /* 
        * ValidateUniqueQuestions:
        * Ensures all selected security questions are unique.
        * If duplicates are found, an alert appears and the last changed ComboBox is reset to no selection.
        */
        private void ValidateUniqueQuestions(object sender, EventArgs e)
        {
            // Get currently selected values
            var q1 = cmbQuestion1.SelectedItem?.ToString();
            var q2 = cmbQuestion2.SelectedItem?.ToString();
            var q3 = cmbQuestion3.SelectedItem?.ToString();

            // Collect into a list (ignoring empty selections)
            var chosen = new List<string> { q1, q2, q3 }
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            // Compare total count to distinct count (duplicates → mismatch)
            if (chosen.Count != chosen.Distinct().Count())
            {
                MessageBox.Show("Each security question must be unique. Please choose different questions.");

                // Reset the ComboBox that caused the duplication
                if (sender is ComboBox combo)
                {
                    combo.SelectedIndex = -1; // Removes the selected item
                }
            }
        }
        /* 
         * btnCreateAccount_Click:
         * Validates input fields, ensures all are filled, then calls DatabaseHelper.RegisterUser() to insert
         * the new user record into the database.
         */
        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            // Collect username and password
            string username = txtNewUsername.Text.Trim();
            string password = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match. Please re-enter them.");
                return;
            }

            // Retrieve selected questions
            string q1 = cmbQuestion1.SelectedItem?.ToString();
            string q2 = cmbQuestion2.SelectedItem?.ToString();
            string q3 = cmbQuestion3.SelectedItem?.ToString();

            // Retrieve typed answers
            string a1 = txtAnswer1.Text.Trim();
            string a2 = txtAnswer2.Text.Trim();
            string a3 = txtAnswer3.Text.Trim();

            // Validation check – ensures all fields are filled
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword) ||
                string.IsNullOrEmpty(q1) || string.IsNullOrEmpty(q2) || string.IsNullOrEmpty(q3) ||
                string.IsNullOrEmpty(a1) || string.IsNullOrEmpty(a2) || string.IsNullOrEmpty(a3))
            {
                MessageBox.Show("Please fill in all fields, including confirming your password.");
                return;
            }


            /* 
             * DATABASE CALL:
             * RegisterUser() handles:
             *  - Password hashing + salting
             *  - Hashing of security answers
             *  - Validation of username uniqueness
             * It returns a boolean indicating success or failure.
             */

            bool success = DatabaseHelper.RegisterUser(
                username,
                password,
                q1, a1,
                q2, a2,
                q3, a3
            );

            // If registration successful
            if (success)
            {
                MessageBox.Show("Account created successfully!");
                this.Close(); // Close registration form
                _loginForm.Show(); // Return user to login form
            }
            else
            {
                MessageBox.Show("Username already exists. Please choose another.");
            }
        }

        /* 
         * btnCancel_Click:
         * Returns user to the login screen if they decide to cancel account creation.
         */
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close(); // Close registration window
            _loginForm.Show();  // Re-show login form
        }

        /* 
         * GetPasswordStrength:
         * Evaluates password complexity and returns a tuple
         * that describes the following:
         *   - Strength label (Weak/Medium/Strong)
         *   - Strength percentage (for progress bar)
         *   - Display color (visual feedback)
         */
        private (string StrengthText, int StrengthValue, System.Drawing.Color Color) GetPasswordStrength(string password)
        {
            int score = 0; // Base score

            if (password.Length >= 12) score += 1;            // minimum length
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"\d")) score += 1;  // has numbers
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]")) score += 1; // has lowercase
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]")) score += 1; // has uppercase
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#$%^&*()_\-+=<>?]")) score += 1; // has symbols

            // Map score to strength
            if (score <= 2) return ("Weak", 33, System.Drawing.Color.Red);
            else if (score == 3 || score == 4) return ("Medium", 66, System.Drawing.Color.Orange);
            else return ("Strong", 100, System.Drawing.Color.Green);
        }

        /* 
         * txtNewPassword_TextChanged:
         * Dynamically updates password strength feedback as the user types in their password.
         * It updates:
         *   - Label text and color
         *   - Progress bar value
         *   - Enables Create button only if "Strong"
         */
        private void txtNewPassword_TextChanged(object sender, EventArgs e)
        {
            var strength = GetPasswordStrength(txtNewPassword.Text); // Evaluate password

            
            lblPasswordStrength.Text = $"Strength: {strength.StrengthText}"; // Update label text
            lblPasswordStrength.ForeColor = strength.Color; // Update label color

            pbPasswordStrength.Value = strength.StrengthValue; // Update progress bar

            // Only allow account creation for strong passwords
            btnCreateAccount.Enabled =
                strength.StrengthText == "Strong" &&
                txtNewPassword.Text == txtConfirmPassword.Text;

        }

        private void txtConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            // Check if main password is strong enough
            var strength = GetPasswordStrength(txtNewPassword.Text);

            // Enable button ONLY if:
            // Password is strong
            // Passwords match
            btnCreateAccount.Enabled =
                strength.StrengthText == "Strong" &&
                txtNewPassword.Text == txtConfirmPassword.Text;
        }
    }
}










