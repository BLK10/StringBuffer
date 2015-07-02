From [http://github.com/BLK10/StringBuffer](http://github.com/BLK10/StringBuffer)

# **StringBuffer** #

The StringBuffer class has no relation to the equivalent of the same name in Java.  
The purpose of this class is to replace the StringBuilder class who suffer from some lack of functionality (without using extension for the main methods).


## Using the StringBuffer Class ##

The String object is immutable. Every time you use one of the methods in the System.String class, you create a new string object in memory, which requires a new allocation of space for that new object.
In situations where you need to perform repeated modifications to a string, the overhead associated with creating a new String object can be costly. The StringBuffer class can be used when you want to modify a string without creating a new object. For example, using the StringBuffer class can boost performance when concatenating many strings together in a loop.


## Instantiating a StringBuffer Object ##

You can create a new instance of the StringBuffer class by initializing your variable with one of the overloaded constructor methods, as illustrated in the following example.

    
    StringBuffer sb = new StringBuffer("Hello World!");
    


## Modifying the StringBuffer String ##

The following table lists the methods you can use to modify the contents of a StringBuffer.

| Method name        | Use                                                                                                                                           |
|--------------------|-----------------------------------------------------------------------------------------------------------------------------------------------|
| Append             | Appends information to the end. |
| AppendLine         | Appends information followed by the default line terminator to the end. |
| AppendFormat       | Appends information to the end and replaces the format specifier with the formatted text. |
| AppendLineFormat   | Appends information followed by the default line terminator to the end and replaces the format specifier with the formatted text. |
| Prepend            | Inserts information to the start. |
| PrependLine        | Inserts information followed by the default line terminator to the start. |
| PrependFormat      | Inserts information to the start and replaces the format specifier with the formatted text. |
| PrependLineFormat  | Inserts information followed by the default line terminator to the start and replaces the format specifier with the formatted text. |
| Insert             | Inserts information at the specified index. |
| InsertLine         | Inserts information followed by the default line terminator at the specified index. |
| InsertFormat       | Inserts information at the specified index and replaces the format specifier with the formatted text. |
| InsertLineFormat   | Inserts information followed by the default line terminator at the specified index and replaces the format specifier with the formatted text. |
| Crop               | Retrieves a substring. |
| Remove             | Removes a range of characters. |
| Substitute         | Replaces **all occurrences of** a specified information. |
| Replace            | Replaces a specified information. |
| ReplaceRange       | Replaces a range of characters. |
| ReplaceBefore      | Replaces a range of characters before a specified information. |
| ReplaceAfter       | Replaces a range of characters after a specified information. |
| ReplaceInside      | Replaces a range of characters after the first and before the second specified information. |
| SubstringRange     | Retrieves a range of characters. |
| SubstringBefore    | Retrieves a range of characters before a specified information. |
| SubstringAfter     | Retrieves a range of characters after a specified information. |
| SubstringInside    | Retrieves a range of characters after the first and before the second specified information. |
| SubstringOutside   | Retrieves a range of characters before the first and after the second specified information. |
| Trim               | Removes all leading and trailing specified information. |
| TrimStart          | Removes all leading specified information. |
| TrimEnd            | Removes all trailing specified information. |
| PadLeft            | Right-aligns the characters by padding them with spaces on the left. |
| PadRight           | Left-aligns the characters by padding them with spaces on the right. |
| ToLower            | To lowercase. |
| ToLowerInvariant   | To lowercase invariant culture. |
| ToUpper            | To uppercase. |
| ToUpperInvariant   | To uppercase invariant culture. |
| FromChar           | Removes all characters and assign the character. |
| FromCharArray      | Removes all characters and assign the array of characters. |
| FromString         | Removes all characters and assign the string. |
| Clear              | Removes all characters. |
| FormatWith         | Replaces the format specifier with the formatted text. |


## Helper methods ##

| Method name        | Use                                                                                                          |
|--------------------|--------------------------------------------------------------------------------------------------------------|
| IndexOf            | Returns a value indicating the position of a specified information. |
| IndexOfAny         | Returns a value indicating the position of **any** specified information. |
| LastIndexOf        | Returns a value indicating the position of a specified information. |
| LastIndexOfAny     | Returns a value indicating the position of **any** specified information. |
| Contains           | Returns a value indicating whether a specified information occurs. |
| ContainsSequence   | Returns a value indicating whether **all** of the specified information occurs and **matches sequentially**. |
| ContainsAll        | Returns a value indicating whether **all** of the specified information occurs. |
| ContainsAny        | Returns a value indicating whether **any** of the specified information occurs. |
| StartsWith         | Returns a value indicating whether a specified information occurs at the beginning. |
| StartsWithSequence | Returns a value indicating whether **all** of the specified information occurs and **matches sequentially** at the beginning. |
| StartsWithAny      | Returns a value indicating whether **any** of the specified information occurs at the beginning. |
| EndsWith           | Returns a value indicating whether a specified information occurs at the end. |
| EndsWithSequence   | Returns a value indicating whether **all** of the specified information occurs and **matches sequentially** at the end. |
| EndsWithAny        | Returns a value indicating whether **any** of the specified information occurs at the end. |
| Split              | Returns an array of String delimited by elements of a specified array information. |
| SplitToBuffer      | Returns an array of StringBuffer delimited by elements of a specified array information. |
| ToCharArray        | Returns the Character array representation. |
| ToString           | Returns the String representation. |
| Copy               | Deep Copy. |


