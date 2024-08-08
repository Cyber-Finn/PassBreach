﻿using MultiArrayTest.Utilities;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

internal class Program
{
    #region member variables
    #region readonly variables
    private static readonly string systemPrefix = "[SYSTEM]";
    private static readonly string[] hashingAlgs = new string[2] { "SHA256", "MD5" };
    private static readonly char[] charsExtended = new char[91]{'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','1','2','3','4','5','6','7','8','9','0','`','~','!','@','#','$','%','^','&','*','(',')','_','+','{','}',':','<','>','/','\\','\'','"','[',']',',','.',' ',';'};
    private static readonly char[] charsLong = new char[15] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o' };
    private static readonly char[] charsMedium = new char[5] { 'a', 'b', 'c', 'd', 'e' };
    private static readonly char[] charsShort = new char[3] { 'a', 'b', 'c' };
    private static readonly List<char[]> TotalChars = new List<char[]>() { charsShort, charsMedium, charsLong, charsExtended };
    private static readonly Dictionary<char[], string> descriptionOfCharSets = new Dictionary<char[], string>() {{charsShort, "English Characters, A-C"}, { charsMedium , "English Characters, A-E"}, {charsLong, "English Characters, A-O"}, {charsExtended, "English Characters, A-Z, with extra symbols" } };
    private static readonly char[] chars = GetUserSelectedCharsArray();
    private static readonly int lengthOfCharsArr = chars.Length;
    #endregion readonly variables

    #region configurable variables
    private static int passLength = 0;
    private static string hashToUse = "";
    private static string userInputHash = "";
    private static string autoGeneratedInputThatGaveUsOurHash = "";
    #endregion configurable variables
    #endregion member variables

