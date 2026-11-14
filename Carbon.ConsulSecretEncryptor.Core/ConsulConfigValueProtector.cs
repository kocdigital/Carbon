using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;

namespace Carbon.ConsulSecretEncryptor.Core
{
    /// <summary>
    /// Decrypts Consul-sourced JSON configuration in place, at two possible granularities that can be mixed
    /// freely:
    /// 1) Whole-document: the entire document is stored as a single wrapper key (<see cref="ConsulSecretCrypto.EncryptedDocumentKey"/>).
    /// 2) Field-level: individual values are stored as "ENC(&lt;base64 envelope&gt;)" and decrypted one at a time,
    ///    leaving the rest of the structure (key names, non-sensitive values) readable in Consul.
    /// Shared by Carbon.ConsoleApplication.Extensions and Carbon.WebApplication so neither has its own copy.
    /// </summary>
    public static class ConsulConfigValueProtector
    {
        private static readonly Regex EncryptedFieldPattern = new Regex(@"^ENC\((?<envelope>[A-Za-z0-9+/=]+)\)$", RegexOptions.Compiled);

        /// <summary>
        /// Decrypts the whole-document key (if present) and any field-level "ENC(...)" values (if present), merging
        /// everything into <paramref name="configuration"/> in place. The certificate is only resolved (and
        /// required) when at least one of these is actually found, so apps that don't use this feature are
        /// unaffected.
        /// </summary>
        public static void DecryptInPlace(IConfiguration configuration, Func<CertificateSelection> resolveCertSelection)
        {
            var wholeDocumentEnvelope = configuration[ConsulSecretCrypto.EncryptedDocumentKey];
            var encryptedFields = configuration.AsEnumerable()
                .Where(kvp => kvp.Value != null && EncryptedFieldPattern.IsMatch(kvp.Value))
                .ToList();

            if (string.IsNullOrEmpty(wholeDocumentEnvelope) && encryptedFields.Count == 0)
                return;

            var selection = resolveCertSelection();
            using var certificate = CertificateResolver.Resolve(selection, requirePrivateKey: true);
            using var rsa = certificate.GetRSAPrivateKey()
                ?? throw new InvalidOperationException($"Certificate '{certificate.Thumbprint}' does not expose an accessible RSA private key. Import it with its private key to decrypt the Consul configuration.");

            var certificateSource = selection.CertFilePath ?? selection.Thumbprint ?? selection.SubjectName;

            if (!string.IsNullOrEmpty(wholeDocumentEnvelope))
            {
                var decryptedJson = ConsulSecretCrypto.DecryptEnvelope(wholeDocumentEnvelope, rsa);

                using var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(decryptedJson));
                var decryptedConfig = new ConfigurationBuilder().AddJsonStream(jsonStream).Build();

                var mergedCount = 0;
                foreach (var entry in decryptedConfig.AsEnumerable())
                {
                    if (entry.Value == null)
                        continue;
                    configuration[entry.Key] = entry.Value;
                    mergedCount++;
                }

                // Clear the still-encrypted placeholder so a reload-triggered re-scan doesn't re-decrypt it.
                configuration[ConsulSecretCrypto.EncryptedDocumentKey] = null;
                Console.WriteLine($"Consul Config: decrypted whole-document config ({mergedCount} value(s)) via certificate.");
            }

            if (encryptedFields.Count > 0)
            {
                foreach (var field in encryptedFields)
                {
                    var fieldEnvelope = EncryptedFieldPattern.Match(field.Value).Groups["envelope"].Value;
                    configuration[field.Key] = ConsulSecretCrypto.DecryptEnvelope(fieldEnvelope, rsa);
                }
                Console.WriteLine($"Consul Config: decrypted {encryptedFields.Count} field-level ENC(...) value(s) via certificate.");
            }
        }
    }
}
