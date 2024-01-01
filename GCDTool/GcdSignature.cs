using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using System.Security.Cryptography;

namespace GCDTool;

/// <summary>
/// Class representing a GCD signature.
/// </summary>
sealed class GcdSignature
{
    /// <summary>
    /// The length of the signature data before signing.
    /// </summary>
    public const int SIGNATURE_LENGTH = 0x74;

    /// <summary>
    /// The length of the signature data after signing.
    /// </summary>
    public const int SIGNATURE_SIGNED_LENGTH = 0x80;

    /// <summary>
    /// AES key Y used for encrypting the ARM 9 and ARM 7 binaries.
    /// </summary>
    public byte[] AesKeyY { get; } = new byte[16];

    /// <summary>
    /// The SHA-1 hash of the 0x180 bytes consisting of header[0..0x100] and header[0x180..0x200].
    /// </summary>
    public byte[] HeaderSha1 { get; } = new byte[20];

    /// <summary>
    /// The SHA-1 hash of the original ARM 9 binary (without padding and AES encryption).
    /// </summary>
    public byte[] Arm9Sha1 { get; } = new byte[20];

    /// <summary>
    /// The SHA-1 hash of the original ARM 7 binary (without padding and AES encryption).
    /// </summary>
    public byte[] Arm7Sha1 { get; } = new byte[20];

    /// <summary>
    /// Serializes the signature as a byte array.
    /// </summary>
    /// <returns>The signature as byte array.</returns>
    public byte[] ToByteArray()
    {
        var signature = new byte[SIGNATURE_LENGTH];
        AesKeyY.CopyTo(signature, 0x00);
        HeaderSha1.CopyTo(signature, 0x10);
        Arm9Sha1.CopyTo(signature, 0x24);
        Arm7Sha1.CopyTo(signature, 0x38);
        SHA1.HashData(signature.AsSpan(0, 0x60), signature.AsSpan(0x60));
        return signature;
    }

    /// <summary>
    /// Serializes the signature as a byte array and signs the result
    /// using the RSA private key specified by <paramref name="derKeyPath"/>.
    /// </summary>
    /// <param name="derKeyPath">The path to the .der file containing the private RSA key to use.</param>
    /// <returns>A byte array containing the signed signature.</returns>
    public byte[] ToSignedByteArray(string derKeyPath)
    {
        var rsaCryptoServiceProvider = new RSACryptoServiceProvider();
        rsaCryptoServiceProvider.ImportRSAPrivateKey(File.ReadAllBytes(derKeyPath), out _);
        string pem = rsaCryptoServiceProvider.ExportRSAPrivateKeyPem();

        var keyPair = (AsymmetricCipherKeyPair)new PemReader(new StringReader(pem)).ReadObject();
        var rsa = new Pkcs1Encoding(new RsaEngine());
        rsa.Init(true, keyPair.Private);
        return rsa.ProcessBlock(ToByteArray(), 0, SIGNATURE_LENGTH);
    }
}
