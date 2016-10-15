
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using Microsoft.Crm.Sdk.Samples;
using DailyContacts.Resources;

using System.Diagnostics;

namespace DailyContacts
{
  public class CRMContactRepository
    {
    private List<CRMContact> CRMContacts
        {
      get
      {
        if (HttpContext.Current.Cache["CRMContacts"] == null)
          HttpContext.Current.Cache["CRMContacts"] = new List<CRMContact>();

        return HttpContext.Current.Cache["CRMContacts"] as List<CRMContact>;
      }
      set
      {
        HttpContext.Current.Cache["CRMContacts"] = value;
      }
    }

    public IEnumerable<CRMContact> Get()
    {
      return CRMContacts;
    }

    public CRMContact Get(int id)
    {
      return CRMContacts.Find(t => t.Id == id);
    }

    public CRMContact Post(CRMContact contact)
    {
      contact.Id = CRMContacts.Max(t => t.Id) + 1;
      CRMContacts.Add(contact);

      return contact;
    }

    public CRMContact Put(CRMContact contact)
    {
      var t = Get(contact.Id);

      if (t == null)
        throw new Exception(string.Format("Contact with id {0} not exists.", contact.Id));
      /*
      t.Description = contact.Description;
      t.Priority = contact.Priority;
      */
      return t;
    }

    public bool Delete(int id)
    {
      var t = Get(id);

      if (t == null)
        return false;

      CRMContacts.Remove(t);

      return true;
    }

        private OrganizationServiceProxy _serviceProxy;
        private IOrganizationService _service;
        // Define the IDs needed for this sample.
        private Guid _accountId;


        public void QueryandAdd(ServerConnection.Configuration serverConfig, string wechatname, string CustomerOpenId, string PaOpenID)
        {
            try
            {

                // Connect to the Organization service. 
                // The using statement assures that the service proxy will be properly disposed.
                using (_serviceProxy = new OrganizationServiceProxy(serverConfig.OrganizationUri, serverConfig.HomeRealmUri, serverConfig.Credentials, serverConfig.DeviceCredentials))
                {
                    _service = (IOrganizationService)_serviceProxy;

                    //<snippetCRUDOperationsDE1>
                    // Instaniate an account object.
                    Microsoft.Xrm.Sdk.Entity account = new Microsoft.Xrm.Sdk.Entity("contact");

                    account["Nickname"] = wechatname;

                    // Create an account record named wechat name.
                    _accountId = _service.Create(account);

                    Debug.WriteLine("{0} {1} created, ", account.LogicalName, account.Attributes["Nickname"]);

                    // Create a column set to define which attributes should be retrieved.
                    ColumnSet attributes = new ColumnSet(new string[] { "name", "PC微信OpenId" });

                    // Retrieve the account and its name and ownerid attributes.
                    account = _service.Retrieve(account.LogicalName, _accountId, attributes);
                    Debug.WriteLine("retrieved, ");

                    // Update the postal code attribute.
                    account["PC微信OpenId"] = CustomerOpenId;

                    // Update the account.
                    _service.Update(account);
                    Debug.WriteLine("and updated.");

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

        public void Execute(string CustomerOpenID, string PaOpenID)
        {
            try
            {
                // Obtain the target organization's Web address and client logon 
                // credentials from the user.
                ServerConnection serverConnect = new ServerConnection();
                ServerConnection.Configuration config = serverConnect.GetServerConfiguration();

                QueryandAdd(config, "NickName", CustomerOpenID, PaOpenID);
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