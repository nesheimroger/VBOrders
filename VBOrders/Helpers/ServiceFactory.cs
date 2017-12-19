using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Xml;
using Visma.BusinessServices.Generic;
using Visma.BusinessServices.Client;
using Visma.BusinessServices.Wrapper;

namespace VBOrders
{
    public class ServiceFactory
    {
        public static readonly int CompanyNo;
        public static readonly CultureId CultureId = CultureId.English__UnitedStates;
        private static readonly Credentials BusinessCredentials;
        private static readonly string GenericServiceUrl;
        private static readonly string CertificateFileAbsolutePath;
        private static readonly string CertificateThumbprint;


        static ServiceFactory()
        {
            var siteId = ConfigurationManager.AppSettings["businessSiteId"] ?? "standard";
            var username = ConfigurationManager.AppSettings["businessUsername"];
            var password = ConfigurationManager.AppSettings["businessPassword"];

            BusinessCredentials = new Credentials(siteId, username, password);
            CompanyNo = Convert.ToInt32(ConfigurationManager.AppSettings["businessCompanyNo"]);
            GenericServiceUrl = ConfigurationManager.AppSettings["businessGenericServiceUrl"];
            CertificateFileAbsolutePath = ConfigurationManager.AppSettings["businessCertificatePath"];
            CertificateThumbprint = ConfigurationManager.AppSettings["businessCertificateThumbprint"];
        }


        public static GenericServiceClient GetGenericClient()
        {
            var client = new GenericServiceClient(GetBinding(), GetEndpointAddress());

            //let Business configure credentials on channel:
            BusinessCredentials.Apply(client.ClientCredentials);

            //we don't validate certificate's chain on client side:
            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

            return client;
        }

        private static WSHttpBinding GetBinding()
        {
            var binding = new WSHttpBinding
            {
                Name = "CustomAuthenticationEndPoint",
                Security = new WSHttpSecurity()
                {
                    Mode = SecurityMode.Message
                },
                MaxBufferPoolSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue,
                AllowCookies = true,
                ReaderQuotas = new XmlDictionaryReaderQuotas()
                {
                    MaxArrayLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue,
                    MaxDepth = 32,
                    MaxNameTableCharCount = int.MaxValue,
                    MaxStringContentLength = int.MaxValue
                }
            };

            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;

            return binding;
        }

        private static EndpointAddress GetEndpointAddress()
        {
            X509Certificate2 cert2 = null;

            if (!string.IsNullOrEmpty(CertificateFileAbsolutePath) && CertificateFileAbsolutePath != "NA")
            {
                cert2 = GetCertificateFromPath(CertificateFileAbsolutePath);
            }
            else
            {
                cert2 = GetCertificateFromStore(CertificateThumbprint);
            }

            var ep = new EndpointAddress(
                new Uri(GenericServiceUrl),
                EndpointIdentity.CreateX509CertificateIdentity(cert2)
            );

            return ep;
        }

        private static X509Certificate2 GetCertificateFromStore(string thumbprint)
        {
            X509Certificate2 cert2 = null;
            X509Store store = new X509Store(StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var result = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            if (result.Count > 0)
            {
                cert2 = result[0];
            }
            store.Close();
            if (cert2 == null)
            {
                throw new Exception("Could not find certificate with thumbprint: " + thumbprint + " in LocalMachine");
            }
            return cert2;
        }

        private static X509Certificate2 GetCertificateFromPath(string certPath)
        {
            if (!new FileInfo(certPath).Exists)
            {
                throw new Exception($"Certificate {certPath} not found");
            }
            var cert1 = X509Certificate.CreateFromCertFile(certPath);
            var cert2 = new X509Certificate2(cert1);
            return cert2;
        }

        public static Context CreateContext(RequestBuilder request)
        {
            var context = request.AddContext();
            context.CompanyNo = CompanyNo;
            context.CultureId = CultureId;

            context.UserName = "system"; // We log on as another user, but get system-user's rights :-)

            return context;
        }
    }
}