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
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace DriveProxy.Utils
{
  internal class Certificate
  {
    public static X509Certificate2 GetItem(string thumbprint)
    {
      try
      {
        return GetItem(StoreName.My, StoreLocation.CurrentUser, thumbprint);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static X509Certificate2 GetItem(StoreName storeName, StoreLocation storeLocation, string thumbprint)
    {
      try
      {
        OpenFlags openFlags = System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly |
                              System.Security.Cryptography.X509Certificates.OpenFlags.OpenExistingOnly;

        return GetItem(storeName, storeLocation, openFlags, thumbprint);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static X509Certificate2 GetItem(StoreName storeName,
                                           StoreLocation storeLocation,
                                           OpenFlags openFlags,
                                           string thumbprint)
    {
      try
      {
        X509Certificate2 certificate = null;
        X509Store store = null;

        try
        {
          Type storeNameType = typeof (StoreName);
          Type storeLocationType = typeof (StoreLocation);
          List<System.Reflection.FieldInfo> storeNameFieldInfos = null;
          List<System.Reflection.FieldInfo> storeLocationFieldInfos = null;
          int storeNameFieldInfoIndex = 0;
          int storeLocationFieldInfoIndex = -1;

          while (true)
          {
            store = new System.Security.Cryptography.X509Certificates.X509Store(storeName, storeLocation);

            store.Open(openFlags);

            foreach (
              System.Security.Cryptography.X509Certificates.X509Certificate2 storeCertificate in store.Certificates)
            {
              if (storeCertificate.Thumbprint == thumbprint)
              {
                certificate = storeCertificate;

                break;
              }
            }

            store.Close();

            if (certificate != null)
            {
              break;
            }

            if (storeNameFieldInfos == null)
            {
              System.Reflection.FieldInfo[] fieldInfos = storeNameType.GetFields();

              storeNameFieldInfos = new List<System.Reflection.FieldInfo>();

              for (int i = 0; i < fieldInfos.Length; i++)
              {
                if (!fieldInfos[i].FieldType.IsEnum)
                {
                  continue;
                }

                storeNameFieldInfos.Add(fieldInfos[i]);
              }
            }

            if (storeLocationFieldInfos == null)
            {
              System.Reflection.FieldInfo[] fieldInfos = storeLocationType.GetFields();

              storeLocationFieldInfos = new List<System.Reflection.FieldInfo>();

              for (int i = 0; i < fieldInfos.Length; i++)
              {
                if (!fieldInfos[i].FieldType.IsEnum)
                {
                  continue;
                }

                storeLocationFieldInfos.Add(fieldInfos[i]);
              }
            }

            if (storeLocationFieldInfoIndex == -1 || storeLocationFieldInfoIndex == storeLocationFieldInfos.Count)
            {
              if (storeNameFieldInfoIndex == storeNameFieldInfos.Count)
              {
                break;
              }

              storeName = (StoreName)storeNameFieldInfos[storeNameFieldInfoIndex].GetRawConstantValue();

              storeNameFieldInfoIndex++;

              storeLocationFieldInfoIndex = 0;
            }

            storeLocation = (StoreLocation)storeLocationFieldInfos[storeLocationFieldInfoIndex].GetRawConstantValue();

            storeLocationFieldInfoIndex++;
          }
        }
        finally
        {
          if (store != null)
          {
            store.Close();
          }
        }

        if (certificate != null)
        {
          VerifyItem(certificate);
        }

        return certificate;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static X509Certificate2 GetItem(string filePath, string thumbprint)
    {
      try
      {
        return GetItem(filePath, null, thumbprint);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static X509Certificate2 GetItem(string filePath, string password, string thumbprint)
    {
      try
      {
        if (String.IsNullOrEmpty(filePath))
        {
          throw new Exception("FilePath is empty.");
        }

        if (!System.IO.File.Exists(filePath))
        {
          throw new Exception("FilePath '" + filePath + "' does not exist.");
        }

        X509Certificate2 certificate = null;

        if (String.IsNullOrEmpty(password))
        {
          certificate = new X509Certificate2(filePath);
        }
        else
        {
          certificate = new X509Certificate2(filePath, password);
        }

        VerifyItem(certificate);

        return certificate;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static void VerifyItem(X509Certificate2 certificate)
    {
      try
      {
        if (certificate == null)
        {
          throw new Exception("Certificate is null.");
        }

        DateTime effectiveDate;

        if (!DateTime.TryParse(certificate.GetEffectiveDateString(), out effectiveDate))
        {
          throw new Exception("Could not parse client certificate effective date.");
        }
        if (effectiveDate > DateTime.Now)
        {
          throw new Exception("The client certificate is not yet effective.");
        }

        DateTime expirationDate;

        if (!DateTime.TryParse(certificate.GetExpirationDateString(), out expirationDate))
        {
          throw new Exception("Could not parse client certificate expiration date.");
        }
        if (expirationDate <= DateTime.Now)
        {
          throw new Exception("The client certificate has expired.");
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }
  }
}
