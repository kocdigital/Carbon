using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Carbon.WebApplication
{
    /// <summary>
    /// Decrypts a Consul ACL token that was encrypted (RSA-OAEP/SHA256) against the public key of a
    /// certificate installed on the target machine. The plaintext token is never stored as an environment
    /// variable; only the ciphertext travels via configuration/environment, and it can only be decrypted
    /// on a machine that holds the matching certificate's private key.
    /// </summary>
    internal static class ConsulAclTokenProtector
    {
        /// <param name="thumbprint">Exact certificate thumbprint. Takes precedence over <paramref name="subjectName"/> if both are supplied.</param>
        /// <param name="subjectName">Certificate subject/CN to search by. Preferred over thumbprint when the certificate is rotated periodically, since the subject can stay the same across renewals.</param>
        public static string Decrypt(string base64CipherText, string thumbprint, string subjectName, StoreName storeName, StoreLocation storeLocation)
        {
            if (string.IsNullOrWhiteSpace(base64CipherText))
                throw new ArgumentException("Encrypted Consul ACL token cannot be empty.", nameof(base64CipherText));

            if (string.IsNullOrWhiteSpace(thumbprint) && string.IsNullOrWhiteSpace(subjectName))
                throw new ArgumentException("Either a certificate thumbprint or a subject name must be supplied to locate the decryption certificate.");

            using var certificate = FindCertificate(thumbprint, subjectName, storeName, storeLocation);
            using var rsa = certificate.GetRSAPrivateKey()
                ?? throw new InvalidOperationException($"Certificate '{certificate.Thumbprint}' does not expose an accessible RSA private key. Import it with its private key to decrypt the Consul ACL token.");

            var cipherBytes = Convert.FromBase64String(base64CipherText);
            var plainBytes = rsa.Decrypt(cipherBytes, RSAEncryptionPadding.OaepSHA256);
            try
            {
                return Encoding.UTF8.GetString(plainBytes);
            }
            finally
            {
                Array.Clear(plainBytes, 0, plainBytes.Length);
            }
        }

        private static X509Certificate2 FindCertificate(string thumbprint, string subjectName, StoreName storeName, StoreLocation storeLocation)
        {
            using var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            if (!string.IsNullOrWhiteSpace(thumbprint))
            {
                var byThumbprint = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false);
                if (byThumbprint.Count == 0)
                    throw new InvalidOperationException($"Certificate with thumbprint '{thumbprint}' was not found in {storeLocation}/{storeName}. It must be installed on this machine (with its private key) to decrypt the Consul ACL token.");

                return byThumbprint[0];
            }

            // Subject lookup supports certificate renewal without touching configuration: pick the
            // currently valid match with a private key, preferring the most recently issued one.
            var bySubject = store.Certificates
                .Find(X509FindType.FindBySubjectName, subjectName, validOnly: false)
                .OfType<X509Certificate2>()
                .Where(c => c.HasPrivateKey && DateTime.Now >= c.NotBefore && DateTime.Now <= c.NotAfter)
                .OrderByDescending(c => c.NotBefore)
                .ToList();

            if (bySubject.Count == 0)
                throw new InvalidOperationException($"No currently valid certificate with subject '{subjectName}' and an accessible private key was found in {storeLocation}/{storeName}.");

            return bySubject[0];
        }
    }
}
