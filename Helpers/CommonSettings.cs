using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Cryptography;
using System.Text;

namespace CRUDWithAuth.Helpers
{
    public class CommonSettings
    {
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "AASR098989895550$%^&*MYQ";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string IPAddress(IActionContextAccessor _accessor)
        {
            string ipadd = "";
            try
            {
                String ip = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
                //HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                //if (string.IsNullOrEmpty(ip))
                //{
                //    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                //}
                ipadd = ip;
            }
            catch (Exception ex)
            {

            }
            return ipadd;
        }
    }
}
