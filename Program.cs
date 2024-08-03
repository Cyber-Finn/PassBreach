﻿using MultiArrayTest.Utilities;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

internal class Program
{
    private static readonly string systemPrefix = "[SYSTEM]";
    private static readonly string[] hashingAlgs = new string[2] { "SHA256", "MD5" };
    private static readonly char[] chars = new char[91]{'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','1','2','3','4','5','6','7','8','9','0','`','~','!','@','#','$','%','^','&','*','(',')','_','+','{','}',':','<','>','/','\\','\'','"','[',']',',','.',' ',';'};
    //private static readonly char[] chars = new char[15]{ 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o' };
    //private static readonly char[] chars = new char[5] { 'a', 'b', 'c', 'd', 'e'};
    //private static readonly char[] chars = new char[3] { 'a', 'b', 'c' };
    private static int lengthOfCharsArr = chars.Length;
    private static int passLength = 0;
    private static string hashToUse = "";

    private static List<List<List<string>>> ListOfListsOfLists_AllPossibleCombs_ForThisManyPasswordChars_forSingleCharacter = new List<List<List<string>>>();
    private static string userInputHash = "";
    private static string autoGeneratedInputThatGaveUsOurHash = "";
    private static void Main(string[] args)
    {
        //clear up all variables
        clearAllVariables();
        //save our user's input -> this will also gen a (SHA256 or MD5, for now) hash for their password
        loadUpUserInput();
        //controller method -> will handle creating all the power sets and comparing the hashes to user input "password" hash
        Controller();
        //write output to console
        writeOutPut();
        //clear up all variables
        clearAllVariables();
    }


    private static void clearAllVariables()
    {
        //clear ListOfListsOfLists_AllPossibleCombs_ForThisManyPasswordChars_forSingleCharacter
        ListOfListsOfLists_AllPossibleCombs_ForThisManyPasswordChars_forSingleCharacter.Clear();
        ListOfListsOfLists_AllPossibleCombs_ForThisManyPasswordChars_forSingleCharacter.TrimExcess();
        //clear userInputHash 
        userInputHash = string.Empty;
        // clear autoGeneratedInputThatGaveUsOurHash 
        autoGeneratedInputThatGaveUsOurHash = string.Empty;
    }

