using HP.HPTRIM.Service.Client;
using HP.HPTRIM.ServiceModel;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAPIOpenIDConnect
{
    public class Program
    {
        private static NameValueCollection appsettings = ConfigurationManager.AppSettings;
        public static void Main(string[] args)
        {
            try
            {
                string accessToken = msalAuthentication();
                TrimClient trimClient = new TrimClient(appsettings["serviceAPIURL"]);
                trimClient.AddHeader("Authorization", $"Bearer {accessToken}");

                //GET
                PostSearch postSearch = new PostSearch()
                {
                    q = $"unique:DharunOT",
                    TrimType = BaseObjectTypes.Location,
                    pageSize = 10,
                    Properties = new List<string>() { "LocationNickName", "NameString" }
                };
                var response = trimClient.Get<LocationsResponse>(postSearch);

                //POST a record
                Record record = new Record()
                {
                    Title = "testOpenID",
                    RecordType = new RecordTypeRef()
                    {
                        Uri = 9000000017
                    }
                };
                var recResponse = trimClient.Post<RecordsResponse>(record);

                Console.WriteLine(recResponse.Results[0].Title);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
        private static string msalAuthentication()
        {
            string[] scopes = new string[] { appsettings["scope"] };
            var pca = PublicClientApplicationBuilder.Create(appsettings["clientID"]).
                    WithAuthority(appsettings["authority"])
                    .Build();

            //Token Cache
            FileCache cacheHelper = FileCache.GetUserCache(Path.Combine(appsettings["tokenCachePath"] ,"TokenCache.dat"));
            pca.UserTokenCache.SetBeforeAccess(cacheHelper.BeforeAccessNotification);
            pca.UserTokenCache.SetAfterAccess(cacheHelper.AfterAccessNotification);

            var accounts = pca.GetAccountsAsync();
            accounts.Wait();
            AuthenticationResult authResult;
            try
            {
                var silentToken = pca.AcquireTokenSilent(scopes, accounts.Result.FirstOrDefault()).ExecuteAsync();
                silentToken.Wait();
                authResult = silentToken.Result;
            }
            catch (AggregateException)
            {
                //Silent token is not available, let's fetch the token using interactive approach
                var uu = pca.AcquireTokenInteractive(scopes)
                    .ExecuteAsync();
                authResult = uu.Result;
            }
            return authResult.AccessToken;
        }

    }
}
