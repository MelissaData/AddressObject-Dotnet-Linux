#!/bin/bash

# Name:    MelissaAddressObjectLinuxDotnet
# Purpose: Use the MelissaUpdater to make the MelissaAddressObjectLinuxDotnet code usable

######################### Constants ##########################

RED='\033[0;31m' #RED
NC='\033[0m' # No Color

######################### Parameters ##########################

address=""
city=""
state=""
zip=""
license=""
quiet="false"

while [ $# -gt 0 ] ; do
  case $1 in
    -a | --address) 
        address="$2"

        if [ "$address" == "-c" ] || [ "$address" == "--city" ] || [ "$address" == "-s" ] || [ "$address" == "--state" ] || [ "$address" == "-z" ] || [ "$address" == "--zip" ] || [ "$address" == "-l" ] || [ "$address" == "--license" ] || [ "$address" == "-q" ] || [ "$address" == "--quiet" ] || [ -z "$address" ];
        then
            printf "${RED}Error: Missing an argument for parameter \'address\'.${NC}\n"  
            exit 1
        fi  
        ;;
	-c | --city) 
        city="$2"

        if [ "$city" == "-a" ] || [ "$city" == "--address" ] || [ "$city" == "-s" ] || [ "$city" == "--state" ] || [ "$city" == "-z" ] || [ "$city" == "--zip" ] || [ "$city" == "-l" ] || [ "$city" == "--license" ] || [ "$city" == "-q" ] || [ "$city" == "--quiet" ] || [ -z "$city" ];
        then
            printf "${RED}Error: Missing an argument for parameter \'city\'.${NC}\n"  
            exit 1
        fi  
        ;;
	-s | --state) 
        state="$2"

        if [ "$state" == "-c" ] || [ "$state" == "--city" ] || [ "$state" == "-a" ] || [ "$state" == "--address" ] || [ "$state" == "-z" ] || [ "$state" == "--zip" ] || [ "$state" == "-l" ] || [ "$state" == "--license" ] || [ "$state" == "-q" ] || [ "$state" == "--quiet" ] || [ -z "$state" ];
        then
            printf "${RED}Error: Missing an argument for parameter \'state\'.${NC}\n"  
            exit 1
        fi   
        ;;
	-z | --zip) 
        zip="$2"

        if [ "$zip" == "-c" ] || [ "$zip" == "--city" ] || [ "$zip" == "-s" ] || [ "$zip" == "--state" ] || [ "$zip" == "-a" ] || [ "$zip" == "--address" ] || [ "$zip" == "-l" ] || [ "$zip" == "--license" ] || [ "$zip" == "-q" ] || [ "$zip" == "--quiet" ] || [ -z "$zip" ];
        then
            printf "${RED}Error: Missing an argument for parameter \'zip\'.${NC}\n"  
            exit 1
        fi   
        ;;		
    -l | --license) 
        license="$2"

        if [ "$license" == "-c" ] || [ "$license" == "--city" ] || [ "$license" == "-s" ] || [ "$license" == "--state" ] || [ "$license" == "-z" ] || [ "$license" == "--zip" ] || [ "$license" == "-a" ] || [ "$license" == "--address" ] || [ "$license" == "-q" ] || [ "$license" == "--quiet" ] || [ -z "$license" ];
        then
            printf "${RED}Error: Missing an argument for parameter \'license\'.${NC}\n"  
            exit 1
        fi    
        ;;
    -q | --quiet) 
        quiet="true" 
        ;;
  esac
  shift
done

######################### Config ###########################

RELEASE_VERSION='2023.06'
ProductName="DQ_ADDR_DATA"

# Uses the location of the .sh file 
# Modify this if you want to use 
CurrentPath=$(pwd)
ProjectPath="$CurrentPath/MelissaAddressObjectLinuxDotnet"
BuildPath="$ProjectPath/Build"
DataPath="$ProjectPath/Data"

if [ ! -d $DataPath ];
then
    mkdir $DataPath
fi

if [ ! -d $BuildPath ];
then
    mkdir $BuildPath
fi

# Config variables for download file(s)
Config_FileName="libmdAddr.so"
Config_ReleaseVersion=$RELEASE_VERSION
Config_OS="LINUX"
Config_Compiler="GCC48"
Config_Architecture="64BIT"
Config_Type="BINARY"

######################## Functions #########################

