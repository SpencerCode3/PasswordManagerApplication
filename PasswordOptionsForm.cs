using System;
using System.Windows.Forms;

namespace PasswordManagerApplication
{
    /*
    * PasswordOptionsForm allows users to select options for generating a secure password.
    * Users can specify length and choose which character sets to include.
    */
    public partial class PasswordOptionsForm : Form
    {
        public int SelectedLength { get; private set; } // Stores selected password length.
        public bool IncludeSymbols { get; private set; } // Whether symbols should be included in the password.
        public bool IncludeNumbers { get; private set; } // Whether numbers should be included.
        public bool IncludeUppercase { get; private set; } // Whether uppercase letters should be included.
        public bool IncludeLowercase { get; private set; } // Whether lowercase letters should be included.

        public PasswordOptionsForm()
        {
            InitializeComponent();
            SetupUI(); // Calls custom method to set up UI elements.
        }

        /*
         * SetupUI configures the form appearance and adds all interactive controls
         */
        private void SetupUI()
        {
            this.Text = "Password Generation Options"; // Sets the window title.
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // Prevents resizing of form.
            this.MaximizeBox = false; // Disables maximize button.
            this.MinimizeBox = false; // Disables minimize button.
            this.StartPosition = FormStartPosition.CenterParent; // Centers this form over its parent.
            this.Width = 360;  // Sets form width.
            this.Height = 300; // Sets form height.

            // Creates a label for password length input.
            Label lblLength = new Label()
            {
                Text = "Password Length:",
                Left = 20, // X position.
                Top = 20, // y position.
                AutoSize = true
            };
            this.Controls.Add(lblLength);

            // Numeric input control for selecting password length.
            NumericUpDown numLength = new NumericUpDown()
            {
                Name = "numLength",
                Left = 160,
                Top = 18,
                Width = 70,
                Minimum = 6, // Minimum length allowed for a password.
                Maximum = 64, // Maximum length allowed.
                Value = 16 // Default value initially shown.
            };
            this.Controls.Add(numLength); // Adds the numeric input to the form.

            // Checkbox to include symbols in generated password
            CheckBox chkSymbols = new CheckBox()
            {
                Text = "Include Symbols (e.g. @#$%)", // Describes this option.
                Left = 20,
                Top = 60,
                Checked = true, // Default is enabled.
                AutoSize = true
            };

            // Checkbox to include numbers in the password.
            CheckBox chkNumbers = new CheckBox()
            {
                Text = "Include Numbers (0–9)",
                Left = 20,
                Top = 90,
                Checked = true,
                AutoSize = true
            };

            // Checkbox to include uppercase letters.
            CheckBox chkUpper = new CheckBox()
            {
                Text = "Include Uppercase Letters (A–Z)",
                Left = 20,
                Top = 120,
                Checked = true,
                AutoSize = true
            };

            // Checkbox to include lowercase letters.
            CheckBox chkLower = new CheckBox()
            {
                Text = "Include Lowercase Letters (a–z)",
                Left = 20,
                Top = 150,
                Checked = true,
                AutoSize = true
            };

            // Add all checkboxes to the UI.
            this.Controls.Add(chkSymbols);
            this.Controls.Add(chkNumbers);
            this.Controls.Add(chkUpper);
            this.Controls.Add(chkLower);

            // OK button confirms user selections.
            Button btnOk = new Button()
            {
                Text = "OK",
                DialogResult = DialogResult.OK, // Sets result when button is clicked.
                Left = 60,
                Top = 210,
                Width = 90
            };
            this.Controls.Add(btnOk); // Adds OK button to the form.

            Button btnCancel = new Button() // Cancel button closes form without applying selections.
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Left = 180,
                Top = 210,
                Width = 90
            };
            this.Controls.Add(btnCancel); // Adds Cancel button.

            /*
             * Event handler for OK button click.
             * Reads user selections and validates input before closing the form.
             */
            btnOk.Click += (s, e) =>
            {
                SelectedLength = (int)numLength.Value; // Converts numeric input to int and stores it.
                IncludeSymbols = chkSymbols.Checked; // Saves checkbox state to public property
                IncludeNumbers = chkNumbers.Checked;
                IncludeUppercase = chkUpper.Checked;
                IncludeLowercase = chkLower.Checked;

                // Validation: user must pick at least one character type.
                if (!IncludeSymbols && !IncludeNumbers && !IncludeUppercase && !IncludeLowercase)
                {
                    MessageBox.Show("Please select at least one character type.");
                    this.DialogResult = DialogResult.None; // Prevents dialog from closing.
                }
            };
        }
    }
}


