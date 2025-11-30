using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManagerApplication
{

    /*
* DatabaseHelper:
* This class acts is the central hub for all database-related operations.
* in the password manager. It handles:
* - Creating and maintaining database tables (Users, Passwords, Categories)
* - Registering and authenticating users
* - Managing encrypted password vault entries (CRUD operations)
* - Handling security questions and password resets
* - Managing categories and favorites
*
* This class also ties in cryptography and key wrapping logic:
* Each user has a unique Vault Key (VK) which is used to encrypt/decrypt their passwords.
* The VK is itself encrypted (wrapped) using the master password and security answers,
* allowing recovery without destroying vault data.
*/

    public static class DatabaseHelper
    {
        // SQLite connection string - points to local file database 
        // "Version=3" indicates we’re using the SQLite3 format
        private static readonly string connectionString = "Data Source=password_manager.db;Version=3;";

        // This constructor runs automatically when the class is first accessed. 
        // It ensures all necessary tables exist, and adds any missing columns.
        static DatabaseHelper()
        {
            // Opens a new SQLite connection
            // using ensures the database connection is closed and disposed even if exceptions occur.
            using (var conn = new SQLiteConnection(connectionString))
            {
                // Opens the connection to password_manager.db.
                conn.Open();

                // Create "Users" table with support for encrypted vault keys and security questions. 
                // The EncryptedVaultKey fields store the user’s encrypted Vault Key wrapped by either 
                // their master password or their security answers.

                // Unique ID for each user.
                // Unique username
                // SHA256(password + salt) is stored.
                // Stores three questions and their hashed answers. (Answers are lowercased, trimmed, salted, and hashed before storage.)
                // Vault Key encrypted with master password
                // Vault Key encrypted with security answer keys
                string userTable = @"
CREATE TABLE IF NOT EXISTS Users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT UNIQUE NOT NULL,
    passwordHash TEXT NOT NULL,
    salt TEXT NOT NULL,
    SecurityQuestion1 TEXT,
    SecurityAnswer1 TEXT,
    SecurityQuestion2 TEXT,
    SecurityAnswer2 TEXT,
    SecurityQuestion3 TEXT,
    SecurityAnswer3 TEXT,
    EncryptedVaultKey   TEXT,  -- VK wrapped by master password
    EncryptedVaultKeyQ1 TEXT,  -- VK wrapped by Answer1-derived key
    EncryptedVaultKeyQ2 TEXT,  -- VK wrapped by Answer2-derived key
    EncryptedVaultKeyQ3 TEXT   -- VK wrapped by Answer3-derived key
);";

                // Create "Passwords" table to store encrypted credentials. 
                // Each entry belongs to a user and references the user's id.

                // id and userID identifies which user owns the entry.
                // Website/service name
                // encrypted password using the user’s Vault Key.
                // Favorites and category filters for organizing entries.
                // Forigen Key: Enforces relational integrity.

                string passwordTable = @"
CREATE TABLE IF NOT EXISTS Passwords (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    userId INTEGER NOT NULL,
    site TEXT NOT NULL,
    password TEXT NOT NULL,     -- encrypted with user's VK
    IsFavorite INTEGER DEFAULT 0,
    Category TEXT,              -- added by upgrade helper
    FOREIGN KEY(userId) REFERENCES Users(id)
);";

                // Create "Categories" table for grouping passwords.
                // Ensures duplicate categories per user are not allowed.

                string categoriesTable = @"
CREATE TABLE IF NOT EXISTS Categories (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER,
    CategoryName TEXT NOT NULL,
    UNIQUE(UserId, CategoryName)
);";

                // Executes all three CREATE TABLE commands
                // ExecuteNonQuery() runs SQL that doesn’t return data.
                using (var cmd = new SQLiteCommand(userTable, conn)) cmd.ExecuteNonQuery();
                using (var cmd = new SQLiteCommand(passwordTable, conn)) cmd.ExecuteNonQuery();
                using (var cmd = new SQLiteCommand(categoriesTable, conn)) cmd.ExecuteNonQuery();
            }

            // Adds columns if missing, without altering or deleting existing rows.
            // Makes the application safe to update: new features get new columns and no data is destroyed.
            EnsureColumnExists("Passwords", "Category", "TEXT");
            EnsureColumnExists("Users", "EncryptedVaultKey", "TEXT");
            EnsureColumnExists("Users", "EncryptedVaultKeyQ1", "TEXT");
            EnsureColumnExists("Users", "EncryptedVaultKeyQ2", "TEXT");
            EnsureColumnExists("Users", "EncryptedVaultKeyQ3", "TEXT");
        }

        // Checks if a column exists in a table; if not, it adds the column. 
        // This allows schema upgrades without destroying existing user data.
        private static void EnsureColumnExists(string table, string col, string type)
        {
            // Opens a new SQLite connection
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                // Executes SQLite’s built-in command which returns one row per column in the table.
                using (var cmd = new SQLiteCommand($"PRAGMA table_info({table});", conn))
                using (var reader = cmd.ExecuteReader()) // Loop through the rows.
                {
                    bool found = false; // Flag that tracks whether it discovers the colum

                    // PRAGMA table_info returns info about all columns. 
                    // Then scan through the column names to check for existence.
                    while (reader.Read())
                    {
                        if (string.Equals(reader["name"]?.ToString(), col, StringComparison.OrdinalIgnoreCase))
                        {
                            found = true; break;
                        }
                    }
                    // If not found, add the column using ALTER TABLE.
                    if (!found)
                    {
                        using (var alter = new SQLiteCommand($"ALTER TABLE {table} ADD COLUMN {col} {type};", conn))
                        {
                            alter.ExecuteNonQuery();
                        }
                    }
                }
            }
        }


        // =============== USER MANAGEMENT SECTION ==============



        // Handles user registration, login, and security question setup.
        // This method creates a brand-new user account. It:
        // Hashes passwords, Hashes security answers, Generates a unique Vault Key (VK), Encrypts the Vault Key with multiple keys, and
        // Stores everything in the Users table.

        public static bool RegisterUser(
            string username, string password,
            string question1, string answer1,
            string question2, string answer2,
            string question3, string answer3)
        {
            // Step 1: Generate a new random salt (unique per user). Salt protects against rainbow-table attacks.
            // It is combined with the user’s password before hashing.
            string salt = GenerateSalt();

            // Step 2: Hash the user's password using the salt
            // prevents storing plaintext passwords.
            string passwordHash = HashPassword(password, salt);

            // Step 3: Normalize security answers (lowercase, trimmed) to prevent trivial mismatches
            string a1Norm = (answer1 ?? "").Trim().ToLowerInvariant();
            string a2Norm = (answer2 ?? "").Trim().ToLowerInvariant();
            string a3Norm = (answer3 ?? "").Trim().ToLowerInvariant();

            // Step 4: Hash each normalized answer with the salt for storage
            string a1Hash = HashPassword(a1Norm, salt);
            string a2Hash = HashPassword(a2Norm, salt);
            string a3Hash = HashPassword(a3Norm, salt);

            // Step 5: Generate a random 32-byte Vault Key (used to encrypt/decrypt stored passwords)
            string vaultKey = GenerateVaultKeyBase64(32);

            // Step 6: Encrypt the Vault Key (VK) with the user's master password.
            // The Vault Key is encrypted (wrapped) using AES
            string encVK_ByPassword = EncryptString(vaultKey, password);

            // Step 7: Derive per-answer wrapping keys for recovery encryption. 
            // Each answer-derived key encrypts a separate copy of the VK, allowing password reset.
            string wrapKeyQ1 = a1Hash; 
            string wrapKeyQ2 = a2Hash;
            string wrapKeyQ3 = a3Hash;

            // Three independent encrypted versions of the Vault Key are created:
            // One encrypted with Answer 1, Answer 2, and Answer 3.
            string encVK_Q1 = EncryptString(vaultKey, wrapKeyQ1);
            string encVK_Q2 = EncryptString(vaultKey, wrapKeyQ2);
            string encVK_Q3 = EncryptString(vaultKey, wrapKeyQ3);

            // Step 8: Insert all user data into the database.
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open(); // Opens a new DB connection to insert all this new user data.

                // SQL inserts: Username, Password hash, Salt, Security questions, Security hashed answers, Encrypted Vault Key (wrapped by password),
                // Encrypted VK wrapped by Answer 1, Encrypted VK wrapped by Answer 2, and Encrypted VK wrapped by Answer 3. 
                string sql = @"
INSERT INTO Users
(username, passwordHash, salt,
 SecurityQuestion1, SecurityAnswer1,
 SecurityQuestion2, SecurityAnswer2,
 SecurityQuestion3, SecurityAnswer3,
 EncryptedVaultKey, EncryptedVaultKeyQ1, EncryptedVaultKeyQ2, EncryptedVaultKeyQ3)
VALUES
(@username, @passwordHash, @salt,
 @q1, @a1,
 @q2, @a2,
 @q3, @a3,
 @vkPwd, @vkQ1, @vkQ2, @vkQ3);";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    // Bind all values to prevent SQL injection
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@salt", salt);

                    cmd.Parameters.AddWithValue("@q1", question1 ?? "");
                    cmd.Parameters.AddWithValue("@a1", a1Hash);
                    cmd.Parameters.AddWithValue("@q2", question2 ?? "");
                    cmd.Parameters.AddWithValue("@a2", a2Hash);
                    cmd.Parameters.AddWithValue("@q3", question3 ?? "");
                    cmd.Parameters.AddWithValue("@a3", a3Hash);

                    cmd.Parameters.AddWithValue("@vkPwd", encVK_ByPassword);
                    cmd.Parameters.AddWithValue("@vkQ1", encVK_Q1);
                    cmd.Parameters.AddWithValue("@vkQ2", encVK_Q2);
                    cmd.Parameters.AddWithValue("@vkQ3", encVK_Q3);

                    try
                    {
                        // Execute insert; if username already exists, an exception occurs.
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (SQLiteException)
                    {
                        // Duplicate username or DB constraint failure
                        return false; 
                    }
                }
            }
        }


        // ===================== USER VALIDATION ============================== 

        // Confirms user login credentials by comparing hashes.
        public static bool ValidateUser(string username, string password, out int userId)
        {
            userId = -1; // Default invalid value

            using (var conn = new SQLiteConnection(connectionString)) //Creates a new connection
            {
                conn.Open();
                string query = "SELECT id, passwordHash, salt FROM Users WHERE username=@u";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username); // Binds the username parameter to avoid SQL injection.
                    using (var r = cmd.ExecuteReader())
                    {
                        // If no record found, user doesn’t exist.
                        if (!r.Read()) return false; // Executes the SQL query. r.Read() reads the first row. If it returns false,  no matching username found and login fails immediately.

                        int id = r.GetInt32(0); //internal user identifier
                        string storedHash = r.GetString(1); // SHA-256 hash of the user's password + salt
                        string salt = r.GetString(2); //unique per user

                        // Hash the provided password with the stored salt.
                        string inputHash = HashPassword(password, salt);

                        // Compare stored vs. computed hashes.
                        if (storedHash == inputHash)
                        {
                            userId = id; //caller gets the userId so the vault can be loaded for this user.
                            return true;
                        }
                        return false;
                    }
                }
            }
        }

        // =============== SECURITY QUESTIONS RETRIEVAL ======================

        // Fetches a user’s three security questions (but not answers).
        public static (int userId, string q1, string q2, string q3)? GetSecurityQuestions(string username)
        {
            using (var conn = new SQLiteConnection(connectionString)) //Opens a new connection to the SQLite database.
            {
                conn.Open();
                string sql = @"SELECT id, SecurityQuestion1, SecurityQuestion2, SecurityQuestion3
                               FROM Users WHERE username=@u";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    using (var r = cmd.ExecuteReader()) // Runs the query and returns a Reader object.
                    {
                        if (r.Read()) // Moves to the first row, returning true only if user exists.
                        {
                            return (r.GetInt32(0), //(index 0)
                                    // Converts null database values into empty strings ,Ensures no null reference exceptions.
                                    r["SecurityQuestion1"]?.ToString() ?? "", // string (nullable)
                                    r["SecurityQuestion2"]?.ToString() ?? "", // string 
                                    r["SecurityQuestion3"]?.ToString() ?? ""); //string 
                        }
                    }
                }
            }
            return null; //reader never finds a matching row (username doesn't exist)
        }

        // =============== SECURITY ANSWER VALIDATION ========================

        // Compares a given security answer with the stored hashed answer.
        // Returns true if the answer is correct, otherwise false.
        public static bool VerifySecurityAnswer(int userId, int questionIndex, string providedAnswer)
        {
            if (userId <= 0) return false; //A user ID must be positive and valid.

            // Determine which answer column to check based on index (1-3)
            string answerCol = questionIndex == 1 ? "SecurityAnswer1"
                               : questionIndex == 2 ? "SecurityAnswer2"
                               : "SecurityAnswer3";

            using (var conn = new SQLiteConnection(connectionString)) //Opens a secure connection
            {
                conn.Open();
                string sql = $"SELECT {answerCol}, salt FROM Users WHERE id=@id"; // Selects the correct hashed answer (SecurityAnswer1/2/3) and user salt
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    using (var r = cmd.ExecuteReader()) // Executes query.
                    {
                        if (!r.Read()) return false; // no row found
                        string storedAnswerHash = r[0]?.ToString() ?? ""; //Retrieves the hashed security answer from the database.
                        string salt = r["salt"]?.ToString() ?? ""; //Retrieves the salt, which was assigned at user creation.

                        // Normalize provided answer before hashing
                        string normalized = (providedAnswer ?? "").Trim().ToLowerInvariant();
                        string inputHash = HashPassword(normalized, salt); //Uses the same hashing logic as registration.
                        return storedAnswerHash == inputHash; // If the computed hash matches the stored hash then answer is correct.
                    }
                }
            }
        }


        // =============== PASSWORD RESET (PRESERVE VAULT) ====================

        // Allows a user to reset their master password using security answers 
        // without losing their encrypted passwords.
        // Each user has a Vault Key (VK) — a random 32-byte key used to encrypt/decrypt all vault passwords.
        // The VK is stored in four encrypted copies:
        // Encrypted using the master password, encrypted using security answer #1, encrypted using security answer #2, and encrypted using security answer #3
        // when a user forgets their master password,
        // they can still recover the vault by decrypting the VK using one of their correct security answers.

        // Cannot reset a password without a valid username or new password is empty.
        public static bool ResetPassword(string username, string newPassword, string providedAnswer) 
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(newPassword)) return false;

            // store the retrieved database values.
            int userId;
            string salt;
            string encVK_Q1, encVK_Q2, encVK_Q3;

            // Step 1: Retrieve the user’s stored recovery-wrapped VKs.
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT id, salt, EncryptedVaultKeyQ1, EncryptedVaultKeyQ2, EncryptedVaultKeyQ3
                               FROM Users WHERE username=@u";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username); //Bind username parameter safely
                    using (var r = cmd.ExecuteReader()) //If username not found, reset fails.
                    {
                        if (!r.Read()) return false;

                        // Extract values
                        userId = r.GetInt32(0);
                        salt = r["salt"]?.ToString() ?? "";
                        encVK_Q1 = r["EncryptedVaultKeyQ1"]?.ToString();
                        encVK_Q2 = r["EncryptedVaultKeyQ2"]?.ToString();
                        encVK_Q3 = r["EncryptedVaultKeyQ3"]?.ToString();
                    }
                }
            }

            // Step 2: Check which question’s answer is correct.
            string normalized = (providedAnswer ?? "").Trim().ToLowerInvariant();

            // Check all three answers in sequence:
            bool okQ1 = VerifySecurityAnswer(userId, 1, normalized);
            bool okQ2 = !okQ1 && VerifySecurityAnswer(userId, 2, normalized);
            bool okQ3 = !okQ1 && !okQ2 && VerifySecurityAnswer(userId, 3, normalized);

            if (!okQ1 && !okQ2 && !okQ3) return false; //If none match then reset fails

            // Step 3: Identify which encrypted VK copy that can be decrypted.
            // If Q1 matched, pick EncryptedVaultKeyQ1
            // Else if Q2 matched, pick EncryptedVaultKeyQ2
            // Else, use EncryptedVaultKeyQ3
            string encVK_FromAnswer = okQ1 ? encVK_Q1 : okQ2 ? encVK_Q2 : encVK_Q3;

            if (string.IsNullOrEmpty(encVK_FromAnswer))
            {
                return false;
            }

            // Step 4: Derive decryption key from answer + salt.
            // This recreates the exact key used during registration to encrypt VK.
            string wrapKeyFromAnswer = HashPassword(normalized, salt);

            // Step 5: Decrypt the VK (if answer verification succeeded)/ Decrypt VK using the answer-derived key
            string vaultKey;
            try
            {
                vaultKey = DecryptString(encVK_FromAnswer, wrapKeyFromAnswer);
            }
            catch
            {
                // Fails if data mismatch or decryption error.
                return false;
            }

            // Step 6: Re-encrypt the VK using the new master password.
            string encVK_ByNewPassword = EncryptString(vaultKey, newPassword);

            // Step 7: Recompute password hash (same salt preserved)
            string newPasswordHash = HashPassword(newPassword, salt);

            // Step 8: Commit updates to database atomically (transaction)
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        // Update master password hash and the EncryptedVaultKey wrapped by new password
                        string upd = @"UPDATE Users
                                       SET passwordHash=@phash, EncryptedVaultKey=@vkPwd
                                       WHERE id=@id";
                        using (var cmd = new SQLiteCommand(upd, conn, tx))
                        {
                            // Bind the variables
                            cmd.Parameters.AddWithValue("@phash", newPasswordHash);
                            cmd.Parameters.AddWithValue("@vkPwd", encVK_ByNewPassword);
                            cmd.Parameters.AddWithValue("@id", userId);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit(); //Commit transaction
                        return true;
                    }
                    catch
                    {
                        tx.Rollback(); //prevents partial updates that would corrupt the user’s account.
                        return false;
                    }
                }
            }
        }
    

        // This method takes a plaintext password, encrypts it using the user’s Vault Key (VK), and stores it into the database.
        // The Vault Key is the core cryptographic secret for the entire vault.
        public static void AddPassword(int userId, string site, string password, string masterPassword)
        {
            // Resolve the Vault Key using the user’s master password.
            // ResolveVaultKey attempts to decrypt the user’s EncryptedVaultKey field using the master password.
            string vaultKey = ResolveVaultKey(userId, masterPassword);
            if (vaultKey == null) throw new InvalidOperationException("Unable to resolve vault key.");

            // Encrypt the password with VK.
            string encryptedPassword = EncryptString(password, vaultKey);

            // Insert new record.
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Passwords (userId, site, password) VALUES (@uid, @site, @pwd)";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    // Creates a prepared SQL command
                    // Executes the insert into the table
                    // The Password column now stores encrypted ciphertext
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@site", site);
                    cmd.Parameters.AddWithValue("@pwd", encryptedPassword);
                    cmd.ExecuteNonQuery();
                }
            }
        }



        // Retrieve all password entries for a user and decrypt them using the user’s Vault Key (VK).
        public static List<(int id, string site, string password, bool IsFavorite, string category)> GetPasswords(int userId, string masterPassword)
        {
            var result = new List<(int, string, string, bool, string)>(); // Creates an empty list

            // Decrypt VK from EncryptedVaultKey using provided masterPassword
            string vaultKey = ResolveVaultKey(userId, masterPassword);
            if (vaultKey == null) return result;

            using (var conn = new SQLiteConnection(connectionString)) //Connects to SQLite
            {
                conn.Open();
                // Fetches all stored password records that belong to the given user.
                string sql = "SELECT id, site, password, IsFavorite, Category FROM Passwords WHERE userId=@uid";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId); //Protects against SQL injection.
                    using (var r = cmd.ExecuteReader()) //Every loop iteration processes one password entry.
                    {
                        while (r.Read())
                        {
                            // Extracts values
                            int id = r.GetInt32(0); //Converts SQLite integer to Boolean
                            string site = r.GetString(1);
                            string enc = r.GetString(2);
                            bool fav = r.GetInt32(3) == 1;
                            string category = r["Category"] != DBNull.Value ? r.GetString(4) : ""; //Handles NULL category values

                            // Decrypts the password: AES, vaultKey used as key, salt extracted from ciphertext
                            string plain;
                            try { plain = DecryptString(enc, vaultKey); }
                            catch { plain = "<decrypt error>"; } // avoid crashing UI

                            result.Add((id, site, plain, fav, category)); //Adds decrypted entry to return list
                        }
                    }
                }
            }
            return result; //Disposes database connection and returns complete list of decrypted entries.
        }



        // This method updates an existing password entry (both the site name and the encrypted password) in the database.
        // Because all stored passwords are encrypted using the Vault Key (VK), the method must retrieve the VK before re-encrypting the new password.
        public static void UpdatePasswordEntry(int entryId, string site, string password, string masterPassword)
        {
            // Retrieves the userId associated with the password entry
            // Uses the provided master password
            // Attempts to decrypt the user’s stored Vault Key (VK)
            string vaultKey = ResolveVaultKeyByEntry(entryId, masterPassword);
            if (vaultKey == null) throw new InvalidOperationException("Unable to resolve vault key."); //vault key cannot be resolved:

            string enc = EncryptString(password, vaultKey); //uses AES encryption

            using (var conn = new SQLiteConnection(connectionString)) //SQLite database connection
            {
                conn.Open();
                string sql = "UPDATE Passwords SET site=@s, password=@p WHERE id=@id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    //Executes the update in the database. Uses parameterized SQL to prevent SQL injection.
                    cmd.Parameters.AddWithValue("@s", site);
                    cmd.Parameters.AddWithValue("@p", enc);
                    cmd.Parameters.AddWithValue("@id", entryId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // This method permanently removes a password entry from the database.
        public static void DeletePassword(int id) //Uses the primary key id to identify which row to delete
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Passwords WHERE id=@id";
                using (var cmd = new SQLiteCommand(sql, conn)) //Creates the SQL command object
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery(); // Executes the DELETE statement
                }
            }
        }

        // This method updates the “favorite” status for a specific password entry.
        public static void UpdateFavoriteStatus(int id, bool isFavorite) //Updates only one password entry, identified by its unique id.
        {
            using (var conn = new SQLiteConnection(connectionString)) //open connection
            {
                conn.Open();
                string sql = "UPDATE Passwords SET IsFavorite=@f WHERE id=@id"; //Sets IsFavorite to a new value
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    // SQLite does not have a native boolean type. It stores booleans as integers. 1 = True 0 = False.
                    cmd.Parameters.AddWithValue("@f", isFavorite ? 1 : 0);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        // ====================== CATEGORY MANAGEMENT ========================

        // This method allows a user to create a new password category (e.g., Banking, Games, Work).
        // Categories help organize vault entries and make filtering easier.
        public static void AddCategory(int userId, string categoryName)
        {
            using (var conn = new SQLiteConnection(connectionString)) // open connection
            {
                conn.Open();
                //If the category already exists for that user (due to the table’s UNIQUE(UserId, CategoryName)),
                // SQLite will ignore the insert without error.
                string sql = "INSERT OR IGNORE INTO Categories (UserId, CategoryName) VALUES (@u, @c)";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    cmd.Parameters.AddWithValue("@c", categoryName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // This method deletes a category from the Categories table for a specific user.
        public static void DeleteCategory(int userId, string categoryName)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Categories WHERE UserId=@u AND CategoryName=@c";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    cmd.Parameters.AddWithValue("@c", categoryName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Retrieves all categories that belong to a specific user from the Categories table in the SQLite database.
        public static List<string> GetCategories(int userId)
        {
            var list = new List<string>(); //Creates an empty list that will be filled with category names read from the database.
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                // Selects only the CategoryName column
                string sql = "SELECT CategoryName FROM Categories WHERE UserId=@u ORDER BY CategoryName ASC";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read()) list.Add(r.GetString(0)); //Loop through all returned rows
                    }
                }
            }
            return list; //Return the results to caller
        }

        // Assigns (or clears) a category for a specific password entry in the Passwords table.
        public static void UpdatePasswordCategory(int passwordId, string category)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Passwords SET Category=@c WHERE id=@id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@c", category); //Binds the category value to the parameter
                    cmd.Parameters.AddWithValue("@id", passwordId); //ensures the UPDATE affects only one specific row, the correct password entry.
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // This method removes a category assignment from all passwords belonging to a specific user.
        public static void RemoveCategoryFromPasswords(int userId, string category)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Passwords SET Category=NULL WHERE userId=@u AND Category=@c"; //Sets Category to NULL (removes category assignment)
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    cmd.Parameters.AddWithValue("@c", category);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        // ======================== CRYPTOGRAPHY =============================
        // Handles AES encryption/decryption and SHA-based hashing. 
        // Each user’s vault data is encrypted with a unique Vault Key (VK), 
        // which is itself encrypted (wrapped) with their master password or security answers.


        //Encrypts a plaintext string using: AES symmetric encryption, A key derived from the provided password, and A fixed IV.
        private static string EncryptString(string plainText, string key)
        {
            byte[] iv = new byte[16]; // Creates a 16-byte IV filled with zeros
            using (Aes aes = Aes.Create()) //Creates an AES encryption provider
            {
                // Derive AES key using PBKDF2 from provided key string
                aes.Key = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes("staticSalt")).GetBytes(32);
                aes.IV = iv; //Assign the zero-filled IV

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV); //Create AES encryption engine
                byte[] buffer = Encoding.UTF8.GetBytes(plainText); //Convert plaintext to bytes
                return Convert.ToBase64String(encryptor.TransformFinalBlock(buffer, 0, buffer.Length)); //Perform encryption & return Base64
            }
        }

        private static string DecryptString(string cipherText, string key) //Does the opposite of EncryptString.
        {
            byte[] iv = new byte[16]; //Same zero-IV used for encryption.
            byte[] buffer = Convert.FromBase64String(cipherText); //Convert Base64 → raw encrypted bytes
            using (Aes aes = Aes.Create()) //Re-derive same AES key using PBKDF2
            {
                aes.Key = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes("staticSalt")).GetBytes(32);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV); //Creates the AES decryptor.
                return Encoding.UTF8.GetString(decryptor.TransformFinalBlock(buffer, 0, buffer.Length)); // AES decrypt to UTF8 plaintext
            }
        }

        // HashPassword uses SHA-256 to combine input + salt.
        // Hashes master password for login, security answers for verification, and keys derived from security questions
        private static string HashPassword(string input, string salt)
        {
            // Append salt before hashing. Makes hashes unique even if two users pick the same password.
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes((input ?? "") + salt);
                return Convert.ToBase64String(sha.ComputeHash(bytes)); //Stored in database.
            }
        }

        // GenerateSalt creates 16 random bytes and encodes to Base64.
        private static string GenerateSalt()
        {
            byte[] salt = new byte[16]; //Create 16 bytes of random data
            using (var rng = RandomNumberGenerator.Create()) //Cryptographically secure RNG
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt); //Store salt as Base64
        }

        // Generates the Vault Key, a strong random symmetric key used for: encrypting user passwords and decrypting vault entries.
        private static string GenerateVaultKeyBase64(int numBytes)
        {
            byte[] key = new byte[numBytes]; //Number of bytes requested.
            using (var rng = RandomNumberGenerator.Create()) //Secure random key
            {
                rng.GetBytes(key);
            }
            return Convert.ToBase64String(key); //Store VK as Base64 in memory
        }


        // ====================== INTERNAL VAULT HELPERS =====================


        // retrieves and decrypts the user's Vault Key (VK), which is required for:
        // Encrypting passwords when adding a new entry
        // Decrypting passwords when loading vault entries
        // Updating passwords
        // Editing categories

        private static string ResolveVaultKey(int userId, string masterPassword)
        {
            using (var conn = new SQLiteConnection(connectionString)) //connection to SQL
            {
                conn.Open();
                string sql = "SELECT EncryptedVaultKey FROM Users WHERE id=@id"; //Requests only the encrypted vault key associated with this specific user.
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    object o = cmd.ExecuteScalar(); //returns the first column of the first row
                    if (o == null || o == DBNull.Value) return null; //If no encrypted VK exists then cannot decrypt anything. Return null.
                    string encVK = (string)o; //Encrypted VK is stored in DB as a Base64 string
                    try { return DecryptString(encVK, masterPassword); } //Attempt to decrypt using the user’s master password
                    catch { return null; } //prevents the entire program from crashing and tells calling code that the vault cannot be decrypted.
                }
            }
        }

        // This method allows the program to decrypt a password when you only know the password entry’s ID, not the user’s ID.
        //
        private static string ResolveVaultKeyByEntry(int entryId, string masterPassword)
        {
            int userId = -1; //Initializes userId. Default value is -1 to indicate “not found.” Will be replaced by the real userId if lookup succeeds.
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                //Creates SQL command to fetch the owner of this entry
                using (var cmd = new SQLiteCommand("SELECT userId FROM Passwords WHERE id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", entryId);
                    var o = cmd.ExecuteScalar(); //returns userId (as an integer or long) or Null
                    if (o == null || o == DBNull.Value) return null; //If the entry does not exist, vault cannot be resolved
                    userId = Convert.ToInt32(o); //Converts the returned value into an integer
                }
            }
            //Now that the correct userId is had, delegate to ResolveVaultKey(userId, masterPassword)
            // The function does the following:
            // Fetches the encrypted VK from the Users table
            // Decrypts it using the master password
            // Returns the real Vault Key or null
            return ResolveVaultKey(userId, masterPassword);
        }
    }
}

























