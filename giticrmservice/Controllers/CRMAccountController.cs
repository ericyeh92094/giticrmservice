using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Samples;

using giticrmservice.Resources;

using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace giticrmservice.Controllers
{
    public class CRMAccountController : ApiController
    {

        // GET: api/CRMAccount
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CRMAccount/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CRMAccount
        public void Post([FromBody]CRMContact account)
        {
            Execute(account);
        }

        // PUT: api/CRMAccount/5
        public void Put(int id, [FromBody]CRMContact account)
        {
        }

        // DELETE: api/CRMAccount/5
        public void Delete(int id)
        {
        }
        private OrganizationServiceProxy _serviceProxy;
        private IOrganizationService _service;
        // Define the IDs needed for this sample.
        private Guid _accountId;

        private void QueryandAdd(string wechatname, string CustomerOpenId, string PaOpenID, string Barcode)
        {
            try
            {
                // Connect to the Organization service. 
                // The using statement assures that the service proxy will be properly disposed.
                Uri OrganizationUri = new Uri("https://gtdevp.giti.com:446/XRMServices/2011/Organization.svc");
                ClientCredentials Credentials = new ClientCredentials();

                Credentials.UserName.UserName = "GTDEV\\lanrong";
                Credentials.UserName.Password = "qwer!1234";

                using (_serviceProxy = new OrganizationServiceProxy(OrganizationUri, null, Credentials, null))
                {
                    _service = (IOrganizationService)_serviceProxy;

                    //<snippetCRUDOperationsDE1>
                    // Instaniate an account object.
                    Microsoft.Xrm.Sdk.Entity account = new Microsoft.Xrm.Sdk.Entity("contact");

                    account["nickname"] = wechatname;
                    account["new_wechatopenid"] = CustomerOpenId;
                    account["new_wechatid"] = PaOpenID;
                    account["description"] = Barcode;

                    // Create an account record named wechat name.
                    _accountId = _service.Create(account);

                    /*
                    Debug.WriteLine("{0} {1} created, ", account.LogicalName, account.Attributes["nickname"]);

                    // Create a column set to define which attributes should be retrieved.
                    ColumnSet attributes = new ColumnSet(new string[] { "name", "new_wechatopenid" });

                    // Retrieve the account and its name and ownerid attributes.
                    account = _service.Retrieve(account.LogicalName, _accountId, attributes);
                    Debug.WriteLine("retrieved, ");

                    // Update the postal code attribute.

                    // Update the account.
                    _service.Update(account);
                    Debug.WriteLine("and updated.");
                    */
                    // Delete the account.
                    bool deleteRecords = false;

                    if (deleteRecords)
                    {
                        _service.Delete("account", _accountId);

                        Debug.WriteLine("Entity record(s) have been deleted.");
                    }
                }
            }

            // Catch any service fault exceptions that Microsoft Dynamics CRM throws.
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
            {
                // You can handle an exception here or pass it back to the calling method.
                throw;
            }
        }

        private void Execute(CRMContact account)
        {
            try
            {
               //ServerConnection serverConnect = new ServerConnection();
                //ServerConnection.Configuration config = serverConnect.GetServerConfiguration();

                QueryandAdd(account.Nickname, account.CustomerOpenID, account.PaOpenID, account.Barcode);
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                Debug.WriteLine("The application terminated with an error.");
                Debug.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
                Debug.WriteLine("Code: {0}", ex.Detail.ErrorCode);
                Debug.WriteLine("Message: {0}", ex.Detail.Message);
                Debug.WriteLine("Plugin Trace: {0}", ex.Detail.TraceText);
                Debug.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (System.TimeoutException ex)
            {
                Debug.WriteLine("The application terminated with an error.");
                Debug.WriteLine("Message: {0}", ex.Message);
                Debug.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Debug.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("The application terminated with an error.");
                Debug.WriteLine(ex.Message);

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    Debug.WriteLine(ex.InnerException.Message);

                    FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> fe = ex.InnerException
                        as FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>;
                    if (fe != null)
                    {
                        Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp);
                        Console.WriteLine("Code: {0}", fe.Detail.ErrorCode);
                        Console.WriteLine("Message: {0}", fe.Detail.Message);
                        Console.WriteLine("Plugin Trace: {0}", fe.Detail.TraceText);
                        Console.WriteLine("Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }
            // Additional exceptions to catch: SecurityTokenValidationException, ExpiredSecurityTokenException,
            // SecurityAccessDeniedException, MessageSecurityException, and SecurityNegotiationException.

            finally
            {

                Debug.WriteLine("Press <Enter> to exit.");
            }
        }
    }
}
