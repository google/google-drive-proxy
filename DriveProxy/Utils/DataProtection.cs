/*
Copyright 2014 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Security.Cryptography;
using System.Text;

namespace DriveProxy.Utils
{
  /// <summary>
  ///   Provides methods for encrypting and decrypting data.
  /// </summary>
  public static class DataProtection
  {
    private static readonly Encoding Encoding = Encoding.BigEndianUnicode;

    //This entropy was generated using new RNGCryptoServiceProvider().GetBytes()
    private static readonly byte[] Entropy =
    {
      174,
      46,
      123,
      237,
      175,
      228,
      164,
      165,
      157,
      113,
      109,
      235,
      231,
      163,
      252,
      35
    };

    /// <summary>
    ///   Protect Data - Encrypts the data in a specified string and returns a string that contains the encrypted data.
    /// </summary>
    /// <param name="value">Data to be encrypted</param>
    /// <returns>Encrypted data</returns>
    public static string Protect(string value)
    {
      byte[] encryptedData = ProtectedData.Protect(Encoding.GetBytes(value),
                                                   Entropy,
                                                   DataProtectionScope.CurrentUser);

      return Convert.ToBase64String(encryptedData);
    }

    /// <summary>
    ///   Unprotect Data - Decrypts the data in a specified string and returns a string that contains the decrypted data.
    /// </summary>
    /// <param name="value">Data to be decrypted</param>
    /// <returns>Decrypted data</returns>
    public static string Unprotect(string value)
    {
      byte[] decryptedData = ProtectedData.Unprotect(Convert.FromBase64String(value),
                                                     Entropy,
                                                     DataProtectionScope.CurrentUser);

      return Encoding.GetString(decryptedData);
    }
  }
}