DownloadDataFiles()
{
    printf "========================== MELISSA UPDATER =========================\n"
    printf "MELISSA UPDATER IS DOWNLOADING DATA FILE(S)...\n"

    ./MelissaUpdater/MelissaUpdater manifest -p $ProductName -r $RELEASE_VERSION -l $1 -t $DataPath 

    if [ $? -ne 0 ];
    then
        printf "\nCannot run Melissa Updater. Please check your license string!\n"
        exit 1
    fi     
    
    printf "Melissa Updater finished downloading data file(s)!\n"
}

DownloadSO() 
{
    printf "\nMELISSA UPDATER IS DOWNLOADING SO(s)...\n"
    
    # Check for quiet mode
    if [ $quiet == "true" ];
    then
        ./MelissaUpdater/MelissaUpdater file --filename $Config_FileName --release_version $Config_ReleaseVersion --license $1 --os $Config_OS --compiler $Config_Compiler --architecture $Config_Architecture --type $Config_Type --target_directory $BuildPath &> /dev/null
        if [ $? -ne 0 ];
        then
            printf "\nCannot run Melissa Updater. Please check your license string!\n"
            exit 1
        fi
    else
        ./MelissaUpdater/MelissaUpdater file --filename $Config_FileName --release_version $Config_ReleaseVersion --license $1 --os $Config_OS --compiler $Config_Compiler --architecture $Config_Architecture --type $Config_Type --target_directory $BuildPath 
        if [ $? -ne 0 ];
        then
            printf "\nCannot run Melissa Updater. Please check your license string!\n"
            exit 1
        fi
    fi
    
    printf "Melissa Updater finished downloading $Config_FileName!\n"
}

CheckSOs() 
{
    if [ ! -f $BuildPath/$Config_FileName ];
    then
        echo "false"
    else
        echo "true"
    fi
}

########################## Main ############################

printf "\n====================== Melissa Address Object ======================\n                    [ .NET | Linux | 64BIT ]\n"

# Get license (either from parameters or user input)
if [ -z "$license" ];
then
  printf "Please enter your license string: "
  read license
fi

# Check for License from Environment Variables 
if [ -z "$license" ];
then
  license=`echo $MD_LICENSE` 
fi

if [ -z "$license" ];
then
  printf "\nLicense String is invalid!\n"
  exit 1
fi

# Use Melissa Updater to download data file(s) 
# Download data file(s) 
DownloadDataFiles $license      # comment out this line if using DQS Release

# Set data file(s) path
#DataPath=""      # uncomment this line and change to your DQS Release data file(s) directory 

#if [ ! -d $DataPath ]; # uncomment this section of code if you are using your own DQS Release data file(s) directory
#then
    #printf "\nData path is invalid!\n"
    #exit 1
#fi

# Download SO(s)
DownloadSO $license 

# Check if all SO(s) have been downloaded. Exit script if missing
printf "\nDouble checking SO file(s) were downloaded...\n"

SOsAreDownloaded=$(CheckSOs)

if [ "$SOsAreDownloaded" == "false" ];
then
    printf "\n$Config_FileName not found"
    printf "\nMissing the above data file(s).  Please check that your license string and directory are correct.\n"

    printf "\nAborting program, see above.\n"
    exit 1
fi

printf "\nAll file(s) have been downloaded/updated!\n"

# Start program
# Build project
printf "\n=========================== BUILD PROJECT ==========================\n"

# Target frameworks net7.0, net5.0, netcoreapp3.1
# Please comment out the version that you don't want to use and uncomment the one that you do want to use
dotnet publish -f="net7.0" -c Release -o $BuildPath MelissaAddressObjectLinuxDotnet/MelissaAddressObjectLinuxDotnet.csproj
#dotnet publish -f="net5.0" -c Release -o $BuildPath MelissaAddressObjectLinuxDotnet/MelissaAddressObjectLinuxDotnet.csproj
#dotnet publish -f="netcoreapp3.1" -c Release -o $BuildPath MelissaAddressObjectLinuxDotnet/MelissaAddressObjectLinuxDotnet.csproj

# Run Project
if [ -z "$address" ] && [ -z "$city" ] && [ -z "$state" ] && [ -z "$zip" ];
then
 	dotnet $BuildPath/MelissaAddressObjectLinuxDotnet.dll --license $license --dataPath $DataPath
else
    dotnet $BuildPath/MelissaAddressObjectLinuxDotnet.dll --license $license --dataPath $DataPath --address "$address" --city "$city" --state "$state" --zip "$zip"
fi
