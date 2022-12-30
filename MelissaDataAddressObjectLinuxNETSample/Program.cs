using System;
using System.IO;
using System.Reflection;
using MelissaData;

namespace MelissaDataAddressObjectLinuxNETSample
{
  class Program
  {
    static void Main(string[] args)
    {
      // Variables
      string license = "";
      string testAddress = "";
      string testCity = "";
      string testState = "";
      string testZip = "";
      string dataPath = "";

      ParseArguments(ref license, ref testAddress, ref testCity, ref testState, ref testZip, ref dataPath, args);
      RunAsConsole(license, testAddress, testCity, testState, testZip, dataPath);
    }

    static void ParseArguments(ref string license, ref string testAddress, ref string testCity, ref string testState, ref string testZip, ref string dataPath, string[] args)
    {
      for (int i = 0; i < args.Length; i++)
      {
        if (args[i].Equals("--license") || args[i].Equals("-l"))
        {
          if (args[i + 1] != null)
          {
            license = args[i + 1];
          }
        }
        if (args[i].Equals("--dataPath") || args[i].Equals("-d"))
        {
          if (args[i + 1] != null)
          {
            dataPath = args[i + 1];
          }
        }
        if (args[i].Equals("--address") || args[i].Equals("-a"))
        {
          if (args[i + 1] != null)
          {
            testAddress = args[i + 1];
          }
        }
        if (args[i].Equals("--city") || args[i].Equals("-c"))
        {
          if (args[i + 1] != null)
          {
            testCity = args[i + 1];
          }
        }
        if (args[i].Equals("--state") || args[i].Equals("-s"))
        {
          testState = args[i + 1];
        }
        if (args[i].Equals("--zip") || args[i].Equals("-z"))
        {
          testZip = args[i + 1];
        }
      }
    }

    static void RunAsConsole(string license, string testAddress, string testCity, string testState, string testZip, string dataPath)
    {
      Console.WriteLine("\n\n====== WELCOME TO MELISSA DATA ADDRESS OBJECT LINUX NET SAMPLE =====\n");

      AddressObject addressObject = new AddressObject(license, dataPath);

      bool shouldContinueRunning = true;

      if (addressObject.mdAddressObj.GetInitializeErrorString() != "No error.")
      {
        shouldContinueRunning = false;
      }

      while (shouldContinueRunning)
      {
        DataContainer dataContainer = new DataContainer();

        if (string.IsNullOrEmpty(testAddress) && string.IsNullOrEmpty(testCity) && string.IsNullOrEmpty(testState) && string.IsNullOrEmpty(testZip))
        {
          Console.WriteLine("\nFill in each value to see the Address Object results");

          Console.Write("Address: ");
          dataContainer.Address = Console.ReadLine();
        
          Console.Write("City: ");
          dataContainer.City = Console.ReadLine();
        
          Console.Write("State: ");
          dataContainer.State = Console.ReadLine();
        
          Console.Write("Zip: ");
          dataContainer.Zip = Console.ReadLine();
        }
        else
        {
          dataContainer.Address = testAddress;
          dataContainer.City = testCity;
          dataContainer.State = testState;
          dataContainer.Zip = testZip;
        }

        // Print user input
        Console.WriteLine("\n============================== INPUTS ==============================\n");
        Console.WriteLine($"               Address Line 1: {dataContainer.Address}");
        Console.WriteLine($"                         City: {dataContainer.City}");
        Console.WriteLine($"                        State: {dataContainer.State}");
        Console.WriteLine($"                          Zip: {dataContainer.Zip}");

        // Execute Address Object
        addressObject.ExecuteObjectAndResultCodes(ref dataContainer);

        // Print output
        Console.WriteLine("\n============================== OUTPUT ==============================\n");
        Console.WriteLine("\n\tAddress Object Information:");
        Console.WriteLine($"\t                     MAK: {addressObject.mdAddressObj.GetMelissaAddressKey()}");
        Console.WriteLine($"\t          Address Line 1: {addressObject.mdAddressObj.GetAddress()}");
        Console.WriteLine($"\t          Address Line 2: {addressObject.mdAddressObj.GetAddress2()}");
        Console.WriteLine($"\t                    City: {addressObject.mdAddressObj.GetCity()}");
        Console.WriteLine($"\t                   State: {addressObject.mdAddressObj.GetState()}");
        Console.WriteLine($"\t                     Zip: {addressObject.mdAddressObj.GetZip()}");
        Console.WriteLine($"\t            Result Codes: {dataContainer.ResultCodes}");

        String[] rs = dataContainer.ResultCodes.Split(',');
        foreach (String r in rs)
          Console.WriteLine($"        {r}: {addressObject.mdAddressObj.GetResultCodeDescription(r, mdAddr.ResultCdDescOpt.ResultCodeDescriptionLong)}");

        bool isValid = false;
        if (!string.IsNullOrEmpty(testAddress + testCity + testState + testZip)) //check individually
        {
          isValid = true;
          shouldContinueRunning = false;
        }
        while (!isValid)
        {
          Console.WriteLine("\nTest another address? (Y/N)");
          string testAnotherResponse = Console.ReadLine();

          if (!string.IsNullOrEmpty(testAnotherResponse))
          {
            testAnotherResponse = testAnotherResponse.ToLower();

            if (testAnotherResponse == "y")
            {
              isValid = true;
            }
            else if (testAnotherResponse == "n")
            {
              isValid = true;
              shouldContinueRunning = false;
            }
            else
            {
              Console.Write("Invalid Response, please respond 'Y' or 'N'");
            }
          }
        }
      }
      Console.WriteLine("\n========= THANK YOU FOR USING MELISSA DATA NET OBJECT ========\n");
    }
  }

