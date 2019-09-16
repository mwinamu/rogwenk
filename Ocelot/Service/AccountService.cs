using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ocelot.Models;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.IO;

namespace Ocelot.Service
{
    public static class AccountService
    {
        private static string DBConn = Constant.DbConn;
        public static void EncryptStringToBytes(string plainText,
            out Byte[] Password, out Byte[] Keys, out Byte[] Four)
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                myRijndael.Padding = PaddingMode.ISO10126;
                myRijndael.GenerateKey();
                myRijndael.GenerateIV();
                byte[] Key = myRijndael.Key;
                byte[] IV = myRijndael.IV;
                // Check arguments.
                if (plainText == null || plainText.Length <= 0)
                    throw new ArgumentNullException("plainText");
                if (Key == null || Key.Length <= 0)
                    throw new ArgumentNullException("Key");
                if (IV == null || IV.Length <= 0)
                    throw new ArgumentNullException("IV");
                byte[] encrypted;
                //string EncyptedPass = String.Empty;
                // Create an RijndaelManaged object
                // with the specified key and IV.
                using (RijndaelManaged rijAlg = new RijndaelManaged())
                {
                    rijAlg.Key = Key;
                    rijAlg.IV = IV;

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {

                                //Write all data to the stream.
                                swEncrypt.Write(plainText);
                            }
                            encrypted = msEncrypt.ToArray();
                            Password = encrypted;
                            Keys = Key;
                            Four = IV;
                        }
                    }
                }
            }

        }

        public static bool IsUser(LoginModel login) 
        {
            bool IsUser = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    string querry = "select * from Users where email = @Email";
                    
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@Email", login.Email);
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.Read())
                        {
                            IsUser = true;
                        }
                    }
                }
            }catch(Exception e)

            {
                IsUser = false;
            }
            
            return IsUser;
        }
        public static bool IsLoggedIn(LoginModel login)
        {
            var user = User(login.Email);
            return IsCorrectPassword(login.Password, user.Password, user.Keys, user.IV);
        }
        public static UserCredential User(string Email)
        {
            var user = new UserCredential();
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                string ulizia = $@"select a.UserID,a.Email,
    b.Password,b.Keys,b.IV from users as a
     inner join UserCredential as b on b.UserID = a.UserID
             where Email = @Email";
                using (SqlCommand cmd = new SqlCommand(ulizia,conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Email", Email);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        user.UserID = Convert.ToInt32(rdr["UserID"]);
                        //user.PayrollNumber = rdr["PayrollNumber"].ToString();
                        user.Email = rdr["Email"].ToString();
                        user.Password = (byte[])rdr["Password"];
                        user.Keys = (byte[])rdr["Keys"];
                        user.IV = (byte[])rdr["IV"];
                    }
                }
            }
            return user;

        }
        public static bool IsCorrectPassword(string InputPassword, byte[] Password, byte[] Key, byte[] IV)
        {
            bool check = false;
            // Check arguments.
            if (Password == null || Password.Length <= 0)
                throw new ArgumentNullException("Password");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Padding = PaddingMode.ISO10126;
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Password))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            
                            plaintext = srDecrypt.ReadLine();
                            plaintext = plaintext.Trim();
                            plaintext.Replace("\\", string.Empty);
                            plaintext.Trim('"');
                            if (plaintext.Equals(InputPassword))
                            {
                                check = true;
                            }
                        }
                    }
                }

            }

            return check;

        }

    }
}