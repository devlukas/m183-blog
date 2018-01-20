using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using M183.Blog.Models;

namespace M183.Blog.Manager
{
    public class UserManager
    {
        private BlogDbContext db = new BlogDbContext();

        /// <summary>
        /// Returns UserViewModel by username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>User View Model</returns>
        public User GetUser(string username)
        {
            return db.Users.First(u => u.Username == username);
        }

        /// <summary>
        /// Registers a new user to the database
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
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
                    Blocked = false,
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

        /// <summary>
        /// Login a User with a Username, Password
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<bool> BaseLogin(LoginViewModel login)
        {
            if (!await IsUserBlocked(login.Username))
            {
                // Check if username and password is valid
                if (await ValidateCredentials(login.Username, login.Password))
                {
                    return true;
                }
                await AddUserLogAsync(login.Username, "WrongPasswordOrToken: Für dieses Konto wurde ein falsches Passwort eingegeben!");
                await BlockUserIfToManySuccessiveWrongAttempts(login.Username);
                throw new BlogError("Das angegebene Passwort oder der Benutzername ist nicht gültig!", BlogErrorType.WrongUsernameOrPassword);

            }
            await AddUserLogAsync(login.Username, "Der Benutzer hat sich versucht einzuloggen obwohl dieser bereits blockiert ist.");
            throw new BlogError("Dieser Benutzer ist blockiert!", BlogErrorType.UserBlocked);
        }

        /// <summary>
        /// Login a User with a Username, Password and Token
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<bool> LoginWithTokenAsync(LoginViewModel login)
        {
            // Basic Userlogin Check (Username & Password)
            if (await BaseLogin(login))
            {
                var token =
                    // search a token string with this content, assigned to this user and which is not expired
                    await db.Tokens.Include(t => t.User).FirstOrDefaultAsync(
                        t =>
                            t.Tokenstring == login.SmsToken && t.Expiry > DateTime.Now &&
                            t.User.Username == login.Username && t.DeletedDate == null);

                // TOD: REMOVE THIS AFTER DEVELOPMENT
                if (login.SmsToken == "test")
                {
                    return true;
                }

                if (token != null)
                {
                    token.DeletedDate = DateTime.Now;
                    await db.SaveChangesAsync();
                    await AddUserLogAsync(login.Username, "Successful Login");
                    return true;
                }
                await AddUserLogAsync(login.Username, "WrongPasswordOrToken: Das angegebene Token ist nicht gültig oder bereits abgelaufen!");
                await BlockUserIfToManySuccessiveWrongAttempts(login.Username);
                throw new BlogError("Das angegebene Token ist nicht gültig!", BlogErrorType.TokenNotValid);
            }
            return false;
        }

        /// <summary>
        /// Returns if a user is blocked or not
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<bool> IsUserBlocked(string username)
        {
            return await db.Users.AnyAsync(u => u.Username == username && u.Blocked);
        }

        /// <summary>
        /// Gets the Role of a given user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetUserRole(string username)
        {
            return db.Users.Include(u => u.Role).First(u => u.Username == username).Role.Key;
        }

        /// <summary>
        /// Checks if a user has a given role
        /// </summary>
        /// <param name="username"></param>
        /// <param name="roleKey"></param>
        /// <returns></returns>
        public bool HasRoles(string username, string roleKey)
        {
            return db.Users.Any(u => u.Username == username && u.Role.Key == roleKey);
        }

        /// <summary>
        /// Blocks the user if he has too many successive wrong attempts
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task BlockUserIfToManySuccessiveWrongAttempts(string username)
        {
            // get the last successful login-date
            var lastSuccessfulLogin = db.Userlogins.Where(u => u.User.Username == username).OrderByDescending(u => u.Metadata.CreationDate).FirstOrDefault();
            DateTime? lastLoginDate = lastSuccessfulLogin?.Metadata.CreationDate;

            // search for log-entries with the key 'WrongPasswordOrToken' since the last successful-login and count them
            int successiveFailedAttempts = await db.Userlogs.CountAsync(u =>
                u.Message.Contains("WrongPasswordOrToken") &&
                (lastLoginDate == null || u.Metadata.CreationDate > lastLoginDate));

            // if there were 3 or more failed attempts
            if (successiveFailedAttempts >= 3)
            {
                // get user and block him
                var user = await db.Users.FirstAsync(u => u.Username == username);
                user.Blocked = true;
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Checks if a given username and password is existing for one user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> ValidateCredentials(string username, string password)
        {
            string encryptedPassword = EncryptPassword(password);
            bool userExists = await db.Users.AnyAsync(u => u.Username == username && u.Password == encryptedPassword);
            return userExists;
        }

        /// <summary>
        /// Generates and sesnds a new login token to the mobilephone of a given user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task GenerateAndSendLoginTokenAsync(string username)
        {
            User user = await db.Users.FirstAsync(u => u.Username == username);

            // generate token
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
            // post the above data to the nexmo api
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

        /// <summary>
        /// Adds a new User-Login Entry for a given Username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="sessionId"></param>
        /// <param name="ipAdress"></param>
        /// <returns></returns>
        public async Task AddUserLoginAsync(string username, string sessionId, string ipAdress)
        {
            var userLogin = new Userlogin
            {
                User = await db.Users.FirstAsync(u => u.Username == username),
                Metadata = new Metadata(username),
                SessionId = sessionId,
                UserIpAdress = ipAdress
            };
            db.Userlogins.Add(userLogin);
            await db.SaveChangesAsync();

        }

        /// <summary>
        /// Adds a new User-Log Entry for a given Username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task AddUserLogAsync(string username, string message)
        {
            var userLog = new Userlog
            {
                User = db.Users.FirstOrDefault(u => u.Username == username),
                Message = message,
                Metadata = new Metadata(username)
            };
            db.Userlogs.Add(userLog);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Writes necessary data to the database to complete the login process
        /// </summary>
        /// <param name="username"></param>
        public async Task Logout(string username)
        {
            var userlogin =
                await db.Userlogins.FirstOrDefaultAsync(u => u.DeletedDate == null && u.User.Username == username);
            userlogin.DeletedDate = DateTime.Now;
            await this.AddUserLogAsync(username, "Logout");
        }

        /// <summary>
        /// Encrypts a given Password with SHA256
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string EncryptPassword(string password)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            return System.Text.Encoding.ASCII.GetString(data);
        }

        /// <summary>
        /// Generates a random token
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private string RandomString(int length)
        {
            var random = new Random();
            // Randomly generates a string with this letters and digits
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}