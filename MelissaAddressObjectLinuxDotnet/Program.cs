using System;
using System.IO;
using System.Reflection;
using MelissaData;

namespace MelissaAddressObjectLinuxDotnet
{
  /// <summary>
  /// Address Object corrects, verifies and enhances U.S. and Canadian addresses.
  /// Use Address Object to remove bad or incomplete information before it invades your database
  /// and creates a negative impact on your data-driven initiatives. You'll reduce undeliverables,
  /// increase communication efforts, and save money on all your marketing campaigns.
  /// </summary>
  /// <remarks>
  /// High-level flow of this sample:
  ///   1. SETUP     - create an mdAddr instance, hand it the license string and the
  ///                  paths to the data files, then InitializeDataFiles() (one time).
  ///   2. INPUT     - feed an address in with SetAddress/SetCity/SetState/SetZip.
  ///   3. PROCESS   - VerifyAddress() validates, standardizes, and corrects the address.
  ///   4. READ      - pull the corrected fields back out with the Get* getters
  ///                  (GetAddress, GetCity, GetState, GetZip, GetMelissaAddressKey, ...).
  ///   5. INTERPRET - GetResults() returns comma-separated result codes describing
  ///                  what the object did/found; each code has a human description.
  ///
  /// The pieces in this file map onto that flow:
  ///   - Program        : console harness (argument parsing + the interactive loop).
  ///   - AddressObject  : thin wrapper around mdAddr that owns setup + the call sequence.
  ///   - DataContainer  : plain holder for one record's input and output.
  ///
  /// Where mdAddr comes from:
  ///   The MelissaData namespace and its mdAddr class live in mdAddr_cSharpCode.cs,
  ///   a generated C# wrapper over libmdAddr.so that the accompanying
  ///   MelissaAddressObjectLinuxDotnet.sh script downloads on every run.
  ///
  /// Reference:
  ///   Quickstart    : https://docs.melissa.com/on-premise-api/address-object/address-object-quickstart.html
  ///   Release notes : https://releasenotes.melissa.com/on-premise-api/address-object/
  ///   Result codes  : https://docs.melissa.com/on-premise-api/address-object/result-codes.html
  /// </remarks>
  class Program
  {
    /// <summary>
    /// Entry point. Reads the optional command-line arguments, then hands control to
    /// RunAsConsole, which performs the actual Address Object setup and processing.
    /// </summary>
    /// <param name="args">The raw command-line arguments</param>
    static void Main(string[] args)
    {
      // Populated by ParseArguments below.
      string license = "";
      string testAddress = "";
      string testCity = "";
      string testState = "";
      string testZip = "";
      string dataPath = "";

      ParseArguments(ref license, ref testAddress, ref testCity, ref testState, ref testZip, ref dataPath, args);
      RunAsConsole(license, testAddress, testCity, testState, testZip, dataPath);
    }

    /// <summary>
    /// Reads the supported command-line options into the ref parameters.
    ///
    /// Recognized flags (each followed by its value, e.g. "--address 22382 Avenida Empresa"):
    ///   --license / -l   : the Melissa license string
    ///   --dataPath / -d  : path to the Address Object data files
    ///   --address / -a   : street address to test in one-shot mode
    ///   --city / -c      : city to test in one-shot mode
    ///   --state / -s     : state to test in one-shot mode
    ///   --zip / -z       : ZIP code to test in one-shot mode
    /// </summary>
    /// <param name="license">Receives the Melissa license string.</param>
    /// <param name="testAddress">Receives the street address to test in one-shot mode.</param>
    /// <param name="testCity">Receives the city to test in one-shot mode.</param>
    /// <param name="testState">Receives the state to test in one-shot mode.</param>
    /// <param name="testZip">Receives the ZIP code to test in one-shot mode.</param>
    /// <param name="dataPath">Receives the path to the Address Object data files.</param>
    /// <param name="args">The raw command-line arguments to parse.</param>
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

