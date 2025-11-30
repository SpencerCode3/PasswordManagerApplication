using System;
using System.Windows.Forms;

namespace PasswordManagerApplication
{
    public partial class ResetPasswordForm : Form
    {
        /* 
         * CLASS OVERVIEW:
         * This form handles the password reset process when a user forgets their master password.
         * 
         * The process flow:
         * 1. User enters their username.
         * 2. App retrieves 3 stored security questions.
         * 3. Randomly selects one question to display.
         * 4. User answers the question for verification.
         * 5. If verified, user can set a new password.
         * 
         * Password strength is validated visually, and the database update is handled via
         * DatabaseHelper.ResetPassword().
         */


        private string _selectedQuestion = ""; // Stores the randomly chosen security question text
        private int _questionIndex = -1; // Stores which question number (1–3) was chosen

        public ResetPasswordForm()
        {
            InitializeComponent(); // Initialize all UI elements
            pnlQuestion.Visible = false; // Hide the question panel until user lookup succeeds
            pnlReset.Visible = false; // Hide the reset panel until verification succeeds
        }

        /* 
         * btnFindUser_Click:
         * This retrieves security questions based on the entered username.
         * 
         * If the username exists:
         *   - Randomly selects one of three questions.
         *   - Displays it for user to answer.
         * Otherwise:
         *   - Shows "Username not found."
         */
        private void btnFindUser_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim(); // Remove whitespace from input
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter a username.");
                return;
            }

            // Retrieve the three security questions for the username
            var questions = DatabaseHelper.GetSecurityQuestions(username);
            if (!questions.HasValue)
            {
                MessageBox.Show("Username not found.");
                pnlQuestion.Visible = false;
                pnlReset.Visible = false;
                return;
            }

            // Randomly select one of the three questions to display
            Random rnd = new Random();
            _questionIndex = rnd.Next(1, 4); // Generates a number between 1 and 3

            // Assign the randomly chosen question to display
            switch (_questionIndex)
            {
                case 1: _selectedQuestion = questions.Value.q1; break;
                case 2: _selectedQuestion = questions.Value.q2; break;
                case 3: _selectedQuestion = questions.Value.q3; break;
            }

            // Display question in the UI
            lblQuestion.Text = _selectedQuestion;
            pnlQuestion.Visible = true; // Show the question/answer section
            pnlReset.Visible = false; // Hide reset area until verified
        }

        /* 
         * btnResetPassword_Click: 
         * This resets the users's master password.
         * 
         * Steps:
         *   1. Validate new password & confirmation.
         *   2. Ensure both fields match.
         *   3. Call DatabaseHelper.ResetPassword() to update the stored credentials.
         *   4. Notify user of result.
         */

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string answer = txtAnswer.Text.Trim();
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // Ensure both password fields are filled
            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please enter and confirm your new password.");
                return;
            }

            // Check for password match
            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            // Attempt to reset via database helper
            bool success = DatabaseHelper.ResetPassword(username, newPassword, answer.Trim());
            if (success)
            {
                MessageBox.Show("Password successfully reset!");
                this.Close(); // Close reset form
            }
            else
            {
                MessageBox.Show("Failed to reset password. Ensure your security answer is correct.");
            }
        }

        /* 
         * btnCancel_Click:
         * Closes the reset form when the user decides not to proceed.
         */
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close(); // Close the reset window
        }

        /* 
         * btnVerifyAnswers_Click:
         * Verifies the user’s answer to their randomly selected security question.
         * If the answer is correct:
         *   - Shows the password reset section.
         * Otherwise:
         *   - Displays an error and hides it again.
         */
        private void btnVerifyAnswers_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string answer = txtAnswer.Text.Trim();

            // Ensure both username and answer are provided
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(answer))
            {
                MessageBox.Show("Please enter a username and answer.");
                return;
            }

            // Retrieve stored questions again for validation
            var qResult = DatabaseHelper.GetSecurityQuestions(username);
            if (!qResult.HasValue)
            {
                MessageBox.Show("Username not found.");
                return;
            }

            int userId = qResult.Value.userId;

            // Check if the provided answer matches the stored one
            bool verified = DatabaseHelper.VerifySecurityAnswer(userId, _questionIndex, answer.Trim());
            if (verified)
            {
                pnlReset.Visible = true; // Show reset panel
                MessageBox.Show("Security answer verified. You can now set a new password.");
            }
            else
            {
                MessageBox.Show("Incorrect answer. Please try again.");
                pnlReset.Visible = false; // Keep reset area hidden if failed
            }
        }

        /* 
         * GetPasswordStrength:
         * Determines how strong a password is by
         * scoring it based on:
         *   - Length
         *   - Numbers
         *   - Lowercase letters
         *   - Uppercase letters
         *   - Special characters
         * 
         * Returns a tuple of:
         *   (TextLabel, Value, Color)
         */
        private (string StrengthText, int StrengthValue, System.Drawing.Color Color) GetPasswordStrength(string password)
        {
            int score = 0; // Initial strength score

            if (password.Length >= 12) score += 1; // Adequate length
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"\d")) score += 1; // Contains numbers
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]")) score += 1; // Contains lowercase letters
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]")) score += 1; // Contains uppercase letters
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#$%^&*()_\-+=<>?]")) score += 1; // Contains symbols

            // Map score to descriptive strength level
            if (score <= 2) return ("Weak", 33, System.Drawing.Color.Red);
            else if (score == 3 || score == 4) return ("Medium", 66, System.Drawing.Color.Orange);
            else return ("Strong", 100, System.Drawing.Color.Green);
        }

        /* 
         * txtNewPassword_TextChanged:
         * Provides real-time visual feedback on password strength as the user types.
         * 
         * Updates:
         *   - Label text and color
         *   - Progress bar fill value
         *   - Enables Reset button when “Strong”
         */
        private void txtNewPassword_TextChanged(object sender, EventArgs e)
        {
            {
                var strength = GetPasswordStrength(txtNewPassword.Text); // Evaluate password

            
            lblNewPasswordStrength.Text = $"Strength: {strength.StrengthText}"; // Update label 
            lblNewPasswordStrength.ForeColor = strength.Color; // Change color

            pbNewPasswordStrength.Value = strength.StrengthValue; //Update progress bar

            // Enable Create Account button only if password is Strong
            btnResetPassword.Enabled = strength.StrengthText == "Strong";
            }
        }

        
    }
}