  class AddressObject
  {
    // Path to address object data files (.dat, etc)
    string dataFilePath;

    // Create instance of Melissa Address Object
    public mdAddr mdAddressObj = new mdAddr();

    public AddressObject(string license, string dataPath)
    {
      // Set license string and set path to data files (.dat, etc)
      mdAddressObj.SetLicenseString(license);
      dataFilePath = dataPath;

      mdAddressObj.SetPathToUSFiles(dataFilePath);
      mdAddressObj.SetPathToAddrKeyDataFiles(dataFilePath);
      mdAddressObj.SetPathToDPVDataFiles(dataFilePath);
      mdAddressObj.SetPathToLACSLinkDataFiles(dataFilePath);
      mdAddressObj.SetPathToRBDIFiles(dataFilePath);
      mdAddressObj.SetPathToSuiteFinderDataFiles(dataFilePath);
      mdAddressObj.SetPathToSuiteLinkDataFiles(dataFilePath);

      // If you see a different date than expected, check your license string and either download the new data files or use the Melissa Updater program to update your data files.  
      mdAddr.ProgramStatus pStatus = mdAddressObj.InitializeDataFiles();

      if (pStatus != mdAddr.ProgramStatus.ErrorNone)
      {
        Console.WriteLine("Failed to Initialize Object.");
        Console.WriteLine(pStatus);
        return;
      }

      Console.WriteLine($"                DataBase Date: {mdAddressObj.GetDatabaseDate()}");
      Console.WriteLine($"              Expiration Date: {mdAddressObj.GetLicenseExpirationDate()}");

      /**
       * This number should match with file properties of the Melissa Data Object binary file.
       * If TEST appears with the build number, there may be a license key issue.
       */
      Console.WriteLine($"               Object Version: {mdAddressObj.GetBuildNumber()}\n");
    }

    // This will call the Lookup function to process the input address, city, state, and zip as well as generate the result codes
    public void ExecuteObjectAndResultCodes(ref DataContainer data)
    {
      mdAddressObj.ClearProperties();

      mdAddressObj.SetAddress(data.Address);
      mdAddressObj.SetCity(data.City);
      mdAddressObj.SetState(data.State);
      mdAddressObj.SetZip(data.Zip);

      mdAddressObj.VerifyAddress();
      data.ResultCodes = mdAddressObj.GetResults();

      // ResultsCodes explain any issues address object has with the object.
      // List of result codes for Address object
      // https://wiki.melissadata.com/?title=Result_Code_Details#Address_Object
    }
  }
  public class DataContainer
  {
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string ResultCodes { get; set; } = "";
  }
}