    /// <summary>
    /// Sets up the Address Object once, then drives the input -> process -> output cycle.
    ///
    /// In interactive mode (no address args) it loops, asking for a new address each pass
    /// until the user answers "N". In one-shot mode (address args supplied) it runs a
    /// single pass and exits.
    /// </summary>
    /// <param name="license">The Melissa license string used to initialize the object.</param>
    /// <param name="testAddress">A street address to process in one-shot mode; if empty, the program prompts interactively.</param>
    /// <param name="testCity">A city to process in one-shot mode.</param>
    /// <param name="testState">A state to process in one-shot mode.</param>
    /// <param name="testZip">A ZIP code to process in one-shot mode.</param>
    /// <param name="dataPath">Path to the Address Object data files.</param>
    static void RunAsConsole(string license, string testAddress, string testCity, string testState, string testZip, string dataPath)
    {
      Console.WriteLine("\n\n=========== WELCOME TO MELISSA ADDRESS OBJECT LINUX DOTNET =========\n");

      // Construct the wrapper. This is where the object is licensed, pointed at the
      // data files, and initialized (see the AddressObject constructor below).
      AddressObject addressObject = new AddressObject(license, dataPath);

      bool shouldContinueRunning = true;

      // Gate the program on a successful initialization. If the data files could not
      // be loaded (bad/expired license, missing or wrong-path data files, ...),
      // GetInitializeErrorString() returns the reason instead of "No error." and we
      // skip the processing loop entirely.
      if (addressObject.mdAddressObj.GetInitializeErrorString() != "No error.")
      {
        shouldContinueRunning = false;
      }

      while (shouldContinueRunning)
      {
        // Holder for this pass's input and result codes.
        DataContainer dataContainer = new DataContainer();

        if (string.IsNullOrEmpty(testAddress) && string.IsNullOrEmpty(testCity) && string.IsNullOrEmpty(testState) && string.IsNullOrEmpty(testZip))
        {
          // Interactive mode: prompt the user for each address component.
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
          // One-shot mode: use the address passed on the command line.
          dataContainer.Address = testAddress;
          dataContainer.City = testCity;
          dataContainer.State = testState;
          dataContainer.Zip = testZip;
        }

        // Print user input
        Console.WriteLine("\n============================== INPUTS ==============================\n");
        Console.WriteLine($"                      Address: {dataContainer.Address}");
        Console.WriteLine($"                         City: {dataContainer.City}");
        Console.WriteLine($"                        State: {dataContainer.State}");
        Console.WriteLine($"                          Zip: {dataContainer.Zip}");

        // Execute Address Object
        // Runs the verify sequence and stores the result codes on dataContainer.
        addressObject.ExecuteObjectAndResultCodes(ref dataContainer);

        // Print output
        // Each Get* getter below returns one component the object produced for the most
        // recently processed address. These read directly from the mdAddr instance, which
        // still holds the results from the Execute call above.
        Console.WriteLine("\n============================== OUTPUT ==============================\n");
        Console.WriteLine("\n\tAddress Object Information:");
        Console.WriteLine($"\t                     MAK: {addressObject.mdAddressObj.GetMelissaAddressKey()}");
        Console.WriteLine($"\t          Address Line 1: {addressObject.mdAddressObj.GetAddress()}");
        Console.WriteLine($"\t          Address Line 2: {addressObject.mdAddressObj.GetAddress2()}");
        Console.WriteLine($"\t                    City: {addressObject.mdAddressObj.GetCity()}");
        Console.WriteLine($"\t                   State: {addressObject.mdAddressObj.GetState()}");
        Console.WriteLine($"\t                     Zip: {addressObject.mdAddressObj.GetZip()}");
        Console.WriteLine($"\t            Result Codes: {dataContainer.ResultCodes}");

        // Result codes come back as a single comma-separated string (e.g. "AS01,AC01").
        // Split it and ask the object for a readable description of each code.
        // ResultCodeDescriptionLong requests the long-form text; a short form is also
        // available via ResultCodeDescriptionShort
        String[] rs = dataContainer.ResultCodes.Split(',');
        foreach (String r in rs)
          Console.WriteLine($"        {r}: {addressObject.mdAddressObj.GetResultCodeDescription(r, mdAddr.ResultCdDescOpt.ResultCodeDescriptionLong)}");

        bool isValid = false;

        // In one-shot mode there is nothing more to do after a single pass:
        // mark the input handled and stop the outer loop.
        if (!string.IsNullOrEmpty(testAddress + testCity + testState + testZip))
        {
          isValid = true;
          shouldContinueRunning = false;
        }

        // Interactive mode: ask whether to process another address. Keep prompting until
        // we get a valid Y/N. "N" ends the program; "Y" falls through to another pass.
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
      Console.WriteLine("\n========= THANK YOU FOR USING MELISSA DOTNET OBJECT ========\n");
    }
  }

  /// <summary>
  /// Wrapper that owns a single Melissa Address Object instance and encapsulates the two
  /// things every Melissa object needs: one-time setup (license + data files) and the
  /// per-record processing sequence. Reuse one instance across many addresses; do NOT
  /// re-initialize per address.
  /// </summary>
  class AddressObject
  {
    // Path to the Address Object data files.
    string dataFilePath;

