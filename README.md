# secure-text-editor

Implementation of a basic text editor with security features in C# which can save and load encrypted files.

All cryptographic functionality was used with bouncyCastle crypto library.

The used cross-plattform UI library is called 'medja' and has been provided by SprintWorx GmbH.

Currently the UI library isn't open sourced so it's not possible to use this editor right now without a private feed.

![Main view](screenshots/main-view-ste.png)

The projects implements following cryptographic functions:

| Cipher | Keysize             |
| ------ | ------------------- |
| AES    | 128 / 192 / 256 bit |
| RC4    | 40 - 2048 bit       |

| Blockmode |
| --------- |
| ECB       |
| CBC       |
| OFB       |

| Padding   |
| --------- |
| NoPadding |
| PKCS7     |
| ZeroByte  |

| PBE (Password based encryption) cipher |
| -------------------------------------- |
| PBKDF2                                 |
| SCRYPT                                 |

| Digital Signing (creation and verification) |
| ------------------------------------------- |
| SHA256 with DSA                             |

| Digest and MACs |
| --------------- |
| SHA256          |
| AESCMAC         |
| HMACSHA256      |

![Save view](screenshots/save-dialog.png)

![Load view](screenshots/load-dialog.png)

The editor stores text files in a json file format:

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

Change directory to root folder of the project and execute doxygen via CLI to create it.