    #region Controller and start methods
    /// <summary>
    /// Main logic of the app, called at startup and handles all functionality
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        //clear up all variables
        ClearAllVariables();
        //save our user's input -> this will also gen a (SHA256 or MD5, for now) hash for their password
        LoadUserInput();
        //controller method -> will handle creating all the power sets and comparing the hashes to user input "password" hash
        ReverseEngineer_Controller();
        //write output to console
        WriteFinalOutput();
        //clear up all variables
        ClearAllVariables();
    }
    /// <summary>
    /// Clears variables across runs for memory management. 
    /// </summary>
    private static void ClearAllVariables()
    {
        userInputHash = string.Empty;
        autoGeneratedInputThatGaveUsOurHash = string.Empty;
        hashToUse = string.Empty;
        passLength = 0;
    }
    private static void WriteFinalOutput()
    {
        if (!autoGeneratedInputThatGaveUsOurHash.Equals(""))
        {
            Console.WriteLine($"{systemPrefix} The User's password was: {autoGeneratedInputThatGaveUsOurHash}");
        }
        else
        {
            Console.WriteLine($"{systemPrefix} Couldn't reverse engineer the user's password. Please check the given password length");
        }
        Console.Read(); //just keep the app open until the user has closed it (gives them a chance to read everything)
    }

    ///<summary>
    /// Controller method for Reverse-Engineering. Starts the data generation and brute forcing
    ///</summary>
    private static void ReverseEngineer_Controller()
    {
        //load up all the required characters
        ReverseEngineerHash_DataCreationAndCheck_FirstStep();
    }
    #endregion Controller and start methods

    #region Load user config and thresholds
    private static void LoadUserInput()
    {
        hashToUse = GetUserSelectedHashAlgorithm();
        
        //check if they already have a hash they want to reverse-engineer, and then handle that if they do
        if(HandleUserAlreadyHasHashToCalc())
        {
            return;
        }

        passLength = GetUserDesiredPassLength();
        LoadUserPasswordText();
    }
    private static char[] GetUserSelectedCharsArray()
    {
        Console.WriteLine($"{systemPrefix} Please choose a set of characters to use for reverse-engineering. \r\n{systemPrefix} There are currently {TotalChars.Count} sets to choose from. Pick a number between 1 and {TotalChars.Count}. " +
                          $"\r\n{systemPrefix} Please note: The more characters we use, the higher the resource costs of reverse-engineering will be." +
                          $"{systemPrefix} These are the currently available options: ");
        int leng = TotalChars.Count;
        for(int i = 0; i < leng; i++)
        {
            Console.WriteLine($"Set {i+1}. Number of characters: {TotalChars[i].Length}. Description: {descriptionOfCharSets[TotalChars[i]]}");
        }
        Console.WriteLine($"{systemPrefix} Please enter your selection:");

        return TotalChars[LoadUserSelectedCharsArray()];
    }
    private static int LoadUserSelectedCharsArray()
    {
        string? strRep = Console.ReadLine();
        try
        {
            if (!String.IsNullOrEmpty(strRep))
            {
                return (int.Parse(strRep)-1);
            }
            return LoadUserSelectedCharsArray();
        }
        catch
        {
            return 0;
        }
    }
    private static bool HandleUserAlreadyHasHashToCalc()
    {
        //check if they already have a hash they want to reverse-engineer
        if (CheckIfUserPredefinedPwordHash())
        {
            //find out what the hash is
            userInputHash = GetuserPredefinedPwordHash();

            //find out how many characters they want to calculate/iterate for
            passLength = GetUserUpperLimitOfChars();

            //if the pword is 0, we need to basically just let it loop until it finds an answer. Easiest way of doing this would be to overwrite the user's input to 64 chars (current upper limit for most pwords)
            if (passLength == 0)
            {
                passLength = 64;
            }

            //we need to tell the app to skip asking the user for input and calculating a new hash now, because we already have one
            return true;
        }
        //else
        //do the regular operations
        return false;
    }
    private static int GetUserUpperLimitOfChars()
    {
        Console.WriteLine($"{systemPrefix} Please enter the maximum number of password characters to reverse-engineer for. \r\n{systemPrefix} Please Note: Higher values result in more resource usage, and using 0 will default the app to run until it finds an answer, with no upper limit. \r\n{systemPrefix} Very Important Note: Please monitor your resources closely, and use a reasonable threshold, as this can be hazardous for your computer");
        int finalAns = LoadUserUpperLimitOfChars();

        return finalAns;
    }
    private static int LoadUserUpperLimitOfChars()
    {
        string? strRep = Console.ReadLine();
        try
        {
            if (!String.IsNullOrEmpty(strRep))
            {
                return int.Parse(strRep);
            }
            return LoadUserUpperLimitOfChars();
        }
        catch
        {
            return 0;
        }
    }
    private static string GetuserPredefinedPwordHash()
    {
        Console.WriteLine($"{systemPrefix} Please enter the Hash to Reverse-Engineer:");
        string finalAns = LoadUserPredefinedHashInput();

        return finalAns;
    }
    private static string LoadUserPredefinedHashInput()
    {
        string? strRep = Console.ReadLine();
        try
        {
            if (!String.IsNullOrEmpty(strRep))
            {
                return strRep.ToLower(); //some places use a hash that has uppercase characters, it's meant to be lowercase lol
            }
            return LoadUserPredefinedHashInput();
        }
        catch
        {
            return string.Empty;
        }
    }
    private static bool CheckIfUserPredefinedPwordHash()
    {
        Console.WriteLine($"{systemPrefix} Do you already have a Hex-representation {hashToUse} Hash to reverse-engineer? \r\n{systemPrefix} Note: Please answer (Y/N) only");
        bool finalAns = UserHasPredefinedHashInput();

        return finalAns;
    }
    private static bool UserHasPredefinedHashInput()
    {
        bool finalAns = false;
        string? strRep = Console.ReadLine();
        try
        {
            if (!String.IsNullOrEmpty(strRep))
            {
                if (strRep.Equals("Y"))
                {
                    finalAns = true;
                    return finalAns;
                }
                else if (strRep.Equals("N"))
                {
                    finalAns = false;
                    return finalAns;
                }
            }
            return UserHasPredefinedHashInput();
        }
        catch
        {
            return false;
        }
    }
    private static int GetUserDesiredPassLength()
    {
        Console.WriteLine($"{systemPrefix} Please insert the length of the password you want to crack (example: 3) \r\n{systemPrefix} Note: The longer the password is, the more time it will take to crack.");
        int finalAns = LoadUserPassLengInput();

        return finalAns;
    }
    private static int LoadUserPassLengInput()
    {
        string? strRep = Console.ReadLine();
        try
        {
            if (!String.IsNullOrEmpty(strRep))
            {
                //we're just going to assume that the user gave an actual number >= 1
                return int.Parse(strRep);
            }
            return LoadUserPassLengInput();
        }
        catch
        {
            return 0;
        }
    }
    private static string GetUserSelectedHashAlgorithm()
    {
        string str = "";
        int hashAlgsCount = hashingAlgs.Count();
        int hashAlgsCountMin1 = hashingAlgs.Count() - 1;

        for(int i = 0; i < hashAlgsCountMin1; i++)
        {
            str += hashingAlgs[i] + ", ";
        }
        str += hashingAlgs[hashAlgsCountMin1];


        Console.WriteLine($"{systemPrefix} The currently available hash algorithms are: {str}.\r\n{systemPrefix} You have {hashAlgsCount} options to choose from. Please select a number between 1 and {hashAlgsCount}");

        string? s = LoadUserSelectedHashAlgorithm();
        Console.WriteLine($"{systemPrefix} Selected hashing algorithm: {s}");

        return s;
    }
    private static string? LoadUserSelectedHashAlgorithm()
    {
        string? s = Console.ReadLine();
        //generate an MD5 hash for the password (MD5 here is arb, we could also use SHA256, in exactly the same way etc.)

        try
        {
            if (!String.IsNullOrEmpty(s))
            {
                //we're just going to assume that the user gave an actual number >= 1
                s = hashingAlgs[int.Parse(s)-1];
                return s;
            }
            return LoadUserSelectedHashAlgorithm();
        }
        catch
        {
            return "";
        }
    }
    private static void LoadUserPasswordText()
    {
        string str = "";
        foreach (char c in chars)
        {
            if (c == chars[chars.Length - 1])
            {
                str += "and " + c;
                continue;
            }
            str += c + ", ";
        }
        Console.WriteLine($"{systemPrefix} These are the {str.Length} characters that you can use to create a {passLength} character-long password:\r\n[ {str} ]");
        Console.WriteLine($"{systemPrefix} Please enter a password:");

        HandleSpecificLogicBasedOnSelectedHashAlg(0);
    }
    private static void HandleSpecificLogicBasedOnSelectedHashAlg(int step)
    {
        switch (step)
        {
            case 0: //get user Password text
                string? s = Console.ReadLine();
                if (hashToUse.Equals(hashingAlgs[0]))
                {
                    //generate a SHA256 hash for the password
                    userInputHash = SHA256Generator.ComputeSHA256Hash(s);
                }
                if (hashToUse.Equals(hashingAlgs[1]))
                {
                    //generate an MD5 hash for the password
                    userInputHash = GetMD5ForUserInput(s);
                }
                Console.WriteLine($"{systemPrefix} Hash of password \"{s}\" was: {userInputHash}. \r\n{systemPrefix} Reverse-engineering password hash..");
                break;

        }
    }
    #endregion Load user config and thresholds

    #region Reverse-Engineering
    /// <summary>
    /// Checks if any of the input data matches the hash
    /// </summary>
    /// <param name="inputListofList"></param>
    /// <returns>False if we need to keep processing, true if we need to stop execution (We have an answer)</returns>
    private static bool HandleCheckIfWeHaveAnswer(ref List<List<string>> inputListofList)
    {
        //this is to stop execution if we've got the answer we wanted to find (Default to true, to do exec until it finds answer)
        bool stopExecution = false;

        if (hashToUse.Equals(hashingAlgs[0]))
        {
            //pass the list to the Sha256 function
            stopExecution = CheckIfWeHaveAnswer_Sha256(ref inputListofList);
        }
        if (hashToUse.Equals(hashingAlgs[1]))
        {
            //pass the list to the MD5 function
            stopExecution = CheckIfWeHaveAnswer_MD5(ref inputListofList);
        }
        return stopExecution;
    }
    /// <summary>
    /// Checks if we were able to reverse-engineer the MD5 hash so far
    /// </summary>
    /// <param name="list"></param>
    /// <returns>True if we have reverse-engineered the hash, False if we have not - and need to keep trying</returns>
    private static bool CheckIfWeHaveAnswer_MD5(ref List<List<string>> inputListofList)
    {
        //this is to stop execution if we've got the answer we wanted to find (Default to true, to do exec until it finds answer)
        bool continueExecution = true;
        //get a count of the elements in the global array
        int countOfInputList = inputListofList.Count;
        //iterate over every List of lists
        for (int i = 0; i < countOfInputList; i++)
        {
            //stop exec if we already have an answer
            if (continueExecution)
            {
                List<string> currList = inputListofList[i];
                continueExecution = ReverseEngineerHash_CheckIfMatch_MD5(ref currList);
            }
            else
            {
                return true; //we dont want to continue, since we have an answer
            }
        }
        return false;
    }
    /// <summary>
    /// Checks if we were able to reverse-engineer the SHA256 hash so far
    /// </summary>
    /// <param name="list"></param>
    /// <returns>True if we have reverse-engineered the hash, False if we have not - and need to keep trying</returns>
    private static bool CheckIfWeHaveAnswer_Sha256(ref List<List<string>> inputListofList)
    {
        //this is to stop execution if we've got the answer we wanted to find (Default to true, to do exec until it finds answer)
        bool continueExecution = true;
        //get a count of the elements in the global array
        int countOfInputList = inputListofList.Count;
        //iterate over every List of lists
        for (int i = 0; i < countOfInputList; i++)
        {
            //stop exec if we already have an answer
            if (continueExecution)
            {
                List<string> currList = inputListofList[i];
                continueExecution = ReverseEngineerHash_CheckIfMatch_SHA256(ref currList);
            }
            else
            {
                return true; //we dont want to continue, since we have an answer
            }
        }
        return false;
    }
    /// <summary>
    /// Checks if we were able to reverse-engineer the SHA256 hash so far
    /// </summary>
    /// <param name="list"></param>
    /// <returns>False if we have reverse-engineered, true if we have not - and need to keep trying</returns>
    private static bool ReverseEngineerHash_CheckIfMatch_SHA256(ref List<string> list)
    {
        string strInputThatMatchedTheHash = "";
        //for every item in the list
        foreach (string autoGeneratedListItem in list)
        {
            //only do stuff if our input is empty. No point asking a question if we already have an answer.
            if (strInputThatMatchedTheHash.Equals(""))
            {
                string hashForThisInputStr = SHA256Generator.ComputeSHA256Hash(autoGeneratedListItem);

                //do the check after our md5 object is destroyed - to not keep it in memory unnecessarily
                //if the hash matches to what the user inputted, set our input to the string we've found to match the hash
                if (CompareHashes(hashForThisInputStr))
                {
                    strInputThatMatchedTheHash = autoGeneratedListItem;
                    break;
                }
            }
        }
        //save the input that gave us the hash we wanted
        autoGeneratedInputThatGaveUsOurHash = strInputThatMatchedTheHash;
        //if we have an answer, we want to stop execution.
        return (strInputThatMatchedTheHash.Equals(""));
    }
    /// <summary>
    /// calculate an MD5 for every given input, then compare the md5 to the user's calculated/given Hash
    /// </summary>
    /// <param name="list"></param>
    /// <returns>False if we have reverse-engineered, true if we have not - and need to keep trying</returns>
    private static bool ReverseEngineerHash_CheckIfMatch_MD5(ref List<string> list)
    {
        string strInputThatMatchedTheHash = "";
        //for every item in the list
        foreach (string autoGeneratedListItem in list)
        {
            //only do stuff if our input is empty. No point asking a question if we already have an answer.
            if (strInputThatMatchedTheHash.Equals(""))
            {
                string hashForThisInputStr = "";
                //generate a new md5
                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    //create a byte[] from the string in our list
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(autoGeneratedListItem);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    //convert the hash to string
                    hashForThisInputStr = Convert.ToHexString(hashBytes); // .NET 5 and up. In .net 4 and lower, we need to do this manually (no method to do it for us)
                }
                //do the check after our md5 object is destroyed - to not keep it in memory unnecessarily
                //if the hash matches to what the user inputted, set our input to the string we've found to match the hash
                if (CompareHashes(hashForThisInputStr))
                {
                    strInputThatMatchedTheHash = autoGeneratedListItem;
                    break;
                }
            }
        }
        //save the input that gave us the hash we wanted
        autoGeneratedInputThatGaveUsOurHash = strInputThatMatchedTheHash;
        //if we have an answer, we want to stop execution.
        return (strInputThatMatchedTheHash.Equals(""));
    }
    /// <summary>
    /// get a hash for the input
    /// </summary>
    /// <param name="s"></param>
    /// <returns>Hex representation MD5 of the input</returns>
    private static string GetMD5ForUserInput(string s)
    {
        string st = "";
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            //create a byte[] from the string in our list
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(s);
            //create a new byte[] that consists of the md5 hash of our input
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            //convert the hash to string
            st = Convert.ToHexString(hashBytes); // .NET 5 and up. In .net 4 and lower, we need to do this manually (no method to do it for us)
        }
        return st;
    }
    private static bool CompareHashes(string hashInput)
    {
        return (userInputHash.Equals(hashInput));
    }
    #endregion Reverse-Engineering

    #region Data generators
    /// <summary>
    /// Starts the process of reverse-engineering. Will trigger other processes depending on the password length
    /// </summary>
    private static void ReverseEngineerHash_DataCreationAndCheck_FirstStep() 
    {
        //we only want to do this if the req no. of password chars is 1 or higher
        if (passLength < 1)
        {
            return;
        }

        if (hashToUse.Equals(hashingAlgs[0]))
        {
            ReverseEngineerHash_DataCreationAndCheck_SeedSha256();
            return;
        }
        if (hashToUse.Equals(hashingAlgs[1]))
        {
            ReverseEngineerHash_DataCreationAndCheck_SeedMD5();
            return;
        }
    }
    /// <summary>
    /// A method that manages data creation and checking if any of the data matches the input hash.
    /// </summary>
    /// <remarks>
    /// This is the SHA256 method. There are other methods for different algorithms
    /// </remarks>
    private static void ReverseEngineerHash_DataCreationAndCheck_SeedSha256()
    {
        //create a list to hold our data
        List<string> baseStringList_InitialChars = new List<string>();
        // for every char in the chars[]
        for (int i = 0; i < lengthOfCharsArr; i++)
        {
            //add our string (of a single char) to the list
            baseStringList_InitialChars.Add(chars[i].ToString());
        }

        foreach (string s in baseStringList_InitialChars)
        {
            //execute only if we don't already have an answer
            if (autoGeneratedInputThatGaveUsOurHash.Equals(""))
            {
                //reuse the same method we had earlier to get a hash for a single string (This method will prod a hash for a single char)
                if (CompareHashes(SHA256Generator.ComputeSHA256Hash(s)))
                {
                    autoGeneratedInputThatGaveUsOurHash = s;
                }
            }
            else //we have an answer, so stop execution
            {
                return;
            }
        }
        int remainingPassLength = passLength - 1;

        /// Note: we only want to get all power sets if the password length is greater than 1 (else, we only generate and compare 1 char)
        if (passLength > 1)
        {
            int count = baseStringList_InitialChars.Count;
            ReverseEngineerHash_DataCreationAndCheck_SecondStep(ref baseStringList_InitialChars, ref count, ref remainingPassLength);
        }
    }
    /// <summary>
    /// A method that manages data creation and checking if any of the data matches the input hash.
    /// </summary>
    /// <remarks>
    /// This is the MD5 method. There are other methods for different algorithms
    /// </remarks>
    private static void ReverseEngineerHash_DataCreationAndCheck_SeedMD5()
    {
        //create a list to hold our data
        List<string> baseStringList_InitialChars = new List<string>();
        // for every char in the chars[]
        for (int i = 0; i < lengthOfCharsArr; i++)
        {
            //add our string (of a single char) to the list
            baseStringList_InitialChars.Add(chars[i].ToString());
        }

        foreach (string s in baseStringList_InitialChars)
        {
            //execute only if we don't already have an answer
            if (autoGeneratedInputThatGaveUsOurHash.Equals(""))
            {
                //reuse the same method we had earlier to get a hash for a single string (This method will prod a hash for a single char)
                if (CompareHashes(GetMD5ForUserInput(s)))
                {
                    autoGeneratedInputThatGaveUsOurHash = s;
                }
            }
            else //we have an answer, so stop execution
            {
                return;
            }
        }
        int remainingPassLength = passLength - 1;

        /// Note: we only want to get all power sets if the password length is greater than 1 (else, we only generate and compare 1 char)
        if (passLength > 1)
        {
            int count = baseStringList_InitialChars.Count;
            ReverseEngineerHash_DataCreationAndCheck_SecondStep(ref baseStringList_InitialChars, ref count, ref remainingPassLength);
        }
    }
    /// <summary>
    /// load up the second char in the password. 
    /// Functionally, this is slightly different to the recursive method <see cref="ReverseEngineerHash_DataCreationAndCheck_RecursiveFinalStep"/> that gets all remaining chars.
    /// <br></br>The reason for this, is simply because the initial List (string of a single char) requires only iteration over that list,
    /// but, as the size of the power set grows, we need to iterate over multiple power sets, this warrants a new separate method where we need to create a List of Lists of all possible combinations.
    /// <br></br> That's what this method does
    /// </summary>
    /// <param name="previousList"></param>
    /// <param name="countOfPreviousList"></param>
    /// <param name="noOfIterationsLeft"></param>
    private static void ReverseEngineerHash_DataCreationAndCheck_SecondStep(ref List<string> previousList, ref int countOfPreviousList, ref int noOfIterationsLeft)
    {
        //we only want to iterate over chars and do stuff, if we need to (-> if there's another letter for us to try to get)
        if(noOfIterationsLeft > 0)
        {
            //this list is going to store all combinations for all elements in the previous list (Remember, we need to now add every character to the end of each element to get all poss combs)
            List<List<string>> listOfAllPossibleCombs = new List<List<string>>();
            //for each element in the previous list -> we now need to add a set of chars to the end of each one
            for (int i = 0; i < countOfPreviousList; i++)
            {
                //get each item in the previous list
                string str = previousList[i];
                //create a list to hold all the new possible combs - for each item in previous list
                List<string> strList = new List<string>();
                //iterate - for each item, for lengthOfCharsArr - to add all possible combs to the end of it
                for (int j = 0; j < lengthOfCharsArr; j++)
                {
                    //for "str" - which was the previousElem - we want to add all possible combs
                    //for this purpose, leave str unchanged (so we can use it again)
                    //create a new string with the new char appended to the end
                    string copyOfStr = str;
                    //simply append the new char to the end..
                    copyOfStr += chars[j];
                    //save the new string (or char[] - if you want to think about it like that) to the list for this set
                    strList.Add(copyOfStr);
                }
                //once we've got a list of all new possible combs for THIS specific "row"/set in the previous list, add it to the new list
                listOfAllPossibleCombs.Add(strList);
            }
            //decrease the noOfIterationsLeft, because we don't want to go to next phase, if we already have the max chars
            noOfIterationsLeft--;

            if(HandleCheckIfWeHaveAnswer(ref listOfAllPossibleCombs))
            {
                ClearMemory(ref listOfAllPossibleCombs);
                ClearMemory(ref previousList);
                return;
            }

            //clear the previous stack's list
            ClearMemory(ref previousList);

            //if noOfIterationsLeft is still > 1 (-> we still have some characters to get), get the remaining chars
            if (noOfIterationsLeft > 0)
            {
                //calculate the no of elements of the current list
                int countOfCurrList = listOfAllPossibleCombs.Count;
                //call the recursive method to continue with the rest of the numbers, until there's no letters left to get
                ReverseEngineerHash_DataCreationAndCheck_RecursiveFinalStep(ref listOfAllPossibleCombs, ref countOfCurrList, ref noOfIterationsLeft);
            }
        }
    }
    /// <summary>
    /// Recursively load up all the remaining chars in the password and check if any of them match
    /// </summary>
    /// <param name="ListOfList_previousCombinationsList"></param>
    /// <param name="countOfPreviousList"></param>
    /// <param name="noOfIterationsLeft"></param>
    private static void ReverseEngineerHash_DataCreationAndCheck_RecursiveFinalStep(ref List<List<string>> ListOfList_previousCombinationsList, ref int countOfPreviousList, ref int noOfIterationsLeft)
    {
        if (noOfIterationsLeft >= 1)
        {
            //this list is going to store all combinations for all elements in the previous list (Remember, we need to now add every character to the end of each element to get all poss combs)
            List<List<string>> listOfAllPossibleCombs = new List<List<string>>();
            //for each element in the previous list
            for (int i = 0; i < countOfPreviousList; i++)
            {
                //calculate the size of the previous list's i-th element, so that we can iterate over all items in THAT list
                int countOfPreviousList_InnerList = ListOfList_previousCombinationsList[i].Count;
                //iterate over every element in that list
                for (int m = 0; m < countOfPreviousList_InnerList; m++)
                {
                    //get each item in the previous list's inner list
                    string str = ListOfList_previousCombinationsList[i][m];
                    //create a list to hold all the new possible combs - for each item in previous list
                    List<string> strList = new List<string>();
                    //iterate - for each item, for lengthOfCharsArr - to add all possible combs to the end of it
                    for (int j = 0; j < lengthOfCharsArr; j++)
                    {
                        //for "str" - which was the previousElem - we want to add all possible combs
                        //for this purpose, leave str unchanged (so we can use it again)
                        //create a new string with the new char appended to the end
                        string copyOfStr = str;
                        //simply append the new char to the end..
                        copyOfStr += chars[j];
                        //save the new string (or char[] - if you want to think about it like that) to the list for this set
                        strList.Add(copyOfStr);
                    }
                    //once we've got a list of all new possible combs for THIS specific "row"/set in the previous list, add it to the new list
                    listOfAllPossibleCombs.Add(strList);
                }
            }

            if (HandleCheckIfWeHaveAnswer(ref listOfAllPossibleCombs))
            {
                ClearMemory(ref listOfAllPossibleCombs);
                ClearMemory(ref ListOfList_previousCombinationsList);
                return;
            }

            //clear the previous stack's list
            ClearMemory(ref ListOfList_previousCombinationsList);

            //decrease the noOfIterationsLeft, because we don't want to go to next phase, if we already have the max chars
            noOfIterationsLeft--;

            //if noOfIterationsLeft is still > 1 (IF we still have some characters to get), get the remaining chars
            if (noOfIterationsLeft >= 1)
            {
                //get the no of elements of the current list
                int countOfCurrList = listOfAllPossibleCombs.Count;
                //call the recursive method to continue with the rest of the numbers
                ReverseEngineerHash_DataCreationAndCheck_RecursiveFinalStep(ref listOfAllPossibleCombs, ref countOfCurrList, ref noOfIterationsLeft);
            }
        }
    }
    #endregion Data generators

    #region Memory Management
    public static void ClearMemory<T>(ref List<T> list)
    {
        int generation = GC.GetGeneration(list);
        list.Clear();
        GC.Collect(generation, GCCollectionMode.Forced);

        //could also set the list to null to uproot the object and let GC manage itself
        //list = null;
    }
    #endregion Memory Management
}