    private static void loadUpUserInput()
    {
        hashToUse = getUserSelectedHashAlgorithm();
        
        //check if they already have a hash they want to reverse-engineer, and then handle that if they do
        if(HandleUserAlreadyHasHashToCalc())
        {
            return;
        }

        passLength = getUserDesiredPassLength();
        getUserPasswordText();
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
            LoadUserUpperLimitOfChars();
        }
        catch
        {
            LoadUserUpperLimitOfChars();
        }
        return 0;
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
                return strRep;
            }
            LoadUserPredefinedHashInput();
        }
        catch
        {
            LoadUserPredefinedHashInput();
        }
        return string.Empty;
    }


    private static bool CheckIfUserPredefinedPwordHash()
    {
        Console.WriteLine($"{systemPrefix} Do you already have a Hex-representation {hashToUse} Hash to reverse-engineer? \r\n{systemPrefix} Note: Please answer (Y/N) only");
        bool finalAns = false; 
        UserHasPredefinedHashInput(out finalAns);

        return finalAns;
    }
    private static void UserHasPredefinedHashInput(out bool finalAns)
    {
        string? strRep = Console.ReadLine();
        try
        {
            if (!String.IsNullOrEmpty(strRep))
            {
                if (strRep.Equals("Y"))
                {
                    finalAns = true;
                    return;
                }
                else if (strRep.Equals("N"))
                {
                    finalAns = false;
                    return;
                }
                else
                {
                    UserHasPredefinedHashInput(out finalAns);
                }
            }
            UserHasPredefinedHashInput(out finalAns);
        }
        catch
        {
            UserHasPredefinedHashInput(out finalAns);
        }
    }

    private static int getUserDesiredPassLength()
    {
        Console.WriteLine($"{systemPrefix} Please insert the length of the password you want to crack (example: 3) \r\n{systemPrefix} Note: The longer the password is, the more time it will take to crack.");
        int finalAns = getuserPassLengInput();

        return finalAns;
    }
    private static int getuserPassLengInput()
    {
        string? strRep = Console.ReadLine();
        int finalAns = 0;
        try
        {
            if (!String.IsNullOrEmpty(strRep))
            {
                //we're just going to assume that the user gave an actual number >= 1
                return int.Parse(strRep);
            }
            getuserPassLengInput();
        }
        catch
        {
            getuserPassLengInput();
        }
        return finalAns;
    }

    private static string getUserSelectedHashAlgorithm()
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

        string? s = getuserAlgInput();
        Console.WriteLine($"{systemPrefix} Selected hashing algorithm: {s}");

        return s;
    }


    private static string? getuserAlgInput()
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
            getuserAlgInput();
        }
        catch
        {
            getuserAlgInput();
        }
        return s;
    }

    private static void getUserPasswordText()
    {
        string str = "";
        foreach (char c in chars)
        {
            str += c + ", ";
        }
        Console.WriteLine($"{systemPrefix} These are the {str.Length} characters that you can use to create a {passLength} character-long password:\r\n{str}");
        Console.WriteLine($"{systemPrefix} Please enter a password:");

        string? s = Console.ReadLine();

        //todo: as mentioned in getUserSelectedHashAlg, we need to make this more modular in future
        if (hashToUse.Equals(hashingAlgs[0]))
        {
            //generate a SHA256 hash for the password
            userInputHash = SHA256Generator.ComputeSHA256Hash(s);
        }
        if (hashToUse.Equals(hashingAlgs[1]))
        {
            //generate an MD5 hash for the password
            userInputHash = getMD5ForUserInput(s);
        }
        Console.WriteLine($"{systemPrefix} Hash of password \"{s}\" was: {userInputHash}");
    }

    private static void writeOutPut()
    {
        if(!autoGeneratedInputThatGaveUsOurHash.Equals(""))
        {
            Console.WriteLine($"{systemPrefix} The User's password was: {autoGeneratedInputThatGaveUsOurHash}");
        }
        else
        {
            Console.WriteLine($"{systemPrefix} Couldn't reverse engineer the user's password. Please check the given password length");
        }
    }

    private static void Controller()
    {
        //load up all the required characters
        intialLoadupOfOurFistCharList();
        //test each combination until we get the original
        dataAccessor();
    }

    /// <summary>
    /// Access the data in our global list of lists of lists
    /// </summary>
    private static void dataAccessor()
    {
        //this is to stop execution if we've got the answer we wanted to find (Default to true, to do exec until it finds answer)
        bool continueExecution = true; 
        //get a count of the elements in the global array
        int countOfGlobalListOfListsOfLists = ListOfListsOfLists_AllPossibleCombs_ForThisManyPasswordChars_forSingleCharacter.Count;
        //iterate over every List of lists in the global array
        for (int i = 0; i < countOfGlobalListOfListsOfLists; i++)
        {
            //stop exec if we already have an answer
            if(continueExecution)
            {
                //get the size of the lists in our inner List
                int countOfCurrentInnerListOfList = ListOfListsOfLists_AllPossibleCombs_ForThisManyPasswordChars_forSingleCharacter[i].Count;
                //iterate over every List in the list of lists
                for (int j = 0; j < countOfCurrentInnerListOfList; j++)
                {
                    //stop exec if we already have an answer
                    if (continueExecution)
                    {
                        List<string> currList = ListOfListsOfLists_AllPossibleCombs_ForThisManyPasswordChars_forSingleCharacter[i][j];

                        //todo: as mentioned in getUserSelectedHashAlg, we need to make this more modular in future
                        if (hashToUse.Equals(hashingAlgs[0]))
                        {
                            //pass the list to the MD5 function
                            continueExecution = getSha256_StagingMethod(ref currList);
                        }
                        if (hashToUse.Equals(hashingAlgs[1]))
                        {
                            //pass the list to the MD5 function
                            continueExecution = getMd5_StagingMethod(ref currList);
                        }
                    }
                }
            }
        }
    }

    private static bool getSha256_StagingMethod(ref List<string> list)
    {
        bool continueExecution = true;
        List<string> newList = new List<string>();

        //foreach element in the list
        foreach (string autoGeneratedListItem in list)
        {
            //we want to now add a new char to the end of the element array
            for (int j = 0; j < lengthOfCharsArr; j++)
            {
                //stop exec if we already have an answer
                if (continueExecution)
                {
                    //create a temp string
                    string s = "";
                    //convert each str to a char[]
                    char[] carr = autoGeneratedListItem.ToCharArray();
                    int lengOfTempArr = ((carr.Length)); // Min1, because it needs to be 0-based index

                    if (lengOfTempArr < passLength)
                    {
                        //add each element to the new string
                        for (int i = 0; i < lengOfTempArr; i++)
                        {
                            s += carr[i];
                        }
                        //add a new char to the string
                        s += chars[j];
                        //add our newly replaced character to the new list
                        newList.Add(s);
                    }
                }
            }
            if (newList.Count > 0)
            {
                continueExecution = getSHA256ForGeneratedInput(ref newList);
                newList.Clear();
                newList.TrimExcess();
            }
        }
        //if we still don't have an answer - likely because newList+newChar didn't equal passleng
        if (continueExecution)
        {
            continueExecution = getSHA256ForGeneratedInput(ref list);
        }
        return continueExecution;
    }

    private static bool getSHA256ForGeneratedInput(ref List<string> list)
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
    /// This method will call the md5 function, and iterate through all possible combs
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private static bool getMd5_StagingMethod(ref List<string> list)
    {
        bool continueExecution = true;
        List<string> newList = new List<string>();

        //foreach element in the list
        foreach (string autoGeneratedListItem in list)
        {
            //we want to now add a new char to the end of the element array
            for (int j = 0; j < lengthOfCharsArr; j++)
            {
                //stop exec if we already have an answer
                if (continueExecution)
                {
                    //create a temp string
                    string s = "";
                    //convert each str to a char[]
                    char[] carr = autoGeneratedListItem.ToCharArray();
                    int lengOfTempArr = ((carr.Length)); // Min1, because it needs to be 0-based index

                    if(lengOfTempArr < passLength)
                    {
                        //add each element to the new string
                        for (int i = 0; i < lengOfTempArr; i++)
                        {
                            s += carr[i];
                        }
                        //add a new char to the string
                        s += chars[j];
                        //add our newly replaced character to the new list
                        newList.Add(s);
                    }
                }
            }
            if(newList.Count>0)
            {
                continueExecution = getMD5ForGeneratedInput(ref newList);
                newList.Clear();
                newList.TrimExcess();
            }
        }
        //if we still don't have an answer - likely because newList+newChar didn't equal passleng
        if (continueExecution)
        {
            continueExecution = getMD5ForGeneratedInput(ref list);
        }
        return continueExecution;
    }

    /// <summary>
    /// calculate an MD5 for every given input, then compare the md5 to the MD5 we calculated on input
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private static bool getMD5ForGeneratedInput(ref List<string> list)
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

    //simply get a hash for the combo a user has inputted
    private static string getMD5ForUserInput(string s)
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

    /// <summary>
    /// get the first char combination - saved to a list
    /// (later, we'll make this check if any of the hashes match to the hash of user)
    /// </summary>
    private static void intialLoadupOfOurFistCharList() 
    {
        //we only want to do this if the req no. of password chars is 1 or higher
        if (passLength < 1)
        {
            return;
        }

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
                if (hashToUse.Equals(hashingAlgs[0]))
                {
                    //reuse the same method we had earlier to get a hash for a single string (This method will prod a hash for a single char)
                    if (CompareHashes(SHA256Generator.ComputeSHA256Hash(s)))
                    {
                        autoGeneratedInputThatGaveUsOurHash = s;
                    }
                }
                if (hashToUse.Equals(hashingAlgs[1]))
                {
                    //reuse the same method we had earlier to get a hash for a single string (This method will prod a hash for a single char)
                    if (CompareHashes(getMD5ForUserInput(s)))
                    {
                        autoGeneratedInputThatGaveUsOurHash = s;
                    }
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
            secondLoadupAttempt(ref baseStringList_InitialChars, ref count, ref remainingPassLength);
        }
    }

    /// <summary>
    /// load up the second char in the password. 
    /// Functionally, this is slightly different to the recursive method (recurseAddBaseStringVariationsToLists) that gets all remaining chars
    ///     reason for this, is simply because the initial List (of a single char) requires only iteration over that list,
    ///        but, as the size of the power set grows, we need to iterate over multiple sets
    ///             this warrants a new method
    /// </summary>
    /// <param name="previousList"></param>
    /// <param name="countOfPreviousList"></param>
    /// <param name="noOfIterationsLeft"></param>
    private static void secondLoadupAttempt(ref List<string> previousList, ref int countOfPreviousList, ref int noOfIterationsLeft)
    {
        //we only want to iterate over chars and do stuff, if we need to (-> if there's another letter for us to try to get)
        if(noOfIterationsLeft >= 1)
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

            //if noOfIterationsLeft is still > 1 (-> we still have some characters to get), get the remaining chars
            if (noOfIterationsLeft >= 1)
            {
                //calculate the no of elements of the current list
                int countOfCurrList = listOfAllPossibleCombs.Count;
                //call the recursive method to continue with the rest of the numbers, until there's no letters left to get
                recurseAddBaseStringVariationsToLists(ref listOfAllPossibleCombs, ref countOfCurrList, ref noOfIterationsLeft);
            }
            //else, we just save what we currently have
            else if (noOfIterationsLeft == 0)
            {
                ListOfListsOfLists_AllPossibleCombs_ForThisManyPasswordChars_forSingleCharacter.Add(listOfAllPossibleCombs);
            }
        }
    }
    /// <summary>
    /// Recursively load up all the remaining chars in the password
    /// </summary>
    /// <param name="ListOfList_previousCombinationsList"></param>
    /// <param name="countOfPreviousList"></param>
    /// <param name="noOfIterationsLeft"></param>
    private static void recurseAddBaseStringVariationsToLists(ref List<List<string>> ListOfList_previousCombinationsList, ref int countOfPreviousList, ref int noOfIterationsLeft)
    {
        if (noOfIterationsLeft >= 1)
        {
            //this list is going to store all combinations for all elements in the previous list (Remember, we need to now add every character to the end of each element to get all poss combs)
            List<List<string>> listOfAllPossibleCombs = new List<List<string>>();
            //for each element in the previous list -> Which was a List, itself
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

            //clear the previous stack's list
            //ListOfList_previousCombinationsList = null;
            ClearMemory(ref ListOfList_previousCombinationsList);

            //decrease the noOfIterationsLeft, because we don't want to go to next phase, if we already have the max chars
            noOfIterationsLeft--;

            //if noOfIterationsLeft is still > 1 (IF we still have some characters to get), get the remaining chars
            if (noOfIterationsLeft >= 1)
            {
                //get the no of elements of the current list
                int countOfCurrList = listOfAllPossibleCombs.Count;
                //call the recursive method to continue with the rest of the numbers
                recurseAddBaseStringVariationsToLists(ref listOfAllPossibleCombs, ref countOfCurrList, ref noOfIterationsLeft);
            }

            //only on the LAST run/iteration, we want to save the data to a global variable, so that we can access it from other parts of our program
            if(noOfIterationsLeft == 0)
            {
                //add the current list to uor global list of lists of lists..
                ListOfListsOfLists_AllPossibleCombs_ForThisManyPasswordChars_forSingleCharacter.Add(listOfAllPossibleCombs);
            }
        }
    }

    public static void ClearMemory<T>(ref List<T> list)
    {
        int generation = GC.GetGeneration(list);
        list.Clear();
        GC.Collect(generation, GCCollectionMode.Forced);

        //could also set the list to null to uproot the object and let GC manage itself
        //list = null;
    }
}