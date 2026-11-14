using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Carbon.ConsulSecretEncryptor.Core
{
    /// <summary>
    /// Encrypts/decrypts Consul secrets consumed by Carbon.ConsoleApplication / Carbon.WebApplication: plain
    /// RSA-OAEP/SHA256 for short secrets (ACL token, "username:password" Basic Auth), and a hybrid
    /// AES-256-GCM + RSA-OAEP/SHA256 envelope for values that can exceed RSA's ~190-byte limit (whole JSON
    /// documents, or individual "ENC(...)" fields).
    /// </summary>
    public static class ConsulSecretCrypto
    {
        /// <summary>Must match <c>ConsulConfigValueProtector.EncryptedDocumentKey</c> in Carbon.ConsoleApplication/Carbon.WebApplication.</summary>
        public const string EncryptedDocumentKey = "CarbonEncryptedConfig";

        private const int NonceSizeBytes = 12;
        private const int TagSizeBytes = 16;

        public static string EncryptPlain(string secret, RSA rsaPublicKey) =>
            Convert.ToBase64String(rsaPublicKey.Encrypt(Encoding.UTF8.GetBytes(secret), RSAEncryptionPadding.OaepSHA256));

        public static string DecryptPlain(string base64CipherText, RSA rsaPrivateKey)
        {
            var plainBytes = rsaPrivateKey.Decrypt(Convert.FromBase64String(base64CipherText), RSAEncryptionPadding.OaepSHA256);
            try
            {
                return Encoding.UTF8.GetString(plainBytes);
            }
            finally
            {
                Array.Clear(plainBytes, 0, plainBytes.Length);
            }
        }

        public static string EncryptBasicAuth(string username, string password, RSA rsaPublicKey) =>
            EncryptPlain($"{username}:{password}", rsaPublicKey);

        /// <summary>Splits a decrypted Basic Auth value (RFC 7617 "username:password" format) on the first ':'.</summary>
        public static (string Username, string Password) SplitBasicAuthCredential(string decrypted)
        {
            var separatorIndex = decrypted.IndexOf(':');
            if (separatorIndex <= 0)
                throw new ArgumentException("Decrypted value must be in \"username:password\" format.", nameof(decrypted));
            return (decrypted.Substring(0, separatorIndex), decrypted.Substring(separatorIndex + 1));
        }

        public static void ValidateJson(string jsonText)
        {
            try
            {
                using var _ = JsonDocument.Parse(jsonText);
            }
            catch (JsonException ex)
            {
                throw new ArgumentException($"Content is not valid JSON: {ex.Message}", ex);
            }
        }

        /// <summary>Encrypts a whole JSON document, returning the ready-to-paste {"CarbonEncryptedConfig":"..."} wrapper.</summary>
        public static string EncryptJsonDocument(string jsonText, RSA rsaPublicKey)
        {
            ValidateJson(jsonText);
            return $"{{\"{EncryptedDocumentKey}\":\"{EncryptEnvelope(jsonText, rsaPublicKey)}\"}}";
        }

        /// <summary>Decrypts a whole JSON document. Accepts either the raw envelope or the full wrapper JSON.</summary>
        public static string DecryptJsonDocument(string input, RSA rsaPrivateKey) =>
            DecryptEnvelope(ExtractDocumentEnvelope(input), rsaPrivateKey);

        /// <summary>Encrypts a single value, returning it wrapped as "ENC(...)".</summary>
        public static string EncryptField(string secret, RSA rsaPublicKey) => $"ENC({EncryptEnvelope(secret, rsaPublicKey)})";

        /// <summary>Decrypts a single field. Accepts either the "ENC(...)"-wrapped value or the raw envelope.</summary>
        public static string DecryptField(string input, RSA rsaPrivateKey) => DecryptEnvelope(ExtractFieldEnvelope(input), rsaPrivateKey);

        public static string ExtractFieldEnvelope(string input)
        {
            var trimmed = input.Trim();
            return trimmed.StartsWith("ENC(", StringComparison.Ordinal) && trimmed.EndsWith(")", StringComparison.Ordinal)
                ? trimmed.Substring(4, trimmed.Length - 5)
                : trimmed;
        }

        public static string ExtractDocumentEnvelope(string input)
        {
            var trimmed = input.Trim();
            if (!trimmed.StartsWith("{", StringComparison.Ordinal))
                return trimmed;

            using var document = JsonDocument.Parse(trimmed);
            if (!document.RootElement.TryGetProperty(EncryptedDocumentKey, out var envelopeProperty))
                throw new ArgumentException($"JSON input does not contain a '{EncryptedDocumentKey}' property.");
            return envelopeProperty.GetString();
        }

        /// <summary>RSA-OAEP/SHA256-wraps a fresh random AES-256-GCM key and uses it to encrypt <paramref name="plaintext"/>.</summary>
        public static string EncryptEnvelope(string plaintext, RSA rsaPublicKey)
        {
            var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            var aesKey = new byte[32];
            var nonce = new byte[NonceSizeBytes];
            RandomNumberGenerator.Fill(aesKey);
            RandomNumberGenerator.Fill(nonce);

            var ciphertext = new byte[plaintextBytes.Length];
            var tag = new byte[TagSizeBytes];
            try
            {
#if NET8_0_OR_GREATER
                using (var aesGcm = new AesGcm(aesKey, TagSizeBytes))
#else
                using (var aesGcm = new AesGcm(aesKey))
#endif
                    aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);

                var encryptedKey = rsaPublicKey.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA256);

                using var ms = new MemoryStream();
                using (var writer = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
                {
                    writer.Write(encryptedKey.Length);
                    writer.Write(encryptedKey);
                    writer.Write(nonce);
                    writer.Write(tag);
                    writer.Write(ciphertext);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
            finally
            {
                Array.Clear(aesKey, 0, aesKey.Length);
            }
        }

        /// <summary>Reverses <see cref="EncryptEnvelope"/>.</summary>
        public static string DecryptEnvelope(string base64Envelope, RSA rsaPrivateKey)
        {
            using var reader = new BinaryReader(new MemoryStream(Convert.FromBase64String(base64Envelope)), Encoding.UTF8);
            var encryptedKey = reader.ReadBytes(reader.ReadInt32());
            var nonce = reader.ReadBytes(NonceSizeBytes);
            var tag = reader.ReadBytes(TagSizeBytes);
            var ciphertext = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));

            var aesKey = rsaPrivateKey.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);
            try
            {
                var plaintext = new byte[ciphertext.Length];
#if NET8_0_OR_GREATER
                using (var aesGcm = new AesGcm(aesKey, TagSizeBytes))
#else
                using (var aesGcm = new AesGcm(aesKey))
#endif
                    aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);

                return Encoding.UTF8.GetString(plaintext);
            }
            finally
            {
                Array.Clear(aesKey, 0, aesKey.Length);
            }
        }
    }
}
