# **StringBuffer** #

The StringBuffer class has no relation to the equivalent of the same name in Java.  
The purpose of this class is to replace the StringBuilder class who suffer from some lack of functionality, without using extension.


## Using the StringBuffer Class ##

The String object is immutable. Every time you use one of the methods in the System.String class, you create a new string object in memory, which requires a new allocation of space for that new object.
In situations where you need to perform repeated modifications to a string, the overhead associated with creating a new String object can be costly. The StringBuffer class can be used when you want to modify a string without creating a new object. For example, using the StringBuffer class can boost performance when concatenating many strings together in a loop.


## Instantiating a StringBuffer Object ##

You can create a new instance of the StringBuffer class by initializing your variable with one of the overloaded constructor methods, as illustrated in the following example.

    
    StringBuffer sb = new StringBuffer("Hello World!");
    


## Modifying the StringBuffer String ##

The following table lists the methods you can use to modify the contents of a StringBuffer.

| Method name                     | Use                                                                                                                                         |
|---------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------|
| StringBuffer.Append             | Appends information to the end. |
| StringBuffer.AppendLine         | Appends information followed by the default line terminator to the end. |
| StringBuffer.AppendFormat       | Appends information to the end and replaces the format specifier with the formatted text. |
| StringBuffer.AppendLineFormat   | Appends information followed by the default line terminator to the end and replaces the format specifier with the formatted text. |
| StringBuffer.Prepend            | Inserts information to the start. |
| StringBuffer.PrependLine        | Inserts information followed by the default line terminator to the start. |
| StringBuffer.PrependFormat      | Inserts information to the start and replaces the format specifier with the formatted text. |
| StringBuffer.PrependLineFormat  | Inserts information followed by the default line terminator to the start and replaces the format specifier with the formatted text. |
| StringBuffer.Insert             | Inserts information at the specified index. |
| StringBuffer.InsertLine         | Inserts information followed by the default line terminator at the specified index. |
| StringBuffer.InsertFormat       | Inserts information at the specified index and replaces the format specifier with the formatted text. |
| StringBuffer.InsertLineFormat   | Inserts information followed by the default line terminator at the specified index and replaces the format specifier with the formatted text. |
| StringBuffer.Crop               | Retrieves a substring. |
| StringBuffer.Remove             | Removes a range of characters. |
| StringBuffer.Substitute         | Replaces **all occurrences of** a specified information. |
| StringBuffer.Replace            | Replaces a specified information. |
| StringBuffer.ReplaceRange       | Replaces a range of characters. |
| StringBuffer.ReplaceBefore      | Replaces a range of characters before a specified information. |
| StringBuffer.ReplaceAfter       | Replaces a range of characters after a specified information. |
| StringBuffer.ReplaceInside      | Replaces a range of characters after the first and before the second specified information. |
| StringBuffer.SubstringRange     | Retrieves a range of characters. |
| StringBuffer.SubstringBefore    | Retrieves a range of characters before a specified information. |
| StringBuffer.SubstringAfter     | Retrieves a range of characters after a specified information. |
| StringBuffer.SubstringInside    | Retrieves a range of characters after the first and before the second specified information. |
| StringBuffer.SubstringOutside   | Retrieves a range of characters before the first and after the second specified information. |
| StringBuffer.Trim               | Removes all leading and trailing specified information. |
| StringBuffer.TrimStart          | Removes all leading specified information. |
| StringBuffer.TrimEnd            | Removes all trailing specified information. |
| StringBuffer.PadLeft            | Right-aligns the characters by padding them with spaces on the left. |
| StringBuffer.PadRight           | Left-aligns the characters by padding them with spaces on the right. |
| StringBuffer.ToLower            | To lowercase. |
| StringBuffer.ToLowerInvariant   | To lowercase invariant culture. |
| StringBuffer.ToUpper            | To uppercase. |
| StringBuffer.ToUpperInvariant   | To uppercase invariant culture. |
| StringBuffer.FromChar           |                                                                    |
| StringBuffer.FromCharArray      |                                                                    |
| StringBuffer.FromString         |                                                                    |
| StringBuffer.Clear              | Removes all characters. |
| StringBuffer.FormatWith         |                                                                    |


## Helper methods ##

| Method name                     | Use                                                                |
|---------------------------------|--------------------------------------------------------------------|
| StringBuffer.IndexOf            |                                                                    |
| StringBuffer.IndexOfAny         |                                                                    |
| StringBuffer.LastIndexOf        |                                                                    |
| StringBuffer.LastIndexOfAny     |                                                                    |
| StringBuffer.Contains           |                                                                    |
| StringBuffer.ContainsSequence   |                                                                    |
| StringBuffer.ContainsAll        |                                                                    |
| StringBuffer.ContainsAny        |                                                                    |
| StringBuffer.StartsWith         |                                                                    |
| StringBuffer.StartsWithSequence |                                                                    |
| StringBuffer.StartsWithAny      |                                                                    |
| StringBuffer.EndsWith           |                                                                    |
| StringBuffer.EndsWithSequence   |                                                                    |
| StringBuffer.EndsWithAny        |                                                                    |
| StringBuffer.Split              |                                                                    |
| StringBuffer.SplitToBuffer      |                                                                    |
| StringBuffer.ToCharArray        |                                                                    |
| StringBuffer.ToString           |                                                                    |
| StringBuffer.Copy               |                                                                    |


