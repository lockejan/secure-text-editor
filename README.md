# secure-text-editor

Implementation of a basic text editor with security features in C# which can save and load encrypted files.

All cryptographic functionality was used with bouncyCastle crypto library.

The used cross-plattform UI library is called 'medja' and has been provided by SprintWorx GmbH.

Currently the UI library isn't open sourced so it's not possible to use this editor right now without a private feed.



The projects implements following cryptographic functions:
- Symmetric cryptography
   - AES (128, 192 and 256 bit)
  
- Stream cipher
   - RC4
   
- Password based cryptography
   - SCRYPT
   - PBKDF2
   
- Digital signing (creation and verification):
   - SHA256 with DSA

Supported paddings are currently:
- No Padding
- PKCS7 Padding
- ZeroByte Padding

Supported Blockmodes:
- ECB
- CBC
- OFB
- CTS
- GCM

Supported digest and macs:
- SHA256
- AESCMAC
- HMACSHA256

The editor stores the text files in a json file format:

```
{
  "FormatVersion": "0.1",
  "Encoding": "System.Text.UTF8Encoding",
  "IvOrSalt": null,
  "Signature": null,
  "Cipher": "1irNqq2d6jX/PDGoNqBvAg==",
  "IsEncryptActive": true,
  "CipherAlgorithm": "AES",
  "KeySize": 128,
  "BlockMode": "ECB",
  "Padding": "Pkcs7",
  "IsPbeActive": false,
  "PbeAlgorithm": "PBKDF2",
  "PbeDigest": "GCM",
  "IsIntegrityActive": false,
  "Integrity": "Digest",
  "IntegrityOptions": "Sha256"
}
```

There is also some sort of code documentation via doxygen.
