using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace PasswordManagerApplication
{
    public partial class VaultForm : Form
    {
        /* 
         * CLASS OVERVIEW:
         * The VaultForm is the main area of  the Password Manager
         * This is where users can:
         * - View, add, edit, and delete credentials.
         * - Copy passwords safely (auto-cleared).
         * - Manage categories and favorites.
         * - Sort and filter entries.
         * - Auto-lock after 5 minutes of inactivity.
         * 
         * Password data is encrypted in the database
         * using the master password passed to this form.
         */


        private int _userId; // Logged-in user's unique ID
        private string _masterPassword; // Used for decryption/encryption of vault items
        private Timer inactivityTimer; // Tracks user inactivity for auto-lock feature
        private bool showingFavoritesOnly = false; // Tracks current filter mode
        private List<string> categories = new List<string>(); // Holds all user-defined categories

        /* 
         * INNER CLASS: PasswordEntry
         * Represents a single password entry in  memory (for display in the ListBox).
         * 
         * Contains:
         *   - Site name
         *   - Encrypted password
         *   - Visibility toggle
         *   - Favorite flag
         *   - Category label
         */

        private class PasswordEntry
        {
            public int Id { get; set; } // Database ID for the password
            public string Site { get; set; } // Name of site or application
            public string Password { get; set; } // Encrypted or decrypted password string
            public bool IsVisible { get; set; } = false; // Whether password is visible in UI
            public bool IsFavorite { get; set; } = false; // Whether marked as favorite
            public DateTime LastAccessed { get; set; } = DateTime.MinValue; // Used for sorting/filtering

            public string Category { get; set; } = ""; // Optional user-defined category

            public override string ToString() // Custom display text for ListBox
            {
                string star = IsFavorite ? "★ " : "";
                string baseDisplay = IsVisible ? $"{star}{Site} : {Password}" : $"{star}{Site} : ********";

                // Append category info if it exists
                if (!string.IsNullOrEmpty(Category))
                    baseDisplay += $"  [Category: {Category}]";

                return baseDisplay;
            }
        }

        /* 
         * CONSTRUCTOR:
         * Accepts the user's ID and master password.
         * Initializes the vault view, loads passwords, categories, and starts inactivity timer.
         */
        public VaultForm(int userId, string masterPassword)
        {
            InitializeComponent();
            _userId = userId;
            _masterPassword = masterPassword;

            LoadPasswords(); // Load user's saved credentials
            LoadCategories(); // Load saved categories


            // Set up inactivity timer (5 minutes)
            inactivityTimer = new Timer();
            inactivityTimer.Interval = 300000;
            inactivityTimer.Tick += InactivityTimer_Tick;
            inactivityTimer.Start();

            // Reset inactivity timer on user activity
            this.MouseMove += (s, e) => ResetInactivityTimer();
            this.KeyDown += (s, e) => ResetInactivityTimer();
            this.MouseClick += (s, e) => ResetInactivityTimer();
        }

        /* 
         * LoadPasswords:
         * Loads all passwords from the database for this user. Supports optional category filter.
         * Each password is decrypted using the master password and displayed as masked
         * (hidden) by default.
         */
        private void LoadPasswords(string categoryFilter = null)
        {
            // Remove all existing items from the ListBox so it starts with a clean slate.
            lstPasswords.Items.Clear();

            // Fetch all stored passwords for this user.
            var passwords = DatabaseHelper.GetPasswords(_userId, _masterPassword);

            foreach (var p in passwords) //// Loop through each password entry retrieved from the database.
            {
                // Only show passwords that match the selected category if a filter is active
                if (categoryFilter != null && !string.Equals(p.category, categoryFilter, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Add a new PasswordEntry object to the ListBox.
                lstPasswords.Items.Add(new PasswordEntry
                {
                    Id = p.id,
                    Site = p.site,
                    Password = p.password,
                    IsFavorite = p.IsFavorite,
                    Category = p.category,
                    IsVisible = false
                });
            }
        }

        /*
         * ADD PASSWORD:
         * Validates user input for site and password,
         * encrypts the password, and stores it in
         * the database via DatabaseHelper.AddPassword().
         */
        private void btnAdd_Click(object sender, EventArgs e)
        {
            
            string site = txtSite.Text.Trim(); // Read and trim the site name entered by the user.
            string password = txtPassword.Text; // Read the password directly as entered.

            // Validate that both fields contain values.
            if (!string.IsNullOrEmpty(site) && !string.IsNullOrEmpty(password))
            {
                DatabaseHelper.AddPassword(_userId, site, password, _masterPassword);
                LoadPasswords();
                txtSite.Clear();
                txtPassword.Clear();
            }
            else
            {
                MessageBox.Show("Please enter both site and password.");
            }
        }

        /* 
         * DELETE PASSWORD:
         * Prompts user for confirmation before deleting selected password entry.
         */
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Check whether the currently selected item in the ListBox is a PasswordEntry.
            if (lstPasswords.SelectedItem is PasswordEntry selectedEntry)
            {
                // Display a confirmation dialog box to the user.
                var confirm = MessageBox.Show(
                    $"Delete password for {selectedEntry.Site}?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo);

                // Call DatabaseHelper to delete the entry from the database.
                // Pass the unique ID of the password entry.
                if (confirm == DialogResult.Yes)
                {
                    DatabaseHelper.DeletePassword(selectedEntry.Id);
                    LoadPasswords();
                }
            }
            else
            {
                MessageBox.Show("Please select a password to delete.");
            }
        }

        /* 
         * COPY PASSWORD:
         * Copies selected password to clipboard and automatically clears it after 30 seconds.
         */
        private void btnCopy_Click(object sender, EventArgs e)
        {
            // Ensure that a password entry is selected in the ListBox.
            // Pattern-matching ensures that it only proceeds if SelectedItem is a PasswordEntry object.
            if (lstPasswords.SelectedItem is PasswordEntry selectedEntry)
            {
                selectedEntry.LastAccessed = DateTime.Now;  // Update the LastAccessed timestamp.
                Clipboard.SetText(selectedEntry.Password); // Copy the decrypted password to the system clipboard.

                Timer clipboardTimer = new Timer();
                clipboardTimer.Interval = 30000;  // Timer interval is in milliseconds
                clipboardTimer.Tick += (s, args) => // Subscribed to the Timer's Tick event using a lambda expression.
                {
                    Clipboard.Clear();
                    clipboardTimer.Stop();
                    clipboardTimer.Dispose();
                };
                clipboardTimer.Start();  // Start the auto-clear timer.
            }
            else
            {
                MessageBox.Show("Please select a password to copy.");
            }
        }

        /* 
         * AUTO-LOCK TIMER:
         * Logs the user out after 5 minutes of inactivity.
         * this event fires when the inactivity timer reaches its interval (5 minutes).
         */
        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            inactivityTimer.Stop(); // Stop the timer so it does not fire again.

            // Hide the vault window so sensitive information is no longer visible.
            // Display the login screen.
            this.Hide();
            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Close();
        }

        private void ResetInactivityTimer()
        {
            inactivityTimer?.Stop();
            inactivityTimer?.Start();
        }

        /* Manual logout */
        private void btnLogout_Click(object sender, EventArgs e)
        {
            inactivityTimer.Stop(); // Stop the auto-lock timer since the user is logging out manually.
            this.Hide();
            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Close();
        }


        /* 
         * PASSWORD GENERATOR:
         * Opens the PasswordOptionsForm to select
         * password criteria, then generates a random password.
         */
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            // Create the options window inside a using block so it is disposed automatically when the user closes it.
            using (var optionsForm = new PasswordOptionsForm())
            {
                // Show the form as a modal dialog.
                // This pauses the current form until the user closes the options form.
                if (optionsForm.ShowDialog() == DialogResult.OK)
                {
                    // Generate a password using the settings selected by the user.
                    string newPassword = GenerateRandomPassword(
                        optionsForm.SelectedLength,
                        optionsForm.IncludeSymbols,
                        optionsForm.IncludeNumbers,
                        optionsForm.IncludeUppercase,
                        optionsForm.IncludeLowercase
                    );

                    // Output the newly generated password into the password textbox.
                    txtPassword.Text = newPassword;
                }
            }
        }


        /* Random password generator logic */
        private string GenerateRandomPassword(int length, bool includeSymbols, bool includeNumbers, bool includeUppercase, bool includeLowercase)
        {
            // four strings each contain the full set of possible characters for each chosen category.
            // static pools that the generator can draw from depending on the user’s selected options
            string symbols = "!@#$%^&*()_-+=<>?/{}[]|~";
            string numbers = "0123456789";
            string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lowercase = "abcdefghijklmnopqrstuvwxyz";

            // Build allowed character set based on selected options
            string validChars = "";
            if (includeSymbols) validChars += symbols;
            if (includeNumbers) validChars += numbers;
            if (includeUppercase) validChars += uppercase;
            if (includeLowercase) validChars += lowercase;

            //If the user unchecks all the options (symbols, numbers, uppercase, lowercase), then it's impossible to generate a password.
            if (string.IsNullOrEmpty(validChars))
                throw new ArgumentException("No character types selected for password generation.");

            // Generate random password
            Random rand = new Random(); //A new instance of Random is created.
            char[] password = new char[length]; //fixed-length char[] array is created to hold the final password characters.
            for (int i = 0; i < length; i++) //Loop runs once per character of the password.
            {
                password[i] = validChars[rand.Next(validChars.Length)]; //generates a random index.
            }

            return new string(password); //string is returned and placed directly into the UI
        }



        /* 
         * EDIT PASSWORD ENTRY:
         * Opens EditPasswordForm to modify existing credentials.
         */
        private void btnEdit_Click(object sender, EventArgs e)
        {
            //Checks whether the user has selected a password entry in the ListBox (lstPasswords).
            if (lstPasswords.SelectedItem is PasswordEntry entry)
            {
                // Creates a new instance of EditPasswordForm, passing in current site name and current decrypted password
                using (var editForm = new EditPasswordForm(entry.Site, entry.Password))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        // Calls the database helper method to update the record.
                        DatabaseHelper.UpdatePasswordEntry(entry.Id, editForm.Site, editForm.Password, _masterPassword);
                        LoadPasswords();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a password entry to edit.");
            }
        }

        /* Toggle password visibility */
        private void btnToggleVisibility_Click(object sender, EventArgs e)
        {
            if (lstPasswords.SelectedItem is PasswordEntry selectedEntry) //Checks whether an item in the ListBox is selected.
            {
                selectedEntry.LastAccessed = DateTime.Now;
                selectedEntry.IsVisible = !selectedEntry.IsVisible; //Flips the boolean flag
                int index = lstPasswords.SelectedIndex; //Stores the index of the currently selected item in the ListBox.
                lstPasswords.Items[index] = selectedEntry; //Reassigns the modified entry back into the ListBox at the same position.
            }
            else
            {
                MessageBox.Show("Please select a password to show/hide.");
            }
        }

        /* 
         * SEARCH FILTER:
         * Filters displayed entries by site name.
         */

        // method runs every time the user types a character in the search textbox.
        private void txtSearch_TextChanged(object sender, EventArgs e) 
        {
            string searchText = txtSearch.Text.Trim().ToLower(); // Reads whatever the user typed into the search field.
            lstPasswords.Items.Clear();
            var passwords = DatabaseHelper.GetPasswords(_userId, _masterPassword); // retrieves all decrypted password entries for the logged-in user.

            foreach (var p in passwords) // Iterates through each password entry returned from the database.
            {
                // Converts the site name to lowercase to match the search filter.
                // performs substring matching:
                if (p.site.ToLower().Contains(searchText)) 
                {
                    // Creates a new PasswordEntry object for any matching site.
                    lstPasswords.Items.Add(new PasswordEntry
                    {
                        Id = p.id,
                        Site = p.site,
                        Password = p.password,
                        IsFavorite = p.IsFavorite,
                        Category = p.category,
                        IsVisible = false
                    });
                }
            }
        }

        /* 
         * SORTING OPTIONS:
         * Allows sorting of passwords alphabetically or by password length.
         */
        private void cmbSortOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSortOptions.SelectedItem == null) return;

            // Converts the selected ComboBox item into a string. This allows the switch statement below to match the user's choice.
            string selectedOption = cmbSortOptions.SelectedItem.ToString();

            // Re-fetch passwords fresh from DB
            var passwords = DatabaseHelper.GetPasswords(_userId, _masterPassword);

            // The switch block performs the sorting depending on the user’s selected option:
            switch (selectedOption)
            {
                case "Alphabetical (A–Z)":
                    passwords = passwords.OrderBy(p => p.site.ToLower()).ToList(); //Sort site names alphabetically.
                    break;

                case "Alphabetical (Z–A)":
                    passwords = passwords.OrderByDescending(p => p.site.ToLower()).ToList(); //Sort site names alphabetically reverse.
                    break;

                case "Password Length (Shortest → Longest)":
                    passwords = passwords.OrderBy(p => p.password.Length).ToList(); // Sorts based on the length of the decrypted password.
                    break;

                case "Password Length (Longest → Shortest)":
                    passwords = passwords.OrderByDescending(p => p.password.Length).ToList(); // Sorts based on the length of the decrypted password.
                    break;
            }

            
            lstPasswords.Items.Clear(); // Removes all existing entries from the ListBox.
            foreach (var p in passwords) //Loops through each sorted password entry.
            {
                // Recreates a PasswordEntry object for each row and adds it to the ListBox.
                lstPasswords.Items.Add(new PasswordEntry
                {
                    Id = p.id,
                    Site = p.site,
                    Password = p.password,
                    IsVisible = false,
                    Category = p.category,
                    IsFavorite = p.IsFavorite
                });
            }
        }


        /* FAVORITES FEATURE */
        // Toggles between only favorite entries and all entries.
        private void btnShowFavorites_Click(object sender, EventArgs e)
        {
            showingFavoritesOnly = !showingFavoritesOnly; //Flips the boolean value each time the button is clicked.

            lstPasswords.Items.Clear();
            var passwords = DatabaseHelper.GetPasswords(_userId, _masterPassword); //Retrieves all decrypted passwords for the user.


            // Uses a ternary operator to decide which list to use:
            // If the mode is Favorites Only, filter the list using Where(p => p.IsFavorite).
            // Otherwise, use the full list of passwords.
            var filtered = showingFavoritesOnly
                ? passwords.Where(p => p.IsFavorite).ToList()
                : passwords;

            foreach (var p in filtered) // Iterates through the filtered list and rebuilds the ListBox display.
            {
                lstPasswords.Items.Add(new PasswordEntry
                {
                    Id = p.id,
                    Site = p.site,
                    Password = p.password,
                    IsFavorite = p.IsFavorite,
                    Category = p.category,
                    IsVisible = false
                });
            }

            //Updates the button label depending on the mode.
            btnShowFavorites.Text = showingFavoritesOnly ? "Show All" : "Show Favorites";
        }

        // Event fires when the user clicks the Star / Unstar button.
        // Toggles whether the selected password entry is marked as a favorite.
        private void btnStarSelected_Click(object sender, EventArgs e)
        {
            if (lstPasswords.SelectedItem is PasswordEntry selectedEntry) //Checks whether the user has selected an item in the password list.
            {
                selectedEntry.IsFavorite = !selectedEntry.IsFavorite; // Flips the favorite state of the selected entry:
                DatabaseHelper.UpdateFavoriteStatus(selectedEntry.Id, selectedEntry.IsFavorite); //Saves the updated status to the database.
                LoadPasswords();
            }
            else
            {
                MessageBox.Show("Please select a password to favorite/unfavorite.");
            }
        }




        /* 
         * CATEGORY MANAGEMENT: 
         * Load, assign, delete, and filter passwords by user-defined categories.
         */


        // Allows the user to remove one of their custom categories from the system.
        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            if (lstCategories.SelectedItem == null)
            {
                MessageBox.Show("Please select a category to delete.");
                return;
            }

            // Stores the name of the selected category as a string.
            string selectedCategory = lstCategories.SelectedItem.ToString();

            // 1Remove category from associated passwords
            DatabaseHelper.RemoveCategoryFromPasswords(_userId, selectedCategory);

            // Delete category from Categories table
            DatabaseHelper.DeleteCategory(_userId, selectedCategory);

            // Remove from local list and listbox
            categories.Remove(selectedCategory);
            lstCategories.Items.Remove(selectedCategory);

            // Refresh the vault to show passwords without the deleted category
            LoadPasswords();
        }


        // This method refreshes the category list in the Vault UI.
        // Called when: the Vault loads for the first time, the user adds a category, and the user deletes a category.
        private void LoadCategories()
        {
            categories = DatabaseHelper.GetCategories(_userId); // Fetch the latest list of categories saved for this specific user.
            lstCategories.Items.Clear();

            foreach (string cat in categories) //Iterates through each category string in the updated categories list.
            {
                lstCategories.Items.Add(cat);
            }
        }

        // Purpose of this method is to link a selected password entry to a selected category.
        private void btnAssignCategory_Click(object sender, EventArgs e)
        {
            // Checks whether the user has selected a password from the password list
            if (lstPasswords.SelectedItem is PasswordEntry selectedEntry)
            {
                // No category is selected
                if (lstCategories.SelectedItem == null)
                {
                    MessageBox.Show("Please select a category to assign.");
                    return;
                }

                //Converts the selected category
                string selectedCategory = lstCategories.SelectedItem.ToString();

                //Calls the database helper method to save the category assignment.
                // Writes the category into the Category column of the Passwords table, for this specific password's ID.
                DatabaseHelper.UpdatePasswordCategory(selectedEntry.Id, selectedCategory);
                MessageBox.Show($"Category '{selectedCategory}' assigned to {selectedEntry.Site}.");

                LoadPasswords(); // Refresh list
            }
            else
            {
                MessageBox.Show("Please select a password to assign a category to.");
            }
        }

        // Method is to filter the displayed password list so that only passwords belonging to the selected category are shown.
        private void btnFilterByCategory_Click(object sender, EventArgs e)
        {
            if (lstCategories.SelectedItem == null)
            {
                MessageBox.Show("Please select a category to filter by.");
                return;
            }

            string selectedCategory = lstCategories.SelectedItem.ToString(); //Converts the selected category into a string.
            // Passing selectedCategory instructs the method to load and display only passwords whose Category field matches the selected value.
            LoadPasswords(selectedCategory);
        }

        //Method for showing all passwords in the user's vault without any filters applied.
        private void btnShowAll_Click(object sender, EventArgs e)
        {
            // Reload everything
            LoadPasswords();

            // Reset favorites toggle
            showingFavoritesOnly = false;
            btnShowFavorites.Text = "Show Favorites";

            
        }

        //Method allows the user to create a new custom category (e.g., "Banking", "Work", "School").
        private void btnAddCategoryPopup_Click(object sender, EventArgs e)
        {
            // Show a simple input dialog for category name
            string newCategory = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter a new category name:",
                "Add Category",
                ""
            ).Trim();

            if (string.IsNullOrEmpty(newCategory))
            {
                return;
            }

            //Prevents duplicate category names.
            if (categories.Contains(newCategory, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show("This category already exists.");
                return;
            }

            
            DatabaseHelper.AddCategory(_userId, newCategory); // Saves the new category to the database using DatabaseHelper.
            categories.Add(newCategory); //Updates the in-memory list of categories tracked by the form.
            lstCategories.Items.Add(newCategory); // Updates the UI ListBox immediately by adding the new category as a displayed item.

        }

        // The users click the help button and the helpText displays instructions for using the app.
        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpText =
                "🔐 Vault Help Guide\n\n" +
                "This vault securely stores all your saved passwords.\n\n" +
                "📋 Main Features:\n" +
                "• **Add** – Enter a site name and password, then click Add to save it.\n" +
                "• **Edit** – Select a saved entry and click Edit to change its details.\n" +
                "• **Delete** – Remove a selected password permanently.\n" +
                "• **Show/Hide** – Toggle between hiding and showing a password entry.\n" +
                "• **Copy** – Copy the selected password to your clipboard (auto-clears after 30 seconds).\n" +
                "• **Generate** – Opens the password generator window to create a strong random password.\n" +
                "• **Search** – Filters your passwords by site name as you type.\n\n" +
                "⭐ Favorites:\n" +
                "• Click the ★ Favorite button to mark important entries.\n" +
                "• Click 'Show Favorites' to view only your starred entries.\n\n" +
                "📁 Categories:\n" +
                "• Use Add Category to create a new label (e.g., Work, Banking, Games).\n" +
                "• Assign passwords to a category with Assign Category.\n" +
                "• Use Filter by Category to display only matching entries.\n" +
                "• Delete Cateogry from password removes the category from associated password.\n" +
                "• Delete Category removes the label and clears it from associated passwords.\n\n" +
                "⏱️ Security:\n" +
                "• Session auto-locks after 5 minutes of inactivity.\n" +
                "• Clipboard is cleared automatically after copying a password.\n" +
                "• All passwords are encrypted.\n\n" +
                "💡 Tips:\n" +
                "• Use long, unique passwords for each site.\n" +
                "• Backup your master password — it’s required to unlock the vault.\n" +
                "• You can reset your password using security questions.";

            MessageBox.Show(helpText, "Vault Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Method removes the assigned category from a selected password entry without deleting the category itself.
        private void btnRemoveCategoryFromPassword_Click(object sender, EventArgs e)
        {
            if (lstPasswords.SelectedItem is PasswordEntry selectedEntry) //Checks whether the user has selected a password entry from the ListBox.
            {
                // Calls the database helper method to remove the category from the password.
                DatabaseHelper.UpdatePasswordCategory(selectedEntry.Id, null);

                MessageBox.Show($"Category removed from {selectedEntry.Site}.");

                LoadPasswords(); // Refresh list
            }
            else
            {
                MessageBox.Show("Please select a password to remove its category.");
            }
        }

    }
}
