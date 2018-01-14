using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using M183.Blog.Models;

namespace M183.Blog.Manager
{
    public class UserManager
    {
        private BlogDbContext db = new BlogDbContext();

        public async Task RegisterAsync(RegisterViewModel viewModel)
        {
            bool userExists = db.Users.Any(u => u.Username == viewModel.Username);
            if (!userExists)
            {
                User newUser = new User
                {
                    Email = viewModel.Email,
                    Username = viewModel.Username,
                    Firstname = viewModel.Firstname,
                    Lastname = viewModel.Lastname,
                    Metadata = new Metadata(viewModel.Username),
                    MobileNumber = viewModel.MobilePhoneNumber,
                    Password = EncryptPassword(viewModel.Password),
                    // Assign Default-Role
                    Role = db.Roles.FirstOrDefault(r => r.Key == "Default")
                };
                db.Users.Add(newUser);
                await db.SaveChangesAsync();
            }
            else
            {
                throw new BlogError("Dieser Benutzer existiert bereits!");
            }
        }

        public async Task<bool> LoginAsync(LoginViewModel login)
        {
            if (await ValidateCredentials(login.Username, login.Password))
            {
                bool tokenValid =
                    // if there is a token string with this content, assigned to this user and which is not expired
                    await db.Tokens.Include(t => t.User).AnyAsync(
                            t =>
                                t.Tokenstring == login.SmsToken && t.Expiry > DateTime.Now &&
                                t.User.Username == login.Username);

                if (tokenValid) return true;
                return false;
            }
            return false;
        }

        public async Task<bool> ValidateCredentials(string username, string password)
        {
            string encryptedPassword = EncryptPassword(password);
            bool userExists = await db.Users.AnyAsync(u => u.Username == username && u.Password == encryptedPassword);
            return userExists;
        }

        public async Task GenerateAndSendLoginTokenAsync(string username)
        {
            User user = await db.Users.FirstAsync(u => u.Username == username);
            string token = RandomString(4);

            HttpClient client = new HttpClient();
            var values = new Dictionary<string, string>
            {
               { "api_key", "d5d0152f" },
               { "api_secret", "ee4863d0896a62d6" },
               { "to", user.MobileNumber },
               { "from", "GIBZ" },
               { "text", $"Ihr Token lautet:\n{token}"}
            };
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://rest.nexmo.com/sms/json", content);
            if (response.IsSuccessStatusCode)
            {
                // create and store token
                var newToken = new Token
                {
                    Expiry = DateTime.Now.AddMinutes(5),
                    Tokenstring = token,
                    User = user
                };
                db.Tokens.Add(newToken);
                await db.SaveChangesAsync();
                // log
                await AddUserLogAsync(username, $"Login Token \"{token}\" generated and sent to phone-number \"{user.MobileNumber}\"");
            }
            else
            {
                throw new BlogError("Fehler beim Versenden des 2-Faktor-Authentifizierungs-Tokens. Ist Ihre Handynummer richtig?");
            }
        }

        public async Task AddUserLogAsync(string username, string message)
        {
            var userLog = new Userlog
            {
                User = db.Users.First(u => u.Username == username),
                Message = message
            };
            db.Userlogs.Add(userLog);
            await db.SaveChangesAsync();
        }

        private string EncryptPassword(string password)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            return System.Text.Encoding.ASCII.GetString(data);
        }

        private string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}