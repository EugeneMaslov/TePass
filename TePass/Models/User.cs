using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TePass.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Test> Tests { get; set; }
        public override bool Equals(object obj)
        {
            User user = obj as User;
            return this.Login == user.Login && this.GetHash(Password) == user.Password;
        }
        public string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
