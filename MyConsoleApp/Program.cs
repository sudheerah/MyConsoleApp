using MbDotNet;
using MbDotNet.Enums;
using MbDotNet.Models.Imposters;
using System;
using System.Net;

namespace MyConsoleApp
{
    internal class Program
    {
        public static MountebankClient MountebankClient { get; set; }

        private static void Main(string[] args)
        {
            Console.WriteLine("Test Harness.");

            MountebankClient = new MountebankClient();

            for (int i = 1; i < 5; i = i + 1)
            {
                int input = int.Parse(Console.ReadLine());

                HttpStatusCode errorCode = (HttpStatusCode)input;

                switch (errorCode)
                {
                    case HttpStatusCode.InternalServerError:

                        MountebankClient.Impersonate(
                            "Magento - Create cart Failure",
                            "/V1/guest-carts",
                            5101,
                            Method.Post,
                            errorCode,
                            new
                            {
                                message = "Magento is down"
                            }
                        );

                        break;

                    case HttpStatusCode.NotFound:

                        MountebankClient.Impersonate(
                            "Magento - Create cart Failure - NotFound",
                            "/V1/guest-carts",
                            5101,
                            Method.Post,
                            errorCode,
                            new
                            {
                                message = "Request does not match any route."
                            }
                        );
                        break;

                    case HttpStatusCode.Forbidden:

                        MountebankClient.Impersonate(
                            "Magento - Create cart Failure - Forbidden",
                            "/V1/guest-carts",
                            5101,
                            Method.Post,
                            errorCode,
                            new
                            {
                                message = "No access."
                            }
                        );
                        break;

                    case HttpStatusCode.BadRequest:

                        MountebankClient.Impersonate(
                            "Magento - Create cart Failure - Bad request",
                            "/V1/guest-carts",
                            5101,
                            Method.Post,
                            errorCode,
                            new
                            {
                                message = "Invalid Magento Request"
                            }
                        );
                        break;
                }
            }

            Console.ReadKey();
        }
    }

    public static class MountebankClientExtensions
    {
        public static HttpImposter CreateOrReplaceHttpImposter(this MountebankClient client,
            int port,
            string name = null,
            bool recordRequests = false)
        {
            client.DeleteImposter(port);
            return client.CreateHttpImposter(port, name, recordRequests);
        }

        public static void Impersonate<T>(
            this MountebankClient theClient,
            string mockFriendlyName,
            string path,
            int port,
            MbDotNet.Enums.Method httpMethod,
            HttpStatusCode returnCode,
            T returnBody)
        {
            HttpImposter imposter = theClient.CreateOrReplaceHttpImposter(port, mockFriendlyName);
            imposter.AddStub()
                .OnPathAndMethodEqual(path, httpMethod)
                .ReturnsJson(returnCode, returnBody);
            theClient.Submit(imposter);
        }
    }
}