    // The underlying Melissa Address Object instance.
    public mdAddr mdAddressObj = new mdAddr();

    /// <summary>
    /// Performs the mandatory one-time setup, in this required order:
    ///   1. SetLicenseString     - authorize the object.
    ///   2. SetPathTo*DataFiles  - tell it where each set of data files lives.
    ///   3. InitializeDataFiles  - load the data into memory.
    /// </summary>
    /// <param name="license">The Melissa license string used to authorize the object.</param>
    /// <param name="dataPath">Path to the folder containing the Address Object data files.</param>
    public AddressObject(string license, string dataPath)
    {
      // Set license string and set path to data files
      mdAddressObj.SetLicenseString(license);
      dataFilePath = dataPath;

      // Address Object draws on several USPS data sets; point each one at the data folder.
      mdAddressObj.SetPathToUSFiles(dataFilePath);
      mdAddressObj.SetPathToAddrKeyDataFiles(dataFilePath);
      mdAddressObj.SetPathToDPVDataFiles(dataFilePath);
      mdAddressObj.SetPathToLACSLinkDataFiles(dataFilePath);
      mdAddressObj.SetPathToRBDIFiles(dataFilePath);
      mdAddressObj.SetPathToSuiteFinderDataFiles(dataFilePath);
      mdAddressObj.SetPathToSuiteLinkDataFiles(dataFilePath);

      // Load the data files. The returned ProgramStatus reports whether initialization succeeded.
      // If you see a different date than expected, check your license string and either download the new data files
      // or use the Melissa Updater program to update your data files.
      mdAddr.ProgramStatus pStatus = mdAddressObj.InitializeDataFiles();

      // If an issue occurred, please investigate the common causes.
      // Common causes: an invalid/expired license, or missing/wrong-path data files.
      if (pStatus != mdAddr.ProgramStatus.ErrorNone)
      {
        Console.WriteLine("Failed to Initialize Object.");
        Console.WriteLine(pStatus);
        return;
      }

      // Diagnostic information, handy for confirming the object loaded the data you expect:

      // Build date of the data files
      Console.WriteLine($"                DataBase Date: {mdAddressObj.GetDatabaseDate()}");

      // When the license stops working
      Console.WriteLine($"              Expiration Date: {mdAddressObj.GetLicenseExpirationDate()}");

      // This number should match with the file properties of the Melissa Object binary file.
      // If TEST appears with the build number, there may be a license key issue.
      Console.WriteLine($"               Object Version: {mdAddressObj.GetBuildNumber()}\n");
    }

    /// <summary>
    /// Runs the full Address Object processing sequence for one address and captures its
    /// result codes. This is the canonical per-record call pattern to copy into your
    /// own application:
    ///   ClearProperties -> SetAddress/SetCity/SetState/SetZip -> VerifyAddress -> GetResults
    /// </summary>
    /// <param name="data">
    /// The record to process. Its Address/City/State/Zip are read as input, and
    /// ResultCodes is populated with this run's result codes.
    /// </param>
    public void ExecuteObjectAndResultCodes(ref DataContainer data)
    {
      // Reset any state left over from a previous address. Important when reusing the same
      // object across multiple records so fields from a prior address don't bleed into this one.
      mdAddressObj.ClearProperties();

      // Supply the raw input fields to process
      mdAddressObj.SetAddress(data.Address);
      mdAddressObj.SetCity(data.City);
      mdAddressObj.SetState(data.State);
      mdAddressObj.SetZip(data.Zip);

      // Validate, standardize, and correct the address
      mdAddressObj.VerifyAddress();

      // Collect the result codes for this run
      // ResultsCodes explain any issues Address Object has with the object.
      // List of result codes for Address Object
      // https://docs.melissa.com/on-premise-api/address-object/result-codes.html
      data.ResultCodes = mdAddressObj.GetResults();
    }
  }
  /// <summary>
  /// Data holder for a single record: carries the input address in and the result codes out.
  /// </summary>
  public class DataContainer
  {
    // Input: the street address to process.
    public string Address { get; set; }

    // Input: the city to process.
    public string City { get; set; }

    // Input: the state to process.
    public string State { get; set; }

    // Input: the ZIP code to process.
    public string Zip { get; set; }

    // Output: comma-separated result codes from GetResults().
    public string ResultCodes { get; set; } = "";
  }
}
