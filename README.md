# Melissa - Address Object Linux Dotnet

## Purpose

This code showcases the Melissa Address Object using C#.

Please feel free to copy or embed this code to your own project. Happy coding!

For the latest Melissa Address Object release notes, please visit: https://releasenotes.melissa.com/on-premise-api/address-object/

For further details, please visit: https://docs.melissa.com/on-premise-api/address-object/address-object-quickstart.html

The console will ask the user for:

- Address
- City
- State
- Zip

And return 

- Melissa Address Key (MAK)
- Address Line 1
- Address Line 2
- City
- State
- Zip
- Result Codes


## Tested Environments

- Linux 64-bit .NET 8.0, Ubuntu 20.04.05 LTS
- Melissa data files for 2025-07

## Required Files and Programs

#### libmdAddr.so

This is the c++ code of the Melissa Object.

#### Data Files
- Addr.dbf
- Congress.csv
- dph256.dte
- dph256.hsa
- dph256.hsc
- dph256.hsf
- dph256.hsn
- dph256.hsv
- dph256.hsx
- ews.txt
- lcd256
- mdAddr.dat
- mdAddr.lic
- mdAddr.nat
- mdAddr.str
- mdAddrKey.db
- mdAddrKeyCA.db
- mdCanada3.db
- mdCanadaPOC.db
- mdLACS256.dat
- mdRBDI.dat
- mdSteLink256.dat
- mdSuiteFinder.db
- month256.dat

## Getting Started
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

#### Install .NET SDK
Before starting, check to see if you already have .NET SDK already installed by entering this command:

`dotnet --list-sdks`

If .NET SDK is already installed, you should see it in the following list:

![alt text](/screenshots/dotnet_output.png)

To download, run the following commands to add the Microsoft package signing key to your list of trusted keys and add the package repository.

```
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
```

Next, you can now run this command to install your desired .NET SDK (replace <VERSION> with .NET version: 8.0, 9.0, etc.):

```
sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-<VERSION>
```

Once all of this is done, you should be able to verify that the SDK is installed with the `dotnet --list-sdks` command.

----------------------------------------

#### Download this project
```
git clone https://github.com/MelissaData/AddressObject-Dotnet-Linux
cd AddressObject-Dotnet-Linux
```

#### Set up Melissa Updater 
Melissa Updater is a CLI application allowing the user to update their Melissa applications/data. 

- In the root directory of the project, create a folder called `MelissaUpdater` by using the command: 

  `mkdir MelissaUpdater`

- Enter the newly created folder using the command:

  `cd MelissaUpdater`

- Proceed to install the Melissa Updater using the curl command: 

  `curl -L -O https://releases.melissadata.net/Download/Library/LINUX/NET/ANY/latest/MelissaUpdater`

- After the Melissa Updater is installed, you will need to change the Melissa Updater to an executable using the command:

  `chmod +x MelissaUpdater`

- Now that the Melissa Updater is set up, you can now proceed to move back into the project folder by using the command:
  
   `cd ..`
----------------------------------------

#### Different ways to get data file(s)
1.  Using Melissa Updater
    - It will handle all of the data download/path and .so file(s) for you. 
2.  If you already have the latest DQS release zip, you can find the data file(s) in there
    - To pass in your own data file path directory, you may either use the '--dataPath' parameter or enter the data file path directly in interactive mode.
    - Comment out this line "DownloadDataFiles $license" in the bash script.
    - This will prevent you from having to redownload all the files.

#### Change Bash Script Permissions
To be able to run the bash script, you must first make it an executable using the command:

`chmod +x MelissaAddressObjectLinuxDotnet.sh`

As an indicator, the filename will change colors once it becomes an executable.

## Run Bash Script
Parameters:
- --address: a test street address (house number & street name)
- --city (optional): a test city
- --state (optional): a test state
- --zip (optional): a test zip code

    These are convenient when you want to get results for a specific address in one run instead of testing multiple addresses in interactive mode.

- --dataPath (optional): a data file path directory to test the Address Object
- --license (optional): a license string to test the Address Object 
- --quiet (optional): add to command if you do not want to get any console output from the Melissa Updater

When you have modified the script to match your data location, let's run the script. There are two modes:
- Interactive
    
    The script will prompt the user for an address, city, state, and zip, then use the provided inputs to test Address Object. For example:
    ```
    ./MelissaAddressObjectLinuxDotnet.sh
    ```
    For quiet mode:
    ```
    ./MelissaAddressObjectLinuxDotnet.sh --quiet
    ```

- Command Line

    You can pass an address, city, state, zip, and a license string into the ```--address```, ```--city```, ```--state```, ```--zip```, and ```--license``` parameters respectively to test Address Object. For example:

    With all parameters:
    ```
    ./MelissaAddressObjectLinuxDotnet.sh --address "22382 Avenida Empresa" --city "Rancho Santa Margarita" --state "CA" --zip "92688"
    ./MelissaAddressObjectLinuxDotnet.sh --address "22382 Avenida Empresa" --city "Rancho Santa Margarita" --state "CA" --zip "92688" --license "<your_license_string>"
    ```

    With any known (optional) parameters:
    ```
    ./MelissaAddressObjectLinuxDotnet.sh --address "22382 Avenida Empresa" --state "CA" 
    ./MelissaAddressObjectLinuxDotnet.sh --address "22382 Avenida Empresa" --state "CA" --license "<your_license_string>"
    ```

    For quiet mode:
    ```
    ./MelissaAddressObjectLinuxDotnet.sh --address "22382 Avenida Empresa" --city "Rancho Santa Margarita" --state "CA" --zip "92688" --quiet
    ./MelissaAddressObjectLinuxDotnet.sh --address "22382 Avenida Empresa" --city "Rancho Santa Margarita" --state "CA" --zip "92688" --license "<your_license_string>" --quiet
    ```
This is the expected outcome of a successful setup for interactive mode:

![alt text](/screenshots/output.png)


## Troubleshooting

Troubleshooting for errors found while running your program.

### C# Errors:

| Error      | Description |
| ----------- | ----------- |
| ErrorRequiredFileNotFound      | Program is missing a required file. Please check your Data folder and refer to the list of required files above. If you are unable to obtain all required files through the Melissa Updater, please contact technical support below. |
| ErrorLicenseExpired   | Expired license string. Please contact technical support below. |


## Contact Us

For free technical support, please call us at 800-MELISSA ext. 4 (800-635-4772 ext. 4) or email us at tech@melissa.com.

To purchase this product, contact the Melissa sales department at 800-MELISSA ext. 3 (800-635-4772 ext. 3).
