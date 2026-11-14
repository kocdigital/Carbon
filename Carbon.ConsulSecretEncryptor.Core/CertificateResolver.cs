using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Carbon.ConsulSecretEncryptor.Core
{
    /// <summary>Describes how to locate the certificate used for encryption/decryption.</summary>
    public sealed class CertificateSelection
    {
        /// <summary>Path to a .cer (public only) or .pfx (with private key) file. Takes precedence over store lookup.</summary>
        public string CertFilePath { get; set; }
        public string CertFilePassword { get; set; }
        /// <summary>Exact thumbprint. Takes precedence over <see cref="SubjectName"/> if both are set.</summary>
        public string Thumbprint { get; set; }
        /// <summary>Subject/CN to search by; picks the most recently issued currently-valid match.</summary>
        public string SubjectName { get; set; }
        public StoreLocation StoreLocation { get; set; } = StoreLocation.CurrentUser;
        public StoreName StoreName { get; set; } = StoreName.My;
    }

    /// <summary>Locates an X.509 certificate from a file or the Windows certificate store.</summary>
    public static class CertificateResolver
    {
        public static X509Certificate2 Resolve(CertificateSelection selection, bool requirePrivateKey)
        {
            if (selection == null)
                throw new ArgumentNullException(nameof(selection));

            if (!string.IsNullOrWhiteSpace(selection.CertFilePath))
            {
                var certificate = string.IsNullOrEmpty(selection.CertFilePassword)
                    ? new X509Certificate2(selection.CertFilePath)
                    : new X509Certificate2(selection.CertFilePath, selection.CertFilePassword);
                if (requirePrivateKey && !certificate.HasPrivateKey)
                    throw new InvalidOperationException($"'{selection.CertFilePath}' does not contain a private key. Supply a .pfx (with its password) to decrypt.");
                return certificate;
            }

            var hasThumbprint = !string.IsNullOrWhiteSpace(selection.Thumbprint);
            var hasSubject = !string.IsNullOrWhiteSpace(selection.SubjectName);
            if (!hasThumbprint && !hasSubject)
                throw new ArgumentException("Specify a certificate file, thumbprint, or subject to locate the certificate.");

            using var store = new X509Store(selection.StoreName, selection.StoreLocation);
            store.Open(OpenFlags.ReadOnly);

            if (hasThumbprint)
            {
                var found = store.Certificates.Find(X509FindType.FindByThumbprint, selection.Thumbprint, validOnly: false);
                if (found.Count == 0)
                    throw new InvalidOperationException($"Certificate with thumbprint '{selection.Thumbprint}' was not found in {selection.StoreLocation}/{selection.StoreName}.");
                if (requirePrivateKey && !found[0].HasPrivateKey)
                    throw new InvalidOperationException($"Certificate with thumbprint '{selection.Thumbprint}' in {selection.StoreLocation}/{selection.StoreName} does not have an accessible private key.");
                return found[0];
            }

            // Prefer the most recently issued, currently valid certificate matching the subject.
            var bySubject = store.Certificates
                .Find(X509FindType.FindBySubjectName, selection.SubjectName, validOnly: false)
                .OfType<X509Certificate2>()
                .Where(c => (!requirePrivateKey || c.HasPrivateKey) && DateTime.Now >= c.NotBefore && DateTime.Now <= c.NotAfter)
                .OrderByDescending(c => c.NotBefore)
                .ToList();
            if (bySubject.Count == 0)
                throw new InvalidOperationException($"No currently valid certificate with subject '{selection.SubjectName}'{(requirePrivateKey ? " and an accessible private key" : string.Empty)} was found in {selection.StoreLocation}/{selection.StoreName}.");
            return bySubject[0];
        }
    }
}
